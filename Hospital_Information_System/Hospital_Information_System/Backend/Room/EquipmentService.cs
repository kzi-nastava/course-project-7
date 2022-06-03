namespace HospitalIS.Backend.Room
{
	internal class EquipmentService
	{
		private IEquipmentRepository _repo;
		public EquipmentService(IEquipmentRepository repo)
		{
			_repo = repo;
		}
	}
}
