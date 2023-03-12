using System;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace fourth_lab
{
    [Serializable]
    public class TextFileMemento
    {
        public string FilePath { get; }
        public string Content { get; }

        public TextFileMemento(string filePath, string content)
        {
            FilePath = filePath;
            Content = content;
        }
    }

    [Serializable]
    public class TextFile
    {
        private List<TextFileMemento> mementos = new List<TextFileMemento>();

        public string FilePath { get; set; }
        public string Content { get; set; }

        public TextFile() { }

        public TextFile(string filePath, string content)
        {
            FilePath = filePath;
            Content = content;
        }

        public TextFileMemento Save()
        {
            var memento = new TextFileMemento(FilePath, Content);
            mementos.Add(memento);
            return memento;
        }

        public void Restore(TextFileMemento memento)
        {
            FilePath = memento.FilePath;
            Content = memento.Content;
        }

        public void SerializeToXml(string fileName)
        {
            try
            {
                using (var stream = new FileStream(fileName, FileMode.Create))
                {
                    var xmlSerializer = new XmlSerializer(typeof(TextFile));
                    xmlSerializer.Serialize(stream, this);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to serialize to XML. Reason: {ex.Message}");
            }
        }

        public static TextFile DeserializeFromXml(string fileName)
        {
            try
            {
                using (var stream = new FileStream(fileName, FileMode.Open))
                {
                    var xmlSerializer = new XmlSerializer(typeof(TextFile));
                    return (TextFile)xmlSerializer.Deserialize(stream);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to deserialize from XML. Reason: {ex.Message}");
                return null;
            }
        }

        public void Undo()
        {
            if (mementos.Count == 0)
            {
                Console.WriteLine("No changes to undo.");
                return;
            }

            var lastMemento = mementos[mementos.Count - 1];
            mementos.RemoveAt(mementos.Count - 1);
            Restore(lastMemento);
        }

        public override string ToString()
        {
            return $"{FilePath}:\n{Content}";
        }

        private Stack<TextFileMemento> _mementos = new Stack<TextFileMemento>();
        private int _mementosCount = 0;
        public void AddMemento(TextFileMemento memento)
        {
            _mementos.Push(memento);
            _mementosCount++;
        }

        public void RemoveMemento(TextFileMemento memento)
        {
            if (_mementos.Contains(memento))
            {
                _mementosCount--;
                _mementos.Pop();
            }
        }

        public IList<TextFileMemento> Mementos => _mementos.ToList();

    }
}
