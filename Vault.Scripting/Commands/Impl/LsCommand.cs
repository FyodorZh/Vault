using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Vault.Repository;

namespace Vault.Scripting
{
    [Guid("1B53D866-2AEB-4DE4-BF70-855177190406")]
    public class LsCommand : Command
    {
        public override string Name => "ls";

        public override void Process(IProcessorContext context)
        {
            context.HumanOutput.WriteLine("Name: " + context.Current.Name);
            if (context.Current.Content.Value != null)
            {
                context.Current.Content.Value.WriteTo(context.HumanOutput);
            }
            else
            {
                context.HumanOutput.WriteLine("Encryption: ???");
            }

            IEnumerable<string> names = context.Current.ChildrenNames.All.Select(
                node => node is IDirectoryNode ? "<" + node.Name + ">" : node.Name);
            
            bool bWritten = false;
            foreach (var elementName in names.Order())
            {
                context.HumanOutput.Write(elementName);
                context.HumanOutput.Write(" ");
                bWritten = true;
            }

            if (bWritten)
            {
                context.HumanOutput.WriteLine();
            }
        }
    }
}