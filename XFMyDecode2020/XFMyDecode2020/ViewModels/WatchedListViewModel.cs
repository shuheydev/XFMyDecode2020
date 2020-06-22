using MvvmHelpers;
using MvvmHelpers.Commands;
using MvvmHelpers.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using XFMyDecode2020.Models;
using XFMyDecode2020.Services;
using Xamarin.Forms.Xaml;
using XFMyDecode2020.Utilities;
using Xamarin.Forms.Internals;

namespace XFMyDecode2020.ViewModels
{
    public class WatchedListViewModel : BaseViewModel
    {
        private MvvmHelpers.ObservableRangeCollection<Session> _sessions;
        public MvvmHelpers.ObservableRangeCollection<Session> Sessions
        {
            get => _sessions;
            set => SetProperty(ref _sessions, value);
        }

        private MvvmHelpers.ObservableRangeCollection<SessionGroup> _groupedSessions;
        public MvvmHelpers.ObservableRangeCollection<SessionGroup> GroupedSessions
        {
            get => _groupedSessions;
            set => SetProperty(ref _groupedSessions, value);
        }

        private string _searchString;
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
                                  .Select(g => new SessionGroup(g.Key, g.FirstOrDefault()?.TrackName, new MvvmHelpers.ObservableRangeCollection<Session>(g.ToList()))));
        }

        public AsyncCommand<string> ShowSessionDetailsCommand { get; }
        private async Task ShowSessionDetails(string sessionId)
        {
            await Shell.Current.GoToAsync($"sessionDetails?sessionId={sessionId}");
        }

        public AsyncCommand<string> ChangeFavoritStateCommand { get; }
        private async Task ChangeFavoritState(string sessionId)
        {
            var session = this.Sessions.FirstOrDefault(s => s.SessionID == sessionId);
            if (session != null)
            {
                session.IsFavorit = !session.IsFavorit;
                _dataService.Save();
            }
        }


        private readonly IDataService _dataService;

        public WatchedListViewModel(IDataService dataService)
        {
            this._dataService = dataService;

            ShowSessionDetailsCommand = new AsyncCommand<string>(ShowSessionDetails);
            ChangeFavoritStateCommand = new AsyncCommand<string>(ChangeFavoritState);
            SearchSessionCommand = new MvvmHelpers.Commands.Command(SearchSession);
        }

        internal async Task LoadSessions()
        {
            //if (this.Sessions != null)
            //    return;

            try
            {
                var allSessions = await _dataService.GetSessionDataAsync();
                var sessions = allSessions.Where(s => s.IsWatched);
                this.Sessions = new MvvmHelpers.ObservableRangeCollection<Session>(sessions);

                this.WatchedSessionsCount = this.Sessions.Count;
                this.TotalSessionsCount = allSessions.Count();

                //Let's grouping
                this.GroupedSessions = new MvvmHelpers.ObservableRangeCollection<SessionGroup>();
                this.GroupedSessions.AddRange(this.Sessions.GroupBy(s => s.TrackID)
                                                      .Select(g => new SessionGroup(g.Key, g.FirstOrDefault()?.TrackName,new MvvmHelpers.ObservableRangeCollection<Session>(g.ToList()))));
            }
            catch
            {
                throw;
            }
        }
    }

}
