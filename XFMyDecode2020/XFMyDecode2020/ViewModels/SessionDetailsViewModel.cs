using Microsoft.Extensions.Primitives;
using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using XFMyDecode2020.Models;
using XFMyDecode2020.Services;


namespace XFMyDecode2020.ViewModels
{
    [QueryProperty("SessionId", "sessionId")]
    public class SessionDetailsViewModel : BaseViewModel
    {
        public string SessionId { get; set; }

        private Session _sessionInfo;
        public Session SessionInfo
        {
            get => _sessionInfo;
            set => SetProperty(ref _sessionInfo, value);
        }

        #region Commands
        public MvvmHelpers.Commands.Command<string> ChangeFavoritStateCommand { get; }
        private void ChangeFavoritState(string sessionId)
        {
            this.SessionInfo.IsFavorit = !this.SessionInfo.IsFavorit;
            _dataService.Save();
        }

        public AsyncCommand<string> TweetSessionCommand { get; }
        private async Task TweetSession(string sessionId)
        {
            string text =System.Web.HttpUtility.UrlEncode( $"\n#decode20 #{sessionId}");

            var canOpen = await Xamarin.Essentials.Launcher.CanOpenAsync("twitter://post");
            if (canOpen)
            {
                await Xamarin.Essentials.Launcher.OpenAsync($"twitter://post?message={text}");
            }
        }

        public AsyncCommand<string> OpenBrowserCommand { get; }
        private async Task OpenBrowser(string uri)
        {
            await Xamarin.Essentials.Browser.OpenAsync(uri);
        }
        #endregion

        private readonly IDataService _dataService;

        public SessionDetailsViewModel(IDataService dataService)
        {
            this._dataService = dataService;

            OpenBrowserCommand = new AsyncCommand<string>(OpenBrowser);
            ChangeFavoritStateCommand = new MvvmHelpers.Commands.Command<string>(ChangeFavoritState);
            TweetSessionCommand = new AsyncCommand<string>(TweetSession);
        }



        internal void LoadSessionDetails()
        {
            this.SessionInfo = _dataService.FindSessionById(SessionId);
        }
    }
}
