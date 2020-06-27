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
using System.Collections.ObjectModel;

namespace XFMyDecode2020.Services
{
    public class DataService : IDataService
    {
        private readonly string _fileName = "Sessions.json";

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

            _sessions = await JsonSerializer.DeserializeAsync<IEnumerable<Session>>(stream);

            Save();
        }

        public const string CacheSessionDataKey = "SessionDataKey";

        /// <summary>
        /// Get all session data.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Session>> GetSessionDataAsync()
        {
            try
            {
                using var stream = File.OpenRead(_filePath);
                _sessions = await JsonSerializer.DeserializeAsync<IEnumerable<Session>>(stream);
            }
            catch
            {
                await Initialize();
            }

            return _sessions;
        }

        public async Task Reset()
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

            var json = JsonSerializer.Serialize(_sessions, options);

            File.WriteAllText(_filePath, json);

        }

        public Session FindSessionById(string sessionId)
        {
            return _sessions.FirstOrDefault(s => s.SessionID == sessionId);
        }
    }
}
