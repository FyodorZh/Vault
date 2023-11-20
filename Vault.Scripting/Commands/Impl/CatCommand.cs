using System.Runtime.InteropServices;
using OrderedSerializer;
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

        public override Result Process(IProcessorContext context)
        {
            string name = Option.Name;
            
            var child = context.Current.ChildrenNames.FindChild(name);
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

            public override void Serialize(IOrderedSerializer serializer)
            {
                serializer.Add(ref _content);
            }
        }
    }
}