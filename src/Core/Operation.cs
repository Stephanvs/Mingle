using System;
using System.Collections.Immutable;
using LanguageExt;

namespace Mingle
{
    public sealed /* immutable */ class Operation
    {
        public Operation(
            Id id,
            Set<Id> deps,
            Cursor cursor,
            Mutation mutation)
        {
            Id = id;
            Deps = deps;
            Cursor = cursor;
            Mutation = mutation;
        }

        public Id Id { get; }

        public Set<Id> Deps { get; }

        public Cursor Cursor { get; }

        public Mutation Mutation { get; }
    }
}