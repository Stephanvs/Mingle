using System;
using System.Numerics;
using LanguageExt;

namespace Mingle
{
    public /* immutable */ sealed class Id
    {
        public Id(bigint opsCounter, ReplicaId replicaId)
        {
            OpsCounter = opsCounter;
            ReplicaId = replicaId;
        }

        public bigint OpsCounter { get; }

        public ReplicaId ReplicaId { get; }
    }
}