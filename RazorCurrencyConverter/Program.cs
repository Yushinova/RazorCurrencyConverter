var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddHttpClient();
var app = builder.Build();

app.MapRazorPages();
app.UseStaticFiles();
app.Run();
