using HIS.Core.PersonModel.DoctorModel;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Core.PollModel.AppointmentPollModel
{
	public class AppointmentPollService : IAppointmentPollService
	{
		private readonly IAppointmentPollRepository _repo;

		public AppointmentPollService(IAppointmentPollRepository repo)
		{
			_repo = repo;
		}

        public AppointmentPoll Add(AppointmentPoll poll)
        {
			return _repo.Add(poll);
        }

        public IEnumerable<AppointmentPoll> GetAll()
		{
			return _repo.GetAll();
		}

        public IEnumerable<AppointmentPoll> GetAll(Doctor doctor)
        {
			return _repo.GetAll(doctor);
        }

		public Dictionary<Doctor, IList<AppointmentPoll>> GetAppointmentPollsByDoctor()
		{
			var doctorPolls = new Dictionary<Doctor, IList<AppointmentPoll>>();
			foreach (var poll in GetAll())
			{
				Doctor doctor = poll.Appointment.Doctor;

				if (!doctorPolls.ContainsKey(doctor))
					doctorPolls[doctor] = new List<AppointmentPoll>();
				doctorPolls[doctor].Add(poll);
			}

			return doctorPolls;
		}

		public IEnumerable<KeyValuePair<Doctor, double>> GetTotalAverageRatingsByDoctor()
		{
			// for each doctor [ for each poll of that doctor [ reduce to ratings and find average rating for that poll ] find the average rating of all polls for that doctor ] sort by rating
			return GetAppointmentPollsByDoctor().Select(kv => new KeyValuePair<Doctor, double>(kv.Key, PollHelpers.ReduceToRatings(kv.Value).Select(kvp => kvp.Value.Average()).ToList().Average()));
		}
	}
}
