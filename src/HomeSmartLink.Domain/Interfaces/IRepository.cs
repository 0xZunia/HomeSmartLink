using HomeSmartLink.Domain.Entities;

namespace HomeSmartLink.Domain.Interfaces;

public interface IRepository<TEntity, TId> where TEntity : Entity<TId> where TId : notnull
{
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
}

public interface IHomeRepository : IRepository<Home, string>
{
    Task<IReadOnlyList<Home>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<Home?> GetWithRoomsAndDevicesAsync(string id, CancellationToken cancellationToken = default);
}

public interface IRoomRepository : IRepository<Room, string>
{
    Task<IReadOnlyList<Room>> GetByHomeIdAsync(string homeId, CancellationToken cancellationToken = default);
    Task<Room?> GetWithDevicesAsync(string id, CancellationToken cancellationToken = default);
}

public interface IDeviceRepository : IRepository<Device, string>
{
    Task<IReadOnlyList<Device>> GetByHomeIdAsync(string homeId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Device>> GetByRoomIdAsync(string roomId, CancellationToken cancellationToken = default);
    Task<Device?> GetByIdentifierAsync(string identifier, CancellationToken cancellationToken = default);
}

public interface IUserRepository : IRepository<User, string>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}

public interface IInvitationRepository : IRepository<Invitation, string>
{
    Task<IReadOnlyList<Invitation>> GetByHomeIdAsync(string homeId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Invitation>> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Invitation?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
}

public interface IConsumptionRepository : IRepository<ConsumptionRecord, string>
{
    Task<IReadOnlyList<ConsumptionRecord>> GetByHomeIdAsync(
        string homeId,
        DateTime from,
        DateTime to,
        ConsumptionPeriod period,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ConsumptionRecord>> GetByRoomIdAsync(
        string roomId,
        DateTime from,
        DateTime to,
        ConsumptionPeriod period,
        CancellationToken cancellationToken = default);
}
