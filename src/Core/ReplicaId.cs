using System;
using System.Runtime.Serialization;
using LanguageExt;

namespace Mingle
{
    public sealed class ReplicaId : NewType<ReplicaId, string>
    {
        public ReplicaId()
            : this(Guid.NewGuid().ToString())
        {
        }

        public ReplicaId(string id)
            : base(id)
        {
        }

        public override string ToString() => $"ReplicaId({Value})";
    }
}