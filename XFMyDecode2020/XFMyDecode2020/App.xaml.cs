﻿using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XFMyDecode2020
{
    public partial class App : Application
    {
        private ILogger<App> _logger;
        private readonly IConfiguration _config;

        public App(ILogger<App> logger,
                   Page appShell,
                   IConfiguration config)
        {
            this._logger = logger;
            this._config = config;

            var licenceKey = this._config["SyncfusionLicenceKey"];
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(licenceKey);

            InitializeComponent();

            MainPage = appShell;
        }

        protected override void OnStart()
        {
            string android = "6cd6319f-1011-4b07-ba06-b2bf890d8b41"; //this._config["AppCenter_AppSecret_Android"];
            string ios = this._config["AppCenter_AppSecret_iOS"];
            string uwp = this._config["AppCenter_AppSecret_UWP"];
            AppCenter.Start($"android={android};" +
                            $"uwp={uwp};" +
                            $"ios={ios}",
                            typeof(Analytics), typeof(Crashes));
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
