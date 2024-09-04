
using dataaccess;
using Microsoft.Extensions.DependencyInjection;

using TestUtilities;
using Xunit.Abstractions;


namespace UnitTests;

    public class GetDoctorById_ThrowsException_WhenNoDoctorExists_Test 
    {
        private readonly TestUtils<HospitalContext> _testUtils;
        
        public GetDoctorById_ThrowsException_WhenNoDoctorExists_Test(
            ITestOutputHelper outputHelper)

        {
            _testUtils = new TestUtils<HospitalContext>(configureServices: services =>
            {
                services.AddTransient<GetById>(serviceProvider =>
                {
                    var context = serviceProvider.GetRequiredService<HospitalContext>();
                    return new GetById(context);
                });
            });
   
        }

        [Fact]
        public void GetDoctorById_ThrowsException_WhenNoDoctorExists()
        {
            Assert.Throws<KeyNotFoundException>(() => _testUtils.ServiceProviderInstance
                .GetRequiredService<GetById>().GetDoctorById(-1));
        }
        
     

    }

    