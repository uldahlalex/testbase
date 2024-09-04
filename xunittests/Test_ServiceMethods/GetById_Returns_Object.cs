using System.Text.Json;
using dataaccess;
using dataaccess.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using service;
using Xunit.Abstractions;
using TestBaseSrc;
using xunittests;

namespace UnitTests
{
    public class GetDoctorById_GetsExistingDoctor_ReturnsDoctorDtoTestA : TestBase<HospitalContext, GetById, object>, IAsyncLifetime
    {
        public GetDoctorById_GetsExistingDoctor_ReturnsDoctorDtoTestA(ITestOutputHelper outputHelper)
            : base()
        {
        }

        [Fact]
        public void GetDoctorById_ThrowsException_WhenNoDoctorExists()
        {
            Assert.Throws<KeyNotFoundException>(() => ServiceInstance.GetDoctorById(1));
        }

    }
}