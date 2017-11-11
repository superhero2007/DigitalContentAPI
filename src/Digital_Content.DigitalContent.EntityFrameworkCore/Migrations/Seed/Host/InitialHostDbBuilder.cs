using Digital_Content.DigitalContent.EntityFrameworkCore;

namespace Digital_Content.DigitalContent.Migrations.Seed.Host
{
    public class InitialHostDbBuilder
    {
        private readonly DigitalContentDbContext _context;

        public InitialHostDbBuilder(DigitalContentDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            new DefaultEditionCreator(_context).Create();
            new DefaultLanguagesCreator(_context).Create();
            new HostRoleAndUserCreator(_context).Create();
            new DefaultSettingsCreator(_context).Create();

            _context.SaveChanges();
        }
    }
}
