#!/usr/bin/env bash

set -x

echo "Testing for $TEST_PLATFORM, Unit Type: $TESTING_TYPE"

CODE_COVERAGE_PACKAGE="com.unity.testtools.codecoverage"
PACKAGE_MANIFEST_PATH="Packages/manifest.json"

# Install NuGet packages through NuGetForUnity. We ignore compiler errors because our scripts depend on these NuGet
# packages, so they will always fail compilation at this step (as we are about to install them).
unity-editor -batchmode -ignoreCompilerError -projectPath $UNITY_DIR -executeMethod NugetForUnity.NugetHelper.Restore

echo -e "\e[0Ksection_start:`date +%s`:nuget_packages_section[collapsed=true]\r\e[0KInstalling NuGet packages through NuGetForUnity"

${UNITY_EXECUTABLE:-xvfb-run --auto-servernum --server-args='-screen 0 640x480x24' unity-editor} \
    -projectPath $UNITY_DIR \
    -executeMethod NugetForUnity.NugetHelper.Restore \
    -logFile /dev/stdout \
    -batchmode \
    -nographics \
    -debugCodeOptimization \
    -quit \
    -ignoreCompilerErrors

UNITY_NUGET_EXIT_CODE=$?

if [ $UNITY_NUGET_EXIT_CODE -eq 0 ]; then
  echo "Package installation finished successfully";
else
  echo "Package installation failed with error code $UNITY_NUGET_EXIT_CODE";
fi

echo -e "\e[0Ksection_end:`date +%s`:nuget_packages_section\r\e[0K"
echo -e "\e[0Ksection_start:`date +%s`:unity_tests_section[collapsed=true]\r\e[0KRunning Unity tests"

${UNITY_EXECUTABLE:-xvfb-run --auto-servernum --server-args='-screen 0 640x480x24' unity-editor} \
  -projectPath $UNITY_DIR \
  -runTests \
  -testPlatform $TEST_PLATFORM \
  -testResults $UNITY_DIR/$TEST_PLATFORM-results.xml \
  -logFile /dev/stdout \
  -batchmode \
  -nographics \
  -enableCodeCoverage \
  -coverageResultsPath $UNITY_DIR/$TEST_PLATFORM-coverage \
  -coverageOptions "generateAdditionalMetrics;generateHtmlReport;generateHtmlReportHistory;generateBadgeReport;" \
  -debugCodeOptimization

UNITY_EXIT_CODE=$?

if [ $UNITY_EXIT_CODE -eq 0 ]; then
  echo "Run succeeded, no failures occurred";
elif [ $UNITY_EXIT_CODE -eq 2 ]; then
  echo "Run succeeded, some tests failed";
  if [ $TESTING_TYPE == 'JUNIT' ]; then
    echo "Converting results to JUNit for analysis";
    saxonb-xslt -s $UNITY_DIR/$TEST_PLATFORM-results.xml -xsl $CI_PROJECT_DIR/ci/nunit-transforms/nunit3-junit.xslt >$UNITY_DIR/$TEST_PLATFORM-junit-results.xml
  fi
elif [ $UNITY_EXIT_CODE -eq 3 ]; then
  echo "Run failure (other failure)";
  if [ $TESTING_TYPE == 'JUNIT' ]; then
    echo "Not converting results to JUNit";
  fi
else
  echo "Unexpected exit code $UNITY_EXIT_CODE";
  if [ $TESTING_TYPE == 'JUNIT' ]; then
    echo "Not converting results to JUNit";
  fi
fi

echo -e "\e[0Ksection_end:`date +%s`:unity_tests_section\r\e[0K"

if grep $CODE_COVERAGE_PACKAGE $PACKAGE_MANIFEST_PATH; then
  cat $UNITY_DIR/$TEST_PLATFORM-coverage/Report/Summary.xml | grep Linecoverage
  mv $UNITY_DIR/$TEST_PLATFORM-coverage/$CI_PROJECT_NAME-opencov/*Mode/TestCoverageResults_*.xml $UNITY_DIR/$TEST_PLATFORM-coverage/coverage.xml
  rm -r $UNITY_DIR/$TEST_PLATFORM-coverage/$CI_PROJECT_NAME-opencov/
else
  {
    echo -e "\033[33mCode Coverage package not found in $PACKAGE_MANIFEST_PATH. Please install the package \"Code Coverage\" through Unity's Package Manager to enable coverage reports.\033[0m"
  } 2> /dev/null
fi

cat $UNITY_DIR/$TEST_PLATFORM-results.xml | grep test-run | grep Passed
exit $UNITY_EXIT_CODE
