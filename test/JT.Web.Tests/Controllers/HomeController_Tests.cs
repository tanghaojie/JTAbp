using System.Threading.Tasks;
using JT.Web.Controllers;
using Shouldly;
using Xunit;

namespace JT.Web.Tests.Controllers
{
    public class HomeController_Tests: JTWebTestBase
    {
        [Fact]
        public async Task Index_Test()
        {
            //Act
            var response = await GetResponseAsStringAsync(
                GetUrl<HomeController>(nameof(HomeController.Index))
            );

            //Assert
            response.ShouldNotBeNullOrEmpty();
        }
    }
}
