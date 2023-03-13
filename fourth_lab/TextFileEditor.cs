using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace fourth_lab
{
    public class TextFileEditor
    {
        private Dictionary<string, Stack<TextFileMemento>> _mementos = new Dictionary<string, Stack<TextFileMemento>>();
        public TextFile OpenFile(string filePath)
        {

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File {filePath} not found.");
            }

            var content = File.ReadAllText(filePath);
            var textFile = new TextFile(filePath, content);

            if (!_mementos.TryGetValue(filePath, out Stack<TextFileMemento> mementos))
            {
                mementos = new Stack<TextFileMemento>();
                _mementos[filePath] = mementos;
            }

            mementos.Push(textFile.Save());

            return textFile;
        }

        public void SaveFile(TextFile textFile)
        {
            File.WriteAllText(textFile.FilePath, textFile.Content);

            if (_mementos.TryGetValue(textFile.FilePath, out Stack<TextFileMemento> mementos))
            {
                mementos.Push(textFile.Save());
            }
            else
            {
                mementos = new Stack<TextFileMemento>();
                mementos.Push(textFile.Save());
                _mementos[textFile.FilePath] = mementos;
            }
        }

        public void UndoChanges(TextFile textFile)
        {
            if (_mementos.TryGetValue(textFile.FilePath, out Stack<TextFileMemento> mementos) && mementos.Count > 1)
            {
                mementos.Pop();
                textFile.Restore(mementos.Peek());
            }
        }
    }

}
