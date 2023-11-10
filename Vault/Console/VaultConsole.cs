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

            if (node is IDirectoryNode)
            {
                name += "[";
                name += ((node.State & LockState.ChildrenName) != 0) ? "?" : "n";
                name += ",";
                name += ((node.State & LockState.Content) != 0) ? "?" : "c";
                name += "]";
                if (showDirectory)
                {
                    name = "<" + name + ">";
                }
            }
            else
            {
                name += "[";
                name += ((node.State & LockState.Content) != 0) ? "?" : "c";
                name += "]";
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


        private void PrintEncryptionInfo(bool locked, EncryptionDesc encryptionDesc)
        {
            Console.Write($"Method='{encryptionDesc.MethodName}' ");
            Console.Write($"State='{(locked ? "Locked" : "Unlocked")}' ");
            if (encryptionDesc.RequireCredentials)
            {
                Console.Write($"Credentials={(encryptionDesc.HasCredentials ? "PRESENT" : "ABSENT")}");
            }
            Console.WriteLine();
        }
        private void PrintDirInfo(IDirectoryNode dir)
        {
            Console.WriteLine("Name: " + (dir.Name ?? "???"));
            Console.WriteLine("Encryption:");
            if (dir.ChildrenNamesEncryption != null)
            {
                Console.Write($"- Children: ");
                PrintEncryptionInfo((dir.State & LockState.ChildrenName) != 0, dir.ChildrenNamesEncryption);
            }
            Console.Write($"- Content: ");
            PrintEncryptionInfo((dir.State & LockState.Content) != 0, dir.ContentEncryption);
        }

        public void Command_ls()
        {
            PrintDirInfo(CurrentNode);
            
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
                    file.Content.WriteTo(Console.Out);
                    Console.WriteLine();
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
            
            CurrentNode.AddChildDirectory(name, new PlaneDataEncryptionSource());
        }
    }
}