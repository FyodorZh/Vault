using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Archivarius;
using Vault.Repository;

namespace Vault.Commands
{
    [Guid("E217B4CB-7666-476E-8ED8-04A8C0E04912")]
    public class SetCredentialsCommand : Command
    {
        private string? _nameAndContentCredentials;
        private string? _nameCredentials;
        private string? _contentCredentials;

        public override string Name => "credentials";
        
        public SetCredentialsCommand()
        {}

        public SetCredentialsCommand(string nameAndContentCredentials)
        {
            _nameAndContentCredentials = nameAndContentCredentials;
        }

        public SetCredentialsCommand(string nameCredentials, string contentCredentials)
        {
            _nameCredentials = nameCredentials;
            _contentCredentials = contentCredentials;
        }

        public override Task<Result> Process(IProcessorContext context)
        {
            if (_nameAndContentCredentials != null)
            {
                context.SetCredentialsInAdvance(CredentialsType.NamesAndContent, _nameAndContentCredentials);
            }
            else if (_nameCredentials != null || _contentCredentials != null)
            {
                if (_nameCredentials != null)
                {
                    context.SetCredentialsInAdvance(CredentialsType.Names, _nameCredentials);
                }

                if (_contentCredentials != null)
                {
                    context.SetCredentialsInAdvance(CredentialsType.Content, _contentCredentials);
                }
            }
            else
            {
                return Fail("Credentials are not provided");
            }

            return Ok;
        }

        public override void Serialize(ISerializer serializer)
        {
            serializer.Add(ref _nameAndContentCredentials);
            serializer.Add(ref _nameCredentials);
            serializer.Add(ref _contentCredentials);
        }
    }
}