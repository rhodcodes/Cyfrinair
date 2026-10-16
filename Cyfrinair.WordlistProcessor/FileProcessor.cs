using System.Globalization;
using System.Text;

namespace Cyfrinair.WordlistProcessor;

public class FileProcessor(FileInfo sourceFile, char delimiter, int column, bool ignoreComments = true)
{
    private FileInfo SourceFile { get; } = sourceFile;
    private char Delimiter { get; } = delimiter;
    private int Column { get; } = column;
    private bool IgnoreComments { get; } = ignoreComments;

    public List<string> Process()
    {
        
        var lines = File.ReadAllLines(SourceFile.FullName);
        var candidates = new HashSet<string>();

        foreach (var line in lines)
        {
            // If we want to ignore comments, and the line is a comment, skip it
            if (IgnoreComments && IsComment(line))
            {
                continue;
            }

            // Ignore empty lines
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }
            
            var word = ProcessLine(line);
            
            // Ignore empty words
            if (string.IsNullOrWhiteSpace(word))
            {
                continue;
            }
            
            word = RemoveDiacritics(word);
            
            candidates.Add(word.Trim());
            
        }
        return candidates.ToList();
    }

    private static bool IsComment(string line)
    {
        var commentDelimiters = new[] { "//", "#" };
        return commentDelimiters.Any(line.StartsWith);
    }
    
    private string ProcessLine(string line)
    {
        var parts = line.Split(Delimiter);
        var word = parts[Column - 1].Trim();

        //Assume that any uppercase letter is a name or something and not a word we want
        if(word.Any(c => !char.IsAsciiLetterLower(c)))
        {
            return string.Empty;
        }
        
        if (word.Length is < 3 or > 12)
        {
            return string.Empty;
        }
        
        // Assume if word contains any of these, it's not a Welsh word
        if( word.Any(c => c is 'q' or 'x' or 'z' or 'v'))
        {
            return string.Empty;
        }

        return word;
    }
    
    private static string RemoveDiacritics(string word)
    {
        var normalizedString = word.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                sb.Append(c);
            }
        }
        return sb.ToString().Normalize(NormalizationForm.FormC);
    }
}