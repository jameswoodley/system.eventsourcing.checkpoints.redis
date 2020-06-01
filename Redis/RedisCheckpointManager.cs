using System.Threading;
using System.Threading.Tasks;
using LightestNight.System.Caching;

namespace LightestNight.System.EventSourcing.Checkpoints.Redis
{
    public class RedisCheckpointManager : ICheckpointManager
    {
        private readonly ICache _cache;

        public RedisCheckpointManager(ICache cache)
        {
            _cache = cache;
        }

        public Task SetCheckpoint(string checkpointName, long checkpoint, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return _cache.Save(checkpointName, checkpoint);
        }

        public async Task<long?> GetCheckpoint(string checkpointName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!await _cache.Exists<long>(checkpointName).ConfigureAwait(false)) 
                return null;
            
            var checkpoint = await _cache.Get<long>(checkpointName).ConfigureAwait(false);
            return checkpoint.Value;
        }

        public Task ClearCheckpoint(string checkpointName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return _cache.Delete<long>(checkpointName);
        }
    }
}