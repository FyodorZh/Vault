using System.Collections.Generic;
using System.Runtime.InteropServices;
using OrderedSerializer;
using Vault.Content;
using Vault.Encryption;
using Vault.Repository;

namespace Vault.Commands
{
    [Guid("1B53D866-2AEB-4DE4-BF70-855177190406")]
    public class LsCommand : Command
    {
        public override string Name => "ls";

        public override Result Process(IProcessorContext context)
        {
            LsResult result = new LsResult(context.Current.Name);
            
            if (context.Current.Content.Value is DirectoryContent directoryContent)
            {
                result.NameEncryption = directoryContent.GetForNames()?.GetDescription();
                result.ContentEncryption = directoryContent.GetForContent()?.GetDescription();
            }

            foreach (var ch in context.Current.ChildrenNames.All)
            {
                result.AddChild(ch.Name, ch is IDirectoryNode);
            }
            
            return result;
        }

        [Guid("2356F5C5-08D9-47EF-8533-8A4AC3E6FBCE")]
        public class LsResult : OkResult
        {
            private string? _dirName;
            private EncryptionDesc? _nameEncryption;
            private EncryptionDesc? _contentEncryption;
            private List<string>? _childrenNames;
            private List<bool>? _childrenDirFlags; // true => child is directory

            public string Name => _dirName ?? "";

            public EncryptionDesc? NameEncryption
            {
                get => _nameEncryption;
                set => _nameEncryption = value;
            }

            public EncryptionDesc? ContentEncryption
            {
                get => _contentEncryption;
                set => _contentEncryption = value;
            }

            public IEnumerable<(string, bool)> Children
            {
                get
                {
                    if (_childrenNames != null && _childrenDirFlags != null)
                    {
                        for (int i = 0; i < _childrenNames.Count; ++i)
                        {
                            yield return (_childrenNames[i], _childrenDirFlags[i]);
                        }
                    }
                }
            }

            public LsResult()
            {}

            public LsResult(string name)
            {
                _dirName = name;
            }

            public void AddChild(string name, bool isDirectory)
            {
                _childrenNames ??= new List<string>();
                _childrenDirFlags ??= new List<bool>();
                _childrenNames.Add(name);
                _childrenDirFlags.Add(isDirectory);
            }
            
            
            public override void WriteTo(IOutputTextStream dst)
            {
                dst.WriteLine("Name: " + _dirName);
                dst.WriteLine("Encryption.Name:    " + (_nameEncryption.HasValue ? _nameEncryption.ToString() : "???"));
                dst.WriteLine("Encryption.Content: " + (_contentEncryption.HasValue ? _contentEncryption.ToString() : "???"));

                List<string> names = new List<string>();
                for (int i = 0; i < (_childrenNames?.Count ?? 0); ++i)
                {
                    names.Add(_childrenDirFlags![i] ? ("<" + _childrenNames![i] + ">") : _childrenNames![i]);
                }
                names.Sort();
                foreach (var name in names)
                {
                    dst.WriteLine(name);
                }
            }

            public override void Serialize(IOrderedSerializer serializer)
            {
                serializer.Add(ref _dirName);
                SerializeEncryptionDesc(serializer, ref _nameEncryption);
                SerializeEncryptionDesc(serializer, ref _contentEncryption);
                serializer.AddCollection<string, List<string>>(ref _childrenNames!);
                serializer.AddCollection<bool, List<bool>>(ref _childrenDirFlags!);
            }

            private void SerializeEncryptionDesc(IOrderedSerializer serializer, ref EncryptionDesc? desc)
            {
                bool isNull = desc == null;
                serializer.Add(ref isNull);
                if (isNull)
                {
                    desc = null;
                }
                else
                {
                    EncryptionDesc tmp = desc ?? new EncryptionDesc();
                    serializer.AddStruct(ref tmp);
                    desc = tmp;
                }
            }
        }
    }
}