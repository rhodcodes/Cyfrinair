using System.ComponentModel;
using Cyfrinair.WordlistProcessor;
using Spectre.Console.Cli;
using Spectre.Console;


var app = new CommandApp<ProcessCommand>();
return app.Run(args);

public class ProcessSettings : CommandSettings
{
    [CommandArgument(0, "<source>")]
    [Description("Path to the file to process")]
    public string SourceFilePath { get; init; } = string.Empty;

    [CommandOption("-o|--output")]
    [Description("Path to the file to write the processed words to")]
    public string OutputFilePath { get; init; } = "Geiriadur.txt";

    [CommandOption("-c|--column")]
    [Description("The column that contains the words to process")]
    [DefaultValue(0)]
    public int Column { get; init; } = 0;

    [CommandOption("-d|--delimiter")]
    [Description("The delimiter typefor splitting the words. Tsv, csv etc")]
    [DefaultValue("tsv")]
    public string Delimiter { get; init; } = "tsv";

    [CommandOption("-i|--ignore-comments")]
    [Description("Whether to ignore comments in the file. Comments are lines that start with # or //.")]
    [DefaultValue(true)]
    public bool IgnoreComments { get; init; } = true;
}

public class ProcessCommand : Command<ProcessSettings>
{
    protected override int Execute(CommandContext context, ProcessSettings settings, CancellationToken cancellation)
    {
        DisplayTitle();
        AnsiConsole.MarkupLine($"[green]Processing file: {settings.SourceFilePath}[/]");
        var delimiter = '\t';
        if (settings.Column < 1)
        {
            AnsiConsole.MarkupLine("[red]Column must be greater than 0[/]");
            Environment.Exit(1);
        }
        var column = settings.Column;
        var ignoreComments = settings.IgnoreComments;
        var wordList = new List<string>();

        switch (settings.Delimiter)
        {
            case "csv":
                delimiter = ',';
                break;
            case "tsv":
                delimiter = '\t';
                break;
            case "pipe":
                delimiter = '|';
                break;
            default:
                AnsiConsole.MarkupLine($"[red] Unknown delimiter - {settings.Delimiter}[/]");
                Environment.Exit(1);
                break;
        }

        var sourceFile = new FileInfo(settings.SourceFilePath);
        AnsiConsole.Status()
            .Start("Checking file accessibility...", ctx =>
            {
                ctx.Spinner(Spinner.Known.BoxBounce);
                var validFile = IsValidAccessibleFile(sourceFile);
                if (!validFile.IsValid)
                {
                    AnsiConsole.MarkupLine($"[red] {validFile.Message}[/]");
                    Environment.Exit(1);
                }
            });
        AnsiConsole.Status()
            .Start("Processing file contents...", ctx =>
            {
                ctx.Spinner(Spinner.Known.Bounce);
                var fileProcessor = new FileProcessor(sourceFile, delimiter, column, ignoreComments);
                wordList = fileProcessor.Process();
                AnsiConsole.MarkupLine($"[green]Found {wordList.Count} words[/]");
                
            });
        AnsiConsole.Status()
            .Start("Writing processed words to file...", ctx =>
            {
                ctx.Spinner(Spinner.Known.Dots);
                CreateOutputFile(wordList, settings.OutputFilePath);
                GenerateStats(wordList);
            });
        
        return 0;
    }

    private static void DisplayTitle()
    {
        var fontPath = Path.Combine(AppContext.BaseDirectory, "fonts", "ANSI Shadow.flf");
        var font = FigletFont.Load(fontPath);
        var titleBanner = new FigletText(font, "Cyfrinair.Cymru")
        {
            Color = Color.Green,
            Justification = Justify.Center
        };

        var panel = new Panel(titleBanner);
        panel.Border(BoxBorder.Rounded);
        panel.BorderStyle(Style.Parse("green dim"));
        panel.Header("Croeso i...");
        AnsiConsole.Write(panel);
        AnsiConsole.WriteLine();
    }


    private (bool IsValid, string Message) IsValidAccessibleFile(FileInfo? file)
    {
        if (file is null)
        {
            return (false, "Source file is null");
        }

        if (!file.Exists)
        {
            return (false, "Source file does not exist");
        }

        if (file.Length == 0)
        {
            return (false, "Source file is empty");
        }

        try
        {
            using var stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
            return (true, "File is accessible");
        }
        catch (Exception ex)
        {
            return (false, $"Error opening file: {ex.Message}");
        }
    }

    private void CreateOutputFile(List<string> contents, string outputFilePath)
    {
        File.Delete(outputFilePath);
        File.WriteAllLines(outputFilePath, contents);
        AnsiConsole.MarkupLine($"[green]Processed words written to {outputFilePath}[/]");
    }

    private void GenerateStats(List<string> wordsList)
    {
        int count = wordsList.Count;

        // Entropy per word
        double entropyPerWord = Math.Log(count, 2);

        // Length stats
        int minLength = wordsList.Min(w => w.Length);
        int maxLength = wordsList.Max(w => w.Length);
        double avgLength = wordsList.Average(w => w.Length);

        // Distribution (optional)
        var lengthBuckets = wordsList
            .GroupBy(w => w.Length)
            .OrderBy(g => g.Key)
            .ToDictionary(g => g.Key, g => g.Count());

        AnsiConsole.WriteLine($"Word count: {count}");
        AnsiConsole.WriteLine($"Entropy per word: {entropyPerWord:F4} bits");
        AnsiConsole.WriteLine($"Min length: {minLength}");
        AnsiConsole.WriteLine($"Max length: {maxLength}");
        AnsiConsole.WriteLine($"Average length: {avgLength:F2}");

        AnsiConsole.WriteLine("\nLength distribution:");
        foreach (var kv in lengthBuckets)
        {
            AnsiConsole.WriteLine($"Length {kv.Key}: {kv.Value} words");
        }

        // Example: entropy for N-word passphrases
        for (int n = 4; n <= 8; n++)
        {
            double totalEntropy = n * entropyPerWord;
            AnsiConsole.WriteLine($"{n} words → {totalEntropy:F2} bits");
        }
    }
}