﻿using MvvmHelpers.Commands;
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

                var searchWords = Regex.Split(this.SearchString, @"\s+?").Where(w => !string.IsNullOrEmpty(w)).ToList();

                //Let's filtering
                var filteredSessions = Sessions.Where(s =>
                {
                    bool result = Utility.CheckIfContainSearchWord(s, searchWords);

                    return result;
                }).ToList();

                this.GroupedSessions.Clear();
                this.GroupedSessions.AddRange(filteredSessions.GroupBy(s => s.TrackID)
                                      .Select(g => new SessionGroup(g.Key, g.FirstOrDefault()?.TrackName, g.ToList())));
            }
        }

        public AsyncCommand<string> ShowSessionDetailsCommand { get; }
        private async Task ShowSessionDetails(string sessionId)
        {
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
            }
        }

        private readonly IDataService _dataService;

        public SessionListViewModel(IDataService dataService)
        {
            this._dataService = dataService;

            ShowSessionDetailsCommand = new AsyncCommand<string>(ShowSessionDetails);
            ChangeFavoritStateCommand = new MvvmHelpers.Commands.Command<string>(ChangeFavoritState);
        }

        internal async Task LoadSessions()
        {
            if (this.Sessions != null)
                return;

            try
            {
                var sessions = await _dataService.GetSessionDataAsync();
                this.Sessions = new MvvmHelpers.ObservableRangeCollection<Session>(sessions);

                //Let's grouping
                this.GroupedSessions = new MvvmHelpers.ObservableRangeCollection<SessionGroup>();
                this.GroupedSessions.AddRange(this.Sessions.GroupBy(s => s.TrackID)
                                                      .Select(g => new SessionGroup(g.Key, g.FirstOrDefault()?.TrackName, g.ToList())));
            }
            catch
            {
                throw;
            }
        }
    }
}
