using System.Runtime.InteropServices;
using Archivarius;

namespace Vault.Content
{
    [Guid("42EBBA80-9719-46B1-B833-68FF1972D9CE")]
    public class StringContent : Content
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

        public override string ToString()
        {
            return Content;
        }

        public override void Serialize(IOrderedSerializer serializer)
        {
            serializer.Add(ref _content);
        }
    }
}