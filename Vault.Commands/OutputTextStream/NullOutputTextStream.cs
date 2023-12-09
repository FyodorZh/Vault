namespace Vault.Commands
{
    public class NullOutputTextStream : IOutputTextStream
    {
        public void Write(string str)
        {
            // Do nothing
        }

        public void WriteLine(string str)
        {
            // Do nothing
        }

        public void FinishBlock()
        {
            // Do nothing
        }
    }
}