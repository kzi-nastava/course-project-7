using HIS.Core.Foundation;
using HIS.Core.PersonModel.UserAccountModel;
using Newtonsoft.Json;

namespace HIS.Core.ModificationRequestModel
{
    public abstract class ModificationRequest : Entity
    {
        [JsonConverter(typeof(UserAccountJSONReferenceConverter))]
        public UserAccount Requestee { get; set; }

        public enum StateType
        {
            PENDING, APPROVED, DENIED
        }

        public StateType State { get; set; }

        public ModificationRequest(UserAccount requestee)
        {
            Requestee = requestee;
            State = StateType.PENDING;
        }
    }
}
