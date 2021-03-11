using System;
using System.Threading;
using TwitchLib.Client;

namespace PokeGuesser
{
    class Program
    {
        public static TwitchClient twitchClient { get; set; }
        public static bool con { get; set; }
        public static Pokemon pkmn { get; set; }

        static void Main(string[] args)
        {
            con = false;
            Console.WriteLine();
            Console.WriteLine("Welcome to the world of PokéGuesser!");
            Console.WriteLine("=================");

            #region Init Settings and Client
            // if settings are blank (default or something broke), initialize settings
            if (Properties.Settings.Default.botname == "" || !Properties.Settings.Default.oauth.StartsWith("oauth:") || Properties.Settings.Default.channel == "")
            {
                Console.WriteLine("Settings not found. Initializing settings.");
                CreateInitSettings();
            }
            else
            {
                ConsoleKeyInfo a;
                Console.WriteLine($"Bot: {Properties.Settings.Default.botname}");
                Console.WriteLine($"OAuth: [hidden]");
                Console.WriteLine($"Channel: {Properties.Settings.Default.channel}");
                Console.WriteLine();
                Console.WriteLine("To edit Twitch settings, type 't'");
                Console.WriteLine("To edit extra settings, type 'e'");
                Console.WriteLine("To continue and connect to Twitch, press enter");
                do
                {
                    a = Console.ReadKey();
                } while (!a.KeyChar.Equals('\r') && !a.KeyChar.Equals('e') && !a.KeyChar.Equals('t'));

                if (a.KeyChar.Equals('t'))
                {
                    Console.WriteLine();
                    CreateInitSettings();
                }
                else if (a.KeyChar.Equals('e'))
                {
                    Console.WriteLine();
                    CreateExtraSettings();
                }
                Console.WriteLine();
            }


            // connection to twitch
            try
            {
                if (!Properties.Settings.Default.oauth.StartsWith("oauth:"))
                {
                    Properties.Settings.Default.oauth = "oauth:" + Properties.Settings.Default.oauth;
                }
                ConnectionCredentials credentials = new ConnectionCredentials(Properties.Settings.Default.botname, Properties.Settings.Default.oauth);
                twitchClient = new TwitchClient();
                twitchClient.Initialize(credentials);
                twitchClient.OnConnected += TwitchClient_OnConnected;
                //twitchClient.OnMessageReceived += TwitchClient_OnMessageReceived;
                twitchClient.OnChatCommandReceived += TwitchClient_OnChatCommandReceived;
            }
            catch (Exception e)
            {
                Console.WriteLine();
                Console.WriteLine("= ERROR =");
                Console.WriteLine(e.Message);
                Properties.Settings.Default.botname = "";
                Properties.Settings.Default.oauth = "";
                Properties.Settings.Default.Save();
                Console.WriteLine("Press any key to close. Please relaunch the application.");
                Console.ReadKey();
                Environment.Exit(1);
            }

            // helper for connection to pokeapi.co
            ApiHelper.InitializeClient();
            #endregion

            twitchClient.Connect();
            do
            {
                //TODO: infinite loop check
                Console.WriteLine("Connecting to Twitch...");
                Thread.Sleep(500);
            } while (!con);
            con = false;
            twitchClient.JoinChannel(Properties.Settings.Default.channel);
            Console.WriteLine($"{Properties.Settings.Default.botname} has connected to {Properties.Settings.Default.channel}'s channel");
            Console.WriteLine();
            Helper();

            // wait forever
            Thread.Sleep(Timeout.Infinite);
            Console.WriteLine("End of program");
        }

        #region Chat Stuff
        private static void TwitchClient_OnConnected(object sender, TwitchLib.Client.Events.OnConnectedArgs e)
        {
            Console.WriteLine("Connected~");
            con = true;
        }

