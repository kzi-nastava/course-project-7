using HIS.Core.AppointmentModel.Util;
using HIS.Core.PersonModel.DoctorModel;
using HIS.Core.PersonModel.PatientModel;
using HIS.Core.PersonModel.UserAccountModel;
using System;
using System.Collections.Generic;

namespace HIS.Core.AppointmentModel
{
    public interface IAppointmentService
    {
        void Copy(IAppointmentService other);
        IEnumerable<Appointment> GetAll();
        IEnumerable<Appointment> GetAll(Patient patient);
        IEnumerable<Appointment> GetAll(Doctor doctor);
        IEnumerable<Appointment> GetModifiable(UserAccount user);
        IEnumerable<Appointment> GetPast(Patient patient);
        IEnumerable<Appointment> GetPollable(Patient patient);
        void Add(Appointment appointment, UserAccount user);
        void Update(Appointment appointment, Appointment updatedAppointment, IEnumerable<AppointmentProperty> propertiesToUpdate, UserAccount user);
        void Remove(Appointment appointment, UserAccount user);
        bool MustRequestModification(Appointment appointment, UserAccount user);
        bool AreColliding(DateTime schedule1, DateTime schedule2);
        void Copy(Appointment target, Appointment source, IEnumerable<AppointmentProperty> whichProperties);
        Appointment FindRecommendedAppointment(AppointmentSearchBundle sb);
    }
}
