using System;
using System.Runtime.InteropServices;
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

        public override Result Process(IProcessorContext context)
        {
            var child = context.Current.ChildrenNames.FindChild(_fileName);
            if (child != null)
            {
                return Fail("File or directory already exists");
            }
            
            context.Current.ChildrenContent.AddChildFile(_fileName, _fileContent);

            return Ok;
        }

        public override void Serialize(IOrderedSerializer serializer)
        {
            serializer.Add(ref _fileName, () => throw new Exception());
            serializer.Add(ref _fileContent, () => throw new Exception());
        }
    }
}