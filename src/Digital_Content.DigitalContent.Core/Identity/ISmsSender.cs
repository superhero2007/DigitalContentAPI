using System.Threading.Tasks;

namespace Digital_Content.DigitalContent.Identity
{
    public interface ISmsSender
    {
        Task SendAsync(string number, string message);
    }
}