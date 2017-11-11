using Abp;

namespace Digital_Content.DigitalContent
{
    /// <summary>
    /// This class can be used as a base class for services in this application.
    /// It has some useful objects property-injected and has some basic methods most of services may need to.
    /// It's suitable for non domain nor application service classes.
    /// For domain services inherit <see cref="DigitalContentDomainServiceBase"/>.
    /// For application services inherit DigitalContentAppServiceBase.
    /// </summary>
    public abstract class DigitalContentServiceBase : AbpServiceBase
    {
        protected DigitalContentServiceBase()
        {
            LocalizationSourceName = DigitalContentConsts.LocalizationSourceName;
        }
    }
}