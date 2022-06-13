using System.Collections.Generic;
using System;

namespace HIS.Core.PatientModel
{
    public interface IPatientService
    {
        IEnumerable<Patient> GetAll();
    }
}
