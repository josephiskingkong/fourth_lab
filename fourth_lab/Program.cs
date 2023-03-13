using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace fourth_lab
{
    class Program
    {
        static void Main(string[] args)
        {
            string directoryPath, keywordsInput;

            if (args.Length < 1)
            {
                Console.WriteLine("Please enter the directory path:");
                directoryPath = Console.ReadLine();
                Console.WriteLine("Please enter the keywords (separated by commas):");
                keywordsInput = Console.ReadLine();
            }
            else
            {
                directoryPath = args[0];
                keywordsInput = string.Join(",", args.Skip(1));
            }

            var keywords = keywordsInput.Split(',');

            var searcher = new TextFileSearcher();
            var matchingFiles = searcher.SearchFiles(directoryPath, keywords);

            foreach (var file in matchingFiles)
            {
                Console.WriteLine(file);
            }

            while (true)
            {
                Console.WriteLine("\nChoose an action:");
                Console.WriteLine("1. Create a new text file");
                Console.WriteLine("2. Edit an existing text file");
                Console.WriteLine("3. Undo last change in a text file");
                Console.WriteLine("4. Search for text files");
                Console.WriteLine("5. Exit");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        CreateTextFile();
                        break;
                    case "2":
                        EditTextFile();
                        break;
                    case "3":
                        UndoTextFileChange();
                        break;
                    case "4":
                        SearchTextFiles();
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Invalid choice, please try again.");
                        break;
                }
            }
        }

        static void CreateTextFile()
        {
            Console.WriteLine("Enter the path and name of the new file:");
            var filePath = Console.ReadLine();
            Console.WriteLine("Enter the content of the new file:");
            var content = Console.ReadLine();

            var file = new TextFile(filePath, content);

            Console.WriteLine("Enter the name of the author:");
            var author = Console.ReadLine();
            file.SetAuthor(author);

            Console.WriteLine("Enter the description:");
            var description = Console.ReadLine();
            file.SetDescription(description);

            Console.WriteLine("Enter the tags (separated by commas):");
            var tagsInput = Console.ReadLine();
            var tags = tagsInput.Split(',').ToList();
            file.SetTags(tags);

            var memento = new TextFileMemento(file);
            file.AddMemento(memento);

            file.SerializeToXml(filePath + ".xml");

            Console.WriteLine("File created successfully.");
        }

        static void EditTextFile()
        {
            Console.WriteLine("Enter the path and name of the file to edit:");
            var filePath = Console.ReadLine();

            try
            {
                var file = TextFile.DeserializeFromXml(filePath + ".xml");
                Console.WriteLine(file);

                Console.WriteLine("Enter the new content:");
                var newContent = Console.ReadLine();
                var previousContent = file.Content;

                var memento = new TextFileMemento(file);
                file.AddMemento(memento);

                file.Content = newContent;

                Console.WriteLine("Enter the name of the author:");
                var author = Console.ReadLine();
                file.SetAuthor(author);

                Console.WriteLine("Enter the description:");
                var description = Console.ReadLine();
                file.SetDescription(description);

                Console.WriteLine("Enter the tags (separated by commas):");
                var tagsInput = Console.ReadLine();
                var tags = tagsInput.Split(',').ToList();
                file.SetTags(tags);

                file.SerializeToXml(filePath + ".xml");

                Console.WriteLine("File edited successfully.");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("File not found.");
                return;
            }
        }

        static void UndoTextFileChange()
        {
            Console.WriteLine("Enter the path and name of the file to undo:");
            var filePath = Console.ReadLine();

            try
            {
                var file = TextFile.DeserializeFromXml(filePath + ".xml");
                var mementosCount = file.Mementos.Count;
                if (mementosCount == 0)
                {
                    Console.WriteLine("No changes to undo.");
                    return;
                }

                var memento = file.Mementos.Last();
                file.RemoveMemento(memento);

                file.Content = memento.Content;
                file.SetAuthor(memento.Author);
                file.SetDescription(memento.Description);
                file.SetTags(memento.Tags);

                file.SerializeToXml(filePath + ".xml");

                Console.WriteLine("Change undone successfully.");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("File not found.");
            }
        }


        public static void SaveMementos(TextFile file)
        {
            var mementos = file.Mementos;
            var filePath = $"{file.FilePath}.mementos.xml";
            var serializer = new XmlSerializer(typeof(List<TextFileMemento>));

            using (var writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, mementos);
            }
        }

        public static List<TextFileMemento> LoadMementos(string filePath)
        {
            var serializer = new XmlSerializer(typeof(List<TextFileMemento>));

            using (var reader = new StreamReader(filePath))
            {
                return (List<TextFileMemento>)serializer.Deserialize(reader);
            }
        }


        static void SearchTextFiles()
        {
            Console.WriteLine("Enter the directory to search in:");
            var directoryPath = Console.ReadLine();
            Console.WriteLine("Enter the keywords to search for (separated by commas):");
            var keywordsInput = Console.ReadLine();

            var keywords = keywordsInput.Split(",");

            var searcher = new TextFileSearcher();
            var matchingFiles = searcher.SearchFiles(directoryPath, keywords);

            if (!matchingFiles.Any())
            {
                Console.WriteLine("No files found.");
                return;
            }

            Console.WriteLine("Matching files:");
            foreach (var file in matchingFiles)
            {
                Console.WriteLine(file);
            }
        }
    }
}
