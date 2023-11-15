using System;
using System.Collections.Generic;
using System.Linq;
using Vault.Content;
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
            Console.WriteLine("Name: " + CurrentNode.Name);
            if (CurrentNode.Content.Value != null)
            {
                CurrentNode.Content.Value.WriteTo(Console.Out);
            }
            else
            {
                Console.WriteLine("Encryption: ???");
            }
            
            bool bWritten = false;
            foreach (var elementName in CurrentNode.ChildrenNames.All.Select(node =>
                     {
                         string name = node.Name;
                         if (node is IDirectoryNode)
                         {
                             name = "<" + name + ">";
                         }
                         return name;
                     }).Order())
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
            var child = CurrentNode.ChildrenNames.FindChild(name);
            if (child == null)
            {
                Console.WriteLine("File " + name + " not found");
            }
            else
            {
                if (child is IFileNode file)
                {
                    if (file.Content.Value != null)
                    {
                        file.Content.Value.WriteTo(Console.Out);
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

            var child = CurrentNode.ChildrenNames.FindChild(name);
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
            var child = CurrentNode.ChildrenNames.FindChild(name);
            if (child != null)
            {
                Console.WriteLine("File or directory already exists");
                return;
            }
            
            CurrentNode.ChildrenContent.AddChildFile(name, new StringContent(text));
        }
        
        public void Command_mkdir(string name)
        {
            var child = CurrentNode.ChildrenNames.FindChild(name);
            if (child != null)
            {
                Console.WriteLine("File or directory already exists");
                return;
            }
            
            CurrentNode.ChildrenContent.AddChildDirectory(name);
        }
        
        private void LockUnlockReport(LockUnlockResult res, string text)
        {
            Console.Write(text + ": ");
            switch (res)
            {
                case LockUnlockResult.NothingToDo:
                    Console.WriteLine("NothingToDo");
                    break;
                case LockUnlockResult.Success:
                    Console.WriteLine("Success");
                    break;
                case LockUnlockResult.Fail:
                    Console.WriteLine("Fail");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(res), res, null);
            }
        }

        public void Command_lock(string cmd)
        {
            switch (cmd)
            {
                case "all":
                    LockUnlockReport(CurrentNode.ChildrenContent.Lock(), "ChildrenContent");
                    LockUnlockReport(CurrentNode.ChildrenNames.Lock(), "ChildrenNames");
                    break;
                case "content":
                    LockUnlockReport(CurrentNode.ChildrenContent.Lock(), "ChildrenContent");
                    break;
                case "names":
                    LockUnlockReport(CurrentNode.ChildrenNames.Lock(), "ChildrenNames");
                    break;
                default:
                    Console.WriteLine("Error: Wrong lock command. Allowed: all/names/content");
                    break;
            }
        }

        public void Command_unlock(string cmd)
        {
            switch (cmd)
            {
                case "all":
                    LockUnlockReport(CurrentNode.ChildrenNames.Unlock(), "ChildrenNames");
                    LockUnlockReport(CurrentNode.ChildrenContent.Unlock(), "ChildrenContent");
                    break;
                case "content":
                    LockUnlockReport(CurrentNode.ChildrenContent.Unlock(), "ChildrenContent");
                    break;
                case "names":
                    LockUnlockReport(CurrentNode.ChildrenNames.Unlock(), "ChildrenNames");
                    break;
                default:
                    Console.WriteLine("Error: Wrong unlock command. Allowed: all/names/content");
                    break;
            }
        }
    }
}