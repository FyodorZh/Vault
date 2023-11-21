using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using NUnit.Framework;
using OrderedSerializer;

namespace Vault.UT
{
    public class Serialization
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test(Description = "Check All IDataStructs for GUIDs and default constructors")]
        public void Check_IDataStructs()
        {
            Dictionary<Guid, Type> guids = new Dictionary<Guid, Type>();

            var dataStruct = typeof(IDataStruct);
            
            var allAssemblies = new Assembly[]
            {
                typeof(Vault.Commands.EntryPoint).Assembly,
                typeof(Vault.Content.EntryPoint).Assembly,
                typeof(Vault.Encryption.EntryPoint).Assembly,
                typeof(Vault.Repository.EntryPoint).Assembly,
                typeof(Vault.Serialization.EntryPoint).Assembly,
                typeof(Vault.Storage.EntryPoint).Assembly
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