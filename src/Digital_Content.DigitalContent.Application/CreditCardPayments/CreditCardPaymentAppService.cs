using Abp.Domain.Repositories;
using Abp.UI;
using Digital_Content.DigitalContent.CreditCardPayments.CreditCardDto;
using Digital_Content.DigitalContent.Wallets;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Linq.Extensions;
using Abp.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using Digital_Content.DigitalContent.Wallets.TransationsDto;
using Abp.Application.Services.Dto;
using Digital_Content.DigitalContent.Authorization.Users;
using Digital_Content.DigitalContent.Currencies;

namespace Digital_Content.DigitalContent.CreditCardPayments
{
    public class CreditCardPaymentAppService : DigitalContentAppServiceBase, ICreditCardPaymentAppService
    {
        public static string StripeApiKey { get; set; }
        private readonly IRepository<CreditCardPayment, long> _creditCardPaymentRepository;
        private readonly IRepository<Transaction, long> _transactionRepository;
        private readonly IUserManager _userManager;
        private readonly IRepository<User, long> _userRepository;
        private readonly ICurrenciesRepository _currenciesRepository;



        public CreditCardPaymentAppService(IRepository<CreditCardPayment, long> creditCardPaymentRepository,
                                           IRepository<Transaction, long> transactionRepository,
                                            IUserManager userManager,
                                            IRepository<User, long> userRepository,
            ICurrenciesRepository currenciesRepository)
        {
            _creditCardPaymentRepository = creditCardPaymentRepository;
            _transactionRepository = transactionRepository;
            _userManager = userManager;
            _userRepository = userRepository;
            _currenciesRepository = currenciesRepository;
        }

