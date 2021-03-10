using System;
using System.IO;
using System.Threading;
using TwitchLib.Client;

namespace PokemonGuesser
{
    class Program
    {
        public static TwitchClient twitchClient;
        public static string channel;
        public static bool con = false;
        public static Pokemon pkmn;

        static void Main(string[] args)
        {
            Console.WriteLine();
            Console.WriteLine("Welcome to the world of Pokémon!");
            Console.WriteLine("=================");

            #region init strings
            string[] lines;
            try
            {
                lines = System.IO.File.ReadAllLines(Directory.GetCurrentDirectory() + "\\credentials.txt");
            }
            catch (Exception)
            {
                Console.WriteLine();
                Console.WriteLine(Directory.GetCurrentDirectory() + "\\credentials.txt was not found. Creating file.");
                bool acc = false;
                do
                {
                    acc = CreateInitFile();
                } while (!acc);

                lines = System.IO.File.ReadAllLines(Directory.GetCurrentDirectory() + "\\credentials.txt");
            }

            string botname, oauth = "";
            botname = lines[0].Substring(11);
            botname = botname.Substring(0, botname.Length - 1);
            channel = lines[2].Substring(10);
            channel = channel.Substring(0, channel.Length - 1);
            oauth = lines[1].Substring(8);
            oauth = oauth.Substring(0, oauth.Length - 1);
            #endregion

            #region init twitch client
            ConnectionCredentials credentials = new ConnectionCredentials(botname, oauth);
            twitchClient = new TwitchClient();
            twitchClient.Initialize(credentials);

            twitchClient.OnConnected += TwitchClient_OnConnected;
            twitchClient.OnMessageReceived += TwitchClient_OnMessageReceived;
            twitchClient.OnChatCommandReceived += TwitchClient_OnChatCommandReceived;

            ApiHelper.InitializeClient();
            #endregion

            twitchClient.Connect();
            do
            {
                Console.WriteLine("Connecting to Twitch...");
                Thread.Sleep(500);
            } while (!con);
            con = false;
            twitchClient.JoinChannel(channel);
            Console.WriteLine($"{botname} has connected to {channel}'s channel");
            Console.WriteLine();
            Herlp();


            // wait forever
            Thread.Sleep(Timeout.Infinite);
            Console.WriteLine("End of program");
        }

        #region Chat stuff
        private static void TwitchClient_OnConnected(object sender, TwitchLib.Client.Events.OnConnectedArgs e)
        {
            Console.WriteLine("Connected~");
            con = true;
        }

        private static void TwitchClient_OnMessageReceived(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
        {

        }

        private static void TwitchClient_OnChatCommandReceived(object sender, TwitchLib.Client.Events.OnChatCommandReceivedArgs e)
        {
            if (e.Command.ChatMessage.IsBroadcaster || e.Command.ChatMessage.UserId == "44114423")
            {
                switch (e.Command.CommandText.ToLower())
                {
                    case "pokemon":
                        if (con)
                        {
                            Random random = new Random();
                            int rand = 0;
                            do
                            {
                                rand = random.Next(pkmn.PokemonSpecies.FlavorTextEntries.Length);
                            } while (pkmn.PokemonSpecies.FlavorTextEntries[rand].Language.Name != "en" || pkmn.PokemonSpecies.FlavorTextEntries[rand].FlavorText.ToLower().Contains(pkmn.Name));
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
                        break;
                    case "hint":
                        if (con)
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
                            catch (Exception ee)
                            {
                                Console.WriteLine(ee.Message);
                                Console.WriteLine("== if you see this, yell at the dev ==");
                            }
                        }

                        break;
                    case "tellme":
                        try
                        {
                            Console.WriteLine();
                            Console.WriteLine("Current Pokémon: " + pkmn.Name.Substring(0, 1).ToUpper() + pkmn.Name.Substring(1));
                            Console.WriteLine();
                        }
                        catch (Exception)
                        {

                        }
                        break;
                    case "help":
                        Herlp();
                        break;
                    case "end":
                        if (con)
                        {
                            con = false;
                            SendMessage("Guessing game ended! The Pokémon was " + pkmn.Name.Substring(0, 1).ToUpper() + pkmn.Name.Substring(1) + "!");
                        }
                        break;
                    default:
                        break;
                }
            }

            if (e.Command.CommandText.ToLower() == "guess" && e.Command.ArgumentsAsList.Count >= 1 && con)
            {
                if (e.Command.ArgumentsAsString.ToLower() == pkmn.Name)
                {
                    SendMessage("Congratulations @" + e.Command.ChatMessage.Username + "! The Pokémon was " + pkmn.Name.Substring(0, 1).ToUpper() + pkmn.Name.Substring(1));
                    con = false;
                }
            }
        }

        private static async void GeneratePokemon(int generation = 0)
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
                } while (pkmn.PokemonSpecies.FlavorTextEntries[rand].Language.Name != "en" || pkmn.PokemonSpecies.FlavorTextEntries[rand].FlavorText.ToLower().Contains(pkmn.Name));

