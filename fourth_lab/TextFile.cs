using System;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace fourth_lab
{
    public class TextFile
    {
        public string FilePath { get; }
        public string Content { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public List<string> Tags { get; private set; }
        public List<TextFileMemento> Mementos { get; private set; }

        public TextFile() // добавляем пустой конструктор
        {
            FilePath = "";
            Content = "";
            Tags = new List<string>();
            Mementos = new List<TextFileMemento>();
        }

        public TextFile(string filePath, string content)
        {
            FilePath = filePath;
            Content = content;
            Tags = new List<string>();
            Mementos = new List<TextFileMemento>();
        }

        public TextFileMemento Save()
        {
            var memento = new TextFileMemento(this);
            Mementos.Add(memento);
            return memento;
        }

        public void Restore(TextFileMemento memento)
        {
            Content = memento.Content;
            Author = memento.Author;
            Description = memento.Description;
            Tags = memento.Tags;
        }

        public void SetAuthor(string author)
        {
            Author = author;
        }

        public void SetDescription(string description)
        {
            Description = description;
        }

        public void SetTags(List<string> tags)
        {
            Tags = tags;
        }

        public void AddMemento(TextFileMemento memento)
        {
            Mementos.Add(memento);
        }

        public void RemoveMemento(TextFileMemento memento)
        {
            Mementos.Remove(memento);
        }


        public void SerializeToXml(string filePath)
        {
            var serializer = new XmlSerializer(typeof(TextFile));

            using (var writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, this);
            }
        }

        public static TextFile DeserializeFromXml(string filePath)
        {
            var serializer = new XmlSerializer(typeof(TextFile));

            using (var reader = new StreamReader(filePath))
            {
                return (TextFile)serializer.Deserialize(reader);
            }
        }
    }

    public class TextFileMemento
    {
        public string Content { get; set; } // изменяем доступ на public
        public string Author { get; set; }
        public string Description { get; set; }
        public List<string> Tags { get; set; }

        public TextFileMemento() // добавляем пустой конструктор
        {
            Content = "";
            Author = "";
            Description = "";
            Tags = new List<string>();
        }

        public TextFileMemento(TextFile file)
        {
            Content = file.Content;
            Author = file.Author;
            Description = file.Description;
            Tags = new List<string>(file.Tags);
        }
    }


    public class TextFileSearcher
    {
        public List<string> SearchFiles(string directoryPath, string[] keywords)
        {
            var matchingFiles = new List<string>();

            foreach (var file in Directory.GetFiles(directoryPath))
            {
                var fileContent = File.ReadAllText(file);

                if (keywords.All(keyword => fileContent.Contains(keyword)))
                {
                    matchingFiles.Add(file);
                }
            }

            return matchingFiles;
        }
    }
}
