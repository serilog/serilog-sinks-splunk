#!/bin/bash
dotnet restore
for path in src/*/project.json; do
    dirname="$(dirname "${path}")"
    dotnet build ${dirname}
done 

for path in sample/project.json; do
    dirname="$(dirname "${path}")"
    dotnet build ${dirname}
done 