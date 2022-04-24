using HospitalIS.Backend.Repository;
using Newtonsoft.Json;
using System.IO;

namespace HospitalIS.Backend
{
	internal class IS
	{
		private static IS _system;
		public static IS Instance
		{
			get
			{
				return _system ??= new IS();
			}
		}

		public Hospital Hospital = new Hospital();
		public RoomRepository RoomRepo = new RoomRepository();
		public EquipmentRepository EquipmentRepo = new EquipmentRepository();
		public EquipmentRelocationRepository EquipmentRelocationRepo = new EquipmentRelocationRepository();
		public PersonRepository PersonRepo = new PersonRepository();
		public PatientRepository PatientRepo = new PatientRepository();
		public DoctorRepository DoctorRepo = new DoctorRepository();
		public AppointmentRepository AppointmentRepo = new AppointmentRepository();

		private readonly JsonSerializerSettings settings;
		private const string fnameRooms = "rooms.json";
		private const string fnameEquipment = "equipment.json";
		private const string fnameEquipmentRelocation = "equipmentRelocation.json";
		private const string fnamePersons = "persons.json";
		private const string fnamePatients = "patients.json";
		private const string fnameDoctors = "doctors.json";
		private const string fnameAppointments = "appointments.json";

		public IS()
		{
			settings = new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects };
		}

		public void Save(string directory)
		{
			EquipmentRepo.Save(Path.Combine(directory, fnameEquipment), settings);
			RoomRepo.Save(Path.Combine(directory, fnameRooms), settings);
			EquipmentRelocationRepo.Save(Path.Combine(directory, fnameEquipmentRelocation), settings);
			PersonRepo.Save(Path.Combine(directory, fnamePersons), settings);
			PatientRepo.Save(Path.Combine(directory, fnamePatients), settings);
			DoctorRepo.Save(Path.Combine(directory, fnameDoctors), settings);
			AppointmentRepo.Save(Path.Combine(directory, fnameAppointments), settings);
		}

		public void Load(string directory)
		{
			EquipmentRepo.Load(Path.Combine(directory, fnameEquipment), settings);
			RoomRepo.Load(Path.Combine(directory, fnameRooms), settings);
			EquipmentRelocationRepo.Load(Path.Combine(directory, fnameEquipmentRelocation), settings);
			PersonRepo.Load(Path.Combine(directory, fnamePersons), settings);
			PatientRepo.Load(Path.Combine(directory, fnamePatients), settings);
			DoctorRepo.Load(Path.Combine(directory, fnameDoctors), settings);
			AppointmentRepo.Load(Path.Combine(directory, fnameAppointments), settings);

			foreach (var relocation in Hospital.EquipmentRelocations)
			{
				Hospital.AddEquipmentRelocationTask(relocation);
			}
		}
	}
}
