using System;
using System.Collections.Generic;
using System.Linq;
using HIS.Core.PersonModel.PatientModel;
using HIS.Core.PersonModel.PatientModel.MedicalRecordModel;
using HIS.Core.PersonModel.UserAccountModel.Util;

namespace HIS.Core.PersonModel.UserAccountModel
{
	public class UserAccountService : IUserAccountService
	{
		private readonly IUserAccountRepository _repo;
        private readonly IMedicalRecordService _medicalRecordService;
        private readonly IPersonService _personService;
        private readonly IPatientService _patientService;

		public UserAccountService(IUserAccountRepository repo, IMedicalRecordService medicalRecordService, IPatientService patientService, IPersonService personService)
		{
			_repo = repo;
            _medicalRecordService = medicalRecordService;
            _patientService = patientService;
            _personService = personService;
        }

        public void Add(UserAccount account)
        {
            Patient patient = new Patient(account.Person);
            _medicalRecordService.Add(patient);
            _patientService.Add(patient);
            _personService.Add(account.Person);
            _repo.Add(account);
        }

        public void Remove(UserAccount account)
        {
            _repo.Remove(account);
        }

        public void Update(UserAccount target, UserAccount source, IEnumerable<AccountProperty> whichProperties)
        {
            if (whichProperties.Contains(AccountProperty.USERNAME)) target.Username = source.Username;
            if (whichProperties.Contains(AccountProperty.PASSWORD)) target.Password = source.Password;
            if (whichProperties.Contains(AccountProperty.FIRSTNAME)) target.Person.FirstName = source.Person.FirstName;
            if (whichProperties.Contains(AccountProperty.LASTNAME)) target.Person.LastName = source.Person.LastName;
            if (whichProperties.Contains(AccountProperty.GENDER)) target.Person.Gender = source.Person.Gender;
        }

        public IEnumerable<UserAccount> GetByUsername(string username)
        {
            return _repo.GetByUsername(username);
        }

        public IEnumerable<UserAccount> GetModifiable(UserAccount user)
        {
            return _repo.GetModifiable(user);
        }

        public IEnumerable<UserAccount> GetNotBlockedPatientAccounts()
        {
            return _repo.GetNotBlockedPatientAccounts();
        }
        
        public IEnumerable<UserAccount> GetBlockedPatientAccounts()
        {
            return _repo.GetBlockedPatientAccounts();
        }
        
        public IEnumerable<UserAccount> GetAll()
        {
            return _repo.GetAll();
        }

        public UserAccount AttemptLogin(string username, string password)
        {
            foreach (UserAccount ua in GetAll())
            {
                if (ua.Username == username && ua.Password == password)
                {
                    if (ua.Blocked != UserAccount.BlockedBy.NONE) throw new InvalidLoginAttemptException("Account is blocked");
                    _medicalRecordService.AddNotifsIfNecessary(ua);
                    return ua;
                }
            }

            throw new InvalidLoginAttemptException("Invalid credentials");
        }

        public void AddCreatedAppointmentTimestamp(UserAccount user, DateTime timestamp)
        {
            user.AppointmentCreatedTimestamps.Add(timestamp);
            DetectTrolling(user);
        }

        public void AddModifiedAppointmentTimestamp(UserAccount user, DateTime timestamp)
        {
            user.AppointmentModifiedTimestamps.Add(timestamp);
            DetectTrolling(user);
        }

        public void DetectTrolling(UserAccount user)
        {
            PruneTimestamps(user);

            if (user.Type != UserAccount.AccountType.PATIENT)
            {
                return;
            }

            if (user.AppointmentCreatedTimestamps.Count > UserAccountConstants.AppointmentCreationsInGracePeriod)
            {
                user.Blocked = UserAccount.BlockedBy.SYSTEM;
                throw new UserAccountForcefullyBlockedException(
                    $"Exceeded possible number of appointment creations ({UserAccountConstants.AppointmentCreationsInGracePeriod}) for the last {UserAccountConstants.PruningGracePeriodInDays} days");
            }

            if (user.AppointmentModifiedTimestamps.Count > UserAccountConstants.AppointmentModificationsInGracePeriod)
            {
                user.Blocked = UserAccount.BlockedBy.SYSTEM;
                throw new UserAccountForcefullyBlockedException(
                    $"Exceeded possible number of appointment modifications ({UserAccountConstants.AppointmentModificationsInGracePeriod}) for the last {UserAccountConstants.PruningGracePeriodInDays} days");
            }
        }

        public void PruneTimestamps(UserAccount user)
        {
            void prune(List<DateTime> timestamps)
            {
                List<DateTime> prunableTimestamps = new List<DateTime>();
                timestamps.ForEach(t => { if ((DateTime.Now - t).TotalDays > UserAccountConstants.PruningGracePeriodInDays) prunableTimestamps.Add(t); });
                prunableTimestamps.ForEach(t => timestamps.Remove(t));
            }

            prune(user.AppointmentCreatedTimestamps);
            prune(user.AppointmentModifiedTimestamps);
        }
    }
}
