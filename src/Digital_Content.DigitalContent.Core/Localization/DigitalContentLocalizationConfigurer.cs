using System.Reflection;
using Abp.Configuration.Startup;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Xml;
using Abp.Reflection.Extensions;

namespace Digital_Content.DigitalContent.Localization
{
    public static class DigitalContentLocalizationConfigurer
    {
        public static void Configure(ILocalizationConfiguration localizationConfiguration)
        {
            localizationConfiguration.Sources.Add(
                new DictionaryBasedLocalizationSource(
                    DigitalContentConsts.LocalizationSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        typeof(DigitalContentLocalizationConfigurer).GetAssembly(),
                        "Digital_Content.DigitalContent.Localization.DigitalContent"
                    )
                )
            );
        }
    }
}