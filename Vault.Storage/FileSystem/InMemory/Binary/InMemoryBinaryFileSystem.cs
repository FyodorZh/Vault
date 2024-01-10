using System.Runtime.InteropServices;

namespace Vault.FileSystem
{
    [Guid("A30BCDE2-F407-4723-935A-3FE3D57255F4")]
    public class InMemoryBinaryFileSystem : 
        InMemoryFileSystem<byte[], InMemoryBinaryFileSystemEntity>
    {
    }
}