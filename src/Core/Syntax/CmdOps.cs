using System;

namespace Mingle
{
    public static class CmdOps
    {
        public static Cmd Append(this Cmd cmd1, Cmd cmd2)
            => new Sequence(cmd1, cmd2);
    }
}