using LanguageExt;

namespace Mingle
{
    public interface Val { }
    public interface LeafVal : Val { }
    public interface BranchVal : Val { }

    public sealed class Num : Record<Num>, LeafVal
    {
        private readonly bigint _value;

        public Num(bigint value)
        {
            _value = value;
        }

        public bigint Value => _value;
    }

    public sealed class Str : Record<Str>, LeafVal
    {
        private readonly string _value;

        public Str(string value)
        {
            _value = value;
        }

        public string Value => _value;
    }
}