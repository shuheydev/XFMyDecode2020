#! /usr/bin/env bash

echo "Variables:"

APP_SETTINGS_FILE=$BUILD_REPOSITORY_LOCALPATH/XFMyDecode2020/XFMyDecode2020/appsettings.json

sed -i '' "s|SYNCFUSION_LICENCE_KEY|$SYNCFUSION_LICENCE_KEY|g" $APP_SETTINGS_FILE

# pring out for reference
cat $APP_SETTINGS_FILE