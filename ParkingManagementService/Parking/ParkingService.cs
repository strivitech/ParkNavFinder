using System.Data.Common;
using ErrorOr;
using Kafka.Events.Contracts.Parking.Management;
using ParkingManagementService.Common;
using ParkingManagementService.Database;
using ParkingManagementService.User;
using ParkingManagementService.Validation;

namespace ParkingManagementService.Parking;

// TODO: To be implemented correctly, events should be published in a transactional way. For example, using Outbox pattern.
internal class ParkingService(
    ParkingDbContext dbContext,
    IParkingServiceEventPublisher eventPublisher,
    ILogger<ParkingService> logger,
    ICurrentUserService currentUserService,
    IRequestValidator requestValidator) : IParkingService
{
    private readonly ParkingDbContext _dbContext = dbContext;
    private readonly IParkingServiceEventPublisher _eventPublisher = eventPublisher;
    private readonly ILogger<ParkingService> _logger = logger;
    private readonly ICurrentUserService _currentUserService = currentUserService;
    private readonly IRequestValidator _requestValidator = requestValidator;

    public async Task<ErrorOr<Created>> AddAsync(AddParkingRequest request)
    {
        var createEntityResult = CreateParkingEntityFromRequest(request);
        
        return await createEntityResult.MatchAsync(
            async parking => await AddParkingAsync(parking, request),
            errors => Task.FromResult<ErrorOr<Created>>(errors));
    }

    public async Task<ErrorOr<Updated>> UpdateAsync(UpdateParkingRequest request)
    {
        var updateEntityResult = await UpdateParkingEntityFromRequest(request);

        return await updateEntityResult.MatchAsync(
            async parking => await UpdateParkingAsync(request),
            errors => Task.FromResult<ErrorOr<Updated>>(errors));
    }

    public async Task<ErrorOr<Deleted>> DeleteAsync(DeleteParkingRequest request)
    {
        var deleteEntityResult = await DeleteParkingEntityFromRequest(request);
        
        return await deleteEntityResult.MatchAsync(
            async parking => await DeleteParkingAsync(parking, request),
            errors => Task.FromResult<ErrorOr<Deleted>>(errors));
    }

    public async Task<ErrorOr<GetParkingResponse>> GetAsync(GetParkingRequest request)
    {
        var errorList = _requestValidator.Validate(request);
        if (errorList.Count != 0)
        {
            return errorList;
        }
        
        var parking = await _dbContext.ParkingSet.FindAsync(request.Id);

        if (parking is null)
        {
            return Errors.Parking.NotFound(request.Id);
        }

        return MappersFinder.Parking.ToGetParkingResponse(parking);
    }

    private async Task<ErrorOr<Created>> AddParkingAsync(ParkingManagementService.Parking.Parking parking, AddParkingRequest request)
    {
        _dbContext.ParkingSet.Add(parking);

        var isSaved = await SaveChangesAsync();
        if (!isSaved)
        {
            return Errors.Parking.AddFailed(parking.Id);
        }

        var isPublished = await PublishParkingAddedEvent(
            new ParkingAddedEvent(parking.Id, request.Latitude, request.Longitude, DateTime.UtcNow),
            parking);

        return isPublished
            ? new Created()
            : Errors.Parking.AddFailed(parking.Id);
    }
    
    private ErrorOr<ParkingManagementService.Parking.Parking> CreateParkingEntityFromRequest(AddParkingRequest request)
    {      
        var errorList = _requestValidator.Validate(request);
        if (errorList.Count != 0)
        {   
            return errorList;
        }
        
        var parking = MappersFinder.Parking.ToEntity(request);
        parking.Id = Guid.NewGuid();
        parking.ProviderId = _currentUserService.SessionData.UserId;
        
        return parking;
    }

    private async Task<ErrorOr<Updated>> UpdateParkingAsync(UpdateParkingRequest request)
    {
        var isSaved = await SaveChangesAsync();
        if (!isSaved)
        {
            return Errors.Parking.UpdateFailed(request.Id);
        }

        await PublishParkingUpdatedEvent(new ParkingUpdatedEvent(request.Id, DateTime.UtcNow));

        return new Updated();
    }
    
    private async Task<ErrorOr<ParkingManagementService.Parking.Parking>> UpdateParkingEntityFromRequest(UpdateParkingRequest request)
    {
        var errorList = _requestValidator.Validate(request);
        if (errorList.Count != 0)
        {
            return errorList;
        }
        
        var parking = await _dbContext.ParkingSet.FindAsync(request.Id);

        if (parking is null)
        {
            return Errors.Parking.NotFound(request.Id);
        }

        if (parking.ProviderId != _currentUserService.SessionData.UserId)
        {
            return Errors.Parking.NotOwnedByCurrentUser(request.Id);
        }

        MappersFinder.Parking.ToEntity(request, parking);
        
        return parking;
    }

    private async Task<ErrorOr<Deleted>> DeleteParkingAsync(ParkingManagementService.Parking.Parking parking, DeleteParkingRequest request)
    {
        _dbContext.ParkingSet.Remove(parking);  

        var isSaved = await SaveChangesAsync();
        if (!isSaved)
        {
            return Errors.Parking.DeleteFailed(request.Id);
        }

        var isPublished = await PublishParkingDeletedEvent(new ParkingDeletedEvent(request.Id, DateTime.UtcNow));
        
        if (!isPublished)
        {
            return Errors.Parking.DeleteFailed(request.Id);
        }

        return new Deleted();
    }
    
    private async Task<ErrorOr<ParkingManagementService.Parking.Parking>> DeleteParkingEntityFromRequest(DeleteParkingRequest request)
    {
        var errorList = _requestValidator.Validate(request);
        if (errorList.Count != 0)   
        {
            return errorList;
        }
        
        var parking = await _dbContext.ParkingSet.FindAsync(request.Id);

        if (parking is null)
        {
            return Errors.Parking.NotFound(request.Id);
        }
        
        if (parking.ProviderId != _currentUserService.SessionData.UserId)
        {
            return Errors.Parking.NotOwnedByCurrentUser(request.Id);
        }
        
        return parking;
    }
    
    private async Task<bool> PublishParkingDeletedEvent(ParkingDeletedEvent parkingDeletedEvent)
    {
        var response = await _eventPublisher.PublishParkingDeletedAsync(parkingDeletedEvent);

        if (response.IsError)
        {
            _logger.LogError("Failed to publish parking deleted event");
        }

        return !response.IsError;
    }

    private async Task<bool> SaveChangesAsync()
    {
        try
        {
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch (DbException ex)
        {
            _logger.LogError(ex, "Failed to save changes to database");
            return false;
        }
    }

    // TODO: Add compensation logic to revert changes in case of failure.
    private async Task PublishParkingUpdatedEvent(ParkingUpdatedEvent parkingUpdatedEvent)
    {
        var response = await _eventPublisher.PublishParkingUpdatedAsync(parkingUpdatedEvent);

        if (response.IsError)
        {
            _logger.LogError("Failed to publish parking updated event");
        }
    }

    private async Task<bool> PublishParkingAddedEvent(ParkingAddedEvent parkingAddedEvent, ParkingManagementService.Parking.Parking parking)
    {
        var response = await _eventPublisher.PublishParkingAddedAsync(parkingAddedEvent);

        if (response.IsError)
        {
            await CompensateOperation();
        }

        return !response.IsError;

        async Task CompensateOperation()
        {
            _dbContext.ParkingSet.Remove(parking);
            await _dbContext.SaveChangesAsync();
        }
    }
}