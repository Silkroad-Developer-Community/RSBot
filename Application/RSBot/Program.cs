using CommandLine;
using CommandLine.Text;
using RSBot.Core;
using RSBot.Core.Components;
using RSBot.Core.Event;
using RSBot.Core.Objects;
using RSBot.Views;
using RSBot.Chat;
using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using RSBot.General;

namespace RSBot;

internal static class Program
{
    public static string AssemblyTitle = Assembly
        .GetExecutingAssembly()
        .GetCustomAttribute<AssemblyProductAttribute>()
        ?.Product;

    public static string AssemblyVersion =
        $"v{Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version}";

    public static string AssemblyDescription = Assembly
        .GetExecutingAssembly()
        .GetCustomAttribute<AssemblyDescriptionAttribute>()
        ?.Description;

    public class CommandLineOptions
    {
        [Option('c', "character", Required = false, HelpText = "Set the character name to use.")]
        public string Character { get; set; }

        [Option('p', "profile", Required = false, HelpText = "Set the profile name to use.")]
        public string Profile { get; set; }

        [Option("launch-client", Required = false, HelpText = "Start with client")]
        public bool LaunchClient { get; set; }

        [Option("launch-clientless", Required = false, HelpText = "Start clientless")]
        public bool LaunchClientless { get; set; }
        [Option("headless", Required = false, HelpText = "Start the bot without graphical user interface")]
        public bool Headless { get; set; }
    }

    private static void DisplayHelp(ParserResult<CommandLineOptions> result)
    {
        var helpText = HelpText.AutoBuild(
            result,
            h =>
            {
                h.AdditionalNewLineAfterOption = false;
                h.AddDashesToOption = true;
                return HelpText.DefaultParsingErrorsHandler(result, h);
            }
        );
        MessageBox.Show(
            helpText,
            AssemblyTitle + " " + AssemblyVersion,
            MessageBoxButtons.OK,
            MessageBoxIcon.Information
        );
    }

    [STAThread]
    private static void Main(string[] args)
    {
        var parser = new Parser(with => with.HelpWriter = Console.Out);
        var parserResult = parser.ParseArguments<CommandLineOptions>(args);

        bool isHeadless = false;

        parserResult
            .WithParsed(options =>
            {
                RunOptions(options);
                isHeadless = options.Headless;
            })
            .WithNotParsed(errs =>
            {
                DisplayHelp(parserResult);
                Environment.Exit(1);
            });

        //CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

        // We need "." instead of "," while saving float numbers
        // Also client data is "." based float digit numbers
        CultureInfo.CurrentCulture = new CultureInfo("en-US");

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        if (isHeadless)
        {
            RunHeadless();
        }
        else
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);

            Main mainForm = new Main();
            SplashScreen splashScreen = new SplashScreen(mainForm);            
            splashScreen.ShowDialog();

            Application.Run(mainForm);
        }
    }
    private static void RunHeadless()
    {
        //Main mainForm = new Main();
        BotCL.Initialize(ProfileManager.SelectedProfile);

        EventManager.FireEvent("OnLoadPlugins");

        EventManager.FireEvent("OnLoadBotbases");

        bool running = true;
        while (running)
        {
            Console.Write("> ");
            var input = Console.ReadLine()?.ToLower().Split(',');
            if (input == null || input.Length == 0) continue;

            var command = input[0];
            var args = input.Skip(1).ToArray();

            switch (command)
            {
                case "start":
                    Kernel.Bot?.Start();
                    Console.WriteLine("Bot started");
                    break;

                case "stop":
                    Kernel.Bot?.Stop();
                    Console.WriteLine("Bot stopped");
                    break;
                case "start client":
                    _ = GeneralPlugin.Instance.Manager.StartClientAsync();
                    Console.WriteLine("Client started");
                    break;
                case "show":
                    ClientManager.SetVisible(true);
                    break;
                case "hide":
                    ClientManager.SetVisible(false);
                    break;
                case "chat":
                    HandleChatCommand(args);
                    break;

                case "status":
                    Console.WriteLine($"Character: {Game.Player?.Name ?? "Not logged in"}");
                    Console.WriteLine($"Bot running: {Kernel.Bot?.Running ?? false}");
                    break;

                default:
                    Log.Warn($"Unknown Command: {command}");
                    break;
            }
        }
    }
    private static void HandleChatCommand(string[] args)
    {
        if (args.Length < 2)
        {
            return;
        }

        if (!Enum.TryParse<ChatType>(args[0], true, out var type)) return;

        var message = args[1];
        var receiver = args.Length > 2 ? args[2] : null;

        ChatManager.Send(type, message, receiver);
        Console.WriteLine($"Message ({type}): {message}.");
    }
    private static void RunOptions(CommandLineOptions options)
    {
        if (options.LaunchClient)
        {
            Kernel.LaunchMode = "client";
            Log.Debug("Launching with client dictated by launch paramaters");
        }
        else if (options.LaunchClientless)
        {
            Kernel.LaunchMode = "clientless";
            Log.Debug("Launching client as clientless dictated by launch paramaters");
        }

        if (!string.IsNullOrEmpty(options.Profile))
        {
            var profile = options.Profile;
            if (ProfileManager.ProfileExists(profile))
                ProfileManager.SetSelectedProfile(profile);
            else
                ProfileManager.Add(profile);

            ProfileManager.IsProfileLoadedByArgs = true;
            Log.Debug($"Selected profile by args: {profile}");
        }

        if (!string.IsNullOrEmpty(options.Character))
        {
            var character = options.Character;
            ProfileManager.SelectedCharacter = character;
            Log.Debug($"Selected character by args: {character}");
        }
    }
}
