using System;
using System.Collections.Generic;
using System.Text;

namespace HIS.Core.PersonModel.DoctorModel
{
    public interface IDoctorService
    {
        IEnumerable<Doctor> GetAll();
        Doctor GetDoctorFromPerson(Person person);
    }
}
