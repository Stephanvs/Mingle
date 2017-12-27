using System;
using LanguageExt;

namespace Mingle
{
    public interface Mutation
    {
    }

    public sealed class AssignM : Record<AssignM>, Mutation
    {
        private readonly Val _value;

        public AssignM(Val value)
        {
            _value = value;
        }

        public Val Value => _value;
    }

    public sealed class InsertM : Record<InsertM>, Mutation
    {
        private readonly Val _value;

        public InsertM(Val value)
        {
            _value = value;
        }

        public Val Value => _value;
    }

    public sealed class DeleteM : Record<DeleteM>, Mutation
    {
    }

    public sealed class MoveVerticalM : Record<MoveVerticalM>, Mutation
    {
        private readonly Cursor _targetCursor;
        private readonly BeforeAfter _aboveBelow;

        public MoveVerticalM(Cursor targetCursor, BeforeAfter aboveBelow)
        {
            _targetCursor = targetCursor;
            _aboveBelow = aboveBelow;
        }

        public Cursor TargetCursor => _targetCursor;

        public BeforeAfter AboveBelow => _aboveBelow;
    }
}