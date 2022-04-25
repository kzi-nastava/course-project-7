using System;
using System.Collections.Generic;
using System.Threading;

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
		public List<Room> Rooms = new List<Room>();
		public List<Equipment> Equipment = new List<Equipment>();
		public List<EquipmentRelocation> EquipmentRelocations = new List<EquipmentRelocation>();
		public List<Thread> EquipmentRelocationTasks = new List<Thread>();
		public List<Person> Persons = new List<Person>();
		public List<Patient> Patients = new List<Patient>();
		public List<Doctor> Doctors = new List<Doctor>();
		public List<Appointment> Appointments = new List<Appointment>();
		public List<UserAccount> UserAccounts = new List<UserAccount>();
		public List<UpdateRequest> UpdateRequests = new List<UpdateRequest>();
		public List<DeleteRequest> DeleteRequests = new List<DeleteRequest>();

		public Room GetWarehouse()
		{
			foreach (var r in Rooms)
			{
				if (r.Type == Room.RoomType.WAREHOUSE)
					return r;
			}
			throw new WarehouseNotFoundException();
		}

		public void AddEquipmentRelocationTask(EquipmentRelocation equipmentRelocation)
		{
			Thread t = new Thread(new ThreadStart(() => IS.Instance.EquipmentRelocationRepo.Execute(equipmentRelocation)));
			EquipmentRelocationTasks.Add(t);
			t.Start();
		}
	}
}
