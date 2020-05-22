using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LightestNight.System.Caching;
using LightestNight.System.Utilities.Extensions;

namespace LightestNight.System.EventSourcing.Checkpoints.Redis
{
    public class RedisCheckpointManager : ICheckpointManager
    {
        private readonly ICache _cache;

        public RedisCheckpointManager(ICache cache)
        {
            _cache = cache;
        }

        public Task SetCheckpoint<TCheckpoint>(TCheckpoint checkpoint,
            [CallerMemberName] string? checkpointName = default, CancellationToken cancellationToken = default)
            where TCheckpoint : notnull
        {
            cancellationToken.ThrowIfCancellationRequested();

            return _cache.Save(checkpointName ?? Guid.NewGuid().ToString(), checkpoint);
        }

        public Task<TCheckpoint> GetCheckpoint<TCheckpoint>([CallerMemberName] string? checkpointName = null,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            return _cache.Get<TCheckpoint>(checkpointName.ThrowIfNull()!);
        }
    }
}