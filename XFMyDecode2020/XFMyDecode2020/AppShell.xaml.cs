using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XFMyDecode2020.ViewModels;
using XFMyDecode2020.Views;

namespace XFMyDecode2020
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppShell : Shell
    {
        public AppShell(ILogger<AppShell> logger)
        {
            InitializeComponent();

            Routing.RegisterRoute("favorits", typeof(FavoritListPage));
            Routing.RegisterRoute("sessions", typeof(SessionListPage));
            Routing.RegisterRoute("watched", typeof(WatchedListPage));
            Routing.RegisterRoute("sessionDetails", typeof(SessionDetailsPage));
        }
    }
}