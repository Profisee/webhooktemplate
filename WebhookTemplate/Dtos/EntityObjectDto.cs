//==============================================================================
// Copyright (c) Profisee Corporation. All Rights Reserved.
//==============================================================================

using System;
using System.Runtime.Serialization;

namespace Profisee.WebhookTemplate.Dtos
{
    /// <summary>
    /// The part of the Subscriber payload that contains information about the related entity.
    /// </summary>
    [DataContract]
    public class EntityObjectDto
    {
        /// <summary>
        /// The uid of the entity in the Profisee Service
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// The name of the entity in the Profisee Service
        /// </summary>
        [DataMember]
        public string Name { get; set; }
    }
}
