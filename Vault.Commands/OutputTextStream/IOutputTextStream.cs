using System;
using System.IO;
using System.Text;

namespace Vault.Commands
{
    public interface IOutputTextStream
    {
        void Write(string str);
        void WriteLine(string str);
        void FinishBlock();
    }
}