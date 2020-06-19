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


namespace XFMyDecode2020.ViewModels
{
    public class SessionListViewModel : BaseViewModel
    {
        private ObservableRangeCollection<Session> _sessions;
        public ObservableRangeCollection<Session> Sessions
        {
            get => _sessions;
            set => SetProperty(ref _sessions, value);
        }

        private ObservableRangeCollection<SessionGroup> _groupedSessions;
        public ObservableRangeCollection<SessionGroup> GroupedSessions
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
                    string target = string.Join(" ", new[] {
                        s.SessionTitle,
                        s.SessionDetails,
                        s.SessionID,
                        s.MainSpeaker.Company,
                        s.MainSpeaker.Name,
                        string.Join(" ", s.SubSpeakerList.Select(sub => $"{sub.Speaker.Company} {sub.Speaker.Name}")) }
                    );

                    return Check(target, searchWords);
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

        private readonly IDataService _dataService;

        public SessionListViewModel(IDataService dataService)
        {
            this._dataService = dataService;

            ShowSessionDetailsCommand = new AsyncCommand<string>(ShowSessionDetails);
        }

        internal async Task LoadSessions()
        {
            if (this.Sessions != null)
                return;

            try
            {
                var sessions = await _dataService.GetSessionDataAsync();
                this.Sessions = new ObservableRangeCollection<Session>(sessions);

                //Let's grouping
                this.GroupedSessions = new ObservableRangeCollection<SessionGroup>();
                this.GroupedSessions.AddRange(this.Sessions.GroupBy(s => s.TrackID)
                                                      .Select(g => new SessionGroup(g.Key, g.FirstOrDefault()?.TrackName, g.ToList())));
            }
            catch
            {
                throw;
            }
        }



        private readonly string excludePrefix = "-";
        private bool Check(string target, IEnumerable<string> searchWords)
        {
            foreach (var word in searchWords)
            {
                //-がついていた場合
                if (word.StartsWith(excludePrefix))
                {
                    string w = word.Substring(excludePrefix.Length);
                    if (target.Contains(w, StringComparison.OrdinalIgnoreCase))
                    {
                        return false;
                    }

                    continue;
                }

                if (!target.Contains(word, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            return true;
        }
    }

    public class SessionGroup : List<Session>
    {
        public string TrackID { get; private set; }
        public string TrackName { get; private set; }

        public SessionGroup(string trackId, string trackName, List<Session> sessions) : base(sessions)
        {
            TrackID = trackId;
            TrackName = trackName;
        }
    }
}
