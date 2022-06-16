using HIS.Core.PersonModel.DoctorModel.DoctorComparers;
using System;
using System.Collections.Generic;
using System.Text;

namespace HIS.Core.PersonModel.DoctorModel
{
    public interface IDoctorService
    {
        IEnumerable<Doctor> GetAll();
        Doctor GetDoctorFromPerson(Person person);
        IEnumerable<Doctor> MatchByString(string query, DoctorComparer comparer, Func<Doctor, string> toStr);
        IEnumerable<Doctor> MatchByFirstName(string query, DoctorComparer comparer);
        IEnumerable<Doctor> MatchByLastName(string query, DoctorComparer comparer);
        IEnumerable<Doctor> MatchBySpecialty(string query, DoctorComparer comparer);
        double CalculateRating(Doctor doctor);
        string VerboseToString(Doctor doctor);
        bool ExistForSpecialty(Doctor.MedicineSpeciality speciality);
        List<Doctor.MedicineSpeciality> GetAllSpecialties();
    }
}
