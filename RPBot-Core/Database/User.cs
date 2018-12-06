using DSharpPlus.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace RPBot
{
    class User
    {
        public ulong UserID;
        public bool IsMuted;

        public string Characters;

        public List<Character> GetCharacters() =>
            JsonConvert.DeserializeObject<List<Character>>(Characters);

        public void SetCharacters(List<Character> characters) =>
            Characters = JsonConvert.SerializeObject(characters);


        public string MutedRoles;

        public DiscordRole GetMutedRoles() =>
            JsonConvert.DeserializeObject<DiscordRole>(MutedRoles);

        public void SetMutedRoles(DiscordRole roles) =>
            MutedRoles = JsonConvert.SerializeObject(roles);
    }
}
