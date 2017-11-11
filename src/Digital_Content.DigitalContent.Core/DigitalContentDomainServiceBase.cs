using Abp.Domain.Services;

namespace Digital_Content.DigitalContent
{
    public abstract class DigitalContentDomainServiceBase : DomainService
    {
        /* Add your common members for all your domain services. */

        protected DigitalContentDomainServiceBase()
        {
            LocalizationSourceName = DigitalContentConsts.LocalizationSourceName;
        }
    }
}
