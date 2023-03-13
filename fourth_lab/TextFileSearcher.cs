using System.Collections.Generic;
using System.IO;
using System.Linq;
using fourth_lab;

namespace fifth_lab
{
    public class TextFileSearcher
    {
        public IEnumerable<TextFile> SearchFiles(string directoryPath, string[] keywords)
        {
            var files = Directory.GetFiles(directoryPath, "*.txt", SearchOption.AllDirectories);
            return files.Select(file => new TextFile(file, File.ReadAllText(file)))
            .Where(textFile => keywords.Any(keyword => textFile.Content.Contains(keyword, StringComparison.OrdinalIgnoreCase)));
        }
    }
}