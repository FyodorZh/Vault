using System.Runtime.InteropServices;
using OrderedSerializer;

namespace Vault.Scripting
{
    [Guid("95FBC04D-66E7-4924-BDFC-7FF27F899F32")]
    public class CdCommand : Command1
    {
        public override string Name => "cd";

        private CdCommand()
        {
        }

        public CdCommand(string param)
            : base(new CommandOption(param))
        {
        }
    }
}