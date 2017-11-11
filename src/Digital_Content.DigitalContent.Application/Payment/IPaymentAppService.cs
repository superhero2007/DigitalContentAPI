using Abp.Application.Services;
using Digital_Content.DigitalContent.Payment.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Content.DigitalContent.Payment
{
    public interface IPaymentAppService : IApplicationService
    { 
        Task CreatePaymentManually(PaymentInput input);
    }
}
