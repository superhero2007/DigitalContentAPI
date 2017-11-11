using Digital_Content.DigitalContent.EntityFrameworkCore;

namespace Digital_Content.DigitalContent.Tests.TestDatas
{
    public class TestDataBuilder
    {
        private readonly DigitalContentDbContext _context;
        private readonly int _tenantId;

        public TestDataBuilder(DigitalContentDbContext context, int tenantId)
        {
            _context = context;
            _tenantId = tenantId;
        }

        public void Create()
        {
            new TestOrganizationUnitsBuilder(_context, _tenantId).Create();
            new TestSubscriptionPaymentBuilder(_context, _tenantId).Create();

            _context.SaveChanges();
        }
    }
}
