using System;
using System.Linq;
using Archivarius;
using Archivarius.BinaryBackend;
using Archivarius.TypeSerializers;

namespace Vault.Serialization
{
    public static class Serializer
    {
        private static bool _initialized;
        
        private static BinaryWriter _writer = null!;
        private static HierarchicalSerializer _serializer = null!;
        private static GuidBasedTypeDeserializer _typeDeserializer = null!;

        private static void Init()
        {
            if (!_initialized)
            {
                _writer = new BinaryWriter();
                
                _serializer = new HierarchicalSerializer(
                    _writer, 
                    new GuidBasedTypeSerializer(), 
                    null,
                    DefaultTypeSet.Version,
                    DefaultTypeSet.DefaultTypes);
                
                _typeDeserializer = new GuidBasedTypeDeserializer(
                    AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName!.StartsWith("Vault")));
                
                _initialized = true;
            }
        }
            
        public static byte[] Serialize(IDataStruct dataStruct)
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
                _serializer.Prepare(DefaultTypeSet.Version, DefaultTypeSet.DefaultTypes);
            }
        }
        
        public static IDataStruct? Deserialize(byte[] data)
        {
            try
            {
                Init();
                
                var reader = new BinaryReader(data);
                
                var deserializer = new HierarchicalDeserializer(
                    reader, _typeDeserializer, null, DefaultTypeSet.Provider);
                
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