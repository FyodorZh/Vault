using System.Runtime.InteropServices;

namespace Vault.Scripting
{
    [Guid("C8C3113A-F10D-4A2F-B3C3-EBD23B97E1D2")]
    public class EncryptCommand : Command2
    {
        public override string Name => "encrypt";

        private EncryptCommand()
        {
        }
        
        public EncryptCommand(string? nameEncryptionType, string? contentEncryptionType)
            : base(new CommandOption("name", nameEncryptionType), 
                new CommandOption("content", contentEncryptionType))
        {}
    }
}