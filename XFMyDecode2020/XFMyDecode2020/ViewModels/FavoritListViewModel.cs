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
    public class FavoritListViewModel : BaseViewModel
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
                                  .Select(g => new SessionGroup(g.Key, g.FirstOrDefault()?.TrackName, new MvvmHelpers.ObservableRangeCollection<Session>(g))));
        }

        public AsyncCommand<string> ShowSessionDetailsCommand { get; }
        private async Task ShowSessionDetails(string sessionId)
        {
            await Shell.Current.GoToAsync($"sessionDetails?sessionId={sessionId}");
        }

        public AsyncCommand<string> ChangeSelectedItemCommand { get; }
        private async Task ChangeSelectedItem(string sessionId)
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

                var group = this.GroupedSessions.FirstOrDefault(g => g.Any(s => s == session));
                group.Remove(session);

                //remove empty group
                if (!group.Any())
                {
                    this.GroupedSessions.Remove(group);
                }
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
            ChangeSelectedItemCommand = new AsyncCommand<string>(ChangeSelectedItem);
        }

        internal async Task LoadSessions()
        {
            try
            {
                if (this.Sessions is null)
                {
                }

                var sessions = (await _dataService.GetSessionDataAsync());
                var removeTargets = sessions.Where(s => this.Sessions.Contains(s) && !s.IsFavorit).ToList();
                var addTargets = sessions.Where(s => !this.Sessions.Contains(s) && s.IsFavorit).ToList();

                this.Sessions.RemoveRange(removeTargets);
                this.Sessions.AddRange(addTargets);


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
                        this.GroupedSessions.Add(new SessionGroup(session.TrackID, session.TrackName, new MvvmHelpers.ObservableRangeCollection<Session>()));
                    }

                    this.GroupedSessions.FirstOrDefault(g => g.TrackID == session.TrackID)?.Add(session);
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
