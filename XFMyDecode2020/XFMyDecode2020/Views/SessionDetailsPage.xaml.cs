﻿using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XFMyDecode2020.Utilities;
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

        private async void Button_Fav_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;

            await MyAnimation.Animation1(button);
        }
    }
}