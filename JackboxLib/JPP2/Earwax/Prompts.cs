using NAudio.Wave;
using System.Diagnostics;
using System.Speech.AudioFormat;
using System.Speech.Synthesis;

namespace JackboxLib.JPP2.Earwax
{
    public class Prompts : Base
    {
        // Allow single line file loading.
        public Prompts() { }

        public Prompts(string file) => Deseralise(file);

        /// <summary>
        /// Content header.
        /// </summary>
        public class FormatData
        {
            [JsonProperty(Order = 1, PropertyName = "content")]
            public List<PromptEntry> Content { get; set; } = new();
        }

        /// <summary>
        /// Actual Prompt Data.
        /// </summary>
        public class PromptEntry
        {
            /// <summary>
            /// ID number, not sure what this is used for, as it doesn't seem to be incremental?
            /// </summary>
            [JsonProperty(Order = 1, PropertyName = "id")]
            public int ID { get; set; } = 24000000;

            /// <summary>
            /// Whether this prompt should be removed if Family Friendly mode is on.
            /// </summary>
            [JsonProperty(Order = 2, PropertyName = "x")]
            public bool Explicit { get; set; } = false;

            /// <summary>
            /// The audio file to play for this prompt.
            /// </summary>
            [JsonProperty(Order = 3, PropertyName = "PromptAudio")]
            public string Audio { get; set; } = "";

            /// <summary>
            /// The displayed text for this prompt.
            /// </summary>
            [JsonProperty(Order = 4, PropertyName = "name")]
            public string Prompt { get; set; } = "";

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
                // Check for the Explicit tag.
                bool _explicit = false;

                if (text[i].Contains("(explicit)"))
                {
                    text[i] = text[i].Replace("(explicit)", "");
                    _explicit = true;
                }

                // Set up the prompt for this.
                PromptEntry prompt = new()
                {
                    ID = i - 1 + 24000000,
                    Explicit = _explicit,
                    Audio = $"custom_{i - 1 + 24000000}",
                    Prompt = text[i]
                };

                Data.Content.Add(prompt);
            }
        }

        public void WriteData(string location)
        {
            // Create the directory for this prompt list.
            Directory.CreateDirectory($"{Path.GetDirectoryName(location)}\\{Path.GetFileNameWithoutExtension(location)}");

            // Loop through each prompt.
            foreach (PromptEntry prompt in Data.Content)
            {
                // Create the WAV for the user to manually make into OGGs (src: https://docs.microsoft.com/en-us/dotnet/api/system.speech.synthesis.speechsynthesizer.setoutputtowavefile?view=netframework-4.8)
                // Initialize a new instance of the SpeechSynthesizer.  
                SpeechSynthesizer synth = new();

                // Configure the audio output.   
                synth.SetOutputToWaveFile($"{Path.GetDirectoryName(location)}\\{Path.GetFileNameWithoutExtension(location)}\\{prompt.Audio}.wav", new SpeechAudioFormatInfo(44100, AudioBitsPerSample.Sixteen, AudioChannel.Mono));

                // Build a prompt, strip out the control tags.  
                PromptBuilder builder = new();
                string tts = prompt.Prompt.Replace("<ANY>", "this player");
                tts = tts.Replace("<i>", "");
                tts = tts.Replace("</i>", "");
                builder.AppendText(tts);

                // Speak the sound to our WAV file.
                synth.Speak(builder);

                // Dispose of the builder so we can actually delete the WAV afterwards.
                synth.Dispose();

                // Convert WAV to OGG, checking if it the WAV and OGG Encoder actually exist.
                if (File.Exists($"{Path.GetDirectoryName(location)}\\{Path.GetFileNameWithoutExtension(location)}\\{prompt.Audio}.wav") && File.Exists($"{Environment.CurrentDirectory}\\ExternalResources\\oggenc2.exe"))
                {
                    // Normalise the WAV file first.
                    string normalisedFile = Normalise($"{Path.GetDirectoryName(location)}\\{Path.GetFileNameWithoutExtension(location)}\\{prompt.Audio}.wav");

                    // Use oggenc2 to convert the normalised WAV to an OGG.
                    using (Process process = new())
                    {
                        process.StartInfo.FileName = $"\"{Environment.CurrentDirectory}\\ExternalResources\\oggenc2.exe\"";
                        process.StartInfo.Arguments = $"\"{normalisedFile}\"";
                        process.StartInfo.UseShellExecute = false;
                        process.StartInfo.RedirectStandardOutput = true;
                        process.StartInfo.CreateNoWindow = true;

                        process.Start();
                        process.BeginOutputReadLine();
                        process.WaitForExit();
                    }

                    // Remove the now useless WAVs.
                    File.Delete($"{Path.GetDirectoryName(location)}\\{Path.GetFileNameWithoutExtension(location)}\\{prompt.Audio}.wav");
                    File.Delete($"{Path.GetDirectoryName(location)}\\{Path.GetFileNameWithoutExtension(location)}\\{prompt.Audio}_normalise.wav");

                    // Rename the OGG file.
                    File.Move($"{Path.GetDirectoryName(location)}\\{Path.GetFileNameWithoutExtension(location)}\\{prompt.Audio}_normalise.ogg", $"{Path.GetDirectoryName(location)}\\{Path.GetFileNameWithoutExtension(location)}\\custom_{prompt.ID}.ogg", true);
                }
            }
        }

        /// <summary>
        /// Normalises the stupidly quiet WAV files this makes.
        /// </summary>
        /// <param name="file">The path to the WAV file to normalise.</param>
        /// <returns>The path to the normalised file.</returns>
        private static string Normalise(string file)
        {
            // Set up the output file as I can't just overwrite the input WAV.
            string output = file.Replace(".wav", "_normalise.wav");

            // Everything else is from here: https://markheath.net/post/normalize-audio-naudio
            float max = 0;

            using (var reader = new AudioFileReader(file))
            {
                // find the max peak
                float[] buffer = new float[reader.WaveFormat.SampleRate];
                int read;
                do
                {
                    read = reader.Read(buffer, 0, buffer.Length);
                    for (int n = 0; n < read; n++)
                    {
                        var abs = Math.Abs(buffer[n]);
                        if (abs > max) max = abs;
                    }
                } while (read > 0);

                if (max == 0 || max > 1.0f)
                    throw new InvalidOperationException("File cannot be normalized");

                // rewind and amplify
                reader.Position = 0;
                reader.Volume = 1.0f / max;

                // write out to a new WAV file
                WaveFileWriter.CreateWaveFile16(output, reader);
            }

            return output;
        }
    }
}
