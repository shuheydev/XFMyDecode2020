using System.Collections.Generic;
using System.Threading.Tasks;
using XFMyDecode2020.Models;

namespace XFMyDecode2020.Services
{
    public interface IDataService
    {
        Task<IEnumerable<Session>> GetSessionDataAsync();

        Task ResetAsync();

        void Save();

        Session FindSessionById(string sessionId);
    }
}
