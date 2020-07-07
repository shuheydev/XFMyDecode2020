using System;
using System.Collections.Generic;
using System.Linq;
using XFMyDecode2020.Models;

namespace XFMyDecode2020.Utilities
{
    public static class Utility
    {
        private static readonly string excludePrefix = "-";
        public static bool CheckIfContainSearchWord(Session s, IEnumerable<string> searchWords)
        {
            string target = string.Join(" ", new[] {
                        s.SessionTitle,
                        s.SessionDetails,
                        $"#{s.SessionID}",
                        s.MainSpeaker.Company,
                        s.MainSpeaker.Name,
                        string.Join(" ", s.SubSpeakerList.Select(sub => $"{sub.Speaker.Company} {sub.Speaker.Name}")) }
                        );

            foreach (var word in searchWords)
            {
                if (word.StartsWith(excludePrefix))
                {
                    string w = word.Substring(excludePrefix.Length);

                    if (CheckFav(w))
                    {
                        if (s.IsFavorit)
                            return false;
                    }
                    else
                    {
                        if (target.Contains(w, StringComparison.OrdinalIgnoreCase))
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    if (CheckFav(word))
                    {
                        if (!s.IsFavorit)
                            return false;
                    }
                    else
                    {
                        if (!target.Contains(word, StringComparison.OrdinalIgnoreCase))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

       private static bool CheckFav(string w)
        {
            return (w.Equals("★") || w.Equals("☆") || w.Equals("fav:") || w.Equals("like:")||w.Equals("star:"));
        }
    }
}
