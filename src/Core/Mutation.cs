using System;

namespace Mingle
{
    public abstract class Mutation
    {
    }

    public sealed class AssignM : Mutation
    {
        public AssignM(Val value)
        {
            Value = value;
        }

        public Val Value { get; }
    }
    
    public sealed class InsertM : Mutation
    {
        public InsertM(Val value)
        {
            Value = value;
        }

        public Val Value { get; }
    }

    public sealed class DeleteM : Mutation
    {
    }

    public sealed class MoveVerticalM : Mutation
    {
        public MoveVerticalM(Cursor targetCursor, BeforeAfter aboveBelow)
        {
            TargetCursor = targetCursor;
            AboveBelow = aboveBelow;
        }

        public Cursor TargetCursor { get; }
        
        public BeforeAfter AboveBelow { get; }
    }
}