using System;
using System.Runtime.InteropServices;
using Vault.Content;

namespace Vault.Commands
{
    [Guid("C0BF4DFF-2764-4DF7-81E1-1585E469863D")]
    public class AddCommand : Command2
    {
        public override string Name => "add";
        
        private AddCommand()
        {}
        
        public AddCommand(string fileName, string fileContent)
            : base(new CommandOption(fileName), new CommandOption(fileContent))
        {}

        public override Result Process(IProcessorContext context)
        {
            string name = Options[0].Name;
            string content = Options[1].Name;
            
            var child = context.Current.ChildrenNames.FindChild(name);
            if (child != null)
            {
                return Fail("File or directory already exists");
            }
            
            context.Current.ChildrenContent.AddChildFile(name, content);

            return Ok;
        }
    }
}