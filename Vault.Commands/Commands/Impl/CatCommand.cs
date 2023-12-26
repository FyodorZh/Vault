using System;
using System.Runtime.InteropServices;
using Archivarius;
using Vault.Repository;

namespace Vault.Commands
{
    [Guid("87DE0D2B-E23A-4DFC-A551-1AF643D02D45")]
    public class CatCommand : Command
    {
        private string _fileName;
        
        public override string Name => "cat";

        private CatCommand()
        {
            _fileName = "";
        }

        public CatCommand(string fileName)
        {
            _fileName = fileName;
        }

        public override Result Process(IProcessorContext context)
        {
            var child = context.Current.ChildrenNames.FindChild(_fileName);
            if (child == null)
            {
                return Fail("File not found");
            }

            if (child is not IFileNode file)
            {
                return Fail("Not a file");
            }

            if (file.Content.Value == null)
            {
                return Fail("File content is not available");
            }

            return new CatResult(file.Content.Value.ToString());
        }

        public override void Serialize(ISerializer serializer)
        {
            serializer.Add(ref _fileName, () => throw new Exception());
        }

        [Guid("3B0F1BE3-BAC7-4F20-A7D6-3CB537E66564")]
        public class CatResult : OkResult
        {
            private string? _content;

            public string? Content => _content;

            public CatResult()
            {
            }

            public CatResult(string? content)
            {
                _content = content;
            }

            public override void WriteTo(IOutputTextStream dst)
            {
                dst.WriteLine(_content ?? "");
            }

            public override void Serialize(ISerializer serializer)
            {
                serializer.Add(ref _content);
            }
        }
    }
}