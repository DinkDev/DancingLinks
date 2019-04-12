namespace DancingLinks.Tests
{
    using System.Text;

    public class IndentWriter
    {
        private readonly StringBuilder _result = new StringBuilder();
        private int _indentLevel;

        public void Indent()
        {
            _indentLevel++;
        }

        public void UnIndent()
        {
            if (_indentLevel > 0)
            {
                _indentLevel--;
            }
        }

        public void WriteLine(string line)
        {
            _result.AppendLine(CreateIndent() + line);
        }

        private string CreateIndent()
        {
            var indent = new StringBuilder();

            for (var i = 0; i < _indentLevel; i++)
            {
                indent.Append("    ");
            }

            return indent.ToString();
        }

        public override string ToString()
        {
            return _result.ToString();
        }
    }
}