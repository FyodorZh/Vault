using System;
using System.Runtime.InteropServices;

namespace Vault.Scripting
{
    [Guid("E81B3238-8B90-4B25-BC6E-2517C5AD7DC9")]
    public class UnlockCommand : LockUnlockCommand
    {
        public override string Name => "unlock";
        
        private UnlockCommand() {}
        
        public UnlockCommand(string scope)
            : base(scope)
        {}

        public override void Process(IProcessorContext context)
        {
            string scope = Option.Name;
            switch (scope)
            {
                case "all":
                    LockUnlockReport(context, context.Current.ChildrenNames.Unlock(), "ChildrenNames");
                    LockUnlockReport(context, context.Current.ChildrenContent.Unlock(), "ChildrenContent");
                    break;
                case "content":
                    LockUnlockReport(context, context.Current.ChildrenContent.Unlock(), "ChildrenContent");
                    break;
                case "names":
                    LockUnlockReport(context, context.Current.ChildrenNames.Unlock(), "ChildrenNames");
                    break;
                default:
                    context.HumanOutput.WriteLine("Error: Wrong unlock command. Allowed: all/names/content");
                    break;
            }
        }
    }
}