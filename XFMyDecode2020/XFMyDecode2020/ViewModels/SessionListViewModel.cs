﻿using MvvmHelpers;
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

        public AsyncCommand<string> ShowSessionDetailsCommand { get; }

        private readonly IDataService _dataService;

        public SessionListViewModel(IDataService dataService)
        {
            this._dataService = dataService;

            ShowSessionDetailsCommand = new AsyncCommand<string>(ShowSessionDetails);
        }

        private async Task ShowSessionDetails(string sessionId)
        {
            await Shell.Current.GoToAsync($"sessionDetails?sessionId={sessionId}");
        }

        public async Task LoadSessions()
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

        private ObservableRangeCollection<SessionGroup> _groupedSessions;
        public ObservableRangeCollection<SessionGroup> GroupedSessions
        {
            get => _groupedSessions;
            set => SetProperty(ref _groupedSessions, value);
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
