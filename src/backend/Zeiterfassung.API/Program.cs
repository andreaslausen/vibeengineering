using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);
// Hier können weitere Konfigurationen ergänzt werden

var app = builder.Build();

app.MapGet("/", () => "Zeiterfassung.API läuft!");

app.Run();
