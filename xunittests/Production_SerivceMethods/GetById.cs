using dataaccess;
using service.responses;

public class GetById(HospitalContext context)
{
    public DoctorDto GetDoctorById(int id)
    {
        try
        {
            var doctor = context.Doctors.Find(id);

            if (doctor == null)
            {
                throw new KeyNotFoundException("Doctor not found");
            }

            return new DoctorDto().FromEntity(doctor);
        }
        catch (InvalidOperationException)
        {
            throw new KeyNotFoundException("Doctor not found");
        }
    }}