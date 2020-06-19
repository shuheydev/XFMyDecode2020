using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XFMyDecode2020.ViewModels;

namespace XFMyDecode2020.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SessionDetailsPage : ContentPage
    {
        private readonly SessionDetailsViewModel _viewModel;

        public SessionDetailsPage()
        {
            InitializeComponent();
            this.BindingContext = _viewModel = Startup.ServiceProvider.GetService<SessionDetailsViewModel>();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.LoadSessionDetails();
        }
    }
}