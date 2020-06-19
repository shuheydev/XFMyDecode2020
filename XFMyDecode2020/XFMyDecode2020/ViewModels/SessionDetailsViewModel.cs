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

        private readonly IDataService _dataService;

        public SessionDetailsViewModel()
        {

        }

        public AsyncCommand<string> OpenBrowserCommand { get; }
        public SessionDetailsViewModel(IDataService dataService)
        {
            this._dataService = dataService;

            OpenBrowserCommand = new AsyncCommand<string>(OpenBrowser);
        }

        private async Task OpenBrowser(string uri)
        {
            await Xamarin.Essentials.Browser.OpenAsync(uri);
        }

        internal void LoadSessionDetails()
        {
            this.SessionInfo = _dataService.FindSessionById(SessionId);
        }
    }
}
