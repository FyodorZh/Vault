using System;
using System.IO;
using System.Text;

namespace Vault.Scripting
{
    public interface IOutputTextStream
    {
        void Write(string str);
        void WriteLine(string str);
        void FinishBlock();
    }
    
    public class OutputTextStream : IOutputTextStream
    {
        private static readonly string[] _delimiters = new string[] { "\r\n", "\r", "\n" };
        
        private readonly TextWriter _dst;
        private readonly Func<string> _linePrefix;
        private readonly Func<string> _prompt;

        private bool _emptyLine;
        private bool _needToWritePrefix;
        
        public OutputTextStream(TextWriter dst, Func<string> linePrefix, Func<string> prompt)
        {
            _dst = dst;
            _linePrefix = linePrefix;
            _prompt = prompt;
            _dst.Write(_prompt());

            _emptyLine = true;
            _needToWritePrefix = true;
        }

        public void Write(string str)
        {
            var lines = str.Split(_delimiters, StringSplitOptions.None);
            for (int i = 0; i < lines.Length; ++i)
            {
                string line = lines[i];

                if (line != "")
                {
                    _emptyLine = false;
                    if (_needToWritePrefix)
                    {
                        _dst.Write(_linePrefix());
                        _needToWritePrefix = false;
                    }
                    _dst.Write(line);
                }

                if (i != lines.Length - 1)
                {
                      _dst.WriteLine();
                      _emptyLine = true;
                      _needToWritePrefix = true;
                }
            }
        }

        public void WriteLine(string str)
        {
            Write(str);
            Write("\n");
        }

        public void FinishBlock()
        {
            if (!_emptyLine)
            {
                _emptyLine = true;
                _dst.WriteLine();
            }
            
            _dst.Write(_prompt());
        }
    }

    public class OutputTextOffset : IOutputTextStream
    {
        private static readonly string[] _delimiters = new string[] { "\r\n", "\r", "\n" };

        private readonly IOutputTextStream _dst;
        private readonly Func<string> _offset;
        private readonly bool _offsetFirstLine;

        private readonly StringBuilder _sb = new StringBuilder();

        public OutputTextOffset(IOutputTextStream dst, Func<string> offset, bool offsetFirstLine)
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