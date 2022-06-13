using System.Diagnostics.CodeAnalysis;

namespace HIS.Core.AppointmentModel.AppointmentComparers
{
    public class AppointmentCompareByDate : AppointmentComparer
    {
        public override int Compare([AllowNull] Appointment x, [AllowNull] Appointment y)
        {
            return x.ScheduledFor.CompareTo(y.ScheduledFor);
        }
     }
}
