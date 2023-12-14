using System;
using System.Collections.Generic;
using System.Text;

namespace Vault.Commands
{
    public interface ICommandsFactory
    {
        Command? Construct(string cmd);
    }
    
    public class CommandsFactory : ICommandsFactory
    {
        private readonly Dictionary<string, Func<IReadOnlyList<string>, Command?>> _ctors = 
            new Dictionary<string, Func<IReadOnlyList<string>, Command?>>();

        private readonly StringBuilder _sb = new StringBuilder();
        private readonly List<string> _split = new List<string>();

        public void Add(string commandName, Func<IReadOnlyList<string>, Command?> ctor)
        {
            _ctors.Add(commandName, ctor);
        }

        public Command? Construct(string cmd)
        {
            _split.Clear();
            _sb.Clear();
            
            char? quote = null;
            foreach (var ch in cmd)
            {
                if (quote == null)
                {
                    if (Char.IsWhiteSpace(ch))
                    {
                        if (_sb.Length > 0)
                        {
                            _split.Add(_sb.ToString());
                            _sb.Clear();
                        }
                    }
                    else if (ch == '\'' || ch == '"')
                    {
                        quote = ch;
                    }
                    else
                    {
                        _sb.Append(ch);
                    }
                }
                else
                {
                    if (ch == quote)
                    {
                        if (_sb.Length > 0)
                        {
                            _split.Add(_sb.ToString());
                            _sb.Clear();
                        }
                    }
                    else
                    {
                        _sb.Append(ch);
                    }
                }
            }

            if (_sb.Length > 0)
            {
                _split.Add(_sb.ToString());
                _sb.Clear();
            }

            if (_split.Count == 0)
            {
                return null;
            }

            if (!_ctors.TryGetValue(_split[0], out var ctor))
            {
                return null;
            }

            try
            {
                return ctor.Invoke(_split);
            }
            finally
            {
                _split.Clear();    
            }
        }

        public static CommandsFactory ConstructFullFactory()
        {
            CommandsFactory factory = new CommandsFactory();
            factory.Add("add", list =>
            {
                if (list.Count != 3)
                {
                    return null;
                }
                return new AddCommand(list[1], list[2]);
            });
            factory.Add("cat", list =>
            {
                if (list.Count != 2)
                {
                    return null;
                }
                return new CatCommand(list[1]);
            });
            factory.Add("cd", list=>
            {
                if (list.Count != 2)
                {
                    return null;
                }
                return new CdCommand(list[1]);
            });
            factory.Add("mkdir", list=>
            {
                if (list.Count != 2)
                {
                    return null;
                }
                return new MkdirCommand(list[1]);
            });
            factory.Add("ls", list=>
            {
                if (list.Count != 1)
                {
                    return null;
                }
                return new LsCommand();
            });
            factory.Add("encrypt", list =>
            {
                switch (list.Count)
                {
                    case 2:
                        return new EncryptCommand(list[1]);
                    case 3:
                        return new EncryptCommand(list[1], list[2]);
                    default:
                        return null;
                }
            });
            factory.Add("lock", list =>
            {
                if (list.Count == 1)
                {
                    return new LockUnlockCommand(LockUnlockCommand.LockUnlockMode.Lock, "all");
                }
                if (list.Count == 2)
                {
                    return new LockUnlockCommand(LockUnlockCommand.LockUnlockMode.Lock, list[1]);
                }
                return null;
            });
            factory.Add("unlock", list =>
            {
                if (list.Count == 1)
                {
                    return new LockUnlockCommand(LockUnlockCommand.LockUnlockMode.Unlock, "all");
                }
                if (list.Count == 2)
                {
                    return new LockUnlockCommand(LockUnlockCommand.LockUnlockMode.Unlock, list[1]);
                }
                return null;
            });
            factory.Add("credentials", list =>
            {
                if (list.Count == 2)
                {
                    return new SetCredentialsCommand(list[1]);
                }

                if (list.Count == 3)
                {
                    return new SetCredentialsCommand(list[1], list[2]);
                }

                return null;
            });

            return factory;
        }
    }
}