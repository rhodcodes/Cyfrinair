using Cyfrinair.Core;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info.Title = "Cyfrinair.Cymru API";
        document.Info.Version = "0.0.1-beta";
        document.Info.Description =
            "API ar gyfer creu cyfrineiriau cyfrinymadroddion Cymraeg a rhifau PIN. <br>" 
            + "API for generating passwords, Welsh passphrases and PINs";
        
        return Task.CompletedTask;
    });
});

var app = builder.Build();

app.MapOpenApi();

app.MapScalarApiReference("/docs",options =>
{
    options.WithTitle("API Cyfrinair.Cymru")
        .WithTheme(ScalarTheme.Solarized)
        .WithDefaultHttpClient(ScalarTarget.PowerShell, ScalarClient.RestMethod)
        .DisableMcp()
        .WithClassicLayout()
        .ExpandAllModelSections()
        .ExpandAllResponses().ExpandAllTags();
    options.ShowSidebar = false;
});
app.UseHttpsRedirection();

var password = app.MapGroup("/password").WithTags("api.cyfrinair.cymru");
var passphrase = app.MapGroup("/passphrase").WithTags("api.cyfrinair.cymru");
var pin = app.MapGroup("/pin").WithTags("api.cyfrinair.cymru");

app.MapGet("/", () => Results.Redirect("/docs"))
    .ExcludeFromDescription()
    .ExcludeFromApiReference();
password.MapGet("/{quantity:int?}", GetPassword);
passphrase.MapGet("{quantity:int?}", GetPassphrase);
pin.MapGet("{quantity:int?}", GetPin);


app.Run();
return;

static IResult GetPassword(int quantity = 1,
    int? length = 25,
    bool includeDigits = true,
    bool includeSymbols = true,
    bool includeAmbiguousChars = false)
{
    
    if (quantity is < 1 or > 1000)
    {
        return TypedResults.BadRequest("Quantity must be between 5 and 1000.");
    }

    if (length is < 5 or > 1000)
    {
        return TypedResults.BadRequest("Length must be between 5 and 1000 characters.");
    }

    var options = new PasswordOptions()
    {
        Length = length ?? 25,
        IncludeDigits = includeDigits,
        IncludeSymbols = includeSymbols,
        IncludeAmbiguousChars = includeAmbiguousChars,
    };
    
    var passwordGenerator = new Password(options);
    var results = Enumerable.Range(0, quantity)
        .Select(_ => passwordGenerator.Generate())
        .ToArray();

    return TypedResults.Ok(results);
}

static IResult GetPassphrase(int quantity = 1,
    int words = 4,
    string separator = "-",
    string digitPlacement = "once",
    string casing = "upper")
{
    if (quantity is < 1 or > 1000) return TypedResults.BadRequest("Quantity must be between 1 and 1000.");
    if (words is < 1 or > 1000) return TypedResults.BadRequest("Words must be between 1 and 1000.");
    
    var options = new PassphraseOptions()
    {
        Words = words,
        Separator = separator[0],
        DigitPlacement = digitPlacement.ToDigitPlacementOption(),
        Casing = casing.ToCasingOption()
    };

    var passphraseGenerator = new Passphrase(options);
    var results = Enumerable.Range(0, quantity)
        .Select(_ => passphraseGenerator.Generate())
        .ToArray();
    return TypedResults.Ok(results);
}

static IResult GetPin(int quantity = 1, int? length = 4)
{
    if (quantity is < 1 or > 1000) return TypedResults.BadRequest("Quantity must be between 1 and 1000");
    if (length is < 1 or > 1000) return TypedResults.BadRequest("Length must be between 1 and 1000");
    
    var pinGenerator = new Pin();
    var results = Enumerable.Range(0, quantity)
        .Select(_ => pinGenerator.Generate(length ?? 4))
        .ToArray();
    return TypedResults.Ok(results);   
}