using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace fourth_lab
{
    public class TextFileEditor
    {
        private Dictionary<string, TextFileMemento> _mementos = new Dictionary<string, TextFileMemento>();
        public TextFile OpenFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File {filePath} not found.");
            }

            var content = File.ReadAllText(filePath);
            var textFile = new TextFile(filePath, content);
            _mementos[filePath] = textFile.Save();

            return textFile;
        }

        public void SaveFile(TextFile textFile)
        {
            File.WriteAllText(textFile.FilePath, textFile.Content);
            _mementos[textFile.FilePath] = textFile.Save();
        }

        public void UndoChanges(TextFile textFile)
        {
            if (_mementos.TryGetValue(textFile.FilePath, out TextFileMemento memento))
            {
                textFile.Restore(memento);
            }
        }
    }
}