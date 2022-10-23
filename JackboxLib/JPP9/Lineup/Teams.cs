using Newtonsoft.Json;

namespace JackboxLib.JPP9.Lineup
{
    public class Teams
    {
        // Allow single line file loading.
        public Teams() { }

        public Teams(string file) => Deseralise(file);

        /// <summary>
        /// Content header.
        /// </summary>
        public class TeamFormatData
        {
            [JsonProperty(Order = 1, PropertyName = "content")]
            public List<QuixortTeamName> Content { get; set; } = new();
        }

        /// <summary>
        /// Actual Team Data.
        /// </summary>
        public class QuixortTeamName
        {
            /// <summary>
            /// Whether this pair of team names should be removed if the Filter US Centric option is on.
            /// </summary>
            [JsonProperty(Order = 1, PropertyName = "countrySpecific")]
            public bool USCentric { get; set; } = false;

            /// <summary>
            /// ID number, not sure what this is used for, as it doesn't seem to be incremental?
            /// </summary>
            [JsonProperty(Order = 2, PropertyName = "id")]
            public int ID { get; set; } = 24240;

            /// <summary>
            /// Unknown, is never set to anything in Quixort.
            /// </summary>
            [JsonProperty(Order = 3, PropertyName = "isValid")]
            public string Valid { get; set; } = "";

            /// <summary>
            /// Name for the team on the left.
            /// </summary>
            [JsonProperty(Order = 4, PropertyName = "teamOne")]
            public string TeamOneName { get; set; } = "One";

            /// <summary>
            /// Name for the team on the right.
            /// </summary>
            [JsonProperty(Order = 5, PropertyName = "teamTwo")]
            public string TeamTwoName { get; set; } = "One";

            /// <summary>
            /// Whether this pair of team names should be removed if Family Friendly mode is on.
            /// </summary>
            [JsonProperty(Order = 6, PropertyName = "x")]
            public bool Explicit { get; set; } = false;

            /// <summary>
            /// Makes the VS Debuger show the team names.
            /// </summary>
            public override string ToString() => $"{TeamOneName}|{TeamTwoName}";
        }

        // Basic setup.
        public TeamFormatData Data = new();

        public void Deseralise(string file) => Data = JsonConvert.DeserializeObject<TeamFormatData>(File.ReadAllText(file));

        public void Seralise(string file) => File.WriteAllText(file, JsonConvert.SerializeObject(Data, Formatting.Indented));
    
        public void Import(string[] text)
        {
            // Loop through the provided text file.
            for (int i = 1; i < text.Length; i++)
            {
                // Check for the US and Explicit tags.
                bool us = false;
                bool _explicit = false;

                if (text[i].Contains("(us)"))
                {
                    text[i] = text[i].Replace("(us)", "");
                    us = true;
                }
                if (text[i].Contains("(explicit)"))
                {
                    text[i] = text[i].Replace("(explicit)", "");
                    _explicit = true;
                }

                // Split each entry based on the | character.
                string[] split = text[i].Split('|');

                // Set up a new team for this line.
                QuixortTeamName team = new()
                {
                    USCentric = us,
                    ID = i - 1 + 24240,
                    TeamOneName = $"TEAM {split[0].ToUpper()}",
                    TeamTwoName = $"TEAM {split[1].ToUpper()}",
                    Explicit = _explicit
                };
                Data.Content.Add(team);
            }
        }
    }
}
