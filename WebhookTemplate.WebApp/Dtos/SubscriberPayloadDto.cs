using System.Runtime.Serialization;

namespace Profisee.WebhookTemplate.WebApp.Dtos
{
    /// <summary>
    /// The payload from the triggering of an event in the Profisee Service.
    /// </summary>
    [DataContract]
    public class SubscriberPayloadDto
    {
        /// <summary>
        /// Contains information about the entity.
        /// </summary>
        [DataMember]
        public EntityObjectDto EntityObject { get; set; }

        /// <summary>
        /// The code of the member/record.
        /// </summary>
        [DataMember]
        public string MemberCode { get; set; }

        /// <summary>
        /// The id of the transaction that triggered the subscriber.
        /// </summary>
        [DataMember]
        public int Transaction { get; set; }

        /// <summary>
        /// The user name.
        /// </summary>
        [DataMember]
        public string UserName { get; set; }

        /// <summary>
        /// The name of the event/internal event scenario that triggered
        /// </summary>
        [DataMember]
        public string EventName { get; set; }

        public SubscriberPayloadDto()
        {
            EntityObject = new EntityObjectDto();
        }
    }
}
