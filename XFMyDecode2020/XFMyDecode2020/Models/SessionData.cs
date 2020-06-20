using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace XFMyDecode2020.Models
{

    public class Rootobject
    {
        public Session[] Property1 { get; set; }
    }

    public class Session : ObservableObject
    {
        public Language Language { get; set; }
        public Mainspeaker MainSpeaker { get; set; }
        public Reflink[] RefLinks { get; set; }
        public string SessionDetails { get; set; }
        public string SessionID { get; set; }
        public string SessionLevel { get; set; }
        public string SessionLevelTitle { get; set; }
        public string SessionTitle { get; set; }
        public string SessionURL { get; set; }
        public Subspeakerlist[] SubSpeakerList { get; set; }
        public Targetlist[] TargetList { get; set; }
        public Topiclist[] TopicList { get; set; }
        public string TrackID { get; set; }
        public string TrackName { get; set; }
        public string SessionVideoURL { get; set; }
        private bool _isFavorit;
        public bool IsFavorit
        {
            get => _isFavorit;
            set => SetProperty(ref _isFavorit, value);
        }
        private bool _isWatched;
        public bool IsWatched
        {
            get => _isWatched;
            set => SetProperty(ref _isWatched, value);
        }
    }

    public class Language
    {
        public string Title { get; set; }
        public string id { get; set; }
    }

    public class Mainspeaker
    {
        public string Company { get; set; }
        public string Name { get; set; }
    }

    public class Reflink
    {
        public Link Link { get; set; }
    }

    public class Link
    {
        public string Description { get; set; }
        public string URL { get; set; }
    }

    public class Subspeakerlist
    {
        public Speaker Speaker { get; set; }
    }

    public class Speaker
    {
        public string Company { get; set; }
        public string Name { get; set; }
    }

    public class Targetlist
    {
        public Target Target { get; set; }
    }

    public class Target
    {
        public string Title { get; set; }
        public string id { get; set; }
    }

    public class Topiclist
    {
        public Topic Topic { get; set; }
    }

    public class Topic
    {
        public string Title { get; set; }
        public string id { get; set; }
    }
}
