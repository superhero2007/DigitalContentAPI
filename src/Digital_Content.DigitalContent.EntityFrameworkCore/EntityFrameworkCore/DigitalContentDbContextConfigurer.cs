using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Digital_Content.DigitalContent.EntityFrameworkCore
{
    public static class DigitalContentDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<DigitalContentDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
        }
    }
}