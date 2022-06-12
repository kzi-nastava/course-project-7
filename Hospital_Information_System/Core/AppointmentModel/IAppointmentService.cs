using HIS.Core.AppointmentModel.SearchUtil;
using HIS.Core.DoctorModel;
using HIS.Core.PatientModel;
using HIS.Core.UserAccountModel;
using System;
using System.Collections.Generic;

namespace HIS.Core.AppointmentModel
{
    public interface IAppointmentService
    {
        IEnumerable<Appointment> GetAll();
        IEnumerable<Appointment> GetAll(Patient patient);
        IEnumerable<Appointment> GetAll(Doctor doctor);
        IEnumerable<Appointment> GetModifiable(UserAccount user);
        void Add(Appointment appointment, UserAccount user);
        void Update(Appointment appointment, Appointment updatedAppointment, IEnumerable<AppointmentProperty> propertiesToUpdate, UserAccount user);
        void Remove(Appointment appointment, UserAccount user);
        bool MustRequestModification(Appointment appointment, UserAccount user);
        bool AreColliding(DateTime schedule1, DateTime schedule2);
        void Copy(Appointment target, Appointment source, IEnumerable<AppointmentProperty> whichProperties);
        Appointment FindRecommendedAppointment(AppointmentSearchBundle sb);
    }
}
