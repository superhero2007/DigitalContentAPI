using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Digital_Content.DigitalContent.Configuration;
using Digital_Content.DigitalContent.Web;

namespace Digital_Content.DigitalContent.EntityFrameworkCore
{
    /* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
    public class DigitalContentDbContextFactory : IDbContextFactory<DigitalContentDbContext>
    {
        public DigitalContentDbContext Create(DbContextFactoryOptions options)
        {
            var builder = new DbContextOptionsBuilder<DigitalContentDbContext>();
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

            DigitalContentDbContextConfigurer.Configure(builder, configuration.GetConnectionString(DigitalContentConsts.ConnectionStringName));
            
            return new DigitalContentDbContext(builder.Options);
        }
    }
}