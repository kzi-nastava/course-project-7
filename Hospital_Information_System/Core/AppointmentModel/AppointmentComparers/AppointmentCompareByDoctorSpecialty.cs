using System.Diagnostics.CodeAnalysis;

namespace HIS.Core.AppointmentModel.AppointmentComparers
{
    public class AppointmentCompareByDoctorSpecialty : AppointmentComparer
    {
        public override int Compare([AllowNull] Appointment x, [AllowNull] Appointment y)
        {
            return x.Doctor.Specialty.CompareTo(y.Doctor.Specialty);
        }
    }
}
