namespace EventMaster.Application.UseCases
{
    public class UploadEventImageUseCase(IUnitOfWork unitOfWork)
    {
        public async Task<Result<string>> Execute(int eventId, UploadImageOrUrlRequest uploadImage)
        {
            var eventItem = await unitOfWork.EventRepository.GetEventByIdAsync(eventId);
            if (eventItem == null)
            {
                return Error.NotFound("EventNotFound", "Event not found.");
            }

            if (uploadImage.ImageFile != null && uploadImage.ImageFile.Length > 0)
            {
                try
                {
                    await unitOfWork.EventRepository.UploadImageFileAsync(eventItem, uploadImage.ImageFile);
                }
                catch (Exception)
                {
                    return Error.Failure("ImageUploadFailed", "An error occurred while uploading the image file.");
                }
            }
            else if (!string.IsNullOrEmpty(uploadImage.ImageUrl))
            {
                try
                {
                    await unitOfWork.EventRepository.UploadImageFromUrlAsync(eventItem, uploadImage.ImageUrl);
                }
                catch (Exception)
                {
                    return Error.Failure("ImageUploadFailed", "An error occurred while uploading the image from URL.");
                }
            }
            else
            {
                return Error.Validation("NoImageProvided", "No file or URL was provided.");
            }

            try
            {
                await unitOfWork.SaveAsync();
                return Result<string>.Success(eventItem.ImagePath ?? string.Empty);
            }
            catch (Exception)
            {
                return Error.Failure("SaveFailed", "An error occurred while saving the event.");
            }
        }
    }
}
