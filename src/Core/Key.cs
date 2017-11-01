using System;

namespace Mingle
{
    public class Key
    {
        public class DocK : Key { }

        public class HeadK : Key { }

        public sealed class IdK : Key
        {
            public IdK(Id id)
            {
                Id = id;
            }

            public Id Id { get; }
        }

        public sealed class StrK : Key
        {
            public StrK(string str)
            {
                Str = str;
            }

            public string Str { get; }
        }
    }
}