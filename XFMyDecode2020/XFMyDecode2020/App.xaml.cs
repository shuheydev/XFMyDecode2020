using Microsoft.AppCenter;
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
#if RELEASE
            AppCenter.Start(appSecret: this._config["AppCenter_AppSecret"],
                            typeof(Analytics),
                            typeof(Crashes));
#endif
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
