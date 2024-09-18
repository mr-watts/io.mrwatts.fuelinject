#!/usr/bin/env bash

set -x
set -o pipefail

echo "Letting Unity generate C# project files"

${UNITY_EXECUTABLE:-xvfb-run --auto-servernum --server-args='-screen 0 640x480x24' unity-editor} \
  -projectPath "$UNITY_DIR" \
  -executeMethod TriggerCSharpProjectEditorScript.GenerateCSharpSDKStyleProjects \
  -logFile /dev/stdout \
  -batchmode \
  -nographics \
  -quit \
  -debugCodeOptimization

UNITY_EXIT_CODE=$?

if [ $UNITY_EXIT_CODE -eq 0 ]; then
  echo "Run succeeded, no failures occurred";
else
  echo "Unexpected exit code $UNITY_EXIT_CODE";
  exit 1
fi

echo "Running .NET build"

DOTNET_BUILD_LOG="/tmp/dotnet-build.log"
JUNIT_JSON_OUTPUT_FILE="$UNITY_DIR/dotnet-build-errors-junit.json"

chmod +x ./ci/dotnet-build-output-to-junit-json.py

dotnet build /nologo /clp:NoSummary ./*.sln | tee $DOTNET_BUILD_LOG

DOTNET_BUILD_EXIT_CODE=$?

./ci/dotnet-build-output-to-junit-json.py < $DOTNET_BUILD_LOG > "$JUNIT_JSON_OUTPUT_FILE"

cat "$JUNIT_JSON_OUTPUT_FILE"

node node_modules/.bin/jsonjunit --json ci --junit ci

rm $DOTNET_BUILD_LOG

exit $DOTNET_BUILD_EXIT_CODE
