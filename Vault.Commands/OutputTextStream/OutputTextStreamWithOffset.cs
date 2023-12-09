using System;
using System.Text;

namespace Vault.Commands
{
    public class OutputTextStreamWithOffset : IOutputTextStream
    {
        private static readonly string[] _delimiters = new string[] { "\r\n", "\r", "\n" };

        private readonly IOutputTextStream _dst;
        private readonly Func<string> _offset;
        private readonly bool _offsetFirstLine;

        private readonly StringBuilder _sb = new StringBuilder();

        public OutputTextStreamWithOffset(IOutputTextStream dst, Func<string> offset, bool offsetFirstLine)
        {
            _dst = dst;
            _offset = offset;
            _offsetFirstLine = offsetFirstLine;
        }

        public void Write(string str)
        {
            _sb.Append(str);
        }

        public void WriteLine(string str)
        {
            _sb.AppendLine(str);
        }

        public void FinishBlock()
        {
            bool ignoreOffset = !_offsetFirstLine;
            foreach (var line in _sb.ToString().Split(_delimiters, StringSplitOptions.None))
            {
                if (!ignoreOffset)
                {
                    _dst.Write(_offset.Invoke());
                }

                ignoreOffset = false;
                _dst.WriteLine(line);
            }
        }
    }
}