using Microsoft.AppCenter.Analytics;
using MvvmHelpers.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;
using XFMyDecode2020.Models;
using XFMyDecode2020.Services;
using XFMyDecode2020.Utilities;

namespace XFMyDecode2020.ViewModels
{
    public class SessionListViewModel : BaseViewModel
    {
        private MvvmHelpers.ObservableRangeCollection<Session> _sessions = new MvvmHelpers.ObservableRangeCollection<Session>();
        public MvvmHelpers.ObservableRangeCollection<Session> Sessions
        {
            get => _sessions;
            set => SetProperty(ref _sessions, value);
        }

        private MvvmHelpers.ObservableRangeCollection<SessionGroup> _groupedSessions = new MvvmHelpers.ObservableRangeCollection<SessionGroup>();
        public MvvmHelpers.ObservableRangeCollection<SessionGroup> GroupedSessions
        {
            get => _groupedSessions;
            set => SetProperty(ref _groupedSessions, value);
        }

        private string _searchString = string.Empty;
        public string SearchString
        {
            get => _searchString;
            set
            {
                SetProperty(ref _searchString, value);

                SearchSession();
            }
        }

        public MvvmHelpers.Commands.Command SearchSessionCommand { get; }
        private void SearchSession()
        {
            var searchWords = Regex.Split(this.SearchString, @"\s+?").Where(w => !string.IsNullOrEmpty(w)).ToList();

            //Let's filtering
            var filteredSessions = Sessions.Where(s =>
            {
                bool result = Utility.CheckIfContainSearchWord(s, searchWords);

                return result;
            }).ToList();

            this.GroupedSessions.Clear();
            this.GroupedSessions.AddRange(filteredSessions.GroupBy(s => s.TrackID)
                                  .Select(g => new SessionGroup(g.Key,
                                                                g.FirstOrDefault().TrackName,
                                                                new MvvmHelpers.ObservableRangeCollection<Session>(g.ToList()))));

            Analytics.TrackEvent("search inputed");
        }

        public AsyncCommand<string> ShowSessionDetailsCommand { get; }
        private async Task ShowSessionDetails(string sessionId)
        {
            Analytics.TrackEvent("sessionSelected", new Dictionary<string, string>
            {
                ["sessionId"] = sessionId
            });

            await Shell.Current.GoToAsync($"sessionDetails?sessionId={sessionId}");
        }

        public MvvmHelpers.Commands.Command<string> ChangeFavoritStateCommand { get; }
        private void ChangeFavoritState(string sessionId)
        {
            var session = this.Sessions.FirstOrDefault(s => s.SessionID == sessionId);

            if (session != null)
            {
                session.IsFavorit = !session.IsFavorit;
                _dataService.Save();

                Analytics.TrackEvent("FavChanged", new Dictionary<string, string>
                {
                    ["sessionId"] = session.SessionID,
                    ["status"] = session.IsFavorit.ToString(),
                });
            }
        }


        private readonly IDataService _dataService;

        public SessionListViewModel(IDataService dataService)
        {
            this._dataService = dataService;

            ShowSessionDetailsCommand = new AsyncCommand<string>(ShowSessionDetails);
            ChangeFavoritStateCommand = new MvvmHelpers.Commands.Command<string>(ChangeFavoritState);
            SearchSessionCommand = new MvvmHelpers.Commands.Command(SearchSession);
        }

        internal async Task LoadSessions()
        {
            try
            {
                if (!this.Sessions.Any())
                {
                    var sessions = await _dataService.GetSessionDataAsync();
                    this.Sessions.AddRange(sessions);

                    //Let's grouping
                    this.GroupedSessions.AddRange(this.Sessions.GroupBy(s => s.TrackID)
                                                               .Select(g => new SessionGroup(g.Key,
                                                                                             g.FirstOrDefault().TrackName,
                                                                                             new MvvmHelpers.ObservableRangeCollection<Session>(g.ToList()))));

                    Analytics.TrackEvent("sessionListLoaded");
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
