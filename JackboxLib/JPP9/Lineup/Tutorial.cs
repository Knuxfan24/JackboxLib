namespace JackboxLib.JPP9.Lineup
{
    public class Tutorial : Base
    {
        // Allow single line file loading.
        public Tutorial() { }

        public Tutorial(string file) => Deseralise(file);

        /// <summary>
        /// Content header.
        /// </summary>
        public class FormatData
        {
            [JsonProperty(Order = 1, PropertyName = "content")]
            public List<TutorialPrompt> Content { get; set; } = new();
        }

        /// <summary>
        /// Actual Prompt Data.
        /// </summary>
        public class TutorialPrompt
        {
            /// <summary>
            /// Whether this prompt should be removed if the Filter US Centric option is on.
            /// </summary>
            [JsonProperty(Order = 1, PropertyName = "countrySpecific")]
            public bool USCentric { get; set; } = false;

            /// <summary>
            /// ID number, not sure what this is used for, as it doesn't seem to be incremental?
            /// </summary>
            [JsonProperty(Order = 2, PropertyName = "id")]
            public int ID { get; set; } = 24000000;

            /// <summary>
            /// Unknown, is never set to anything in Quixort.
            /// </summary>
            [JsonProperty(Order = 3, PropertyName = "isValid")]
            public string Valid { get; set; } = "";

            /// <summary>
            /// The items that need sorting in this prompt.
            /// </summary>
            [JsonProperty(Order = 4, PropertyName = "items")]
            public List<string> Items { get; set; } = new();

            /// <summary>
            /// The displayed name for this prompt.
            /// </summary>
            [JsonProperty(Order = 5, PropertyName = "prompt")]
            public string Prompt { get; set; } = "";

            /// <summary>
            /// Whether this prompt should be removed if Family Friendly mode is on.
            /// </summary>
            [JsonProperty(Order = 6, PropertyName = "x")]
            public bool Explicit { get; set; } = false;

            /// <summary>
            /// Makes the VS Debuger show the prompt.
            /// </summary>
            public override string ToString() => Prompt;
        }

        // Basic setup.
        public FormatData Data = new();

        public void Deseralise(string file)
        {
            var origData = JsonConvert.DeserializeObject<FormatData>(File.ReadAllText(file));

            foreach (var prompt in origData.Content)
                Data.Content.Add(prompt);
        }

        public void Seralise(string file) => File.WriteAllText(file, JsonConvert.SerializeObject(Data, Formatting.Indented));

        // Importing Methods.
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

                // Set up a new prompt entry for this line.
                TutorialPrompt prompt = new()
                {
                    USCentric = us,
                    ID = i - 1 + 24000000,
                    Prompt = split[0],
                    Explicit = _explicit
                };

                // Fill in the prompt options
                for (int p = 1; p < split.Length; p++)
                    prompt.Items.Add(split[p]);

                Data.Content.Add(prompt);
            }
        }
    }
}
