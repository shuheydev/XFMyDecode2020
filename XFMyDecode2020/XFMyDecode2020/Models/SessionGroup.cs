﻿using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace XFMyDecode2020.Models
{
    public class SessionGroup : ObservableRangeCollection<Session>
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
