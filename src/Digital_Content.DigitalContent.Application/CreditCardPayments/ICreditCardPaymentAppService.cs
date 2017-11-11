using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Digital_Content.DigitalContent.CreditCardPayments.CreditCardDto;
using Digital_Content.DigitalContent.Wallets;
using Digital_Content.DigitalContent.Wallets.TransationsDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Content.DigitalContent.CreditCardPayments
{
    public interface ICreditCardPaymentAppService: IApplicationService
    {
        Task<PaymentResponceDto> SubmitPaymentAsync(CreditPayment body);

        Task<PagedResultDto<TransactionDto>> GetAllTransactions(GetTransactionInput input);

        Task RescindCommitment(TransactionDto commitment);
    }
}
