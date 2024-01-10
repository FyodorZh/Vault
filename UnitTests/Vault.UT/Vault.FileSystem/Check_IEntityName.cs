using System;
using NUnit.Framework;
using Vault.FileSystem;
using Archivarius;

namespace Vault.UT
{
    public static class Check_IEntityName
    {
        [Test]
        public static void Test()
        {
            TestImplementation(EntityName.Root);
            
            Assert.That(EntityName.Root, Is.EqualTo(new EntityName()));
            EntityName ab = new EntityName(
                new EntityName(EntityName.Root, "a"), 
                "b");

            var clone = ab.Copy();
            Assert.That(ab, Is.EqualTo(clone));
        }

        private static void TestImplementation(EntityName root)
        {
            Assert.IsTrue(root.IsRoot);
            Assert.IsNull(root.Parent);
            Assert.That(root.Name, Is.EqualTo(""));
            Assert.That(root.FullName, Is.EqualTo(""));

            var a = root.Sub("a");
            Assert.IsFalse(a.IsRoot);
            Assert.That(a.Parent, Is.EqualTo(root));
            Assert.That(a.Name, Is.EqualTo("a"));
            Assert.That(a.FullName, Is.EqualTo("/a"));

            Assert.Throws<ArgumentException>(() => root.Sub(""));
            
            var b = a.Sub("b");
            Assert.IsFalse(b.IsRoot);
            Assert.That(b.Parent, Is.EqualTo(a));
            Assert.That(b.Name, Is.EqualTo("b"));
            Assert.That(b.FullName, Is.EqualTo("/a/b"));
        }
    }
}