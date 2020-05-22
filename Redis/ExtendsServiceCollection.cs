using System;
using LightestNight.System.Caching.Redis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LightestNight.System.EventSourcing.Checkpoints.Redis
{
    public static class ExtendsServiceCollection
    {
        public static IServiceCollection AddRedisCheckpointManagement(this IServiceCollection services,
            Action<CacheConfig>? configAction = null)
        {
            if (configAction != null)
                services.AddRedisCache(configAction);

            services.TryAddSingleton<ICheckpointManager, RedisCheckpointManager>();

            return services;
        }
    }
}