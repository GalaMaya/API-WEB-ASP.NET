namespace WebDevToCSharp.Endpoints;

public static class FileEndpoints
{
    public static void MapFileEndpoints(this WebApplication app)
    {
        app.MapPost("/api/files/upload", async (IFormFile file) =>
        {
            if (file == null || file.Length == 0)
                return Results.BadRequest(new { message = "No file uploaded" });

            // Validasi ukuran (max 5MB)
            if (file.Length > 5 * 1024 * 1024)
                return Results.BadRequest(new { message = "File too large (max 5MB)" });

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // Generate unique filename
            var ext = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return Results.Ok(new
            {
                message = "File uploaded successfully",
                fileName = fileName,
                size = file.Length,
                url = $"/uploads/{fileName}"
            });
        }).DisableAntiforgery(); // Perlu untuk API (bukan form web)
    }
}