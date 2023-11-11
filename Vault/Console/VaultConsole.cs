using System;
using System.Collections.Generic;
using System.Linq;
using Vault.Content;
using Vault.Encryption;
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

        private static string FormatName(INode node, bool showDirectory)
        {
            string name = node.Name ?? (node.Id.ToString()+ "*");
            name += "[";
            name += ((node.State & LockState.Content) != 0) ? "?" : "c";
            name += "]";
            if (node is IDirectoryNode && showDirectory)
            {
                name = "<" + name + ">";
            }
            
            return name;
        }

        public void Prompt()
        {
            string prompt = FormatName(CurrentNode, false);
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
            Console.WriteLine("Name: " + (CurrentNode.Name ?? "???"));
            if (CurrentNode.Content != null)
            {
                CurrentNode.Content.WriteTo(Console.Out);
            }
            else
            {
                Console.WriteLine("Encryption: ???");
            }
            
            bool bWritten = false;
            foreach (var elementName in CurrentNode.Children.Select(node => FormatName(node, true)).Order())
            {
                Console.Write(elementName);
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
            var child = CurrentNode.FindChild(name);
            if (child == null)
            {
                Console.WriteLine("File " + name + " not found");
            }
            else
            {
                if (child is IFileNode file)
                {
                    if (file.Content != null)
                    {
                        file.Content.WriteTo(Console.Out);
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.WriteLine("Content is not available!");
                    }
                }
                else
                {
                    Console.WriteLine("Not a file!");
                }
                //child.Dispose();
            }
        }

        public void Command_cd(string name)
        {
            if (name == "..")
            {
                if (_stack.Count > 1)
                {
                    _stack.Pop();//.Dispose();
                }
                return;
            }

            var child = CurrentNode.FindChild(name);
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
                    //child.Dispose();
                }
            }
        }

        public void Command_add(string name, string text)
        {
            var child = CurrentNode.FindChild(name);
            if (child != null)
            {
                Console.WriteLine("File or directory already exists");
                return;
            }
            
            CurrentNode.AddChildFile(name, new StringContent(text));
        }
        
        public void Command_mkdir(string name)
        {
            var child = CurrentNode.FindChild(name);
            if (child != null)
            {
                Console.WriteLine("File or directory already exists");
                return;
            }
            
            CurrentNode.AddChildDirectory(name);
        }
    }
}