using Microsoft.AppCenter.Analytics;
using MvvmHelpers.Commands;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using XFMyDecode2020.Models;
using XFMyDecode2020.Services;


namespace XFMyDecode2020.ViewModels
{
    [QueryProperty("SessionId", "sessionId")]
    public class SessionDetailsViewModel : BaseViewModel
    {
        private string _sessionId = string.Empty;
        public string SessionId
        {
            get => _sessionId;
            set => SetProperty(ref _sessionId, value);
        }

        private Session _sessionInfo = new Session();
        public Session SessionInfo
        {
            get => _sessionInfo;
            set => SetProperty(ref _sessionInfo, value);
        }

        #region Commands
        public ICommand ChangeFavoritStateCommand { get; }
        private void ChangeFavoritState(string sessionId)
        {
            this.SessionInfo.IsFavorit = !this.SessionInfo.IsFavorit;
            _dataService.Save();

            Analytics.TrackEvent("FavChanged", new Dictionary<string, string>
            {
                ["sessionId"] = SessionInfo.SessionID,
                ["status"] = SessionInfo.IsFavorit.ToString(),
            });
        }

        public ICommand ChangeWatchStateCommand { get; }
        private void ChangeWatchState(string sessionId)
        {
            //bindingで変更されている
            this.SessionInfo.IsWatched = !this.SessionInfo.IsWatched;
            _dataService.Save();

            Analytics.TrackEvent("WatchedChanged", new Dictionary<string, string>
            {
                ["sessionId"] = SessionInfo.SessionID,
                ["status"] = SessionInfo.IsWatched.ToString(),
            });
        }

        public ICommand TweetSessionCommand { get; }
        private async Task TweetSession(string sessionId)
        {
            string text = System.Web.HttpUtility.UrlEncode($"\n#decode20 #{sessionId}");

            var canOpen = await Xamarin.Essentials.Launcher.CanOpenAsync("twitter://post");
            if (canOpen)
            {
                await Xamarin.Essentials.Launcher.OpenAsync($"twitter://post?message={text}");
            }

            Analytics.TrackEvent("TweetButtonPushed", new Dictionary<string, string>
            {
                ["sessionId"] = SessionInfo.SessionID,
            });
        }

        public ICommand OpenBrowserCommand { get; }
        private async Task OpenBrowser(string uri)
        {
            Analytics.TrackEvent("OpenBrowser", new Dictionary<string, string>
            {
                ["sessionId"] = SessionInfo.SessionID,
                ["uri"] = uri,
            });
            await Xamarin.Essentials.Browser.OpenAsync(uri);
        }
        #endregion

        private readonly IDataService _dataService;

        public SessionDetailsViewModel(IDataService dataService)
        {
            this._dataService = dataService;

            OpenBrowserCommand = new AsyncCommand<string>(OpenBrowser);
            ChangeFavoritStateCommand = new MvvmHelpers.Commands.Command<string>(ChangeFavoritState);
            ChangeWatchStateCommand = new MvvmHelpers.Commands.Command<string>(ChangeWatchState);
            TweetSessionCommand = new AsyncCommand<string>(TweetSession);
        }


        internal void LoadSessionDetails()
        {
            this.SessionInfo = _dataService.FindSessionById(SessionId);

            Analytics.TrackEvent("SessionLoaded", new Dictionary<string, string>
            {
                ["sessionId"] = SessionInfo.SessionID,
                ["sessionName"] = SessionInfo.TrackName,
            });
        }
    }
}
