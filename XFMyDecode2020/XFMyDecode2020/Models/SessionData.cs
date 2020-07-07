using MvvmHelpers;
using System.Collections.ObjectModel;

namespace XFMyDecode2020.Models
{
    public class Rootobject
    {
        public Collection<Session> Sessions { get; set; } = new Collection<Session>();
    }

    public class Session : ObservableObject
    {
        public Language Language { get; set; } = new Language();
        public Mainspeaker MainSpeaker { get; set; } = new Mainspeaker();
        public Collection<Reflink> RefLinks { get; set; } = new Collection<Reflink>();
        public string SessionDetails { get; set; } = string.Empty;
        public string SessionID { get; set; } = string.Empty;
        public string SessionLevel { get; set; } = string.Empty;
        public string SessionLevelTitle { get; set; } = string.Empty;
        public string SessionTitle { get; set; } = string.Empty;
        public string SessionURL { get; set; } = string.Empty;
        public Collection<Subspeakerlist> SubSpeakerList { get; set; } = new Collection<Subspeakerlist>();
        public Collection<Targetlist> TargetList { get; set; } = new Collection<Targetlist>();
        public Collection<Topiclist> TopicList { get; set; } = new Collection<Topiclist>();
        public string TrackID { get; set; } = string.Empty;
        public string TrackName { get; set; } = string.Empty;
        public string SessionVideoURL { get; set; } = string.Empty;
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
        public string Title { get; set; } = string.Empty;
        public string id { get; set; } = string.Empty;
    }

    public class Mainspeaker
    {
        public string Company { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    public class Reflink
    {
        public Link Link { get; set; } = new Link();
    }

    public class Link
    {
        public string Description { get; set; } = string.Empty;
        public string URL { get; set; } = string.Empty;
    }

    public class Subspeakerlist
    {
        public Speaker Speaker { get; set; } = new Speaker();
    }

    public class Speaker
    {
        public string Company { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    public class Targetlist
    {
        public Target Target { get; set; } = new Target();
    }

    public class Target
    {
        public string Title { get; set; } = string.Empty;
        public string id { get; set; } = string.Empty;
    }

    public class Topiclist
    {
        public Topic Topic { get; set; } = new Topic();
    }

    public class Topic
    {
        public string Title { get; set; } = string.Empty;
        public string id { get; set; } = string.Empty;
    }
}
