namespace HospitalIS.Backend.EquipmentModel
{
	internal class EquipmentService : IEquipmentService
	{
		private readonly IEquipmentRepository _repo;

		public EquipmentService(IEquipmentRepository repo)
		{
			_repo = repo;
		}
	}
}
