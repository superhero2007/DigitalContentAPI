using System.Threading.Tasks;
using Abp.Application.Services;
using Digital_Content.DigitalContent.MultiTenancy.Dto;
using Digital_Content.DigitalContent.MultiTenancy.Payments.Dto;
using Abp.Application.Services.Dto;

namespace Digital_Content.DigitalContent.MultiTenancy.Payments
{
    public interface IPaymentAppService : IApplicationService
    {
        Task<PaymentInfoDto> GetPaymentInfo(PaymentInfoInput input);

        Task<CreatePaymentResponse> CreatePayment(CreatePaymentDto input);

        Task<ExecutePaymentResponse> ExecutePayment(ExecutePaymentDto input);

        Task<PagedResultDto<SubscriptionPaymentListDto>> GetPaymentHistory(GetPaymentHistoryInput input);
    }
}
