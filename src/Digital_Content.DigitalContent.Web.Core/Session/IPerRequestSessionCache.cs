using System.Threading.Tasks;
using Digital_Content.DigitalContent.Sessions.Dto;

namespace Digital_Content.DigitalContent.Web.Session
{
    public interface IPerRequestSessionCache
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformationsAsync();
    }
}
