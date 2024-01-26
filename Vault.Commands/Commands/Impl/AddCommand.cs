using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Archivarius;
using Vault.Content;

namespace Vault.Commands
{
    [Guid("C0BF4DFF-2764-4DF7-81E1-1585E469863D")]
    public class AddCommand : Command
    {
        private string _fileName;
        private string _fileContent;
        
        public override string Name => "add";

        private AddCommand()
        {
            _fileContent = _fileName = "";
        }

        public AddCommand(string fileName, string fileContent)
        {
            _fileName = fileName;
            _fileContent = fileContent;
        }

        public override async Task<Result> Process(IProcessorContext context)
        {
            var child = await context.Current.ChildrenNames.FindChild(_fileName);
            if (child != null)
            {
                return await Fail("File or directory already exists");
            }
            
            await context.Current.ChildrenContent.AddChildFile(_fileName, _fileContent);

            return await Ok;
        }

        public override void Serialize(ISerializer serializer)
        {
            serializer.Add(ref _fileName, () => throw new Exception());
            serializer.Add(ref _fileContent, () => throw new Exception());
        }
    }
}