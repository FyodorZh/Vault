using System;
using System.Collections.Generic;
using System.Linq;
using OrderedSerializer.JsonBackend;
using Vault;
using Vault.Content;
using Vault.Encryption;
using Vault.Repository;
using Vault.Repository.V1;
using Vault.Commands;
using Vault.Storage;
using Vault.Storage.InMemory;

public static class VaultEntryPoint
{
    private class TmpCredentialsProvider : ICredentialsProvider
    {
        string? ICredentialsProvider.GetCredentials(IDirectoryNode dir, EncryptionDesc encryptionDesc, string text)
        {
            Console.Write("Enter credentials for " + text + ": ");
            return Console.ReadLine();
        }
    }

    public static void Main()
    {
        RunAll();
        
        InMemoryStorage storage = new InMemoryStorage(
            new Box<StringContent>(new StringContent("root")),
            new Box<DirectoryContent>(new DirectoryContent()));

        IRepository repository = new RepositoryV1(storage, new TmpCredentialsProvider());
        var root = repository.GetRoot();
        //root.Unlock(LockState.All);
        var a = root.ChildrenContent.AddChildFile("a", "Text for A");
        var b = root.ChildrenContent.AddChildDirectory("b");
        var c = root.ChildrenContent.AddChildFile("c", "Text for C");
        b.ChildrenContent.AddChildFile("bb", "Text for BBBB");

        //root.LockAll();
        //root.Unlock(LockState.ChildrenName);
        
        VaultConsole vaultConsole = new VaultConsole(root); 
        
        
        Console.Clear();
        while (true)
        {
            vaultConsole.Prompt();
            var str = Console.ReadLine()?.Trim(' ');
            if (str == null)
            {
                break;
            }
            string[] cmd = str.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries );
            if (cmd.Length == 0 || cmd[0] == "exit")
            {
                break;
            }

            switch (cmd[0])
            {
                case "cls":
                    Console.Clear();
                    break;
                case "ls":
                    if (cmd.Length == 1)
                    {
                        vaultConsole.Command_ls();
                    }
                    else
                    {
                        Console.WriteLine("Error!");
                    }
                    break;
                case "cat":
                    if (cmd.Length == 2)
                    {
                        vaultConsole.Command_cat(cmd[1]);
                    }
                    else
                    {
                        Console.WriteLine("Error!");
                    }
                    break;
                case "cd":
                    if (cmd.Length == 2)
                    {
                        vaultConsole.Command_cd(cmd[1]);
                    }
                    else
                    {
                        Console.WriteLine("Error!");
                    }
                    break;
                case "add":
                    if (cmd.Length == 3)
                    {
                        vaultConsole.Command_add(cmd[1], cmd[2]);
                    }
                    else
                    {
                        Console.WriteLine("Error!");
                    }
                    break;
                case "mkdir":
                    if (cmd.Length == 2)
                    {
                        vaultConsole.Command_mkdir(cmd[1]);
                    }
                    else
                    {
                        Console.WriteLine("Error!");
                    }
                    break;
                case "lock":
                    if (cmd.Length == 1)
                    {
                        vaultConsole.Command_lock("all");
                    }
                    else if (cmd.Length == 2)
                    {
                        vaultConsole.Command_lock(cmd[1]);
                    }
                    else
                    {
                        Console.WriteLine("Error");
                    }
                    break;
                case "unlock":
                    if (cmd.Length == 1)
                    {
                        vaultConsole.Command_unlock("all");
                    }
                    else if (cmd.Length == 2)
                    {
                        vaultConsole.Command_unlock(cmd[1]);
                    }
                    else
                    {
                        Console.WriteLine("Error");
                    }
                    break;
                case "encrypt":
                    if (cmd.Length == 3)
                    {
                        vaultConsole.Command_encrypt(cmd[1], cmd[2]);
                    }
                    else
                    {
                        Console.WriteLine("Error");
                    }
                    break;
                case "help":
                    break;
                default:
                    Console.WriteLine("Unknown command: " + cmd[0]);
                    break;
            }
        }
        Console.WriteLine("Exited.");
    }

    private static readonly List<ICommand> _initCommands = new List<ICommand>()
    {
        new AddCommand("a", "text for a"),
        new AddCommand("b", "text for b"),
        new MkdirCommand("d"),
        new CdCommand("d"),
        new AddCommand("file", "1234"),
        new CdCommand("..")
    };

    private static void RunAll()
    {
        OrderedSerializer.JsonBackend.JsonWriter jsonWriter = new JsonWriter();

        jsonWriter.WriteBool(true);
        jsonWriter.WriteByte(1);
        jsonWriter.WriteChar('a');
        jsonWriter.WriteShort(-1);
        jsonWriter.WriteInt(100000);
        jsonWriter.WriteFloat(0.5f);
        jsonWriter.WriteString(null);
        jsonWriter.WriteString("hel\\lo\n\"");
        jsonWriter.WriteLong(1L << 60);
        jsonWriter.BeginSection();
        jsonWriter.WriteString("Section");
        jsonWriter.WriteDouble(0.3);
        jsonWriter.EndSection();
        string str = jsonWriter.ToString();


        InMemoryStorage storage = new InMemoryStorage(
            new Box<StringContent>(new StringContent("root")),
            new Box<DirectoryContent>(new DirectoryContent()));

        var commandsFactory = CommandsFactory.ConstructFullFactory();

        var commandsProcessor = new CommandsProcessor(storage);

        var commandSource = new TextReaderCommandSource(Console.In, commandsFactory);
        commandSource.OnError += ex => Console.WriteLine(ex);

        var consoleOutput = new OutputTextStream(Console.Out, () => ".", () =>
        {
            string prompt = commandsProcessor.Current.Name;
            INode? c = commandsProcessor.Current.Parent;
            while (c != null)
            {
                prompt = c.Name + "/" + prompt;
                c = c.Parent;
            }

            return prompt + "> ";
        });
        
        foreach (var cmd in _initCommands.Concat(commandSource.GetAll()))
        {
            var result = commandsProcessor.Process(cmd);
            result.WriteTo(consoleOutput);
            consoleOutput.FinishBlock();
        }
    }
}