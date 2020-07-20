using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using GetYoutubeUrl;
using GetYoutubeUrl.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;

namespace Test_GetYoutubeUrl
{
    public static class Setup
    {
        public static ServiceProvider ServiceProvider { get; private set; }

        public static void Init()
        {
            if (ServiceProvider != null)
                return;

            var services = new ServiceCollection();

            services.AddHttpClient(Settings.HttpClientName, c =>
            {
                c.BaseAddress = new Uri(Settings.BaseURL);
            });

            services.AddSingleton<IDecodeScraper, DecodeScraper>();

            ServiceProvider = services.BuildServiceProvider();
        }
    }

    public class Test_DecodeScraper
    {
        private readonly IDecodeScraper _scraper;
        private const string _sessionID = "A03";

        public Test_DecodeScraper()
        {
            Setup.Init();

            var decodeScraper = Setup.ServiceProvider;
            this._scraper = decodeScraper.GetService<IDecodeScraper>();
        }

        private string _html;
        private IHtmlDocument _doc;

        private async Task Init()
        {
            _html ??= await _scraper.GetSearchResultHtmlAsync(_sessionID);
            _doc ??= await _scraper.ParseHtmlDocumentAsync(_html);
        }

        [Fact(DisplayName = "対象のページのHTMLを取得できること")]
        public async Task Test_GetHtmlString()
        {
            var html = await _scraper.GetSearchResultHtmlAsync(_sessionID);

            Assert.True(!string.IsNullOrEmpty(html));
            Assert.Contains("div", html);
        }

        [Fact(DisplayName = "HTML文字列をパースできること")]
        public async Task Test_ParseHtmlString()
        {
            await Init();
            Assert.NotNull(_doc);
        }

        [Fact(DisplayName = "Htmlから所望のURLを返すこと")]
        public async Task Test_GetSessionInfo()
        {
            await Init();

            var videoUrl = _scraper.GetFirstSession(_html);

            Assert.Matches(new Regex(@$"{Settings.BaseURL}watch\?v=.+?"),videoUrl);
        }

        [Fact(DisplayName ="指定したSessionIdの動画URLを返すこと")]
        public async Task Test_GetVideoUrl()
        {
            var videoUrl = await _scraper.GetVideoUrlAsync("A04");
            Assert.Matches(new Regex(@$"{Settings.BaseURL}watch\?v=.+?"), videoUrl);
        }
    }
}
