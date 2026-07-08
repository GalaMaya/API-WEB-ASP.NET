namespace WebDevToCSharp.Endpoints;

public static class FileEndpoints
{
    public static void MapFileEndpoints(this WebApplication app)
    {
        app.MapPost("/api/files/upload", async (IFormFile file, HttpContext http) =>
        {
            if (file == null || file.Length == 0)
                return Results.BadRequest(new { message = "No file uploaded" });

            // Validasi ukuran (max 5MB)
            if (file.Length > 5 * 1024 * 1024)
                return Results.BadRequest(new { message = "File too large (max 5MB)" });

            // File Type validation
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(ext))
                return Results.BadRequest(new { message = "Invalid file type" });

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // Generate unique filename
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
        })
        .RequireAuthorization()
        .DisableAntiforgery();
    }
}