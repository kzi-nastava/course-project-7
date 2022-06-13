using HIS.Core.AppointmentModel.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace HIS.Core.AppointmentModel.AppointmentAvailability
{
    public interface IAppointmentAvailabilityService
    {
        Appointment FindRecommendedAppointment(AppointmentSearchBundle sb);
    }
}
