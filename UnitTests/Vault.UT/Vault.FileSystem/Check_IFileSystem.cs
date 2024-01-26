using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Vault.FileSystem;

namespace Vault.UT
{
    public class Check_IFileSystem
    {
        [Test]
        public Task InMemoryFileSystem_Bytes()
        {
            return GenericCheck<byte[]>(new InMemoryBinaryFileSystem(), b => new byte[] { b });
        }
        
        [Test]
        public Task InMemoryFileSystem_Text()
        {
            return GenericCheck<string>(new InMemoryTextFileSystem(), b => b.ToString());
        }

        private async Task GenericCheck<T>(IFileSystem<T> fs, Func<byte, T> encode) where T : class
        {
            Assert.That(await fs.GetChildren(EntityName.Root), Is.Empty);
            Assert.That(await fs.Delete(EntityName.Root), Is.False);

            {
                Assert.That(await fs.GetEntity(EntityName.Root.Sub("a")), Is.Null);
                IEntity<T>? entity = await fs.Add(EntityName.Root.Sub("a"), encode(7));
                Assert.That(entity, Is.Not.Null);
                Assert.That(await fs.Add(EntityName.Root.Sub("a"), encode(8)), Is.Null);
                Assert.That(entity!.Name, Is.EqualTo(EntityName.Root.Sub("a")));
                Assert.That((bool)entity.IsValid, Is.True);
                
                Assert.That(await entity.Read(), Is.EqualTo(encode(7)));
                await entity.Write(encode(9));
                Assert.That(await entity.Read(), Is.EqualTo(encode(9)));

                await fs.Delete(EntityName.Root.Sub("a"));
                Assert.That((bool)entity.IsValid, Is.False);
            }
        }
    }
}