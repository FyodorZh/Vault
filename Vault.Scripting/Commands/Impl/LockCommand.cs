using System;
using System.Runtime.InteropServices;
using Vault.Repository;

namespace Vault.Scripting
{
    [Guid("B716E624-B7C9-4345-AB92-E85F825BC1FE")]
    public class LockCommand : LockUnlockCommand
    {
        public override string Name => "lock";
        
        private LockCommand() {}
        
        public LockCommand(string scope)
            : base(scope)
        {}

        public override void Process(IProcessorContext context)
        {
            string scope = Option.Name;
            
            switch (scope)
            {
                case "all":
                    LockUnlockReport(context, context.Current.ChildrenContent.Lock(), "ChildrenContent");
                    LockUnlockReport(context, context.Current.ChildrenNames.Lock(), "ChildrenNames");
                    break;
                case "content":
                    LockUnlockReport(context, context.Current.ChildrenContent.Lock(), "ChildrenContent");
                    break;
                case "names":
                    LockUnlockReport(context, context.Current.ChildrenNames.Lock(), "ChildrenNames");
                    break;
                default:
                    context.HumanOutput.WriteLine("Error: Wrong lock command. Allowed: all/names/content");
                    break;
            }
        }
    }
    
    public abstract class LockUnlockCommand : Command1
    {
        protected LockUnlockCommand()
        {
        }

        protected LockUnlockCommand(string scope)
            : base(new CommandOption(scope))
        {}
        
        protected void LockUnlockReport(IProcessorContext context, LockUnlockResult res, string text)
        {
            context.HumanOutput.Write(text + ": ");
            switch (res)
            {
                case LockUnlockResult.NothingToDo:
                    context.HumanOutput.WriteLine("NothingToDo");
                    break;
                case LockUnlockResult.Success:
                    context.HumanOutput.WriteLine("Success");
                    break;
                case LockUnlockResult.Fail:
                    context.HumanOutput.WriteLine("Fail");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(res), res, null);
            }
        }
    }
}