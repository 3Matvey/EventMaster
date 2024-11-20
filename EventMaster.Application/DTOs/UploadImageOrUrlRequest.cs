using Microsoft.AspNetCore.Http;

namespace EventMaster.Application.DTOs
{
    public record UploadImageOrUrlRequest
    (
        IFormFile? ImageFile,
        string? ImageUrl
    );
}
