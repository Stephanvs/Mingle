using System;
using LanguageExt;

namespace Mingle
{
    public interface Mutation
    {
    }

    public sealed class AssignM : Record<AssignM>, Mutation
    {
        public readonly Val Value;

        public AssignM(Val value)
        {
            Value = value;
        }
    }

    public sealed class InsertM : Record<InsertM>, Mutation
    {
        public readonly Val Value;

        public InsertM(Val value)
        {
            Value = value;
        }
    }

    public sealed class DeleteM : Record<DeleteM>, Mutation
    {
    }

    public sealed class MoveVerticalM : Record<MoveVerticalM>, Mutation
    {
        public readonly Cursor TargetCursor;
        public readonly BeforeAfter AboveBelow;

        public MoveVerticalM(Cursor targetCursor, BeforeAfter aboveBelow)
        {
            TargetCursor = targetCursor;
            AboveBelow = aboveBelow;
        }
    }
}