        private static void TwitchClient_OnChatCommandReceived(object sender, TwitchLib.Client.Events.OnChatCommandReceivedArgs e)
        {

            switch (e.Command.CommandText.ToLower())
            {
                case "pokemon":
                    if (e.Command.ChatMessage.IsBroadcaster || (e.Command.ChatMessage.IsModerator && Properties.Settings.Default.modStart))
                    {
                        if (con)
                        {
                            Random random = new Random();
                            int rand = 0;
                            do
                            {
                                rand = random.Next(pkmn.PokemonSpecies.FlavorTextEntries.Length);
                            } while (pkmn.PokemonSpecies.FlavorTextEntries[rand].Language.Name != "en" || pkmn.PokemonSpecies.FlavorTextEntries[rand].FlavorText.ToLower().Contains(pkmn.Name));
                            pkmn.PokemonSpecies.FlavorCurrent = rand;
                            SendMessage("Bonus entry: " + pkmn.PokemonSpecies.FlavorTextEntries[rand].FlavorText);
                        }
                        else
                        {
                            try
                            {
                                GeneratePokemon(Int32.Parse(e.Command.ArgumentsAsList[0]));
                            }
                            catch (Exception)
                            {
                                GeneratePokemon();
                            }
                        }
                    }
                    else
                    {
                        if (con)
                        {
                            SendMessage(pkmn.PokemonSpecies.FlavorTextEntries[pkmn.PokemonSpecies.FlavorCurrent].FlavorText);
                        }
                    }

                    break;
                case "hint":
                    if (con)
                    {
                        if (e.Command.ChatMessage.IsBroadcaster || (e.Command.ChatMessage.IsModerator && Properties.Settings.Default.modHint))
                        {
                            try
                            {
                                string a;
                                try
                                {
                                    a = e.Command.ArgumentsAsList[0].ToLower();
                                }
                                catch (Exception)
                                {
                                    a = "type";
                                }

                                if (a == "random")
                                {
                                    Random r = new Random();
                                    string[] table = { "type", "ability", "height", "weight", "colour", "special", "random" };
                                    a = table[r.Next(table.Length)];
                                }

                                switch (a)
                                {
                                    case "type":
                                        string s = "";
                                        foreach (var type in pkmn.Types)
                                        {
                                            s += (type.Type.Name) + " ";
                                        }
                                        SendMessage("This Pokémon's typing is " + s);
                                        break;
                                    case "ability":
                                    case "abilities":
                                        string t = "";
                                        foreach (var abs in pkmn.Abilities)
                                        {
                                            t += (abs.Ability.Name) + " ";
                                        }
                                        SendMessage("This Pokémon's abilities are: " + t);
                                        break;
                                    case "height":
                                    case "weight":
                                    case "biometrics":
                                        SendMessage("This Pokémon is " + (pkmn.Height / 10.0) + "m in size and weighs " + (pkmn.Weight / 10.0) + "kg");
                                        break;
                                    case "color":
                                    case "colour":
                                        SendMessage("This Pokémon is " + pkmn.PokemonSpecies.Color.Name);
                                        break;
                                    case "special":
                                        if (pkmn.PokemonSpecies.IsBaby)
                                        {
                                            SendMessage("This is a baby Pokémon");
                                        }
                                        else if (pkmn.PokemonSpecies.IsLegendary)
                                        {
                                            SendMessage("This is a legendary Pokémon");
                                        }
                                        else if (pkmn.PokemonSpecies.IsMythical)
                                        {
                                            SendMessage("This is a mythical Pokémon");
                                        }
                                        else
                                        {
                                            SendMessage("This is not a baby, legendary, or mythical Pokémon");
                                        }
                                        break;
                                    case "name":
                                        SendMessage("This Pokémon's Name is " + pkmn.Name.Length + " characters long");
                                        break;
                                    case "help":
                                        SendMessage("The arguments are: type, ability, height/weight/biometrics, color/colour, special, name, random");
                                        break;
                                    default:
                                        SendMessage("No arguments identified. Possible arguments are: type, ability, height, weight, colour, special, random");
                                        break;
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine();
                                Console.WriteLine("== ERROR ==");
                                if (pkmn == null)
                                {
                                    Console.WriteLine("pkmn is null");
                                }
                                Console.WriteLine(ex.Message);
                                Console.WriteLine("== if you see this, yell at the dev ==");
                                Console.WriteLine("The program should be safe to continue without relaunching");
                            }
                        }

                    }

                    break;
                case "tellme":
                    if (e.Command.ChatMessage.IsBroadcaster)
                    {
                        try
                        {
                            Console.WriteLine();
                            Console.WriteLine("Current Pokémon: " + pkmn.PokemonSpecies.Name.Substring(0, 1).ToUpper() + pkmn.PokemonSpecies.Name.Substring(1));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine();
                            Console.WriteLine("== ERROR ==");
                            Console.WriteLine(ex.Message);
                        }
                    }
                    break;
                case "help":
                    if (con && e.Command.ChatMessage.IsBroadcaster)
                    {
                        Helper();
                    }
                    break;
                case "end":
                    if (con && (e.Command.ChatMessage.IsBroadcaster || (e.Command.ChatMessage.IsModerator && Properties.Settings.Default.modStart)))
                    {
                        con = false;
                        SendMessage("Guessing game ended! The Pokémon was " + pkmn.Name.Substring(0, 1).ToUpper() + pkmn.Name.Substring(1) + "!");
                    }
                    break;
                case "guess":
                    if (e.Command.ArgumentsAsList.Count >= 1 && con)
                    {
                        if (e.Command.ArgumentsAsString.ToLower() == pkmn.Name || e.Command.ArgumentsAsString.ToLower() == pkmn.PokemonSpecies.Name)
                        {
                            SendMessage("Congratulations @" + e.Command.ChatMessage.Username + "! The Pokémon was " + pkmn.Name.Substring(0, 1).ToUpper() + pkmn.Name.Substring(1));
                            con = false;
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private static void GeneratePokemon()
        {
            Random r = new Random();
            int generation;
            generation = r.Next(Properties.Settings.Default.defaultGenerationMin, (Properties.Settings.Default.defaultGenerationMax + 1));
            GeneratePokemon(generation);
        }

        private static async void GeneratePokemon(int generation)
        {
            try
            {
                pkmn = null;

                Random r = new Random();

                int id;
                int rand;

                switch (generation)
                {
                    case 1:
                        id = r.Next(1, 152);
                        break;
                    case 2:
                        id = r.Next(152, 252);
                        break;
                    case 3:
                        id = r.Next(252, 387);
                        break;
                    case 4:
                        id = r.Next(387, 495);
                        break;
                    case 5:
                        id = r.Next(495, 650);
                        break;
                    case 6:
                        id = r.Next(650, 722);
                        break;
                    case 7:
                        id = r.Next(722, 810);
                        break;
                    case 8:
                        id = r.Next(810, 898);
                        break;
                    default:
                        id = r.Next(1, 898);
                        break;
                }

                pkmn = await ApiGetPokemon.GetPokemon(id + "");

                do
                {
                    rand = r.Next(pkmn.PokemonSpecies.FlavorTextEntries.Length);
                } while (pkmn.PokemonSpecies.FlavorTextEntries[rand].Language.Name != "en" || pkmn.PokemonSpecies.FlavorTextEntries[rand].FlavorText.ToLower().Contains(pkmn.Name) || pkmn.PokemonSpecies.FlavorTextEntries[rand].FlavorText.ToLower().Contains(pkmn.PokemonSpecies.Name));

                SendMessage(pkmn.PokemonSpecies.FlavorTextEntries[rand].FlavorText + " (" + pkmn.PokemonSpecies.Generation.Name + ")");
                if (Properties.Settings.Default.tellMe)
                {
                    Console.WriteLine();
                    Console.WriteLine("Current Pokémon: " + pkmn.PokemonSpecies.Name.Substring(0, 1).ToUpper() + pkmn.PokemonSpecies.Name.Substring(1));
                }
                con = true;
            }
            catch (Exception e)
            {
                Console.WriteLine();
                Console.WriteLine("== ERROR ==");
                Console.WriteLine(e.Message);
                if (pkmn == null)
                {
                    Console.WriteLine("pkmn is null");
                }
                Console.WriteLine("== if you see this, yell at the dev ==");
                Console.WriteLine("The program should be safe to continue without relaunching");
            }
        }

        static void SendMessage(string s)
        {
            if (twitchClient.IsConnected)
            {
                twitchClient.SendMessage(Properties.Settings.Default.channel, s);
            }
        }
        #endregion

        private static void Helper()
        {
            Console.WriteLine("========== HOW TO USE ==========");
            Console.WriteLine("The broadcaster has multiple commands for the guessing game");
            Console.WriteLine("!pokemon - start a guessing game");
            Console.WriteLine(" - If one is already started, show another dex entry for that Pokémon");
            Console.WriteLine(" - You can specify a generation with a number. ex: !pokemon 3 for gen 3");
            Console.WriteLine("!hint - display a hint");
            Console.WriteLine(" - Different hints can be specified as an argument. ex: !hint type");
            Console.WriteLine(" - The hints are: type, ability, biometrics, colour, special, name, random");
            Console.WriteLine(" - biometrics gives the height and weight of the Pokémon");
            Console.WriteLine("'height' and 'weight' can also be used as arguments instead");
            Console.WriteLine(" - special denotes if a Pokémon is a baby, legendary or mythical Pokémon");
            Console.WriteLine(" - name will give the length of the Pokémon's name");
            Console.WriteLine("!tellme - tells you the Pokémon");
            Console.WriteLine(" - this command will display the Pokémon in this Console");
            Console.WriteLine("!end - end the current game");
            Console.WriteLine("Viewers can use the !guess command to guess the Pokémon. ex: !guess sandshrew");
            Console.WriteLine("If someone guesses correctly, the game will end");
            Console.WriteLine("================================");
            Console.WriteLine();
        }

        private static bool CreateInitSettings()
        {
            Console.WriteLine();
            Console.WriteLine("===== Settings =====");
            Console.WriteLine("Enter the bot's twitch username:");
            Properties.Settings.Default.botname = Console.ReadLine().ToLower();
            Console.WriteLine("Enter the bot's OAuth token - you can find it at twitchapps.com/tmi");
            Properties.Settings.Default.oauth = Console.ReadLine();
            Console.WriteLine("Enter the twitch account the bot will join:");
            Properties.Settings.Default.channel = Console.ReadLine();

            string accept;
            do
            {
                Console.WriteLine();
                Console.WriteLine("Would you like to edit the extra settings? (y/n)");
                accept = Console.ReadLine().ToLower();
            } while (accept != "y" && accept != "n");
            if (accept == "y") CreateExtraSettings();

            string creds =
    $"Bot Username: \"{Properties.Settings.Default.botname}\"{Environment.NewLine}" +
    $"OAuth: \"{Properties.Settings.Default.oauth}\"{Environment.NewLine}" +
    $"Channel: \"{Properties.Settings.Default.channel}\"{Environment.NewLine}" +
    $"Default generations: {Properties.Settings.Default.defaultGenerationMin}-{Properties.Settings.Default.defaultGenerationMax}{Environment.NewLine}" +
    $"Answer in console: {Properties.Settings.Default.tellMe}{Environment.NewLine}" +
    $"Moderators can start game: {Properties.Settings.Default.modStart}{Environment.NewLine}" +
    $"Moderators can request hint: {Properties.Settings.Default.modHint}";

            do
            {
                Console.WriteLine();
                Console.WriteLine("=================" + Environment.NewLine + creds + Environment.NewLine + "Are these settings ok? (y/n)");
                accept = Console.ReadLine().ToLower();
            } while (accept != "y" && accept != "n");


            if (accept == "y")
            {
                Properties.Settings.Default.Save();
                Console.WriteLine("Settings have been saved");

                return true;
            }
            else
            {
                return false;
            }
        }

        private static void CreateExtraSettings()
        {
            Console.WriteLine();
            Console.WriteLine("== Extra settings ==");
            bool goodInt;
            string tempstring = "";
            int tempint = 0;
            do
            {
                do
                {
                    Console.WriteLine("Enter the minimum generation by default (1-8, or type help)");
                    tempstring = Console.ReadLine();
                    goodInt = Int32.TryParse(tempstring, out tempint);
                    if (tempstring.ToLower() == "help")
                    {
                        Console.WriteLine();
                        Console.WriteLine("By default, the !pokemon command will pick a random generation from gen 1-8");
                        Console.WriteLine("if you would like to exclude certain generations, you can edit the default");
                        Console.WriteLine("ex: if the minimum generation is 2, all gen 1 Pokémon will be excluded,");
                        Console.WriteLine("  but gen 1 Pokémon can still be picked with '!pokemon 1'");
                    }
                } while (!goodInt);
                Properties.Settings.Default.defaultGenerationMin = tempint;

                do
                {
                    Console.WriteLine();
                    Console.WriteLine("Enter the maximum generation by default (1-8, or type help)");
                    tempstring = Console.ReadLine();
                    goodInt = Int32.TryParse(tempstring, out tempint);
                    if (tempstring.ToLower() == "help")
                    {
                        Console.WriteLine();
                        Console.WriteLine("By default, the !pokemon command will pick a random generation from gen 1-8");
                        Console.WriteLine("if you would like to exclude certain generations, you can edit the default");
                        Console.WriteLine("ex: if the maximum generation is 3, all gen 4+ Pokémon will be excluded,");
                        Console.WriteLine("  but gen 4 Pokémon can still be picked with '!pokemon 4'");
                        Console.WriteLine("For defaults, the maximum must be equal or larger than the minimum.");
                    }
                } while (!goodInt);
                Properties.Settings.Default.defaultGenerationMax = tempint;
                if (Properties.Settings.Default.defaultGenerationMin > Properties.Settings.Default.defaultGenerationMax)
                {
                    Console.WriteLine("The maximum default generation must be equal or larger than the minimum.");
                    Console.WriteLine("================");
                }
            } while (Properties.Settings.Default.defaultGenerationMin > Properties.Settings.Default.defaultGenerationMax);

            string accept;
            do
            {
                Console.WriteLine();
                Console.WriteLine("Should the Pokémon be posted in this console when the game starts? (y/n)");
                accept = Console.ReadLine().ToLower();
            } while (accept != "y" && accept != "n");
            Properties.Settings.Default.tellMe = ((accept == "y") ? true : false);

            do
            {
                Console.WriteLine();
                Console.WriteLine("Should moderators be allowed to start and end the game? (y/n)");
                accept = Console.ReadLine().ToLower();
            } while (accept != "y" && accept != "n");
            Properties.Settings.Default.modStart = ((accept == "y") ? true : false);

            do
            {
                Console.WriteLine();
                Console.WriteLine("Should moderators be allowed to ask for hints? (y/n)");
                accept = Console.ReadLine().ToLower();
            } while (accept != "y" && accept != "n");
            Properties.Settings.Default.modHint = ((accept == "y") ? true : false);
        }
    }
}
