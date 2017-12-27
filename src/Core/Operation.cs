using LanguageExt;

namespace Mingle
{
    public sealed /* immutable */ class Operation : Record<Operation>
    {
        private readonly Id _id;
        private readonly Set<Id> _deps;
        private readonly Cursor _cursor;
        private readonly Mutation _mutation;

        public Operation(
            Id id,
            Set<Id> deps,
            Cursor cursor,
            Mutation mutation)
        {
            _id = id;
            _deps = deps;
            _cursor = cursor;
            _mutation = mutation;
        }

        public Id Id => _id;

        public Set<Id> Deps => _deps;

        public Cursor Cursor => _cursor;

        public Mutation Mutation => _mutation;

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