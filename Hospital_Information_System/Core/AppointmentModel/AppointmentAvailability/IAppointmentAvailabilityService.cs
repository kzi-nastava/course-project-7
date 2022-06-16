using HIS.Core.AppointmentModel.Util;
using System;
using System.Collections.Generic;
using System.Text;
using HIS.Core.PersonModel.DoctorModel;

namespace HIS.Core.AppointmentModel.AppointmentAvailability
{
    public interface IAppointmentAvailabilityService
    {
        Appointment FindRecommendedAppointment(AppointmentSearchBundle sb);
        Appointment FindUrgentAppointmentSlot(AppointmentSearchBundle sb, Doctor.MedicineSpeciality speciality);
    }
}
