using System.Data.SqlClient;
using Shouldly;
using Xunit;

namespace Digital_Content.DigitalContent.Tests.General
{
    public class ConnectionString_Tests
    {
        [Fact]
        public void SqlConnectionStringBuilder_Test()
        {
            var csb = new SqlConnectionStringBuilder("Server=localhost; Database=DigitalContent; Trusted_Connection=True;");
            csb["Database"].ShouldBe("DigitalContent");
        }
    }
}
