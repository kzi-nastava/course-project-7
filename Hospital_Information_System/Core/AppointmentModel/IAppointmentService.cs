using System;
using System.Collections.Generic;

namespace HIS.Core.AppointmentModel
{
    public interface IAppointmentService
    {
        IEnumerable<Appointment> GetAll();
    }
}
