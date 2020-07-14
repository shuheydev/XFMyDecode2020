using Microsoft.Extensions.Logging;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XFMyDecode2020.Services;
using XFMyDecode2020.Views;

namespace XFMyDecode2020
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppShell : Shell
    {
        private readonly IStatusBarColorManager _statusBarColorManager;

        public AppShell(ILogger<AppShell> logger,IStatusBarColorManager statusBarColorManager)
        {
            InitializeComponent();

            Routing.RegisterRoute("favorits", typeof(FavoritListPage));
            Routing.RegisterRoute("sessions", typeof(SessionListPage));
            Routing.RegisterRoute("watched", typeof(WatchedListPage));
            Routing.RegisterRoute("sessionDetails", typeof(SessionDetailsPage));

            this._statusBarColorManager = statusBarColorManager;
        }

        protected override void OnNavigated(ShellNavigatedEventArgs args)
        {
            base.OnNavigated(args);
            
            if (args.Current.Location.OriginalString.Contains("sessions"))
            {
                App.Current.Resources["CurrentAccentColor"] = App.Current.Resources["AppPrimaryColor"];
            }
            else if (args.Current.Location.OriginalString.Contains("favorits"))
            {
                App.Current.Resources["CurrentAccentColor"] = App.Current.Resources["FavoritListShellColor"];
            }
            else if (args.Current.Location.OriginalString.Contains("watched"))
            {
                App.Current.Resources["CurrentAccentColor"] = App.Current.Resources["WatchedListShellColor"];
            }
            else
            {
                throw new ArgumentException(message: "Navigated from Unknown Page exception.");
            }
          
            var color = (Xamarin.Forms.Color)App.Current.Resources["CurrentAccentColor"];
            this._statusBarColorManager.SetColor(color, false);
        }
    }
}