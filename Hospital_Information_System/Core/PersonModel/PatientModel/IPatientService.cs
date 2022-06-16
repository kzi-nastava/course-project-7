using System.Collections.Generic;
using System;
using HIS.Core.PersonModel.UserAccountModel;

namespace HIS.Core.PersonModel.PatientModel
{
    public interface IPatientService
    {
        IEnumerable<Patient> GetAll();
        Patient GetPatientFromPerson(Person person);
        void Add(Patient patient);
    }
}
