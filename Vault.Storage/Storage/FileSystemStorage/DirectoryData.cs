using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Archivarius;
using Vault.Content;
using Vault.FileSystem;

namespace Vault.Storage.FileSystem
{
    public class DirectoryData : NodeData, IDirectoryData
    {
        private DirectoryDataModel _dataModel;
        
        public IBox<IDirectoryContent> DirContent => _dataModel.Content;

        public DirectoryData(IEntity fsEntity, DirectoryDataModel dataModel) 
            : base(fsEntity, dataModel)
        {
            _dataModel = dataModel;
        }

        public async Task SetContent(Box<IDirectoryContent> encryptedContent)
        {
            _dataModel.Content = encryptedContent;
            await _fsEntity.WriteModel(_dataModel);
        }

        public async Task<IDirectoryData> AddDirectory(
            EntityName name,
            NodeId id,
            Box<StringContent> encryptedName,
            Box<DirectoryContent> encryptedContent)
        {
            var dir = await FsEntity.FS.Add(name);
            if (dir == null)
            {
                throw new Exception();
            }
            
            DirectoryDataModel model = new DirectoryDataModel()
            {
                Id = id,
                ParentId = _dataModel.Id,
                Name = encryptedName,
                Content = encryptedContent,
            };

            var dirData = new DirectoryData(_fsEntity, model);
            await dir.WriteModel(model);
            
            return dirData;
        }

        public async Task<IFileData> AddFile(
            EntityName name,
            NodeId id,
            Box<StringContent> encryptedName,
            Box<FileContent> encryptedContent)
        {
            var file = await FsEntity.FS.Add(name);
            if (file == null)
            {
                throw new Exception();
            }
            
            FileData.FileDataModel model = new FileData.FileDataModel()
            {
                Id = id,
                ParentId = _dataModel.Id,
                Name = encryptedName,
                Content = encryptedContent,
            };

            var fileData = new FileData(_fsEntity, model);
            await file.WriteModel(model);
            
            return fileData;
        }
        
        [Guid("84B367B2-5130-45FB-A6EB-C1D289CF39B8")]
        public class DirectoryDataModel : NodeDataModel
        {
            public IBox<IDirectoryContent> Content = null!;
            
            public override void Serialize(ISerializer serializer)
            {
                base.Serialize(serializer);
                serializer.AddClass(ref Content, () => throw new Exception());
            }
        }
    }
}