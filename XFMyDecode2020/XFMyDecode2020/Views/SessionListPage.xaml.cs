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
    public partial class SessionListPage : ContentPage
    {
        private SessionListViewModel _viewModel;
        public SessionListPage()
        {
            InitializeComponent();
            this.BindingContext = _viewModel = Startup.ServiceProvider.GetService<SessionListViewModel>();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadSessions();
        }
    }
}