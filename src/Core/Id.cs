using System;
using LanguageExt;

namespace Mingle
{
    public /* immutable */ sealed class Id : Record<Id>, IComparable, IComparable<Id>
    {
        private readonly bigint _opsCounter;
        private readonly ReplicaId _replicaId;

        public Id(bigint opsCounter, ReplicaId replicaId)
        {
            _opsCounter = opsCounter;
            _replicaId = replicaId;
        }

        public bigint OpsCounter => _opsCounter;

        public ReplicaId ReplicaId => _replicaId;

        public override int CompareTo(Id other)
            => RecordType<Id>.Compare(this, other);

        public int CompareTo(object obj)
            => (obj is Id id) ? CompareTo(id) : 0;
    }
}