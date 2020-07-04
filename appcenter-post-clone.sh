#! /usr/bin/env bash

echo "Variables:"

sed -i '' "s|SYNCFUSION_LICENCE_KEY|$SYNCFUSION_LICENCE_KEY|g" $BUILD_REPOSITORY_LOCALPATH/XFMyDecode2020/appsettings.json

# pring out for reference
cat $BUILD_REPOSITORY_LOCALPATH/XFMyDecode2020/appsettings.json