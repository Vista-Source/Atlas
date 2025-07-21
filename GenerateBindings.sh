#!/bin/bash

dotnet build
dotnet Build/Atlas.CLI.dll --t ../ --n SourceSDK -l client --extensions Atlas.ProjectFactory

read -p "Press Enter to exit..."