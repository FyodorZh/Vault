using System;
using System.Runtime.InteropServices;
using OrderedSerializer;

namespace Vault.Commands
{
    [Guid("E81B3238-8B90-4B25-BC6E-2517C5AD7DC9")]
    public class UnlockCommand : Command
    {
        private string _scope;
        
        public override string Name => "unlock";

        private UnlockCommand()
        {
            _scope = "";
        }

        public UnlockCommand(string scope)
        {
            _scope = scope;
        }

        public override Result Process(IProcessorContext context)
        {
            var res = new LockUnlock_Result();
            
            switch (_scope)
            {
                case "all":
                    res.Name = context.Current.ChildrenNames.Unlock();
                    res.Content = context.Current.ChildrenContent.Unlock();
                    break;
                case "content":
                    res.Content = context.Current.ChildrenContent.Unlock();
                    break;
                case "names":
                    res.Name = context.Current.ChildrenNames.Unlock();
                    break;
                default:
                    return Fail("Wrong unlock command. Allowed: all/names/content");
            }

            return res;
        }

        public override void Serialize(IOrderedSerializer serializer)
        {
            serializer.Add(ref _scope, () => throw new Exception());
        }
    }
}