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

                // TODO: Refactor how this works so I can use the Process function.
                // While this is OK, I'd rather not be duping this shit for things like Fibbage and Quiplash later on.
                case "[JBB9QuixortPrompts]":
                    // Set up a Quixort Prompt Sequence.
                    JackboxLib.JPP9.Lineup.Sequence quixortSequence = new();

                    // If another file is provided, try to deseralise it as the original prompt list.
                    if (args.Length > 1)
                        quixortSequence.Deseralise(args[1]);

                    // Import this data (the saving is done as part of this function).
                    quixortSequence.Import(text, args[0]);
                    break;

                default: throw new Exception("No data found to import.");
            }
        }

        /// <summary>
        /// Process the provided file(s) with the approriate type based on the header.
        /// </summary>
        /// <param name="args">The argument list (so we can check for another file).</param>
        /// <param name="text">The file we're proessing.</param>
        /// <param name="dataType">The type we're processing this data as.</param>
        public static void Process(string[] args, string[] text, Type dataType)
        {
            // Set up the approriate data type.
            object? data = Activator.CreateInstance(dataType);

            // Set up a method info object.
            MethodInfo? methodInfo;

            // If another file is provided, try to deseralise it as the original team list.
            if (args.Length > 1)
            {
                methodInfo = dataType.GetMethod("Deseralise");
                methodInfo.Invoke(data, new object[] { args[1] });
            }

            // Import the data from this file.
            methodInfo = dataType.GetMethod("Import");
            methodInfo.Invoke(data, new object[] { text });

            // Save this data.
            methodInfo = dataType.GetMethod("Seralise");
            methodInfo.Invoke(data, new object[] { $"{Path.GetDirectoryName(args[0])}\\{Path.GetFileNameWithoutExtension(args[0])}.jet" });
        }
    }
}