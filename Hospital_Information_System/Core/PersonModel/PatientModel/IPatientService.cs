using System.Collections.Generic;
using System;

namespace HIS.Core.PersonModel.PatientModel
{
    public interface IPatientService
    {
        IEnumerable<Patient> GetAll();
    }
}
