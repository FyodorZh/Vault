using System.IO;
using System.Runtime.InteropServices;
using OrderedSerializer;

namespace Vault.Content
{
    [Guid("42EBBA80-9719-46B1-B833-68FF1972D9CE")]
    public class StringContent : Content, IContent<string>
    {
        private string? _content;
        
        public string Content => _content ?? "";

        public StringContent()
        {
            _content = "";
        }

        public StringContent(string content)
        {
            _content = content;
        }

        public override void WriteTo(TextWriter dst)
        {
            dst.Write(_content);
        }

        public override void Serialize(IOrderedSerializer serializer)
        {
            serializer.Add(ref _content);
        }
    }
}