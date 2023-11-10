using System;
using System.Collections.Generic;
using System.Reflection;
using OrderedSerializer;
using OrderedSerializer.TypeSerializers;

namespace Vault.Serialization
{
    public static class Serializer
    {
        private static bool _initialized;
        
        private static OrderedSerializer.BinaryBackend.BinaryWriter _writer = null!;
        private static HierarchicalSerializer _serializer = null!;
        private static GuidBasedTypeDeserializer _typeDeserializer = null!;

        private static void Init()
        {
            if (!_initialized)
            {
                _writer = new OrderedSerializer.BinaryBackend.BinaryWriter();
                _serializer = new HierarchicalSerializer(_writer, new GuidBasedTypeSerializer());
                _typeDeserializer = new GuidBasedTypeDeserializer(AppDomain.CurrentDomain.GetAssemblies());
                _initialized = true;
            }
        }
            
        public static IReadOnlyList<byte> Serialize(IDataStruct dataStruct)
        {
            try
            {
                Init();
                _serializer.AddClass(ref dataStruct!);
                return _writer.GetBuffer();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                _writer.Clear();
                _serializer.Prepare();
            }
        }
        
        public static IDataStruct? Deserialize(IReadOnlyList<byte> data)
        {
            try
            {
                Init();
                var bytes = new byte[data.Count];
                for (int i = data.Count - 1; i >= 0; --i)
                {
                    bytes[i] = data[i];
                }

                var reader = new OrderedSerializer.BinaryBackend.BinaryReader(bytes);
                var deserializer = new HierarchicalDeserializer(reader, _typeDeserializer);
                IDataStruct? dataStruct = null;
                deserializer.AddClass(ref dataStruct);
                return dataStruct;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}