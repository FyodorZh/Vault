using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Archivarius;
using Vault.Content;
using Vault.FileSystem;

namespace Vault.Storage.FileSystem
{
    public class FileData : NodeData, IFileData
    {
        private readonly FileDataModel _dataModel;

        public IBox<IFileContent> FileContent => _dataModel.Content;
        
        public FileData(IEntity fsEntity, FileDataModel dataModel) 
            : base(fsEntity, dataModel)
        {
            _dataModel = dataModel;
        }

        public async Task SetContent(Box<IFileContent> encryptedContent)
        {
            _dataModel.Content = encryptedContent;
            await _fsEntity.WriteModel(_dataModel);
        }
        
        [Guid("A04D8AFD-0192-4B90-9BEB-2B837EA8B560")]
        public class FileDataModel : NodeDataModel
        {
            public IBox<IFileContent> Content = null!;
            
            public override void Serialize(ISerializer serializer)
            {
                base.Serialize(serializer);
                serializer.AddClass(ref Content, () => throw new Exception());
            }
        }
    }
}