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

        public override Result Process(IProcessorContext context)
        {
            string name = Option.Name;
            
            if (name == "..")
            {
                if (context.Current.Parent != null)
                {
                    context.Current = context.Current.Parent;
                }
                
                return Ok;
            }

            var child = context.Current.ChildrenNames.FindChild(name);
            if (child == null)
            {
                return Fail("Directory not found");
            }

            if (child is not IDirectoryNode dir)
            {
                return Fail("Not a directory!");
            }

            context.Current = dir;
            return Ok;
        }
    }
}