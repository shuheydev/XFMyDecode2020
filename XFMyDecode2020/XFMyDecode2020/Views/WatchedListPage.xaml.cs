﻿using System;
using System.Linq;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XFMyDecode2020.Utilities;
using XFMyDecode2020.ViewModels;

namespace XFMyDecode2020.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WatchedListPage : ContentPage
    {
        private string _message;
        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        private WatchedListViewModel _viewModel;
        public WatchedListPage()
        {
            InitializeComponent();
            this.BindingContext = _viewModel = Startup.ServiceProvider.GetService<WatchedListViewModel>();

            SetHeaderBehaviorByScroll();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadSessions();

            ResetFrameHeaderPosition();
        }

        private void ResetFrameHeaderPosition()
        {
            frame_Header.TranslationY = 0;
        }

        private readonly double _slideToggleYPosition = 110;
        private void SetHeaderBehaviorByScroll()
        {
            //ヘッダーを隠す側の動作
            var hide = Observable.FromEventPattern<ItemsViewScrolledEventArgs>(CollectionsView_Sessions, nameof(CollectionsView_Sessions.Scrolled));
            hide.Where(x => x.EventArgs.VerticalDelta >= 0 && x.EventArgs.VerticalOffset >= _slideToggleYPosition)
                .Repeat()
                .Subscribe(x =>
                {
                    double nextTransY = frame_Header.TranslationY - x.EventArgs.VerticalDelta;

                    nextTransY = Math.Max(-(frame_Header.Height + frame_Header.Y), nextTransY);
                    frame_Header.TranslationY = nextTransY;
                });

            //ヘッダーを表示する側の動作
            var show = Observable.FromEventPattern<ItemsViewScrolledEventArgs>(CollectionsView_Sessions, nameof(CollectionsView_Sessions.Scrolled));
            show.Where(x => x.EventArgs.VerticalDelta < 0)
                .Repeat()
                .Subscribe(x =>
                {
                    double nextTransY = frame_Header.TranslationY - x.EventArgs.VerticalDelta;
                    if (nextTransY > 0)
                    {
                        nextTransY = 0;
                    }

                    frame_Header.TranslationY = nextTransY;
                });
        }

        private async void Button_Fav_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;

            await MyAnimation.Animation1(button);
        }
    }
}