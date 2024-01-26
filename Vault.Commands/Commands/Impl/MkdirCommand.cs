using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Archivarius;

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

        public override async Task<Result> Process(IProcessorContext context)
        {
            var child = await context.Current.ChildrenNames.FindChild(_dirName);
            if (child != null)
            {
                return await Fail("File or directory already exists");
            }
            
            await context.Current.ChildrenContent.AddChildDirectory(_dirName);
            return await Ok;
        }

        public override void Serialize(ISerializer serializer)
        {
            serializer.Add(ref _dirName, () => throw new Exception());
        }
    }
}