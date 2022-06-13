using HIS.Core.PersonModel.UserAccountModel;
using HIS.Core.Util;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Core.AppointmentModel.Util
{
    public static class AppointmentPropertyHelpers
    {
        public static IEnumerable<AppointmentProperty> GetModifiableProperties(UserAccount user)
        {
            // Hack.
            List<AppointmentProperty> modifiableProperties = (List<AppointmentProperty>)GetProperties();

            if (user.Type == UserAccount.AccountType.PATIENT)
            {
                modifiableProperties.Remove(AppointmentProperty.PATIENT);
                modifiableProperties.Remove(AppointmentProperty.ROOM);
                modifiableProperties.Remove(AppointmentProperty.ANAMNESIS);
            }

            if (user.Type == UserAccount.AccountType.DOCTOR)
            {
                modifiableProperties.Remove(AppointmentProperty.DOCTOR);
                modifiableProperties.Remove(AppointmentProperty.ROOM);
                modifiableProperties.Remove(AppointmentProperty.ANAMNESIS); //only allowed during an appointment
            }

            return modifiableProperties;
        }

        public static IEnumerable<AppointmentProperty> GetProperties()
        {
            return Utility.GetEnumValues<AppointmentProperty>();
        }

        public static IEnumerable<AppointmentProperty> GetPrioritizableProperties()
        {
            return GetProperties().Where(ap => ap == AppointmentProperty.DOCTOR || ap == AppointmentProperty.SCHEDULED_FOR);
        }
    }
}
