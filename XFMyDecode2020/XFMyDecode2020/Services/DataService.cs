using MonkeyCache;
using MonkeyCache.FileStore;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using XFMyDecode2020.Models;
using System.Text.Json;
using System.Linq;
using System.IO;
using Microsoft.Extensions.Primitives;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace XFMyDecode2020.Services
{
    public class DataService : IDataService
    {
        private IEnumerable<Session> _sessions;
        private string _filePath;

        public DataService()
        {
            _filePath = Path.Combine(FileSystem.AppDataDirectory, "Sessions.json");

            if (!File.Exists(_filePath))
            {
                Initialize();
            }
        }

        private void Initialize()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream("XFMyDecode2020.Data.SessionData.json");
            _sessions = JsonSerializer.DeserializeAsync<SessionData>(stream).Result.SessionList.Select(sl => sl.Session);

            Save();
        }

        public const string CacheSessionDataKey = "SessionDataKey";

        /// <summary>
        /// Get all session data.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Session>> GetSessionDataAsync()
        {
            if (_sessions == null)
            {
                var stream = File.OpenRead(_filePath);
                _sessions = await JsonSerializer.DeserializeAsync<IEnumerable<Session>>(stream);
            }

            return _sessions;
        }

        public void Reset()
        {
            Initialize();
        }

        public void Save()
        {
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(_sessions, options);

            File.WriteAllText(_filePath, json);
        }

        public Session FindSessionById(string sessionId)
        {
            return _sessions.FirstOrDefault(s => s.SessionID == sessionId);
        }
    }
}
