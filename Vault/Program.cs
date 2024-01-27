using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vault.Content;
using Vault.Encryption;
using Vault.Repository;
using Vault.Commands;
using Vault.FileSystem;
using Vault.Serialization;
using Vault.Storage;
using Vault.Storage.FileSystem;

public static class VaultEntryPoint
{
    public static Task Main()
    {
        return RunAll();
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

    private static async Task RunAll()
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

        var fileSystem = new InMemoryTextFileSystem();
        await FileSystemStorage.InitializeFS(fileSystem);
        var storage = new FileSystemStorage(
            fileSystem,
            new IncrementalNodeIdSource());
        
        var commandsFactory = CommandsFactory.ConstructFullFactory();

        var commandsProcessor = new CommandsProcessor(storage, Console.ReadLine, Console.Out);
        await commandsProcessor.Prepare();
        
        var consoleOutput = new OutputTextStream(Console.Out, () => ".", () =>
        {
            string prompt = commandsProcessor.Current.GetName();
            INode? c = commandsProcessor.Current.Parent;
            while (c != null)
            {
                prompt = c.GetName() + "/" + prompt;
                c = c.Parent;
            }

            return prompt + "> ";
        });

        foreach (var cmd in _initCommands)
        {
            var result = await commandsProcessor.Process(cmd);
            if (result is FailResult fail)
            {
                fail.WriteTo(consoleOutput);
            }
        }
        
        //Console.WriteLine(SerializerJson.Serialize(fileSystem));

        var commandSource = new TextReaderCommandSource(Console.In, commandsFactory);
        commandSource.OnError += ex => Console.WriteLine(ex);
        
        
        foreach (var cmd in commandSource.GetAll())
        {
            var result = await commandsProcessor.Process(cmd);
            result.WriteTo(consoleOutput);
            consoleOutput.FinishBlock();
        }
    }
}