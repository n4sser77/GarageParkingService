using Microsoft.Extensions.FileProviders;

namespace FrontEnd
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            // Serve files from the "public" folder
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "public")),
                RequestPath = "" // optional, this can be left empty to serve at root, or you can set a specific route like "/assets"
            });

            // Redirect the root to serve index.html
            app.MapGet("/", () => Results.Redirect("/index.html"));

            app.Run();

        }
    }
}
