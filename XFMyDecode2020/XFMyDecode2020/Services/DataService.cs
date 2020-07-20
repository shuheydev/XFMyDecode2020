using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;
using Xamarin.Essentials;
using XFMyDecode2020.Models;

namespace XFMyDecode2020.Services
{
    public class DataService : IDataService
    {
        private readonly string _fileName = "Sessions.json";
        //YoutubeのUrlに更新済みかどうか
        private readonly string _usingYoutubeUrlKey = "UsingYoutubeUrl";

        private IEnumerable<Session> _sessions;
        private readonly string _filePath;

        public DataService()
        {
            _sessions = new Collection<Session>();
            _filePath = Path.Combine(FileSystem.AppDataDirectory, _fileName);
        }

        private async Task Initialize()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream("XFMyDecode2020.Data.SessionData.json");

            _sessions = await JsonSerializer.DeserializeAsync<IEnumerable<Session>>(stream).ConfigureAwait(false);

            Save();
        }

        public const string CacheSessionDataKey = "SessionDataKey";

        /// <summary>
        /// Get all session data.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Session>> GetSessionDataAsync()
        {
            if (this._sessions.Any())
            {
                return this._sessions;
            }

            try
            {
                if (!File.Exists(_filePath))
                {
                    await Initialize().ConfigureAwait(false);
                    Preferences.Set(_usingYoutubeUrlKey, true);

                    return this._sessions;
                }

                using var stream = File.OpenRead(_filePath);
                _sessions = await JsonSerializer.DeserializeAsync<IEnumerable<Session>>(stream).ConfigureAwait(false);

                //de:code2020のセッション動画がYoutubeで公開されたことへの対応.
                //YoutubeのUrlで上書きする.
                if (Preferences.Get(_usingYoutubeUrlKey, false) == false)
                {
                    var assembly = Assembly.GetExecutingAssembly();
                    using var streamFromEmbedded = assembly.GetManifestResourceStream("XFMyDecode2020.Data.SessionData.json");
                    var sessionsEmbedded = await JsonSerializer.DeserializeAsync<IEnumerable<Session>>(streamFromEmbedded).ConfigureAwait(false);

                    foreach (var session in _sessions)
                    {
                        var sessionEmbedded = sessionsEmbedded.First(s => s.SessionID == session.SessionID);
                        session.SessionVideoURL = sessionEmbedded.SessionVideoURL;
                    }

                    Preferences.Set(_usingYoutubeUrlKey, true);
                }
            }
            catch
            {
                await Initialize().ConfigureAwait(false);
                Preferences.Set(_usingYoutubeUrlKey, true);
            }

            return _sessions;
        }

        public async Task ResetAsync()
        {
            await Initialize();
        }

        public void Save()
        {
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };

            string json = JsonSerializer.Serialize(_sessions, options);

            File.WriteAllText(_filePath, json);
        }

        public Session FindSessionById(string sessionId)
        {
            return _sessions.FirstOrDefault(s => s.SessionID == sessionId);
        }
    }
}
