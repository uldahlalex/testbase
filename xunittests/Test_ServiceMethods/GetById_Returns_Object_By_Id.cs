
using dataaccess;
using dataaccess.Models;
using Microsoft.Extensions.DependencyInjection;

using TestUtilities;
using Xunit.Abstractions;


namespace UnitTests;

    public class GetDoctorById_GetsExistingDoctor_ReturnsDoctorDtoTest 
    {
        private readonly TestUtils<HospitalContext> _testUtils;
        
        public GetDoctorById_GetsExistingDoctor_ReturnsDoctorDtoTest(
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
            //arrange
            var doctor = new Doctor() { Name = "Bob", Specialty = "General", YearsExperience = 3, Id = 1 };
            _testUtils.DbContextInstance.Doctors.Add(doctor);
            
            //act
            var doctorDto = _testUtils.ServiceProviderInstance.GetRequiredService<GetById>().GetDoctorById(1);
            
            //assert
            Assert.Equal(doctor.Name, doctorDto.Name);
            Assert.Equal(doctor.Specialty, doctorDto.Specialty);
            Assert.Equal(doctor.YearsExperience, doctorDto.YearsExperience);
            Assert.Equal(doctor.Id, doctorDto.Id);
        }
        
     

    }

    