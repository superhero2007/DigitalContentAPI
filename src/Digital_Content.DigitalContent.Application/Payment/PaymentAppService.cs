using Abp.Domain.Repositories;
using Abp.UI;
using Digital_Content.DigitalContent.Authorization.Users;
using Digital_Content.DigitalContent.Currencies;
using Digital_Content.DigitalContent.Payment.Dto;
using Digital_Content.DigitalContent.Wallet;
using Digital_Content.DigitalContent.Wallets;
using System;
using System.Threading.Tasks;

namespace Digital_Content.DigitalContent.Payment
{
    public class PaymentAppService : DigitalContentAppServiceBase, IPaymentAppService
    {
        private readonly IRepository<Transaction, long> _transactionRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<Payments.Payment, long> _paymentRepository;
        private readonly IWalletAppService _walletAppService;
        private readonly ICurrenciesRepository _currenciesRepository;
        public PaymentAppService
            (IRepository<Transaction, long> transactionRepository,
             IRepository<User, long> userRepository,
             IRepository<Payments.Payment, long> paymentRepository,
             IWalletAppService  walletAppService,
             ICurrenciesRepository currenciesRepository)
        {
            _transactionRepository = transactionRepository;
            _userRepository = userRepository;
            _paymentRepository = paymentRepository;
            _walletAppService = walletAppService;
            _currenciesRepository = currenciesRepository;
        }
        //ToDo: AddPermission

        //[AbpAuthorize(AppPermissions.Pages_Administration_Users_Create)]
        public async Task CreatePaymentManually(PaymentInput input)
        {
            try
            {
                long? transactionId = null;
                //get latestUnpaidTransaction
                if (input.TransactionId != null)
                {
                    //Demo step
                    transactionId = (long)input.TransactionId;
                }
                else
                {
                    //maybe also partially paid 
                    var transactionByWaletId = await _transactionRepository.FirstOrDefaultAsync(x => x.WalletId == input.WalletId && x.TransactionStatus == TransactionStatusEnum.Commited);
                    if (transactionByWaletId != null)
                    {
                        transactionId = transactionByWaletId.Id;
                    }
                }
                if (transactionId == null)
                {
                    throw new UserFriendlyException("We are working on that feature");
                    // todo in case you have a user
                    //transactionId = _transactionRepository.InsertAndGetIdAsync(new Transaction() { })
                }
                var transaction = await _transactionRepository.GetAsync((long)transactionId);
                //create payment
                var payment = new Payments.Payment()
                {
                    AmountOfFunds = input.AmountOfFunds,
                    CreationTime = DateTime.UtcNow,
                    //TODO: old CurrencyType = (CurrentTypeEnum)(Enum.Parse(typeof(CurrentTypeEnum), input.CurrencyType)),
                    CurrencyId = input.CurrencyId,
                    PaymentReceivedTime = input.PaymentReceivedTime,
                    WalletId = transaction.WalletId,
                    TrackingNumber = input.TrackingNumber,
                    TenantId = AbpSession.TenantId
                };
            
                payment.PaymentTransaction.Add(new PaymentTransaction.PaymentTransaction() { PaymentId = payment.Id, TransactionId = transaction.Id });
                await _paymentRepository.InsertAsync(payment);
                //Convert currency and calculate Tokens Amount 
                if (transaction.UserId == null)
                {
                    throw new UserFriendlyException("There is no user in found transaction!");
                } 
                var currencies =  await _walletAppService.GetBtcEthValue();
                var currentCurency = new  Wallet.Dto.ExchageRateDto();

                var currency = await _currenciesRepository.FirstOrDefaultAsync(input.CurrencyId);

                if (currency == null)
                {
                    throw new UserFriendlyException(L("CurrencyNotFound"));
                }

                if (!currency.IsCrypto)
                {
                    currentCurency.Price = 1;
                }
                else
                {
                    currentCurency = currencies.Find(x => x.Currency.Id == input.CurrencyId);
                }

                decimal amountTokens = (decimal)(((decimal)currentCurency.Price * input.AmountOfFunds )/ 0.25M);
                var user = await _userRepository.GetAsync((long)transaction.UserId);
                user.AmountTokens += amountTokens;
                user.DcntTokensBalance += amountTokens;
                await _userRepository.UpdateAsync(user);
                if (amountTokens > transaction.TokensIssued)
                {
                    //pay for current transaction
                    transaction.TransactionStatus = TransactionStatusEnum.Paid;
                    transaction.TokensIssued = (long)amountTokens;
                    //search new unpaid transaction or create one  
                }
                else if (amountTokens == transaction.TokensIssued)
                {
                    //Demo step
                    transaction.TransactionStatus = TransactionStatusEnum.Paid;
                    transaction.TokensIssued = (long)amountTokens;
                }
                else if (amountTokens < transaction.TokensIssued)
                {
                    transaction.TransactionStatus = TransactionStatusEnum.PartiallyPaid;
                }
            }catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
    }
}
