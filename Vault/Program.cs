﻿using System;
using Vault;
using Vault.Content;
using Vault.Encryption;
using Vault.Repository;
using Vault.Repository.V1;
using Vault.Storage;
using Vault.Storage.InMemory;

public static class VaultEntryPoint
{
    private class TmpCredentialsProvider : ICredentialsProvider
    {
        string? ICredentialsProvider.GetCredentials(IDirectoryNode dir, EncryptionDesc encryptionDesc)
        {
            Console.Write("Enter credentials: ");
            return Console.ReadLine();
        }
    }

    public static void Main()
    {
        InMemoryStorage storage = new InMemoryStorage(
            new Box<StringContent>(new StringContent("root")),
            new Box<DirectoryContent>(new DirectoryContent()));

        IRepository repository = new RepositoryV1(storage, new TmpCredentialsProvider());
        var root = repository.GetRoot();
        //root.Unlock(LockState.All);
        var a = root.Children2.AddChildFile("a", new StringContent("Text for A"));
        var b = root.Children2.AddChildDirectory("b");
        var c = root.Children2.AddChildFile("c", new StringContent("Text for C"));
        b.Children2.AddChildFile("bb", new StringContent("Text for BBBB"));

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
                        vaultConsole.Command_lock();
                    }
                    else
                    {
                        Console.WriteLine("Error");
                    }
                    break;
                case "unlock":
                    if (cmd.Length == 1)
                    {
                        vaultConsole.Command_unlock();
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
}