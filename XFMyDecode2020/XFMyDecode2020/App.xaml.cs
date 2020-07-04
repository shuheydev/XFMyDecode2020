using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XFMyDecode2020
{
    public partial class App : Application
    {
        public App(ILogger<App> logger,
                   Page appShell,
                   IConfiguration config)
        {
            InitializeComponent();

            MainPage = appShell;
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
