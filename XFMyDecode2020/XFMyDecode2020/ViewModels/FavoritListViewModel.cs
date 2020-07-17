using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using XFMyDecode2020.Models;
using XFMyDecode2020.Services;
using XFMyDecode2020.Utilities;

namespace XFMyDecode2020.ViewModels
{
    public class FavoritListViewModel : BaseViewModel
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
                SetProperty(ref _searchString, value, onChanged: SearchSession);
            }
        }

        public ICommand SearchSessionCommand { get; }
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
                                                                new MvvmHelpers.ObservableRangeCollection<Session>(g))));

            Analytics.TrackEvent("SearchInputed");
        }

        public ICommand ShowSessionDetailsCommand { get; }
        private async Task ShowSessionDetails(string sessionId)
        {
            Analytics.TrackEvent("SessionSelected", new Dictionary<string, string>
            {
                ["sessionId"] = sessionId
            });

            await Shell.Current.GoToAsync($"sessionDetails?sessionId={sessionId}");
        }
        public ICommand ChangeFavoritStateCommand { get; }
        private void ChangeFavoritState(string sessionId)
        {
            var session = this.Sessions.FirstOrDefault(s => s.SessionID == sessionId);
            session.IsFavorit = !session.IsFavorit;
            _dataService.Save();

            Analytics.TrackEvent("FavChanged", new Dictionary<string, string>
            {
                ["sessionId"] = session.SessionID,
                ["status"] = session.IsFavorit.ToString()
            });

            SessionGroup group = this.GroupedSessions.FirstOrDefault(g => g.Any(s => s == session));
            if (group == null)
                return;

            group.Remove(session);

            //remove empty group
            if (!group.Any())
            {
                this.GroupedSessions.Remove(group);
            }
        }


        private readonly IDataService _dataService;

        public FavoritListViewModel(IDataService dataService)
        {
            this._dataService = dataService;

            this.Sessions = new MvvmHelpers.ObservableRangeCollection<Session>();
            this.GroupedSessions = new MvvmHelpers.ObservableRangeCollection<SessionGroup>();

            ShowSessionDetailsCommand = new AsyncCommand<string>(ShowSessionDetails);
            ChangeFavoritStateCommand = new MvvmHelpers.Commands.Command<string>(ChangeFavoritState);
            SearchSessionCommand = new MvvmHelpers.Commands.Command(SearchSession);

            LoadSessionsAsync().Wait();
        }

        private async Task LoadSessionsAsync()
        {
            //詳細ページからの遷移時にリストがリセットされてスクロール位置が先頭に
            //戻ることを防ぐため
            try
            {
                var sessions = await _dataService.GetSessionDataAsync().ConfigureAwait(false);
                var removeTargets = sessions.Where(s => this.Sessions.Contains(s) && !s.IsFavorit).ToList();
                var addTargets = sessions.Where(s => !this.Sessions.Contains(s) && s.IsFavorit).ToList();

                this.Sessions.RemoveRange(removeTargets);
                this.Sessions.AddRange(addTargets);
                this.Sessions.OrderBy(s => s.TrackID);

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
                        var trackIds = this.GroupedSessions.Select(g => g.TrackID).ToList();
                        trackIds.Add(session.TrackID);
                        var orderedTrackIDs = trackIds.OrderBy(t => t);
                        int index = orderedTrackIDs.IndexOf(t => t == session.TrackID);
                        this.GroupedSessions.Insert(index, new SessionGroup(session.TrackID,
                                                                            session.TrackName,
                                                                            new MvvmHelpers.ObservableRangeCollection<Session>()));
                    }

                    this.GroupedSessions.FirstOrDefault(g => g.TrackID == session.TrackID)?.Add(session);
                }

                Analytics.TrackEvent("SessionListLoaded");
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                throw;
            }
        }
    }
}
