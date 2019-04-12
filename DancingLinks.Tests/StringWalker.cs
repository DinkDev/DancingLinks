namespace DancingLinks.Tests
{
    public class StringWalker
    {
        private readonly string _s;

        public StringWalker(string s)
        {
            _s = s;
            Index = -1;
        }

        public int Index { get; private set; }
        public bool IsEscaped { get; private set; }
        public char CurrentChar { get; private set; }

        public bool MoveNext()
        {
            var rv = false;

            if (Index != _s.Length - 1)
            {
                IsEscaped = !IsEscaped && CurrentChar == '\\';

                Index++;
                CurrentChar = _s[Index];
                rv = true;
            }

            return rv;
        }
    }
}