using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MomentumLauncher;
using Newtonsoft.Json;
using Spectre.Console;

namespace ChannelLauncher
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            Console.Title = "Momentum Launcher";

            while (true)
            {
                AnsiConsole.Clear();
                DisplayMainMenu();

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[bold dodgerblue1]What would you like to do?[/]")
                        .PageSize(5)
                        .AddChoices(new[]
                        {
                            "Start game", "Change Fortnite path", "Join Discord", "Exit"
                        }));

                switch (choice)
                {
                    case "Start game":
                        await StartGame();
                        break;
                    case "Change Fortnite path":
                        await ChangeFortnitePath();
                        break;
                    case "Join Discord":
                        JoinDiscord();
                        break;
                    case "Exit":
                        Environment.Exit(0);
                        break;
                }
            }
        }

        private static void DisplayMainMenu()
        {
            AnsiConsole.MarkupLine($"Welcome, {Environment.UserName}, to the [bold underline dodgerblue1]Momentum launcher.[/]");
            AnsiConsole.MarkupLine("You can select an option using the arrow keys [bold dodgerblue1]UP[/] and [bold dodgerblue1]DOWN[/].");
        }

        private static async Task StartGame()
        {
            string appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MomentumLauncher");
            string dllName = "Buzz.dll";
            string dllDownload = "https://cdn.nexusfn.net/file/2023/06/TV.dll";

            if (!File.Exists(Path.Combine(appData, "path.txt")))
            {
                AnsiConsole.MarkupLine("[red]Fortnite path is missing, please set it first![/]");
                await Task.Delay(2000);
                return;
            }

            if (!File.Exists(Path.Combine(appData, "email.txt")) || !File.Exists(Path.Combine(appData, "password.txt")))
            {
                AnsiConsole.MarkupLine("[red]Please Enter Your Email and Password! (Account Made From Discord Bot.)[/]");

                Console.Write("Email: ");
                string email = Console.ReadLine();
                File.WriteAllText(Path.Combine(appData, "email.txt"), email);

                Console.Write("Password: ");
                string password = Console.ReadLine();
                File.WriteAllText(Path.Combine(appData, "password.txt"), password);

                AnsiConsole.MarkupLine("[green]Email and Password Saved![/]");
            }

            if (!File.Exists("redirect.json"))
            {
                string fileContent = JsonConvert.SerializeObject(new { name = dllName, download = dllDownload });
                File.WriteAllText("redirect.json", fileContent);
            }

            string fileData = File.ReadAllText("redirect.json");
            var jsonData = JsonConvert.DeserializeObject<Dictionary<string, string>>(fileData);

            if (!jsonData.TryGetValue("name", out dllName) || !jsonData.TryGetValue("download", out dllDownload))
            {
                throw new Exception("Invalid JSON structure: 'name' or 'download' keys are missing or not strings");
            }

            if (!dllName.EndsWith(".dll"))
            {
                dllName += ".dll";
            }

            Utilities.StartGame(File.ReadAllText(Path.Combine(appData, "path.txt")), dllDownload, dllName);
            AnsiConsole.MarkupLine("Starting the client");
            await Task.Delay(2000);
        }

        private static async Task ChangeFortnitePath()
        {
            AnsiConsole.MarkupLine("Please enter the path to your Fortnite folder");
            string setPath = AnsiConsole.Ask<string>("Path: ");
            string fortnitePath = Path.Combine(setPath, "FortniteGame", "Binaries", "Win64");

            if (Directory.Exists(fortnitePath))
            {
                Console.WriteLine(fortnitePath);
                string appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MomentumLauncher");
                File.WriteAllText(Path.Combine(appData, "path.txt"), setPath);
                AnsiConsole.MarkupLine("Path changed, you can now start the game");
                await Task.Delay(2000);
            }
            else
            {
                Console.WriteLine(fortnitePath);
                AnsiConsole.MarkupLine("[red]Path is invalid![/]");
                await Task.Delay(2000);
            }
        }

        private static void JoinDiscord()
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "https://discord.gg/G4BnqZK9Q8",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Failed to open Discord: {ex.Message}[/]");
            }
        }

    }
}
