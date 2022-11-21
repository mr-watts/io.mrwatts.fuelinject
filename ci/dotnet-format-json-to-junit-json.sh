#!/bin/sh
#

jq '{reportTitle: "dotnet format", stats: {start: now}, suites:{suites:[.[] | {title: .FilePath, tests: [{title: "\(.FileName) (\(.FileChanges[0].DiagnosticId))", state: "failed", fail: true, duration: 0, err: {message: .FileChanges | map("\(.LineNumber):\(.CharNumber): \(.FormatDescription)") | join("\n") }}]}]}}' < /dev/stdin