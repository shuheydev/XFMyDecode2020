using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using MvvmHelpers.Commands;
using System;
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
    public class WatchedListViewModel : BaseViewModel
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

        private int _watchedSessionsCount;
        public int WatchedSessionsCount
        {
            get => _watchedSessionsCount;
            set => SetProperty(ref _watchedSessionsCount, value);
        }

        private int _totalSessionsCount;
        public int TotalSessionsCount
        {
            get => _totalSessionsCount;
            set => SetProperty(ref _totalSessionsCount, value);
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
            Analytics.TrackEvent("SessionSelected", new Dictionary<string, string>
            {
                ["sessionId"] = sessionId,
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

        public WatchedListViewModel(IDataService dataService)
        {
            this._dataService = dataService;

            this.Sessions = new MvvmHelpers.ObservableRangeCollection<Session>();
            this.GroupedSessions = new MvvmHelpers.ObservableRangeCollection<SessionGroup>();

            ShowSessionDetailsCommand = new AsyncCommand<string>(ShowSessionDetails);
            ChangeFavoritStateCommand = new MvvmHelpers.Commands.Command<string>(ChangeFavoritState);
            SearchSessionCommand = new MvvmHelpers.Commands.Command(SearchSession);
        }

        internal async Task LoadSessions()
        {
            //FavoritListと同様.
            try
            {
                var sessions = (await _dataService.GetSessionDataAsync());
                var removeTargets = sessions.Where(s => this.Sessions.Contains(s) && !s.IsWatched).ToList();
                var addTargets = sessions.Where(s => !this.Sessions.Contains(s) && s.IsWatched).ToList();

                this.Sessions.RemoveRange(removeTargets);
                this.Sessions.AddRange(addTargets);
                this.Sessions.OrderBy(s => s.TrackID);

                this.WatchedSessionsCount = this.Sessions.Count;
                this.TotalSessionsCount = sessions.Count();

                foreach (var session in removeTargets)
                {
                    var group = this.GroupedSessions.FirstOrDefault(g => g.TrackID == session.TrackID);

                    if (group is null)
                    {
                        continue;
                    }

                    group.Remove(session);

                    //remove empty group
                    if (!group.Any())
                    {
                        this.GroupedSessions.Remove(group);
                    }
                }

                foreach (var session in addTargets)
                {
                    var group = this.GroupedSessions.FirstOrDefault(g => g.TrackID == session.TrackID);

                    if (group is null)
                    {
                        this.GroupedSessions.Add(new SessionGroup(session.TrackID,
                                                                  session.TrackName,
                                                                  new MvvmHelpers.ObservableRangeCollection<Session>()));
                    }

                    this.GroupedSessions.FirstOrDefault(g => g.TrackID == session.TrackID)?.Add(session);
                }

                Analytics.TrackEvent("sessionListLoaded");
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                throw;
            }

            this.GroupedSessions.OrderBy(g => g.TrackID);
        }
    }
}
