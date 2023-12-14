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
using Vault.Serialization;
using Vault.Storage;
using Vault.Storage.InMemory;

public static class VaultEntryPoint
{
    public static void Main()
    {
        RunAll();
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
        DefaultTypeSet.Setup(1, v =>
        {
            return new[]
            {
                typeof(StringContent),
                typeof(FileContent),
                typeof(DirectoryContent),
                typeof(Box<StringContent>),
                typeof(Box<FileContent>),
                typeof(Box<DirectoryContent>),
                typeof(PlaneDataEncryptionSource),
                typeof(XorEncryptionSource)
            };
        });
        
        InMemoryStorage storage = new InMemoryStorage(
            new Box<StringContent>(new StringContent("root")),
            new Box<DirectoryContent>(new DirectoryContent(new PlaneDataEncryptionSource())));

        var commandsFactory = CommandsFactory.ConstructFullFactory();

        var commandsProcessor = new CommandsProcessor(storage, Console.ReadLine, Console.Out);
        
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

        foreach (var cmd in _initCommands)
        {
            var result = commandsProcessor.Process(cmd);
            if (result is FailResult fail)
            {
                fail.WriteTo(consoleOutput);
            }
        }
        
        Console.WriteLine(SerializerJson.Serialize(storage));

        var commandSource = new TextReaderCommandSource(Console.In, commandsFactory);
        commandSource.OnError += ex => Console.WriteLine(ex);
        
        
        foreach (var cmd in commandSource.GetAll())
        {
            var result = commandsProcessor.Process(cmd);
            result.WriteTo(consoleOutput);
            consoleOutput.FinishBlock();
        }
    }
}