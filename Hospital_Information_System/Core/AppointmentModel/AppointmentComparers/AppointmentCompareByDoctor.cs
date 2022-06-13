using System.Diagnostics.CodeAnalysis;

namespace HIS.Core.AppointmentModel.AppointmentComparers
{
    public class AppointmentCompareByDoctor : AppointmentComparer
    {
        public override int Compare([AllowNull] Appointment x, [AllowNull] Appointment y)
        {
            return x.Doctor.Id.CompareTo(y.Doctor.Id);
        }
    }
}
