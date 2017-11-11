using System.Threading.Tasks;
using Abp.Domain.Policies;

namespace Digital_Content.DigitalContent.Authorization.Users
{
    public interface IUserPolicy : IPolicy
    {
        Task CheckMaxUserCountAsync(int tenantId);
    }
}
