using JackboxLib.JPP9.Lineup;

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

                case "[JPP9QuixortTeams]":
                    // Set up a Quixort Team.
                    Teams quixortTeams = new();

                    // If another file is provided, try to deseralise it as the original team list.
                    if (args.Length > 1)
                        quixortTeams.Deseralise(args[1]);

                    // Import this data.
                    quixortTeams.Import(text);

                    // Save this data.
                    quixortTeams.Seralise($"{Path.GetDirectoryName(args[0])}\\{Path.GetFileNameWithoutExtension(args[0])}.jet");
                    break;

                default: throw new Exception("No data found to import.");
            }
        }
    }
}