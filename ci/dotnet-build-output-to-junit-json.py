#!/usr/bin/env python3
#

import re
import json
import time
import fileinput

from pathlib import Path

expression = re.compile('(?P<file>.+)\((?P<line>\d+),(?P<column>\d+)\): (?P<severity>error|warning) (?:(?P<analyzerCode>\w+): )?(?P<message>.+)\[(?P<csprojPath>.+)\]')

tests_by_file = {}

for line in fileinput.input():
    matches = expression.match(line)

    if matches is None:
        continue;

    file = matches.group('file')

    if file not in tests_by_file:
        tests_by_file[file] = []

    file_name = Path(file).name

    tests_by_file[file].append({
        "title": f"{file_name} ({matches.group('analyzerCode') or 'Unknown'})",
        "state": "failed",
        "fail": True,
        "duration": 0,
        "err": {
            "message": f"{matches.group('severity').upper()}: {matches.group('line')}:{matches.group('column')}: {matches.group('message').strip()}"
        }
    })

suites = []

for file, tests in tests_by_file.items():
    suites.append({
        "title": file,
        "tests": tests
    });

print(json.dumps({
    "reportTitle": "dotnet build",
    "stats": {
        # "start": datetime.datetime.now(datetime.timezone.utc).strftime('%d/%m/%Y %H:%M:%S.%f')
        "start": time.time()
    },
    "suites": {
        "suites": suites
    }
}))

# {
#   "reportTitle": "dotnet format",
#   "stats": {
#     "start": 1690290890.572147
#   },
#   "suites": {
#     "suites": [
#       {
#         "title": "/home/werk/Projecten/Internal/Packages/unity-fuel-inject/Assets/Scripts/Services/MonoBehaviourWithGameObjectInstantiatorDependency.cs",
#         "tests": [
#           {
#             "title": "MonoBehaviourWithGameObjectInstantiatorDependency.cs (WHITESPACE)",
#             "state": "failed",
#             "fail": true,
#             "duration": 0,
#             "err": {
#               "message": "5:90: Fix whitespace formatting. Insert '\\n\\s\\s\\s\\s'."
#             }
#           }
#         ]
#       },
#       {
#         "title": "/home/werk/Projecten/Internal/Packages/unity-fuel-inject/Assets/Scripts/Services/Bar.cs",
#         "tests": [
#           {
#             "title": "Bar.cs (WHITESPACE)",
#             "state": "failed",
#             "fail": true,
#             "duration": 0,
#             "err": {
#               "message": "3:35: Fix whitespace formatting. Insert '\\s'.\n3:36: Fix whitespace formatting. Insert '\\s'."
#             }
#           }
#         ]
#       }
#     ]
#   }
# }
