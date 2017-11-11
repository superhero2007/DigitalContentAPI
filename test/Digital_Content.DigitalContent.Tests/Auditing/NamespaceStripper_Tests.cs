using Digital_Content.DigitalContent.Auditing;
using Shouldly;
using Xunit;

namespace Digital_Content.DigitalContent.Tests.Auditing
{
    public class NamespaceStripper_Tests: AppTestBase
    {
        private readonly INamespaceStripper _namespaceStripper;

        public NamespaceStripper_Tests()
        {
            _namespaceStripper = Resolve<INamespaceStripper>();
        }

        [Fact]
        public void Should_Stripe_Namespace()
        {
            var controllerName = _namespaceStripper.StripNameSpace("Digital_Content.DigitalContent.Web.Controllers.HomeController");
            controllerName.ShouldBe("HomeController");
        }

        [Theory]
        [InlineData("Digital_Content.DigitalContent.Auditing.GenericEntityService`1[[Digital_Content.DigitalContent.Storage.BinaryObject, Digital_Content.DigitalContent.Core, Version=1.10.1.0, Culture=neutral, PublicKeyToken=null]]", "GenericEntityService<BinaryObject>")]
        [InlineData("CompanyName.ProductName.Services.Base.EntityService`6[[CompanyName.ProductName.Entity.Book, CompanyName.ProductName.Core, Version=1.10.1.0, Culture=neutral, PublicKeyToken=null],[CompanyName.ProductName.Services.Dto.Book.CreateInput, N...", "EntityService<Book, CreateInput>")]
        [InlineData("Digital_Content.DigitalContent.Auditing.XEntityService`1[Digital_Content.DigitalContent.Auditing.AService`5[[Digital_Content.DigitalContent.Storage.BinaryObject, Digital_Content.DigitalContent.Core, Version=1.10.1.0, Culture=neutral, PublicKeyToken=null],[Digital_Content.DigitalContent.Storage.TestObject, Digital_Content.DigitalContent.Core, Version=1.10.1.0, Culture=neutral, PublicKeyToken=null],]]", "XEntityService<AService<BinaryObject, TestObject>>")]
        public void Should_Stripe_Generic_Namespace(string serviceName, string result)
        {
            var genericServiceName = _namespaceStripper.StripNameSpace(serviceName);
            genericServiceName.ShouldBe(result);
        }
    }
}
