using System.Text.Json;
using System;
using System.Reflection;
using GenerateSessionData.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.IO;
using GenerateSessionData.Models.Official;

namespace GenerateSessionData
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var assembly = Assembly.GetExecutingAssembly();

            using var streamSessionDataOfficial = assembly.GetManifestResourceStream("GenerateSessionData.Data.SessionData_from_Official.json");
            using var streamSessionDataTY = assembly.GetManifestResourceStream("GenerateSessionData.Data.SessionData_by_TaikiYoshida.json");

            var sessionsOfficial = (await JsonSerializer.DeserializeAsync<SessionDataOfficial>(streamSessionDataOfficial)).SessionList.Select(sl => sl.Session);
            var sessionsTY = await JsonSerializer.DeserializeAsync<GenerateSessionData.Models.TY.Session[]>(streamSessionDataTY);

            foreach (var session in sessionsOfficial)
            {
                string videoId = sessionsTY.FirstOrDefault(s => s.code == session.SessionID)?.id;
                session.SessionVideoURL = $"https://decode20-vevent.cloud-config.jp/session/{videoId}";
            }

            //output to file
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true,
            };

            string json = JsonSerializer.Serialize(sessionsOfficial, options);
            string outputFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "SessionData.json");
            await File.WriteAllTextAsync(outputFilePath, json);
        }
    }
}
