using Abp.AspNetCore.Mvc.Views;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Mvc.Razor.Internal;

namespace Digital_Content.DigitalContent.Web.Public.Views
{
    public abstract class DigitalContentRazorPage<TModel> : AbpRazorPage<TModel>
    {
        [RazorInject]
        public IAbpSession AbpSession { get; set; }

        protected DigitalContentRazorPage()
        {
            LocalizationSourceName = DigitalContentConsts.LocalizationSourceName;
        }
    }
}
