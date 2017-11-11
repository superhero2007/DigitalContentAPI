using Abp.AspNetCore.Mvc.ViewComponents;

namespace Digital_Content.DigitalContent.Web.Public.Views
{
    public abstract class DigitalContentViewComponent : AbpViewComponent
    {
        protected DigitalContentViewComponent()
        {
            LocalizationSourceName = DigitalContentConsts.LocalizationSourceName;
        }
    }
}