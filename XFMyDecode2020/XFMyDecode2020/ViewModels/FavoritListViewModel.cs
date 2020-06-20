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

namespace XFMyDecode2020.ViewModels
{
    public class FavoritListViewModel : BaseViewModel
    {
        private MvvmHelpers. ObservableRangeCollection<Session> _sessions;
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
                    string target = string.Join(" ", new[] {
                        s.SessionTitle,
                        s.SessionDetails,
                        s.SessionID,
                        s.MainSpeaker.Company,
                        s.MainSpeaker.Name,
                        string.Join(" ", s.SubSpeakerList.Select(sub => $"{sub.Speaker.Company} {sub.Speaker.Name}")) }
                    );

                    return Utility.CheckIfContainSearchWord(s, searchWords);
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

        public FavoritListViewModel(IDataService dataService)
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
                var sessions = (await _dataService.GetSessionDataAsync());
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
