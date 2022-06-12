using HIS.Core.AppointmentModel;
using System;
using System.Collections.Generic;
using System.Linq;
namespace HIS.Core.AppointmentModel
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _repo;

        public AppointmentService(IAppointmentRepository repo)
        {
            _repo = repo;
        }

        public IEnumerable<Appointment> GetAll()
        {
            return _repo.GetAll();
        }
    }
}
