using Abp.Authorization;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Net.Mail;
using Abp.UI;
using Amazon.S3;
using Amazon.S3.Transfer;
using Digital_Content.DigitalContent.Authorization;
using Digital_Content.DigitalContent.Authorization.Users;
using Digital_Content.DigitalContent.Configuration.Host;
using Digital_Content.DigitalContent.Configuration.Host.Dto;
using Digital_Content.DigitalContent.Configuration.Tenants;
using Digital_Content.DigitalContent.Configuration.Tenants.Dto;
using Digital_Content.DigitalContent.Currencies;
using Digital_Content.DigitalContent.Currencies.Dto;
using Digital_Content.DigitalContent.Dto;
using Digital_Content.DigitalContent.ExchangeRates;
using Digital_Content.DigitalContent.MultiTenancy;
using Digital_Content.DigitalContent.Wallet.Dto;
using Digital_Content.DigitalContent.Wallets;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Content.DigitalContent.Wallet
{

    
    public class WalletAppService: DigitalContentAppServiceBase, IWalletAppService
    {
        private readonly IRepository<Transaction, long> _walletLogsRepository;
        private readonly IEmailSender _emailSender;
        private readonly IRepository<User, long> _userRepository;
        private readonly IUserEmailer _userEmailer;
        private readonly IHostSettingsAppService _hostSettingsAppService;
        private readonly ISettingManager _settingManager;
        private readonly ITenantSettingsAppService _tenantSettingService;
        private const decimal _dcntInUsd = 17.94M;
        private readonly IRepository<ExchangeRate, long> _exchangeRateRepository; 
        private readonly IRepository<SupportedCurrency, long> _supportedCurrencyRepository;
        private readonly ICurrenciesRepository _currenciesRepository;
        private readonly IExchangeRatesManager _exchangeManager;
        private readonly IRepository<Tenant> _tenantRepository;

        IUnitOfWorkManager _unitOfWorkManager;

        private string _accessUrl = "https://s3-us-west-2.amazonaws.com/sw-currencies/";

        public WalletAppService(IRepository<Transaction, long> walletLogsRepository,
                                IEmailSender emailSender,
                                IRepository<User, long> userRepository,
                                IUserEmailer userEmailer,
                                IHostSettingsAppService hostSettingsAppService,
                                ISettingManager settingManager,
                                ITenantSettingsAppService tenantSettingService,
                                IRepository<ExchangeRate, long> exchangeRateRepository,
            ICurrenciesRepository currenciesRepository,
            IRepository<SupportedCurrency, long> supportedCurrencyRepository, IExchangeRatesManager exchangeManager,
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<Tenant> tenantRepository)

        {
            _walletLogsRepository = walletLogsRepository;
            _emailSender = emailSender;
            _userRepository = userRepository;
            _userEmailer = userEmailer;
            _hostSettingsAppService = hostSettingsAppService;
            _settingManager = settingManager;
            _tenantSettingService = tenantSettingService;
            _exchangeRateRepository = exchangeRateRepository;
            _currenciesRepository = currenciesRepository;
            _supportedCurrencyRepository = supportedCurrencyRepository;
            _exchangeManager = exchangeManager;
            _unitOfWorkManager = unitOfWorkManager;
            _tenantRepository = tenantRepository;
        }

        public async Task<PaymentOutput> PurchaseTokens(WalletLogsDto input)
        {
            try
            {
                if (input != null)
                {
                    var user = _userRepository.Get(input.UserId);
                    

                    var dcnt = input.Amount;
                    var dollar = dcnt * _dcntInUsd;
                    var dcntInBtcOrEthOrUsd = dollar;

                    var currency = await _currenciesRepository.FirstOrDefaultAsync(input.CurrencyId);
                    if (!currency.IsCrypto)
                    {
                        var users = await UserManager.GetUsersInRoleAsync("Admin");
                        var instruction = users.FirstOrDefault(item => item.WiredInstruction != null).WiredInstruction;
                        return new PaymentOutput()
                        {
                            Instruction = instruction
                        };
                    }
                    if (currency == null)
                    {
                        throw new UserFriendlyException(L("CurrencyNotFound"));
                    }

                    var exchangeRate = await _exchangeRateRepository.FirstOrDefaultAsync(e => e.CurrencyId.Equals(currency.Id));
                    if (exchangeRate == null)
                    {
                        throw new UserFriendlyException(L("CurrencyNotFound"));
                    }

                    if (currency.IsCrypto)
                    {
                        if (Convert.ToDouble(input.Amount) != 0.0)
                        {
                            dcntInBtcOrEthOrUsd = (_dcntInUsd * input.Amount) / exchangeRate.Price;
                        }
                    }

                    var transaction = new Transaction()
                    {
                        UserId = input.UserId,
                        AmountDue = dcntInBtcOrEthOrUsd,
                        //TODO: old CurrencyType = (int)Enum.Parse(typeof(CurrentTypeEnum), input.CurrentType),
                        WalletId = input.WalletId,
                        CurrencyId = currency.Id,
                        TransactionStatus = TransactionStatusEnum.Commited,
                        IsDeleted = false,
                        FirstName = user.Name,
                        LastName = user.Surname,
                        TokensIssued = (long)input.Amount,
                        EmailAddress = user.EmailAddress, 
                        TransactionDate = DateTime.Now,
                        TenantId =  AbpSession.TenantId 
                    }; 
                    await _walletLogsRepository.InsertAsync(transaction);
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    var walletId = (await _supportedCurrencyRepository.FirstOrDefaultAsync(x => x.CurrencyId == currency.Id)).WalletId;
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(walletId, QRCodeGenerator.ECCLevel.Q);
                    QRCode qrCode = new QRCode(qrCodeData);
                    Bitmap qrCodeImage = qrCode.GetGraphic(4);
                    System.IO.MemoryStream ms = new System.IO.MemoryStream();
                    qrCodeImage.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                    byte[] byteImage = ms.ToArray();
                    var qrCodeBase64 = Convert.ToBase64String(byteImage);

                    var qrcodeDto = new PaymentOutput()
                    {
                        QrCode = qrCodeBase64,
                        Address = walletId
                    };

                    var tenant = await _tenantRepository.FirstOrDefaultAsync(item => item.Id == AbpSession.TenantId);
                    var currencyImage = StoreClientAgreement(ms, currency.Name);
                    //TODO email
                    //Uri uri = new Uri(tenant.LogoUrl);
                    var emailData = new DcntEmailTemplateInput()
                    {
                        UserName = user.Name,
                        WalletId = walletId,
                        Amount = input.Amount,
                        CurrentType = currency.Name,
                        Dcnt = dcnt,
                        Dollar = dollar,
                        Uri = tenant.LogoUrl,
                        DcntInBtcOrEthOrUsd = dcntInBtcOrEthOrUsd,
                        QrCode = currencyImage
                    };
                    
                    var receiver = new MailAddress(user.EmailAddress, user.FullName);
                    var body = _userEmailer.GetDCEmailTemplate(emailData);
                    await _userEmailer.SendEmailWithSendGrid(receiver, body);
                    return qrcodeDto;

                }
            }
            catch(Exception ex)
            {

            }
            return new PaymentOutput() ;
        }

        private string StoreClientAgreement(Stream image, string agreementDocumentKey)
        {
            try
            {
                var fileTransferUtility =
                    new TransferUtility(new AmazonS3Client("AKIAIE7PXSOKFEUVFLSA",
                        "tF9U+XFDaxuYvBJJlblDpA+2q33vbVP/0O+IdWZF",
                        Amazon.RegionEndpoint.USEast1));

                var request = new TransferUtilityUploadRequest
                {
                    CannedACL = S3CannedACL.PublicRead,
                    BucketName = "sw-currencies",
                    InputStream = image,
                    Key = agreementDocumentKey.Trim() + AbpSession.TenantId.ToString()
                };
                fileTransferUtility.Upload(request);
                var currencyName = agreementDocumentKey.Trim();
                var result = _accessUrl + request.Key;
                return result.Trim();
            }
            catch (AmazonS3Exception s3Exception)
            {
                throw new UserFriendlyException("Anazone storing failed: " + s3Exception.Message);
            }
        }

        public async Task<List<ExchageRateDto>> GetBtcEthValue()
        {
            //temp need remove
            var usdCurrency = await _currenciesRepository.FirstOrDefaultAsync(c => c.Code.Equals("USD"));

            var supportedCurrency = await _supportedCurrencyRepository.FirstOrDefaultAsync(c => c.CurrencyId.Equals(usdCurrency.Id));

            if (supportedCurrency == null)
            {
                supportedCurrency = new SupportedCurrency
                {
                    CreationTime = DateTime.Now.ToUniversalTime(),
                    CurrencyId = usdCurrency.Id,
                    TenantId = AbpSession.TenantId,
                };

                await _supportedCurrencyRepository.InsertAsync(supportedCurrency);

                var exchangeRate = await _exchangeRateRepository.FirstOrDefaultAsync(er => er.CurrencyId.Equals(supportedCurrency.CurrencyId));

                if (exchangeRate == null)
                {
                    exchangeRate = new ExchangeRate { CurrencyId = usdCurrency.Id };
                    exchangeRate.CreationTime = DateTime.Now.ToUniversalTime();
                    exchangeRate.Price = 0.25M;
                    exchangeRate.IsAutomaticUpdate = true;
                    exchangeRate.UpdatedBy = null;

                    await _exchangeRateRepository.InsertAndGetIdAsync(exchangeRate);
                }
            }
            ////

            var supportedCurrenciesQueryable = _supportedCurrencyRepository.GetAll();

            var exchangeRatesQueryable = _exchangeRateRepository.GetAll();

            var currenciesQueryable = _currenciesRepository.GetAll();

            var queryable = supportedCurrenciesQueryable.Join(exchangeRatesQueryable, er => er.CurrencyId, sc => sc.CurrencyId, (sc, er) =>
            new { ExchangeRate = er, SupportedCurrency = sc }).Join(currenciesQueryable, o => o.SupportedCurrency.CurrencyId, c => c.Id, (o, c) =>
            new { ExchangeRate = o.ExchangeRate, Currency = c });

            return await queryable.Select(o =>
                new ExchageRateDto
                {
                    Id = o.ExchangeRate.Id,
                    Currency = ObjectMapper.Map<CurrencyDto>(o.Currency),
                    Price = o.ExchangeRate.Price,
                    IsAutoWallet = o.ExchangeRate.IsAutoWallet
                }).ToListAsync();
        }
    }
}
