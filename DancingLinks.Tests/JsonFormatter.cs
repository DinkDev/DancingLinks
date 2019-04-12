namespace DancingLinks.Tests
{
    using System.Text;

    public class JsonFormatter
    {
        private readonly StringWalker _walker;
        private readonly IndentWriter _writer = new IndentWriter();
        private readonly StringBuilder _currentLine = new StringBuilder();
        private bool _quoted;

        public JsonFormatter(string json)
        {
            _walker = new StringWalker(json);
        }

        public string Format()
        {
            while (MoveNextChar())
            {
                if (!_quoted && IsOpenBracket())
                {
                    WriteCurrentLine();
                    AddCharToLine();
                    WriteCurrentLine();
                    _writer.Indent();
                }
                else if (!_quoted && IsCloseBracket())
                {
                    WriteCurrentLine();
                    _writer.UnIndent();
                    AddCharToLine();
                }
                else if (!_quoted && IsColon())
                {
                    AddCharToLine();
                    WriteCurrentLine();
                }
                else
                {
                    AddCharToLine();
                }
            }

            WriteCurrentLine();
            return _writer.ToString();
        }

        private bool MoveNextChar()
        {
            var success = _walker.MoveNext();

            if (IsApostrophe())
            {
                _quoted = !_quoted;
            }

            return success;
        }

        public bool IsApostrophe()
        {
            return _walker.CurrentChar == '"' && !_walker.IsEscaped;
        }

        public bool IsOpenBracket()
        {
            return _walker.CurrentChar == '{' || _walker.CurrentChar == '[';
        }

        public bool IsCloseBracket()
        {
            return _walker.CurrentChar == '}' || _walker.CurrentChar == ']';
        }

        public bool IsColon()
        {
            return _walker.CurrentChar == ',';
        }

        private void AddCharToLine()
        {
            _currentLine.Append(_walker.CurrentChar);
        }

        private void WriteCurrentLine()
        {
            var line = _currentLine.ToString().Trim();

            if (line.Length > 0)
            {
                _writer.WriteLine(line);
            }

            // reset the line
            _currentLine.Length = 0;
        }
    }
}