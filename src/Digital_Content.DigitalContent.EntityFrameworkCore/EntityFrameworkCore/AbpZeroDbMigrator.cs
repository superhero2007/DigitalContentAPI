using Abp.Domain.Uow;
using Abp.EntityFrameworkCore;
using Abp.MultiTenancy;
using Abp.Zero.EntityFrameworkCore;

namespace Digital_Content.DigitalContent.EntityFrameworkCore
{
    public class AbpZeroDbMigrator : AbpZeroDbMigrator<DigitalContentDbContext>
    {
        public AbpZeroDbMigrator(
            IUnitOfWorkManager unitOfWorkManager,
            IDbPerTenantConnectionStringResolver connectionStringResolver,
            IDbContextResolver dbContextResolver) :
            base(
                unitOfWorkManager,
                connectionStringResolver,
                dbContextResolver)
        {

        }
    }
}
