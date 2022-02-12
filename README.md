# PokéGuesser
## Introduction
Hello there! This program runs a Twitch bot that lets you and your chat play a guessing game with Pokémon! The bot will be run locally, on whoever's machine, and it can be connected to any Twitch account you have access to (including your own). 
## Starting up
The program will first ask to initialize settings, this will only need to be done once. The bot name can be the same as the channel it will be connected to. To find the bot's OAuth, use [this website](https://www.twitchapps.com/tmi/). There are a few extra settings you can manage as well, which allow or disallow moderators to start the game, tells you or hides from you the answer, and sets the default generations to randomely pick from. Once everything is set up, the program will give you the option to change everything when it launches, but will save all the settings locally.
## Commands
### !pokemon
#### Broadcaster only, moderator allowed if settings have been set
Starts a guessing game! This will pull one random Pokédex entry and post it in chat. You can specify a generation to pick from (ex: !pokemon 3 for a generation 3 'mon), or you can leave the argument blank for the bot to pick from any of the default generations that were set.

There is a bit of validation on the dex entry to make sure it doesn't spoil the answer in the flavor text. For example, Blastoise will not have his entry from Ruby/Saphier, which is "Blastoise has water spouts that protrude from its shell[...]".

If a game has already started, this command will pull up a new dex entry from the same Pokémon. Viewers can also use this command to see the first Pokédex entry as well, if a game is running.

### !hint
#### Broadcaster only, moderator allowed if settings have been set
This command will post a hint in chat. By default, the hint will be the Pokémon's type(s), but an argument can be specified for a specific hint:
!hint [X]
- type - Posts the Pokémon's type(s)
- ability/abilities - Posts the Pokémon's abilitie(s)
- biometrics/height/weight - Posts the Pokémon's height and weight in meters and kilograms
- color/colour - Posts the Pokémon's colour, [according to the Pokédex](https://bulbapedia.bulbagarden.net/wiki/List_of_Pok%C3%A9mon_by_color#List_of_Pok.C3.A9mon_by_color)
- special - Posts whether or not the Pokémon is a baby, a legendary or a mythical Pokémon
- name - Posts the length of the Pokémon's name
- random - Posts a random hint from the above
- help - If you forget the arguments, this will tell you the list

### !tellme
#### Broadcaster only
This command will tell you the current answer to the game in the program's console.

### !help
#### Broadcaster only
This will post a wall of text in the console with information on the program.

### !end
#### Broadcaster only, moderator allowed if settings have been set
The current guessing game will end, and the answer will be posted in chat.

### !guess
#### Everyone
Take a guess at the answer. The bot will let you know if you are right.

## Contributors
Just little ol me. ([@ironically_quo](https://twitter.com/ironically_quo))

## Credits

- [TwitchLib](https://github.com/TwitchLib/TwitchLib) - for having an amazing library
- [pokeapi.co](https://pokeapi.co/) - the literal backbone of the project, bless everything they do
- [Fullaire](https://www.twitch.tv/fullaire) - this was her idea originally, I was just the code monkey
- [MrsCandyria](https://www.twitch.tv/mrscandyria) - tested and found bugs
