using EventMaster.Server.UnitOfWork;

namespace EventMaster.Server.UseCases
{
    public class UploadEventImageUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public UploadEventImageUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<(bool Success, string Message, string? ImagePath)> Execute(int eventId, IFormFile? imageFile, string? imageUrl)
        {
            var eventItem = await _unitOfWork.EventRepository.GetEventByIdAsync(eventId);
            if (eventItem == null)
            {
                return (false, "Event not found", null);
            }

            if (imageFile != null && imageFile.Length > 0)
            {
                await _unitOfWork.EventRepository.UploadImageFileAsync(eventItem, imageFile);
            }
            else if (!string.IsNullOrEmpty(imageUrl))
            {
                await _unitOfWork.EventRepository.UploadImageFromUrlAsync(eventItem, imageUrl);
            }
            else
            {
                return (false, "No file or URL was provided.", null);
            }

            await _unitOfWork.SaveAsync();
            return (true, "Image successfully uploaded.", eventItem.ImagePath);
        }
    }
}
