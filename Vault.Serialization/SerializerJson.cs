using System;
using System.Collections.Generic;
using System.Linq;
using OrderedSerializer;
using OrderedSerializer.JsonBackend;
using OrderedSerializer.TypeSerializers;

namespace Vault.Serialization
{
    public static class SerializerJson
    {
        private static bool _initialized;
        
        private static JsonWriter _writer = null!;
        private static HierarchicalSerializer _serializer = null!;
        private static GuidBasedTypeDeserializer _typeDeserializer = null!;

        private static void Init()
        {
            if (!_initialized)
            {
                _writer = new JsonWriter();
                
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
            
        public static string Serialize(IDataStruct dataStruct)
        {
            try
            {
                Init();
                _serializer.AddClass(ref dataStruct!);
                return _writer.ToJsonString();
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
        
        public static IDataStruct? Deserialize(string json)
        {
            try
            {
                Init();

                var reader = new JsonReader(json);
                
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