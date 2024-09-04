using dataaccess;
using service.responses;

namespace xunittests;

public class GetById(HospitalContext context)
{
    public DoctorDto GetDoctorById(int id) => new DoctorDto().FromEntity(context.Doctors.Find(id) ?? throw new KeyNotFoundException("Doctor not found"));
}