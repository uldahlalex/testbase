
using dataaccess;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Nito.AsyncEx;
using TestUtilities;
using Xunit.Abstractions;


namespace UnitTests;

    public class WebAppFactoryWorks :  IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly TestUtils<HospitalContext> _testUtils;
        private readonly HttpClient _client;
        
        public WebAppFactoryWorks(
            ITestOutputHelper outputHelper,
            WebApplicationFactory<Startup> factory)

        {
            _client = factory.CreateClient();
            _testUtils = new TestUtils<HospitalContext>();
            AsyncContext.Run(() => _testUtils.Setup());
 
        }
        
        
            [Fact]
            public async Task CanGetHelloWorldWhenUsingHttpClient()
            {
                var response = await _client.GetAsync("/"); 
                var responseString = await response.Content.ReadAsStringAsync();
                Assert.Equal("Hello World!", responseString);
                Assert.Equal(200, (int)response.StatusCode);
            }

    }

    