using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using XFMyDecode2020.Models;
using XFMyDecode2020.Services;
using Xamarin.Forms.Xaml;
using System.Linq;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace XFMyDecode2020.Controls
{
    public class SessionSearchHandler : SearchHandler
    {
        private IEnumerable<Session> _sessions;

        public SessionSearchHandler()
        {
            _sessions = new Collection<Session>();
        }

        protected override async void OnQueryChanged(string oldValue, string newValue)
        {
            base.OnQueryChanged(oldValue, newValue);

            _sessions = await Startup.ServiceProvider.GetService<IDataService>().GetSessionDataAsync();

            if (string.IsNullOrWhiteSpace(newValue))
            {
                ItemsSource = null;
            }
            else
            {
                var searchWords = Regex.Split(newValue, @"\s+?").Where(w => !string.IsNullOrEmpty(w));

                //Let's filtering
                ItemsSource = _sessions.Where(s =>
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
                });
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

        protected override async void OnItemSelected(object item)
        {
            base.OnItemSelected(item);

            await Shell.Current.GoToAsync($"sessionDetails?sessionId={((Session)item).SessionID}");
        }
    }
}
