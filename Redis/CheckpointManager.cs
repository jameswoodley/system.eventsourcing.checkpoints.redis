﻿using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LightestNight.System.Caching;
using LightestNight.System.Utilities.Extensions;

namespace LightestNight.System.EventSourcing.Checkpoints.Redis
{
    public class CheckpointManager
    {
        private readonly ICache _cache;

        public CheckpointManager(ICache cache)
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
            CancellationToken cancellationToken = new CancellationToken())
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            return _cache.Get<TCheckpoint>(checkpointName.ThrowIfNull()!);
        }
    }
}