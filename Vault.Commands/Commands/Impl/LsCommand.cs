using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Archivarius;
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
                var nameEncryption = directoryContent.GetForNames();
                var contentEncryption = directoryContent.GetForContent();

                if (nameEncryption == contentEncryption)
                {
                    result.SetEncryption(nameEncryption.GetDescription());
                }
                else
                {
                    result.SetEncryption(nameEncryption.GetDescription(), contentEncryption.GetDescription());
                }
            }

            foreach (var ch in context.Current.ChildrenNames.All)
            {
                result.AddChild(ch.Name, ch is IDirectoryNode);
            }
            
            return result;
        }

        public override void Serialize(ISerializer serializer)
        {
            // DO NOTHING
        }

        [Guid("2356F5C5-08D9-47EF-8533-8A4AC3E6FBCE")]
        public class LsResult : OkResult
        {
            private string? _dirName;
            private EncryptionDesc? _nameEncryption;
            private EncryptionDesc? _contentEncryption;
            private EncryptionDesc? _globalEncryption;
            private List<string>? _childrenNames;
            private List<bool>? _childrenDirFlags; // true => child is directory

            public string Name => _dirName ?? "";

            public EncryptionDesc? GlobalEncryption => _globalEncryption;
            
            public EncryptionDesc? NameEncryption => _nameEncryption;

            public EncryptionDesc? ContentEncryption => _contentEncryption;

            public void SetEncryption(EncryptionDesc nameAndContentEncryption)
            {
                _globalEncryption = nameAndContentEncryption;
                _nameEncryption = null;
                _contentEncryption = null;
            }

            public void SetEncryption(EncryptionDesc nameEncryption, EncryptionDesc contentEncryption)
            {
                _globalEncryption = null;
                _nameEncryption = nameEncryption;
                _contentEncryption = contentEncryption;
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
                if (_nameEncryption != null && _contentEncryption != null)
                {
                    dst.WriteLine("Encryption.Name:    " + _nameEncryption);
                    dst.WriteLine("Encryption.Content: " + _contentEncryption);
                }
                else if (_globalEncryption != null)
                {
                    dst.WriteLine("Encryption:    " + _globalEncryption);
                }
                else
                {
                    dst.WriteLine("Encryption: INVALID STATE");
                }

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

            public override void Serialize(ISerializer serializer)
            {
                serializer.Add(ref _dirName);
                serializer.Add(ref _globalEncryption);
                serializer.Add(ref _nameEncryption);
                serializer.Add(ref _contentEncryption);
                serializer.AddList(ref _childrenNames, () => throw new Exception());
                serializer.AddList(ref _childrenDirFlags);
            }
        }
    }
}