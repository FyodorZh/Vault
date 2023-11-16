using System.Runtime.InteropServices;

namespace Vault.Scripting
{
    [Guid("1B53D866-2AEB-4DE4-BF70-855177190406")]
    public class LsCommand : Command
    {
        public override string Name => "ls";
    }
}