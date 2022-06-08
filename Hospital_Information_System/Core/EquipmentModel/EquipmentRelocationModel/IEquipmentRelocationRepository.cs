using HIS.Core.RoomModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HIS.Core.EquipmentModel.EquipmentRelocationModel
{
	public interface IEquipmentRelocationRepository
	{
		public void Save();
		public IEnumerable<EquipmentRelocation> Get();
		public EquipmentRelocation Get(int id);
		public void Add(EquipmentRelocation obj);
		public void Remove(EquipmentRelocation obj);
		public IEnumerable<EquipmentRelocation> Get(Room ofRoom)
		{
			return Get().Where(reloc => reloc.RoomFrom == ofRoom || reloc.RoomTo == ofRoom);
		}

		int GetNextId();
	}
}
