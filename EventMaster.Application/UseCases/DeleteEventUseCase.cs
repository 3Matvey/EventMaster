namespace EventMaster.Application.UseCases
{
    public class DeleteEventUseCase(IUnitOfWork unitOfWork)
    {
        public async Task<Result> Execute(int eventId)
        {
            var eventItem = await unitOfWork.EventRepository.GetEventByIdAsync(eventId);
            if (eventItem == null)
            {
                return Error.NotFound("EventNotFound", "Event not found.");
            }

            try
            {
                await unitOfWork.EventRepository.DeleteEventAsync(eventItem);
                await unitOfWork.SaveAsync();

                return Result.Success();
            }
            catch (Exception)
            {
                return Error.Failure("DeleteEventError", "An error occurred while deleting the event.");
            }
        }
    }
}