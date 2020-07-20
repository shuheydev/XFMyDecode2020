using GetYoutubeUrl.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading;
using System.Threading.Tasks;
using XFMyDecode2020.Models;

namespace GetYoutubeUrl
{
    class Program
    {
        static async Task Main(string[] args)
        {
            #region Add Services
            var services = new ServiceCollection();

            services.AddHttpClient(Settings.HttpClientName, c =>
            {
                c.BaseAddress = new Uri(Settings.BaseURL);
            });

            services.AddSingleton<IDecodeScraper, DecodeScraper>();
            #endregion

            //Create Service Provider
            var serviceProvider = services.BuildServiceProvider();

            //Get Scraper Instance
            var scraper = serviceProvider.GetService<IDecodeScraper>();

            #region Read old version Data from embedded resource
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream("GetYoutubeUrl.Data.SessionData.json");

            var sessions = await JsonSerializer.DeserializeAsync<IEnumerable<Session>>(stream);
            #endregion

            #region Over write SessionVideoUrl by new Url(Youtube)
            foreach (var session in sessions)
            {
                Console.WriteLine($"Session Id: {session.SessionID} : Start Processing");
                
                Console.WriteLine($"    Getting Url");

                var sessionId = session.SessionID;
                var videoUrl = await scraper.GetVideoUrlAsync(sessionId);

                Console.WriteLine($"    Got Url : {videoUrl}");

                Console.WriteLine($"    Over writing");

                session.SessionVideoURL = videoUrl;

                Console.WriteLine($"    Over writed by : {session.SessionVideoURL}");

                Console.WriteLine("    Interval for next request 1 sec.");

                await Task.Delay(TimeSpan.FromSeconds(1));

                Console.WriteLine($"Session Id: {session.SessionID} : Finish Processing");
            }
            #endregion

            #region Serialize and Output Data to file on desktop
            Console.WriteLine("Serializing");
            
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true,
            };
            var json = JsonSerializer.Serialize(sessions, options);

            var outputFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "SessionData_Youtube.json");
            
            Console.WriteLine($"Exporting to : {outputFilePath}");

            File.WriteAllText(outputFilePath, json);
            
            Console.WriteLine($"Exported");

            #endregion

            Console.WriteLine("Finished");
        }
    }
}
