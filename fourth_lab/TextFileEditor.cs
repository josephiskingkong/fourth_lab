using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace fourth_lab
{
    public class TextFileEditor
    {
        private Dictionary<string, Stack<TextFileMemento>> _Mementos = new Dictionary<string, Stack<TextFileMemento>>();
        public TextFile OpenFile(string filePath)
        {

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File {filePath} not found.");
            }

            var Content = File.ReadAllText(filePath);
            var TextFile = new TextFile(filePath, Content);

            if (!_Mementos.TryGetValue(filePath, out Stack<TextFileMemento> Mementos))
            {
                Mementos = new Stack<TextFileMemento>();
                _Mementos[filePath] = Mementos;
            }

            Mementos.Push(TextFile.Save());

            return TextFile;
        }

        public void SaveFile(TextFile TextFile)
        {
            File.WriteAllText(TextFile.FilePath, TextFile.Content);

            if (_Mementos.TryGetValue(TextFile.FilePath, out Stack<TextFileMemento> Mementos))
            {
                Mementos.Push(TextFile.Save());
            }
            else
            {
                Mementos = new Stack<TextFileMemento>();
                Mementos.Push(TextFile.Save());
                _Mementos[TextFile.FilePath] = Mementos;
            }
        }

        public void UndoChanges(TextFile TextFile)
        {
            if (_Mementos.TryGetValue(TextFile.FilePath, out Stack<TextFileMemento> Mementos) && Mementos.Count > 1)
            {
                Mementos.Pop();
                TextFile.Restore(Mementos.Peek());
            }
        }
    }

}
