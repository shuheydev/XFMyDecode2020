using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XFMyDecode2020.Views;

namespace XFMyDecode2020
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("sessions", typeof(SessionListPage));
            Routing.RegisterRoute("sessionDetails", typeof(SessionDetailsPage));
        }
    }
}