using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace GetYoutubeUrl.Services
{
    public class DecodeScraper : IDecodeScraper
    {
        private readonly HttpClient _httpClient;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public DecodeScraper(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient(Settings.HttpClientName);
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public async Task<string> GetSearchResultHtmlAsync(string sessionID)
        {
            var response = await _httpClient.GetAsync($"results?search_query=de%3Acode+セッション+{sessionID}");
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        public async Task<IHtmlDocument> ParseHtmlDocumentAsync(string htmlString)
        {
            var parser = new HtmlParser();

            var token = _cancellationTokenSource.Token;
            var doc = await parser.ParseDocumentAsync(htmlString, token).ConfigureAwait(false);

            return doc;
        }

        private Regex _reFirstSession = new Regex(@"\{""webCommandMetadata"":\{""url"":""(/watch\?v=.+?)""", RegexOptions.Compiled);
        public string GetFirstSession(string html)
        {
            var result = _reFirstSession.Match(html);
            var subUrlString = result.Groups[1].Value;

            var baseUrl = new Uri(Settings.BaseURL);
            var fullUrl = new Uri(baseUrl, subUrlString);
            return fullUrl.ToString();
        }


        public async Task<string> GetVideoUrlAsync(string sessionId)
        {
            var html = await GetSearchResultHtmlAsync(sessionId).ConfigureAwait(false);
            var videoUrl = GetFirstSession(html);

            return videoUrl;
        }
    }
}
