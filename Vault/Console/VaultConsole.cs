using Vault.Core;
using Vault.Repository;

namespace Vault
{
    public class VaultConsole
    {
        private readonly Stack<IDirectoryNode> _stack = new Stack<IDirectoryNode>();
        
        private IDirectoryNode CurrentNode => _stack.Peek();

        public VaultConsole(IDirectoryNode root)
        {
            _stack.Push(root);
        }

        public void Prompt()
        {
            string prompt = CurrentNode.Name;
            INode? c = CurrentNode.Parent;
            while (c != null)
            {
                prompt = c.Name + "/" + prompt;
                c = c.Parent;
            }
            Console.Write(prompt + "> ");
        }

        public void Command_ls()
        {
            bool bWritten = false;
            foreach (var element in CurrentNode.Children.OrderBy(ch => ch.Name))
            {
                if (element is IFileNode)
                {
                    Console.Write(element.Name);
                }
                else
                {
                    Console.Write("<" + element.Name + ">");
                }
                Console.Write(" ");
                bWritten = true;
            }

            if (bWritten)
            {
                Console.WriteLine();
            }
        }

        public void Command_cat(string name)
        {
            using var child = CurrentNode.RequestChild(name);
            if (child == null)
            {
                Console.WriteLine("File " + name + " not found");
            }
            else
            {
                if (child is IFileNode file)
                {
                    file.Content.WriteTo(Console.Out);
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine("Not a file!");
                }
                child.Dispose();
            }
        }

        public void Command_cd(string name)
        {
            if (name == "..")
            {
                if (_stack.Count > 1)
                {
                    _stack.Pop().Dispose();
                }
                return;
            }

            var child = CurrentNode.RequestChild(name);
            if (child == null)
            {
                Console.WriteLine("Directory " + name + " not found");
            }
            else
            {
                if (child is IDirectoryNode dir)
                {
                    _stack.Push(dir);    
                }
                else
                {
                    Console.WriteLine("Not a directory!");
                    child.Dispose();
                }
            }
        }

        public void Command_add(string name, string text)
        {
            using var child = CurrentNode.RequestChild(name);
            if (child != null)
            {
                Console.WriteLine("File or directory already exists");
                return;
            }
            
            CurrentNode.AddChildFile(name, new ValueContent<string>(text));
        }
        
        public void Command_mkdir(string name)
        {
            using var child = CurrentNode.RequestChild(name);
            if (child != null)
            {
                Console.WriteLine("File or directory already exists");
                return;
            }
            
            CurrentNode.AddChildDirectory(name);
        }
    }
}