                SendMessage(pkmn.PokemonSpecies.FlavorTextEntries[rand].FlavorText + " (" + pkmn.PokemonSpecies.Generation.Name + ")");
                con = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("== if you see this, yell at the dev ==");
            }
        }

        static void SendMessage(string s)
        {
            if (twitchClient.IsConnected)
            {
                twitchClient.SendMessage(channel, s);
            }
        }
        #endregion

        private static void Herlp()
        {
            Console.WriteLine("========== HOW TO USE ==========");
            Console.WriteLine("The broadcaster has multiple commands for the guessing game");
            Console.WriteLine("!pokemon - start a guessing game");
            Console.WriteLine(" - If one is already started, show another dex entry for that Pokémon");
            Console.WriteLine(" - You can specify a generation with a number. ex: !pokemon 3 for gen 3");
            Console.WriteLine("!hint - display a hint");
            Console.WriteLine(" - There are different hints, they can be specified as an argument. ex: !hint type");
            Console.WriteLine(" - The hints are: type, ability, biometrics, colour, special, name");
            Console.WriteLine(" - biometrics gives the height and weight, 'height' and 'weight' can also be used as arguments instead");
            Console.WriteLine(" - special denotes if a Pokémon is a baby, legendary or mythical Pokémon");
            Console.WriteLine(" - name will give the length of the Pokmon's name");
            Console.WriteLine("!tellme - tells you the Pokémon");
            Console.WriteLine(" - this command will display the Pokémon in this Console");
            Console.WriteLine("!end - end the current game");
            Console.WriteLine("Viewers can use the !guess command to attempt to guess the Pokémon. ex: !guess sandshrew");
            Console.WriteLine("================================");
            Console.WriteLine();
        }

        private static bool CreateInitFile()
        {
            string creds = "";
            Console.WriteLine();
            Console.WriteLine("=================");
            Console.WriteLine("Enter the bot's twitch username:");
            creds += "Username: \"" + Console.ReadLine().ToLower() + "\"" + Environment.NewLine;
            Console.WriteLine("Enter the bot's OAuth token - you can find it at twitchapps.com/tmi");
            creds += "OAuth: \"" + Console.ReadLine() + "\"" + Environment.NewLine;
            Console.WriteLine("Enter the twitch account the bot will join:");
            creds += "Channel: \"" + Console.ReadLine() + "\"" + Environment.NewLine;

            string accept;
            do
            {
                Console.WriteLine();
                Console.WriteLine("Are these settings ok? (y/n)" + Environment.NewLine + creds);
                accept = Console.ReadLine().ToLower();
            } while (accept != "y" && accept != "n");


            if (accept == "y".ToLower())
            {
                creds += Environment.NewLine + "You may edit any of these variables here." + Environment.NewLine +
                    "For the bot username and channel, make sure they are lowercase." + Environment.NewLine +
                    "OAuth can be found here: https://www.twitchapps.com/tmi/";

                try
                {
                    File.WriteAllText(Directory.GetCurrentDirectory() + "\\credentials.txt", creds);
                    Console.WriteLine();
                    Console.WriteLine(Directory.GetCurrentDirectory() + "\\credentials.txt has been written.");
                }
                catch (Exception)
                {
                    Console.WriteLine("Could not write to " + Directory.GetCurrentDirectory() + " - the file will be written in the folder the program is run from, please move it to a writable folder.");
                    Console.WriteLine("Press any key to close");
                    Console.ReadLine();
                    Environment.Exit(1);
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
