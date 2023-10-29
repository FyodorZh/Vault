using Vault;
using Vault.Core;
using Vault.Repository;
using Vault.Repository.InMemory;

public static class VaultEntryPoint
{
    public static void Main()
    {
        IRepository repository = new InMemoryRepository();
        IDirectoryNode root = repository.InitNew();
        
        var a = root.AddChildFile("a", new ValueContent<string>("Text for A"));
        var b = root.AddChildDirectory("b");
        var c = root.AddChildFile("c", new ValueContent<string>("Text for C"));

        b.AddChildFile("bb", new ValueContent<string>("Text for BBBB"));

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