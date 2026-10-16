using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Cyfrinair.BlazorWasm;
using Cyfrinair.Core;
using Microsoft.JSInterop;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<Password>(sp => new Password(new PasswordOptions()));
builder.Services.AddScoped<Passphrase>(sp => new Passphrase(new PassphraseOptions()));
builder.Services.AddScoped<Pin>();
builder.Services.AddClipboardCapability();

await builder.Build().RunAsync();