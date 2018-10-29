using LanguageExt;

namespace Mingle
{
    public sealed /* immutable */ class Operation : Record<Operation>
    {
        public readonly Id Id;
        public readonly Set<Id> Deps;
        public readonly Cursor Cursor;
        public readonly Mutation Mutation;

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

        public Operation Copy(
            Id id = null,
            Set<Id>? deps = null,
            Cursor cursor = null,
            Mutation mutation = null)
        {
            return new Operation(
                id ?? Id,
                deps ?? Deps,
                cursor ?? Cursor,
                mutation ?? Mutation);
        }
    }
}