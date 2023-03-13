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
            string DirectoryPath, KeywordsInput;

            if (args.Length < 1)
            {
                Console.WriteLine("Please enter the Directory path:");
                DirectoryPath = Console.ReadLine();
                Console.WriteLine("Please enter the Keywords (separated by commas):");
                KeywordsInput = Console.ReadLine();
            }
            else
            {
                DirectoryPath = args[0];
                KeywordsInput = string.Join(",", args.Skip(1));
            }

            var Keywords = KeywordsInput.Split(',');

            var Searcher = new TextFileSearcher();
            var matchingFiles = Searcher.SearchFiles(DirectoryPath, Keywords);

            foreach (var File in matchingFiles)
            {
                Console.WriteLine(File);
            }

            while (true)
            {
                Console.WriteLine("\nChoose an action:");
                Console.WriteLine("1. Create a new text File");
                Console.WriteLine("2. Edit an existing text File");
                Console.WriteLine("3. Undo last change in a text File");
                Console.WriteLine("4. Search for text Files");
                Console.WriteLine("5. Exit");

                var Choice = Console.ReadLine();

                switch (Choice)
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
                        Console.WriteLine("Invalid Choice, please try again.");
                        break;
                }
            }
        }

        static void CreateTextFile()
        {
            Console.WriteLine("Enter the path and name of the new File:");
            var FilePath = Console.ReadLine();
            Console.WriteLine("Enter the content of the new File:");
            var content = Console.ReadLine();

            var File = new TextFile(FilePath, content);

            Console.WriteLine("Enter the name of the author:");
            var author = Console.ReadLine();
            File.SetAuthor(author);

            Console.WriteLine("Enter the description:");
            var description = Console.ReadLine();
            File.SetDescription(description);

            Console.WriteLine("Enter the Tags (separated by commas):");
            var TagsInput = Console.ReadLine();
            var Tags = TagsInput.Split(',').ToList();
            File.SetTags(Tags);

            var Memento = new TextFileMemento(File);
            File.AddMemento(Memento);

            File.SerializeToXml(FilePath + ".xml");

            Console.WriteLine("File created successfully.");
        }

        static void EditTextFile()
        {
            Console.WriteLine("Enter the path and name of the File to edit:");
            var FilePath = Console.ReadLine();

            try
            {
                var File = TextFile.DeserializeFromXml(FilePath + ".xml");
                Console.WriteLine(File);

                Console.WriteLine("Enter the new content:");
                var NewContent = Console.ReadLine();
                var previousContent = File.Content;

                var Memento = new TextFileMemento(File);
                File.AddMemento(Memento);

                File.Content = NewContent;

                Console.WriteLine("Enter the name of the author:");
                var author = Console.ReadLine();
                File.SetAuthor(author);

                Console.WriteLine("Enter the description:");
                var description = Console.ReadLine();
                File.SetDescription(description);

                Console.WriteLine("Enter the Tags (separated by commas):");
                var TagsInput = Console.ReadLine();
                var Tags = TagsInput.Split(',').ToList();
                File.SetTags(Tags);

                File.SerializeToXml(FilePath + ".xml");

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
            Console.WriteLine("Enter the path and name of the File to undo:");
            var FilePath = Console.ReadLine();

            try
            {
                var File = TextFile.DeserializeFromXml(FilePath + ".xml");
                var MementosCount = File.Mementos.Count;
                if (MementosCount == 0)
                {
                    Console.WriteLine("No changes to undo.");
                    return;
                }

                var Memento = File.Mementos.Last();
                File.RemoveMemento(Memento);

                File.Content = Memento.Content;
                File.SetAuthor(Memento.Author);
                File.SetDescription(Memento.Description);
                File.SetTags(Memento.Tags);

                File.SerializeToXml(FilePath + ".xml");

                Console.WriteLine("Change undone successfully.");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("File not found.");
            }
        }


        public static void SaveMementos(TextFile File)
        {
            var Mementos = File.Mementos;
            var FilePath = $"{File.FilePath}.Mementos.xml";
            var serializer = new XmlSerializer(typeof(List<TextFileMemento>));

            using (var Writer = new StreamWriter(FilePath))
            {
                serializer.Serialize(Writer, Mementos);
            }
        }

        public static List<TextFileMemento> LoadMementos(string FilePath)
        {
            var serializer = new XmlSerializer(typeof(List<TextFileMemento>));

            using (var Reader = new StreamReader(FilePath))
            {
                return (List<TextFileMemento>)serializer.Deserialize(Reader);
            }
        }


        static void SearchTextFiles()
        {
            Console.WriteLine("Enter the Directory to search in:");
            var DirectoryPath = Console.ReadLine();
            Console.WriteLine("Enter the Keywords to search for (separated by commas):");
            var KeywordsInput = Console.ReadLine();

            var Keywords = KeywordsInput.Split(",");

            var Searcher = new TextFileSearcher();
            var matchingFiles = Searcher.SearchFiles(DirectoryPath, Keywords);

            if (!matchingFiles.Any())
            {
                Console.WriteLine("No Files found.");
                return;
            }

            Console.WriteLine("Matching Files:");
            foreach (var File in matchingFiles)
            {
                Console.WriteLine(File);
            }
        }
    }
}
