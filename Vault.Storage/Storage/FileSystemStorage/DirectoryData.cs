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

        public async Task SetContent(Box<DirectoryContent> encryptedContent)
        {
            _dataModel.Content = encryptedContent;
            await _fsEntity.WriteModel(_dataModel);
        }

        public async Task<DirectoryData> AddDirectory(
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

            DirectoryDataModel model = new DirectoryDataModel(
                id, _dataModel.Id, encryptedName, encryptedContent);

            await dir.WriteModel(model);
            var dirData = new DirectoryData(_fsEntity, model);
            
            
            return dirData;
        }

        public async Task<FileData> AddFile(
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
            
            FileData.FileDataModel model = new FileData.FileDataModel(
                id, _dataModel.Id, encryptedName, encryptedContent);

            await file.WriteModel(model);
            var fileData = new FileData(_fsEntity, model);

            return fileData;
        }
        
        [Guid("84B367B2-5130-45FB-A6EB-C1D289CF39B8")]
        public class DirectoryDataModel : NodeDataModel
        {
            public IBox<DirectoryContent> Content = null!;
            
            public DirectoryDataModel()
            {}

            public DirectoryDataModel(NodeId id, NodeId parentId, 
                IBox<StringContent> name, IBox<DirectoryContent> content)
                : base(id, parentId, name)
            {
                Content = content;
            }
            
            public override void Serialize(ISerializer serializer)
            {
                base.Serialize(serializer);
                serializer.AddClass(ref Content, () => throw new Exception());
            }
        }
    }
}