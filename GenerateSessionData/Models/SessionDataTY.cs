using System;
using System.Collections.Generic;
using System.Text;

namespace GenerateSessionData.Models.TY
{
    public class Rootobject
    {
        public Session[] Property1 { get; set; }
    }

    public class Session
    {
        public string id { get; set; }
        public string code { get; set; }
        public int publishtime { get; set; }
        public string title { get; set; }
        public string thumbnail { get; set; }
        public string status { get; set; }
        public Track track { get; set; }
        public Topic[] topics { get; set; }
        public Level level { get; set; }
        public Target[] targets { get; set; }
        public Language language { get; set; }
        public int visitors { get; set; }
        public int views { get; set; }
        public Day day { get; set; }
    }

    public class Track
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Level
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Language
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Day
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Topic
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Target
    {
        public string id { get; set; }
        public string name { get; set; }
    }

}
