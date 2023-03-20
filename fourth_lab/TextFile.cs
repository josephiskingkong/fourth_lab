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

        public TextFile(string FilePath, string content)
        {
            FilePath = FilePath;
            Content = content;
            Tags = new List<string>();
            Mementos = new List<TextFileMemento>();
        }

        public TextFileMemento Save()
        {
            var Memento = new TextFileMemento(this);
            Mementos.Add(Memento);
            return Memento;
        }

        public void Restore(TextFileMemento Memento)
        {
            Content = Memento.Content;
            Author = Memento.Author;
            Description = Memento.Description;
            Tags = Memento.Tags;
        }

        public void SetAuthor(string Author)
        {
            Author = Author;
        }

        public void SetDescription(string Description)
        {
            Description = Description;
        }

        public void SetTags(List<string> Tags)
        {
            Tags = Tags;
        }

        public void AddMemento(TextFileMemento Memento)
        {
            Mementos.Add(Memento);
        }

        public void RemoveMemento(TextFileMemento Memento)
        {
            Mementos.Remove(Memento);
        }


        public void SerializeToXml(string FilePath)
        {
            var Serializer = new XmlSerializer(typeof(TextFile));

            using (var Writer = new StreamWriter(FilePath))
            {
                Serializer.Serialize(Writer, this);
            }
        }

        public static TextFile DeserializeFromXml(string FilePath)
        {
            var Serializer = new XmlSerializer(typeof(TextFile));

            using (var Reader = new StreamReader(FilePath))
            {
                return (TextFile)Serializer.Deserialize(Reader);
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

        public TextFileMemento(TextFile File)
        {
            Content = File.Content;
            Author = File.Author;
            Description = File.Description;
            Tags = new List<string>(File.Tags);
        }
    }

}
