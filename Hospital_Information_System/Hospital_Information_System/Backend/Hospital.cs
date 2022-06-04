using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HospitalIS.Backend
{
	internal class WarehouseNotFoundException : Exception
	{
		public WarehouseNotFoundException()
		{
		}

		public WarehouseNotFoundException(string message) : base(message)
		{
		}

		public WarehouseNotFoundException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}

	internal class Hospital : Entity
	{
		internal List<Room> Rooms = new List<Room>();
		internal List<Equipment> Equipment = new List<Equipment>();
		internal List<EquipmentRelocation> EquipmentRelocations = new List<EquipmentRelocation>();
		internal List<Task> EquipmentRelocationTasks = new List<Task>();
		internal List<Person> Persons = new List<Person>();
		internal List<Patient> Patients = new List<Patient>();
		internal List<Doctor> Doctors = new List<Doctor>();
		internal List<Appointment> Appointments = new List<Appointment>();
		internal List<UserAccount> UserAccounts = new List<UserAccount>();
		internal List<UpdateRequest> UpdateRequests = new List<UpdateRequest>();
		internal List<DeleteRequest> DeleteRequests = new List<DeleteRequest>();
		internal List<Renovation> Renovations = new List<Renovation>();
		internal List<Task> RenovationTasks = new List<Task>();
		internal List<Referral> Referrals = new List<Referral>();
		internal List<Ingredient> Ingredients = new List<Ingredient>();
		internal List<Medication> Medications = new List<Medication>();
		internal List<Prescription> Prescriptions = new List<Prescription>();
		internal List<MedicalRecord> MedicalRecords = new List<MedicalRecord>();
		internal List<MedicationRequest> MedicationRequests = new List<MedicationRequest>();
		internal List<RequestEquipment> RequestsEquipment = new List<RequestEquipment>();
		internal List<Task> RequestEquipmentTasks = new List<Task>();
		internal List<AppointmentRating> AppointmentRatings = new List<AppointmentRating>();

	}
}
