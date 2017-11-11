using LanguageExt;

namespace Mingle
{
    public /* immutable */ sealed class Id : Record<Id>
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
    }
}