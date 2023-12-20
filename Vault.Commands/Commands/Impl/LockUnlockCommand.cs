using System;
using System.Runtime.InteropServices;
using Archivarius;
using Vault.Repository;

namespace Vault.Commands
{
    [Guid("0D3BD1AD-9D2C-4B6C-BAA3-117221B63356")]
    public class LockUnlockCommand : Command
    {
        public enum LockUnlockMode : byte
        {
            Unknown = 0,
            Lock = 1,
            Unlock = 2
        }
        
        public enum Scope : byte
        {
            Unknown = 0,
            All = 1,
            Names = 2,
            Content = 3,
        }

        private LockUnlockMode _mode;
        private Scope _scope;

        public override string Name
        {
            get
            {
                switch (_mode)
                {
                    case LockUnlockMode.Lock:
                        return "lock";
                    case LockUnlockMode.Unlock:
                        return "unlock";
                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        private LockUnlockCommand()
        {
            _mode = LockUnlockMode.Unknown;
            _scope = Scope.Unknown;
        }

        public LockUnlockCommand(LockUnlockMode mode, string scope)
        {
            if (mode != LockUnlockMode.Lock && mode != LockUnlockMode.Unlock)
            {
                throw new InvalidOperationException();
            }

            _mode = mode;

            switch (scope)
            {
                case "all":
                    _scope = Scope.All;
                    break;
                case "names":
                    _scope = Scope.Names;
                    break;
                case "content":
                    _scope = Scope.Content;
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }
            
        public override Result Process(IProcessorContext context)
        {
            LockUnlockCommandResult result = new LockUnlockCommandResult();
            
            switch (_mode)
            {
                case LockUnlockMode.Lock:
                {
                    switch (_scope)
                    {
                        case Scope.All:
                            result.Content = context.Current.ChildrenContent.Lock();
                            result.Name = context.Current.ChildrenNames.Lock();
                            break;
                        case Scope.Content:
                            result.Content = context.Current.ChildrenContent.Lock();
                            break;
                        case Scope.Names:
                            result.Name = context.Current.ChildrenNames.Lock();
                            break;
                        default:
                            return Fail("Wrong lock command. Allowed: all/names/content");
                    }

                    return result;
                }
                case LockUnlockMode.Unlock:
                {
                    switch (_scope)
                    {
                        case Scope.All:
                            result.Name = context.Current.ChildrenNames.Unlock();
                            result.Content = context.Current.ChildrenContent.Unlock();
                            break;
                        case Scope.Content:
                            result.Content = context.Current.ChildrenContent.Unlock();
                            break;
                        case Scope.Names:
                            result.Name = context.Current.ChildrenNames.Unlock();
                            break;
                        default:
                            return Fail("Wrong unlock command. Allowed: all/names/content");
                    }

                    return result;
                }
                default:
                    return Fail("Invalid lock/unlock mode");
            }
        }

        public override void Serialize(IOrderedSerializer serializer)
        {
            serializer.Add(ref _mode);
            serializer.Add(ref _scope);
        }
        
        [Guid("9440FE81-EEF1-4956-B949-95406E472D87")]
        public class LockUnlockCommandResult : OkResult
        {
            private LockUnlockResult _name;
            private LockUnlockResult _content;

            public LockUnlockResult Name
            {
                get => _name;
                set => _name = value;
            }
            
            public LockUnlockResult Content
            {
                get => _content;
                set => _content = value;
            }
        
            public override void WriteTo(IOutputTextStream dst)
            {
                dst.WriteLine("Name: " + _name);
                dst.WriteLine("Content: " + _content);
            }

            public override void Serialize(IOrderedSerializer serializer)
            {
                serializer.Add(ref _name);
                serializer.Add(ref _content);
            }
        }
    }
}