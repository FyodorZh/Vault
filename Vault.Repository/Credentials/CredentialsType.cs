using System;

namespace Vault.Repository
{
    [Flags]
    public enum CredentialsType : byte
    {
        Unknown = 0,
        Names = 1,
        Content = 2,
        NamesAndContent = Names | Content,
    }
}