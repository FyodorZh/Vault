using System.Runtime.InteropServices;

namespace Vault.FileSystem
{
    [Guid("2F9221D1-5264-485E-9A51-FD3DDD478318")]
    public class InMemoryTextFileSystem : 
        InMemoryFileSystem<string, InMemoryTextFileSystemEntity, ITextEntity>
    {
    }
}