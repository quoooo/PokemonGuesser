using TwitchLib.Client.Models;

namespace PokeGuesser
{
    internal class ConnectionCredentials : TwitchLib.Client.Models.ConnectionCredentials
    {
        public ConnectionCredentials(string twitchUsername, string twitchOAuth, string twitchWebsocketURI = "wss://irc-ws.chat.twitch.tv:443", bool disableUsernameCheck = false, Capabilities capabilities = null) : base(twitchUsername, twitchOAuth, twitchWebsocketURI, disableUsernameCheck, capabilities)
        {
        }
    }
}