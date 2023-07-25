#!/usr/bin/env bash

set -x
set -o pipefail

echo "Running .NET build"

DOTNET_BUILD_LOG="/tmp/dotnet-build.log"
JUNIT_JSON_OUTPUT_FILE="$UNITY_DIR/dotnet-build-errors-junit.json"

chmod +x ./ci/dotnet-build-output-to-junit-json.py

dotnet build /nologo /clp:NoSummary | tee $DOTNET_BUILD_LOG

DOTNET_BUILD_EXIT_CODE=$?

./ci/dotnet-build-output-to-junit-json.py < $DOTNET_BUILD_LOG > "$JUNIT_JSON_OUTPUT_FILE"

cat "$JUNIT_JSON_OUTPUT_FILE"

node node_modules/.bin/jsonjunit --json ci --junit ci

rm $DOTNET_BUILD_LOG

exit $DOTNET_BUILD_EXIT_CODE
