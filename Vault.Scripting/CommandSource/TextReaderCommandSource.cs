using System;
using System.Collections.Generic;
using System.IO;

namespace Vault.Scripting
{
    public class TextReaderCommandSource : ICommandSource
    {
        private readonly TextReader _source;
        private readonly ICommandsFactory _factory;
        
        public TextReaderCommandSource(TextReader source, ICommandsFactory factory)
        {
            _source = source;
            _factory = factory;
        }

        public event ICommandSource.CommandParseError? OnError;

        public IEnumerable<ICommand> GetAll()
        {
            while (true)
            {
                var line = _source.ReadLine();
                if (line == null || line.ToLowerInvariant().Trim() == "exit")
                {
                    yield break;
                }

                var cmd = _factory.Construct(line);
                if (cmd != null)
                {
                    yield return cmd;
                }
                else
                {
                    Exception ex = new Exception(line);
                    if (OnError != null)
                    {
                        OnError.Invoke(ex);
                    }
                    else
                    {
                        throw ex;
                    }
                }
            }
        }
    }
}