        public async Task<PaymentResponceDto> SubmitPaymentAsync(CreditPayment body)
        {
            try
            {
                var currency = await _currenciesRepository.FirstOrDefaultAsync(body.UserData.UserPaymentData.CurrencyId);

                if (currency == null)
                {
                    throw new UserFriendlyException(L("CurrencyNotFound"));
                }

                StripeConfiguration.SetApiKey(StripeApiKey);


                var chargeOptions = new StripeChargeCreateOptions()
                {
                    Amount = Convert.ToInt32(body.UserData.UserPaymentData.Amount) * 100,
                    //TODO old Currency = body.UserData.UserPaymentData.CurrentType,
                    Currency = currency.Code,
                    Description = "Charge for " + body.UserData.FirstName + " " + body.UserData.LastName,
                    SourceTokenOrExistingSourceId = body.PaymentData.Id
                };

                var transaction = _transactionRepository.GetAll().LastOrDefault(tran => tran.AmountDue == body.UserData.UserPaymentData.Amount &&
                tran.TransactionStatus == TransactionStatusEnum.Commited &&
                tran.UserId == body.UserData.UserPaymentData.UserId);

                var chargeService = new StripeChargeService();
                StripeCharge charge = chargeService.Create(chargeOptions);
                if (charge.Status.Equals("succeeded"))
                {
                    if (transaction != null)
                    {
                        transaction.TransactionStatus = TransactionStatusEnum.Paid;
                        await _transactionRepository.UpdateAsync(transaction);
                    }
                    await SavePaymentDataAsync(charge, body.UserData.UserPaymentData.UserId);
                }
                return new PaymentResponceDto
                {
                    Status = charge.Status
                };
            }
            catch (StripeException ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        private async Task SavePaymentDataAsync(StripeCharge charge, long userId)
        {
            try
            {
                var creditCardPayment = new CreditCardPayment()
                {
                    CreditCardUserId = userId,
                    Amount = charge.Amount,
                    BalanceTransactionId = charge.BalanceTransactionId,
                    CreationTime = DateTime.Now,
                    Currency = charge.Currency,
                    Description = charge.Description,
                    ChargeId = charge.Id,
                    Status = charge.Status,
                    Outcome = new PaymentOutcome()
                    {
                        NetworkStatus = charge.Outcome.NetworkStatus,
                        RiskLevel = charge.Outcome.RiskLevel,
                        SellerMessage = charge.Outcome.SellerMessage,
                        Type = charge.Outcome.Type
                    },
                    Source = new PaymentSource()
                    {
                        SourceId = charge.Source.Id,
                        Card = new CreditCard()
                        {
                            Brand = charge.Source.Card.Brand,
                            Country = charge.Source.Card.Country,
                            ExpirationMonth = charge.Source.Card.ExpirationMonth,
                            ExpirationYear = charge.Source.Card.ExpirationYear,
                            FingerPrint = charge.Source.Card.Fingerprint,
                            Funding = charge.Source.Card.Funding,
                            CardId = charge.Source.Card.Id,
                            Last4 = charge.Source.Card.Last4
                        }
                    }
                };
                var paymentId = await _creditCardPaymentRepository.InsertAndGetIdAsync(creditCardPayment);
                if (paymentId == 0)
                    throw new NullReferenceException("Save to data base was failed");
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<PagedResultDto<TransactionDto>> GetAllTransactions(GetTransactionInput input)
        {
            try
            {
                var user = await _userRepository.FirstOrDefaultAsync(item => item.Id == AbpSession.UserId);
                var role = await _userManager.GetRolesAsync(user);
                IQueryable<Transaction> query;
                if (role.Contains("Admin"))
                {
                    query = _transactionRepository.GetAll()
                    .WhereIf(
                    !input.WalletId.IsNullOrWhiteSpace(),
                    t =>
                        t.WalletId.Contains(input.WalletId)
                    ).WhereIf(input.IsRescind == false, item => item.TransactionStatus != TransactionStatusEnum.Rescind);
                }
                else
                {
                    query = _transactionRepository.GetAll()
                    .WhereIf(
                    !input.WalletId.IsNullOrWhiteSpace(),
                    t =>
                        t.WalletId.Contains(input.WalletId)
                    )
                    .Where(item => item.UserId == user.Id)
                    .WhereIf(input.IsRescind == false, item => item.TransactionStatus != TransactionStatusEnum.Rescind);
                }

                var currenciesQueryable = _currenciesRepository.GetAll();

                var a = query.Join(currenciesQueryable, t => t.CurrencyId, c => c.Id, (t, c) =>
          new { Transaction = t, Currency = c }).Select(o => Mapper(o.Transaction, o.Currency));

                var transactionsCount = await a.CountAsync();
                var transactions = new List<TransactionDto>();
                if (input.Sorting == string.Empty)
                {
                    transactions = await a
                   .OrderByDescending(item => item.Id)
                   .PageBy(input)
                   .ToListAsync();
                }
                else
                {
                    transactions = await a
                   .OrderBy(input.Sorting)
                   .PageBy(input)
                   .ToListAsync();
                }

                //temp
                //var transactionListDtos = ObjectMapper.Map<List<TransactionDto>>(transactions);

                return new PagedResultDto<TransactionDto>(
                transactionsCount,
                transactions
                );
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task RescindCommitment(TransactionDto commitment)
        {
            try
            {
                var commitmentUp = await _transactionRepository.FirstOrDefaultAsync(item => item.Id == commitment.Id);
                if (commitmentUp != null)
                {
                    commitmentUp.TransactionStatus = TransactionStatusEnum.Rescind;
                    await _transactionRepository.UpdateAsync(commitmentUp);
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException();
            }
        }

        //temp
        private TransactionDto Mapper(Transaction transaction, Currency currency)
        {
            return new TransactionDto
            {
                Id = transaction.Id,

                UserId = transaction.UserId,

                WalletId = transaction.WalletId,

                AmountDue = transaction.AmountDue.ToString(),

                Currency = currency,

                Address = transaction.Address,

                CreationTime = transaction.CreationTime,

                TransactionStatus = transaction.TransactionStatus.ToString(),

                IsDeleted = transaction.IsDeleted,

                IsActive = transaction.IsActive,

                FirstName = transaction.FirstName,

                LastName = transaction.LastName,

                TransactionDate = transaction.TransactionDate,

                TokensIssued = transaction.TokensIssued,

                PaymentAmount = transaction.PaymentAmount,

                EmailAddress = transaction.EmailAddress,
            };
        }
    }
}
