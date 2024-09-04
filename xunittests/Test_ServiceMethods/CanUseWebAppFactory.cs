using System.Net.Http;
using System.Threading.Tasks;
using dataaccess;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TestBaseSrc;
using Xunit;
using Xunit.Abstractions;
using xunittests;

namespace UnitTests
{
    public class GetDoctorById_GetsExistingDoctor_ReturnsDoctorDtoTestB : TestBase<HospitalContext, GetById, Startup>, IAsyncLifetime
    {
        public GetDoctorById_GetsExistingDoctor_ReturnsDoctorDtoTestB(ITestOutputHelper outputHelper)
            : base(new CustomWebApplicationFactory<Startup>())
        {
        }

        [Fact]
        public async Task CanGetHelloWorldWhenUsingHttpClient()
        {
            var client = Factory.CreateClient();
            var response = await client.GetAsync("/");
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Equal("Hello World!", responseString);
            Assert.Equal(200, (int)response.StatusCode);
        }
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", () => "Hello World!");
            });
        }
    }

    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<TStartup>();
                });
        }
    }
}