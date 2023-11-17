using System;
using System.Runtime.InteropServices;
using OrderedSerializer;
using Vault.Repository;

namespace Vault.Scripting
{
    [Guid("95FBC04D-66E7-4924-BDFC-7FF27F899F32")]
    public class CdCommand : Command1
    {
        public override string Name => "cd";

        private CdCommand()
        {
        }

        public CdCommand(string param)
            : base(new CommandOption(param))
        {
        }

        public override void Process(IProcessorContext context)
        {
            string name = Option.Name;
            
            if (name == "..")
            {
                if (context.Current.Parent != null)
                {
                    context.Current = context.Current.Parent;
                }
                
                return;
            }

            var child = context.Current.ChildrenNames.FindChild(name);
            if (child == null)
            {
                context.HumanOutput.WriteLine("Directory " + name + " not found");
            }
            else
            {
                if (child is IDirectoryNode dir)
                {
                    context.Current = dir;
                }
                else
                {
                    context.HumanOutput.WriteLine("Not a directory!");
                }
            }
        }
    }
}