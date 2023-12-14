using System;
using System.Runtime.InteropServices;
using OrderedSerializer;

namespace Vault.Commands
{
    [Guid("E36473AC-8AA7-41E1-A0F9-EC7D28D0CEC7")]
    public class MkdirCommand : Command
    {
        private string _dirName;
        
        public override string Name => "mkdir";

        private MkdirCommand()
        {
            _dirName = "";
        }

        public MkdirCommand(string dirName)
        {
            _dirName = dirName;
        }

        public override Result Process(IProcessorContext context)
        {
            var child = context.Current.ChildrenNames.FindChild(_dirName);
            if (child != null)
            {
                return Fail("File or directory already exists");
            }
            
            context.Current.ChildrenContent.AddChildDirectory(_dirName);
            return Ok;
        }

        public override void Serialize(IOrderedSerializer serializer)
        {
            serializer.Add(ref _dirName, () => throw new Exception());
        }
    }
}