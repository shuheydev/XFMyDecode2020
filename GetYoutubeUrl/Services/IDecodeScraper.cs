using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GetYoutubeUrl.Services
{
    public interface IDecodeScraper
    {
        Task<string> GetSearchResultHtmlAsync(string sessionID);
        Task<IHtmlDocument> ParseHtmlDocumentAsync(string htmlString);
        string GetFirstSession(string html);
        Task<string> GetVideoUrlAsync(string sessionId);
    }
}
