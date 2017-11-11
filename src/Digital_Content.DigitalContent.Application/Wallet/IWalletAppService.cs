using Abp.Application.Services;
using Digital_Content.DigitalContent.ExchangeRates;
using Digital_Content.DigitalContent.Wallet.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Content.DigitalContent.Wallet
{
    public interface IWalletAppService: IApplicationService
    {
        Task<PaymentOutput> PurchaseTokens(WalletLogsDto walletLogs);

        Task<List<ExchageRateDto>> GetBtcEthValue();
    }
}
