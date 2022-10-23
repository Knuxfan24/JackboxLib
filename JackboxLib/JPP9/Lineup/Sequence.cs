using Newtonsoft.Json;

namespace JackboxLib.JPP9.Lineup
{
    public class Sequence
    {
        // Allow single line file loading.
        public Sequence() { }

        public Sequence(string file) => Deseralise(file);

        /// <summary>
        /// Content header.
        /// </summary>
        public class SequenceFormatData
        {
            [JsonProperty(Order = 1, PropertyName = "content")]
            public List<SequenceEntry> Content { get; set; } = new();
        }

        /// <summary>
        /// Actual Prompt Data.
        /// </summary>
        public class SequenceEntry
        {
            /// <summary>
            /// The categories this prompt is in.
            /// </summary>
            [JsonProperty(Order = 1, PropertyName = "categories")]
            public List<string> Categories { get; set; } = new();

            /// <summary>
            /// Whether this prompt should be removed if the Filter US Centric option is on.
            /// </summary>
            [JsonProperty(Order = 2, PropertyName = "countrySpecific")]
            public bool USCentric { get; set; } = false;

            /// <summary>
            /// The difficulty of this prompt, defaults to medium.
            /// </summary>
            [JsonProperty(Order = 3, PropertyName = "difficulty")]
            public string Difficulty { get; set; } = "medium";

            /// <summary>
            /// ID number, not sure what this is used for, as it doesn't seem to be incremental?
            /// </summary>
            [JsonProperty(Order = 4, PropertyName = "id")]
            public int ID { get; set; } = 24240;

            /// <summary>
            /// Unknown, is never set to anything in Quixort.
            /// </summary>
            [JsonProperty(Order = 5, PropertyName = "isValid")]
            public string Valid { get; set; } = "";

            /// <summary>
            /// The items that need sorting in this prompt.
            /// </summary>
            [JsonProperty(Order = 6, PropertyName = "items")]
            public List<SequenceItem> Items { get; set; } = new();

            /// <summary>
            /// The value in the left side label for this prompt.
            /// </summary>
            [JsonProperty(Order = 7, PropertyName = "leftLabel")]
            public string Least { get; set; } = "";

            /// <summary>
            /// The displayed name for this prompt.
            /// </summary>
            [JsonProperty(Order = 8, PropertyName = "prompt")]
            public string Prompt { get; set; } = "";

            /// <summary>
            /// The value in the left side label for this prompt.
            /// </summary>
            [JsonProperty(Order = 9, PropertyName = "rightLabel")]
            public string Most { get; set; } = "";

            /// <summary>
            /// The items that are fake in this prompt.
            /// </summary>
            [JsonProperty(Order = 10, PropertyName = "trash")]
            public List<SequenceTrash> Trash { get; set; } = new();

            /// <summary>
            /// Whether this prompt should be removed if Family Friendly mode is on.
            /// </summary>
            [JsonProperty(Order = 11, PropertyName = "x")]
            public bool Explicit { get; set; } = false;

            /// <summary>
            /// Makes the VS Debuger show the prompt.
            /// </summary>
            public override string ToString() => Prompt;
        }

        /// <summary>
        /// A real item that is sorted in this prompt.
        /// </summary>
        public class SequenceItem
        {
            /// <summary>
            /// The value displayed when showing the final ordering.
            /// </summary>
            [JsonProperty(Order = 1, PropertyName = "displayValue")]
            public string Value { get; set; } = "";

            /// <summary>
            /// The name shown for this item on the top display?
            /// </summary>
            [JsonProperty(Order = 2, PropertyName = "longText")]
            public string Long { get; set; } = "";

            /// <summary>
            /// The name shown for this item on the block?
            /// </summary>
            [JsonProperty(Order = 3, PropertyName = "shortText")]
            public string Short { get; set; } = "";

            /// <summary>
            /// Makes the VS Debuger show the item's longText value.
            /// </summary>
            public override string ToString() => Long;
        }

        /// <summary>
        /// A fake item that is meant to be trashed in this prompt.
        /// </summary>
        public class SequenceTrash
        {
            /// <summary>
            /// The name shown for this item on the top display?
            /// </summary>
            [JsonProperty(Order = 1, PropertyName = "longText")]
            public string Long { get; set; } = "";

            /// <summary>
            /// The name shown for this item on the block?
            /// </summary>
            [JsonProperty(Order = 2, PropertyName = "shortText")]
            public string Short { get; set; } = "";

            /// <summary>
            /// Makes the VS Debuger show the item's longText value.
            /// </summary>
            public override string ToString() => Long;
        }

        // Basic setup.
        public SequenceFormatData Data = new();

        public void Deseralise(string file) => Data = JsonConvert.DeserializeObject<SequenceFormatData>(File.ReadAllText(file));

        public void Seralise(string file) => File.WriteAllText(file, JsonConvert.SerializeObject(Data, Formatting.Indented));
    }
}
