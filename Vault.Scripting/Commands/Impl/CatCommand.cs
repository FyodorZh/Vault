using System;
using System.Runtime.InteropServices;
using Vault.Repository;

namespace Vault.Scripting
{
    [Guid("87DE0D2B-E23A-4DFC-A551-1AF643D02D45")]
    public class CatCommand : Command1
    {
        public override string Name => "cat";
        
        private CatCommand()
        {}

        public CatCommand(string fileName)
            : base(new CommandOption(fileName))
        {
        }

        public override void Process(IProcessorContext context)
        {
            string name = Option.Name;
            
            var child = context.Current.ChildrenNames.FindChild(name);
            if (child == null)
            {
                context.HumanOutput.WriteLine("File " + name + " not found");
            }
            else
            {
                if (child is IFileNode file)
                {
                    if (file.Content.Value != null)
                    {
                        file.Content.Value.WriteTo(context.HumanOutput);
                        context.HumanOutput.WriteLine();
                    }
                    else
                    {
                        context.HumanOutput.WriteLine("Content is not available!");
                    }
                }
                else
                {
                    context.HumanOutput.WriteLine("Not a file!");
                }
            }
        }
    }
}