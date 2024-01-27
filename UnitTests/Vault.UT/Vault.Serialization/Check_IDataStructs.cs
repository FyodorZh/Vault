using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using NUnit.Framework;
using Archivarius;

namespace Vault.UT
{
    public class Check_IDataStructs
    {
        [Test]
        public void Guid_And_Ctor()
        {
            Dictionary<Guid, Type> guids = new Dictionary<Guid, Type>();

            var dataStruct = typeof(IDataStruct);
            
            var allAssemblies = new []
            {
                typeof(Commands.EntryPoint).Assembly,
                typeof(Content.EntryPoint).Assembly,
                typeof(Encryption.EntryPoint).Assembly,
                typeof(Repository.EntryPoint).Assembly,
                typeof(Serialization.EntryPoint).Assembly,
                typeof(Storage.EntryPoint).Assembly,
                typeof(FileSystem.EntryPoint).Assembly
            };
            
            foreach (var assembly in allAssemblies)
            {
                if (assembly.FullName!.StartsWith("Vault"))
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        if (type.IsClass && !type.IsAbstract && dataStruct.IsAssignableFrom(type))
                        {
                            var guidAttr = Attribute.GetCustomAttribute(type, typeof(GuidAttribute), false) as GuidAttribute;
                            if (guidAttr == null || !Guid.TryParse(guidAttr.Value, out var guid))
                            {
                                Assert.Fail("Type " + type + " has no valid GUID");
                                continue;
                            }

                            if (guids.TryGetValue(guid, out var otherType))
                            {
                                Assert.Fail("Types guid collision: " + type + " and " + otherType);
                                continue;
                            }

                            guids.Add(guid, type);

                            if (type.GetConstructor(
                                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, 
                                    Array.Empty<Type>()) == null)
                            {
                                Assert.Fail("Type " + type + " has no default constructor (even private one)");
                            }
                        }
                    }
                }
            }
            Assert.Pass();
        }
    }
}