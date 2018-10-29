using System;
using System.Numerics;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;

namespace Mingle
{
    public /* immutable */ sealed class Id : Record<Id>
    {
        public readonly BigInteger OpsCounter;
        public readonly ReplicaId ReplicaId;

        public Id(BigInteger opsCounter, ReplicaId replicaId)
        {
            OpsCounter = opsCounter;
            ReplicaId = replicaId;
        }
    }
}