using System;
using System.Runtime.InteropServices;
using Archivarius;
using Vault.Repository;

namespace Vault.Commands
{
    [Guid("95FBC04D-66E7-4924-BDFC-7FF27F899F32")]
    public class CdCommand : Command
    {
        private string _cdParam;
        
        public override string Name => "cd";

        private CdCommand()
        {
            _cdParam = "";
        }

        public CdCommand(string param)
        {
            _cdParam = param;
        }

        public override Result Process(IProcessorContext context)
        {
            if (_cdParam == "..")
            {
                if (context.Current.Parent != null)
                {
                    context.Current = context.Current.Parent;
                }
                
                return Ok;
            }

            var child = context.Current.ChildrenNames.FindChild(_cdParam);
            if (child == null)
            {
                return Fail("Directory not found");
            }

            if (child is not IDirectoryNode dir)
            {
                return Fail("Not a directory!");
            }

            context.Current = dir;
            return Ok;
        }

        public override void Serialize(IOrderedSerializer serializer)
        {
            serializer.Add(ref _cdParam, () => throw new Exception());
        }
    }
}