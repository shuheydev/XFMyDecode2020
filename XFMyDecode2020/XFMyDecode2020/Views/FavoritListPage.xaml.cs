using Acr.UserDialogs;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XFMyDecode2020.Utilities;
using XFMyDecode2020.ViewModels;

namespace XFMyDecode2020.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FavoritListPage : ContentPage
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

        private FavoritListViewModel _viewModel;
        public FavoritListPage()
        {
            InitializeComponent();
            this.BindingContext = _viewModel = Startup.ServiceProvider.GetService<FavoritListViewModel>();

            SetHeaderBehaviorByScroll();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadSessions();

            ResetFrameHeaderPosition();

            App.Current.Resources["CurrentAccentColor"] = App.Current.Resources["FavoritListShellColor"];
        }

        private void ResetFrameHeaderPosition()
        {
            Frame_SearchBox.TranslationY = 0;
        }

        private double _slideMargin;
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            _slideMargin = (Frame_SearchBox.Height + Frame_SearchBox.Margin.Top);
        }

        private void SetHeaderBehaviorByScroll()
        {
            //ヘッダーを隠す側の動作
            var hide = Observable.FromEventPattern<ItemsViewScrolledEventArgs>(CollectionsView_Sessions, nameof(CollectionsView_Sessions.Scrolled));
            hide.Where(x => x.EventArgs.VerticalDelta >= 0)
                .Repeat()
                .Subscribe(x =>
                {
                    double nextTransY = Frame_SearchBox.TranslationY - x.EventArgs.VerticalDelta;

                    if (nextTransY < -_slideMargin)
                    {
                        nextTransY = -_slideMargin;
                    }
                    nextTransY = Math.Max(-(Frame_SearchBox.Height + Frame_SearchBox.Y), nextTransY);
                    Frame_SearchBox.TranslationY = nextTransY;
                });

            //ヘッダーを表示する側の動作
            var show = Observable.FromEventPattern<ItemsViewScrolledEventArgs>(CollectionsView_Sessions, nameof(CollectionsView_Sessions.Scrolled));
            show.Where(x => x.EventArgs.VerticalDelta < 0)
                .Repeat()
                .Subscribe(x =>
                {
                    double nextTransY = Frame_SearchBox.TranslationY - x.EventArgs.VerticalDelta;
                    if (nextTransY > 0)
                    {
                        nextTransY = 0;
                    }

                    Frame_SearchBox.TranslationY = nextTransY;
                });
        }

        private async void FilterButton_Tapped(object sender, EventArgs e)
        {
            var choices = _viewModel.GroupedSessions.Select(g => $"{g.TrackID} : {g.TrackName}");

            string choice = await UserDialogs.Instance.ActionSheetAsync("Choose a track", "Cancel", null, CancellationToken.None, choices.ToArray());

            if (choice.Equals("Cancel"))
            {
                return;
            }

            var trackId = choice.Substring(0, 1);

            var group = _viewModel.GroupedSessions.FirstOrDefault(g => g.TrackID == trackId);
            var item = group.FirstOrDefault();

            CollectionsView_Sessions.ScrollTo(item, group, ScrollToPosition.Center, animate: false);
        }

        private async void Button_Fav_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;

            await MyAnimation.Animation1(button);
        }
    }
}
