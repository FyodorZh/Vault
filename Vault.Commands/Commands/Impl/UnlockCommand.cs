using System;
using System.Runtime.InteropServices;

namespace Vault.Commands
{
    [Guid("E81B3238-8B90-4B25-BC6E-2517C5AD7DC9")]
    public class UnlockCommand : Command1
    {
        public override string Name => "unlock";
        
        private UnlockCommand() {}
        
        public UnlockCommand(string scope)
            : base(new CommandOption(scope))
        {}

        public override Result Process(IProcessorContext context)
        {
            string scope = Option.Name;

            var res = new LockUnlock_Result();
            
            switch (scope)
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
    }
}