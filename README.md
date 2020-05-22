# Lightest Night
## Event Sourcing > Checkpoints > Redis

The elements required to manage a Stream checkpoint inside a Redis data store

#### How To Use
##### Registration
* Asp.Net Standard/Core Dependency Injection
  * Use the provided `services.AddRedisCheckpointManagement(Action<CacheConfig>? configAction = null)` method
    * If the configAction is supplied, an attempt to use it registering the ICache will be made, if not supplied then it is assumed ICache has already been registered

* Other Containers
  * Register an instance of `IRedisCacheProvider` as a Singleton
  * Register an instance of `ICache` as a Singleton
  * Register an instance of `ICheckpointManager` as a Singleton against the concrete class `RedisCheckpointManager`

##### Usage
* `Task SetCheckpoint<TCheckpoint>(TCheckpoint checkpoint, [CallerMemberName] string? checkpointName = default, CancellationToken cancellationToken = default)`
  * An asynchronous function to call when setting the checkpoint
* `Task<TCheckpoint> GetCheckpoint<TCheckpoint>([CallerMemberName] string? checkpointName = null, CancellationToken cancellationToken = default)`
  * An asynchronous function to call when getting the checkpoint