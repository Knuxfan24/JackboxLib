using System.Diagnostics;

namespace JackboxLib.JPP2.Earwax
{
    public class Audio : Base
    {
        // Allow single line file loading.
        public Audio() { }

        public Audio(string file) => Deseralise(file);

        /// <summary>
        /// Content header.
        /// </summary>
        public class FormatData
        {
            [JsonProperty(Order = 1, PropertyName = "episodeid")]
            public int EpisodeID { get; set; } = 1234;

            [JsonProperty(Order = 2, PropertyName = "content")]
            public List<AudioEntry> Content { get; set; } = new();
        }

        /// <summary>
        /// Actual Prompt Data.
        /// </summary>
        public class AudioEntry
        {
            /// <summary>
            /// Whether this sound should be removed if Family Friendly mode is on.
            /// </summary>
            [JsonProperty(Order = 1, PropertyName = "x")]
            public bool Explicit { get; set; } = false;

            /// <summary>
            /// The displayed text for this sound on the player's device.
            /// </summary>
            [JsonProperty(Order = 2, PropertyName = "name")]
            public string Name { get; set; } = "";

            /// <summary>
            /// The displayed text for this sound in the game.
            /// </summary>
            [JsonProperty(Order = 3, PropertyName = "short")]
            public string ShortName { get; set; } = "";

            /// <summary>
            /// ID number, not sure what this is used for, as it doesn't seem to be incremental?
            /// </summary>
            [JsonProperty(Order = 4, PropertyName = "id")]
            public int ID { get; set; } = 24000000;

            /// <summary>
            /// The categories this sound is part of.
            /// </summary>
            [JsonProperty(Order = 5, PropertyName = "categories")]
            public List<string> Categories { get; set; } = new();

            /// <summary>
            /// Temporary value only used for importing, holds the imported entry's file path.
            /// </summary>
            [JsonIgnore]
            public string? Filepath { get; set; }

            /// <summary>
            /// Makes the VS Debuger show the sound's name.
            /// </summary>
            public override string ToString() => Name;
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
                // Check for the Explicit tag.
                bool _explicit = false;

                if (text[i].Contains("(explicit)"))
                {
                    text[i] = text[i].Replace("(explicit)", "");
                    _explicit = true;
                }

                // Split each entry based on the | character.
                string[] split = text[i].Split('|');

                // Set up the audio entry for this.
                AudioEntry audio = new()
                {
                    Explicit = _explicit,
                    Name = split[2],
                    ShortName = split[2],
                    ID = i - 1 + 24000000,
                    Filepath = split[0]
                };
                
                // If this audio entry has a short name specified, then set it.
                if (split.Length == 4)
                    audio.ShortName = split[3];

                // Add this audio entry's categories.
                foreach (string category in split[1].Split(','))
                    audio.Categories.Add(category);

                Data.Content.Add(audio);
            }
        }

        public void WriteData(string location)
        {
            // Create the basic directories.
            Directory.CreateDirectory($"{Path.GetDirectoryName(location)}\\{Path.GetFileNameWithoutExtension(location)}\\Audio");
            Directory.CreateDirectory($"{Path.GetDirectoryName(location)}\\{Path.GetFileNameWithoutExtension(location)}\\Spectrum");

            // Loop through each prompt.
            foreach (AudioEntry prompt in Data.Content)
            {
                // If this file's already an OGG, then copy it, if not then convert it.
                // TODO, test more formats than just WAVs.
                if (Path.GetExtension(prompt.Filepath) == ".ogg")
                {
                    File.Copy(prompt.Filepath, $"{Path.GetDirectoryName(location)}\\{Path.GetFileNameWithoutExtension(location)}\\Audio\\{prompt.ID}.ogg", true);
                }
                else
                {
                    using (Process process = new())
                    {
                        process.StartInfo.FileName = $"\"{Environment.CurrentDirectory}\\ExternalResources\\oggenc2.exe\"";
                        process.StartInfo.Arguments = $"-o \"{Path.GetDirectoryName(location)}\\{Path.GetFileNameWithoutExtension(location)}\\Audio\\{prompt.ID}.ogg\" \"{prompt.Filepath}\"";
                        process.StartInfo.UseShellExecute = false;
                        process.StartInfo.RedirectStandardOutput = true;
                        process.StartInfo.CreateNoWindow = true;

                        process.Start();
                        process.BeginOutputReadLine();
                        process.WaitForExit();
                    }
                }

                // Sloppily create a dummy spectrum file so the game doesn't hang.
                List<string> DummySpectrum = new()
                {
                    "{",
                    "\t\"Refresh\":23,",
                    "\t\"Frequencies\":[",
                    "\t\t{\"left\":[ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],",
                    "\t\t \"right\":[ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]}",
                    "\t],",
                    "\t\"Peak\":100",
                    "}"
                };
                File.WriteAllLines($"{Path.GetDirectoryName(location)}\\{Path.GetFileNameWithoutExtension(location)}\\Spectrum\\{prompt.ID}.jet", DummySpectrum);
            }
        }
    }
}
