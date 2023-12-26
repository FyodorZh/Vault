using System.Runtime.InteropServices;
using Archivarius;

namespace Vault.Content
{
    public interface IFileContent : IContent
    {
    }
    
    [Guid("E096A418-D729-4387-92DA-FA284EDBEC13")]
    public class FileContent : IFileContent
    {
        private string? _data;

        public string Data => _data ?? "";

        private FileContent()
        {
        }

        public FileContent(string data)
        {
            _data = data;
        }

        public override string ToString()
        {
            return Data;
        }

        public void Serialize(ISerializer serializer)
        {
            serializer.Add(ref _data);
        }

        public byte Version => 0;
    }
}