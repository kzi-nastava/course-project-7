using System.Collections.Generic;
using System;

namespace HIS.Core.PersonModel.PatientModel
{
    public interface IPatientService
    {
        IEnumerable<Patient> GetAll();
        Patient GetPatientFromPerson(Person person);
        void Add(Patient patient);
    }
}
