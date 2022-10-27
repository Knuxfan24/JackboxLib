using System.Reflection;

namespace JackboxLibCmd
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Complain if we don't have a file to parse.
            if (args.Length == 0)
            {
                Console.WriteLine("Alan please add details.");
                Console.ReadKey();
                return;
            }
            // Complain if the first file isn't a plain text file.
            if (Path.GetExtension(args[0]) != ".txt")
            {
                Console.WriteLine("Alan please add details.");
                Console.ReadKey();
                return;
            }

            // Split text file.
            string[] text = File.ReadAllLines(args[0]);

            // TODO: Finish setting up all this.
            // Find the file type.
            switch (text[0])
            {
                case "[JPP1DrawfulPrompts]": throw new Exception("Drawful prompt importing not yet supported.");
                case "[JPP1FibbageQuestions]": throw new Exception("Fibbage question importing not yet supported.");
                case "[JPP1LieSwatterQuestions]": throw new Exception("Lie Swatter question importing not yet supported.");
                case "[JPP1WordSpudWords]": throw new Exception("Word Spud starter word importing not yet supported.");
                case "[JPP1YDKJEpisode]": throw new Exception("You Don't Know Jack 2015 Episode creation not yet supported.");

                case "[JPP2BidiotsPrompts]": throw new Exception("Bidiots prompt and image importing not yet supported.");
                case "[JPP2EarwaxPrompts]": throw new Exception("Earwax prompt importing not yet supported.");
                case "[JPP2EarwaxAudio]": throw new Exception("Earwax sound importing not yet supported.");
                case "[JPP2FibbageQuestions]": throw new Exception("Fibbage 2 question importing not yet supported.");
                case "[JPP2QuiplashQuestions]": throw new Exception("Quiplash 2 question importing not yet supported.");

                case "[JPP3TKOSuggestions]": throw new Exception("Tee-KO suggestion importing not yet supported.");
                case "[JPP3FakingItWriting]": throw new Exception("Faking It writing task importing not yet supported.");
                case "[JPP3FakingItAction]": throw new Exception("Faking It action task importing not yet supported.");
                case "[JPP3GuesspionageQuestion]": throw new Exception("Guesspionage normal question importing not yet supported.");
                case "[JPP3GuesspionageQuestionFinal]": throw new Exception("Guesspionage final round question importing not yet supported.");

                case "[JPP9QuixortTeams]": Process(args, text, typeof(JackboxLib.JPP9.Lineup.Teams)); break;
                case "[JBB9QuixortTutorial]": Process(args, text, typeof(JackboxLib.JPP9.Lineup.Tutorial));  break;
                case "[JBB9QuixortPrompts]": Process(args, text, typeof(JackboxLib.JPP9.Lineup.Sequence), true);  break;

                default: throw new Exception("No data found to import.");
            }
        }

        /// <summary>
        /// Process the provided file(s) with the approriate type based on the header.
        /// </summary>
        /// <param name="args">The argument list (so we can check for another file).</param>
        /// <param name="text">The file we're processing.</param>
        /// <param name="dataType">The type we're processing this data as.</param>
        /// <param name="dataJet">Whether or not this data also needs seperate data.jet files writing.</param>
        public static void Process(string[] args, string[] text, Type dataType, bool dataJet = false)
        {
            // Set up the approriate data type.
            object? data = Activator.CreateInstance(dataType);

            // Set up a method info object.
            MethodInfo? methodInfo;

            // Import the data from this file.
            methodInfo = dataType.GetMethod("Import");
            methodInfo.Invoke(data, new object[] { text });

            // Write the data.jet files for this format if we have to (do this here so we don't write any for the original data).
            if (dataJet == true)
            {
                methodInfo = dataType.GetMethod("WriteData");
                methodInfo.Invoke(data, new object[] { args[0] });
            }

            // If another file is provided, try to deseralise it as the original data.
            if (args.Length > 1)
            {
                methodInfo = dataType.GetMethod("Deseralise");
                methodInfo.Invoke(data, new object[] { args[1] });
            }

            // Save this data.
            methodInfo = dataType.GetMethod("Seralise");
            methodInfo.Invoke(data, new object[] { $"{Path.GetDirectoryName(args[0])}\\{Path.GetFileNameWithoutExtension(args[0])}.jet" });
        }
    }
}