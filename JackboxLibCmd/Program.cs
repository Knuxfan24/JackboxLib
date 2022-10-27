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

            // Find the file type and process it.
            switch (text[0])
            {
                case "[JPP9QuixortTeams]": Process(args, text, typeof(JackboxLib.JPP9.Lineup.Teams)); break;
                case "[JBB9QuixortTutorial]": Process(args, text, typeof(JackboxLib.JPP9.Lineup.Tutorial));  break;
                case "[JBB9QuixortPrompts]": Process(args, text, typeof(JackboxLib.JPP9.Lineup.Sequence), true);  break;

                default: throw new Exception("No supported data found to import.");
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

            // If another file is provided, try to deseralise it as the original data to append to the end of our custom data.
            if (args.Length > 1)
            {
                methodInfo = dataType.GetMethod("Deseralise");
                methodInfo.Invoke(data, new object[] { args[1] });
            }

            // Save this data to a jet file with the same name as the imported file.
            methodInfo = dataType.GetMethod("Seralise");
            methodInfo.Invoke(data, new object[] { $"{Path.GetDirectoryName(args[0])}\\{Path.GetFileNameWithoutExtension(args[0])}.jet" });
        }
    }
}