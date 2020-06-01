using System;
using System.Threading;
using System.Threading.Tasks;
using LightestNight.System.Caching;
using Moq;
using Shouldly;
using Xunit;

namespace LightestNight.System.EventSourcing.Checkpoints.Redis.Tests
{
    public class RedisCheckpointManagerTests
    {
        private const string CheckpointName = "TestCheckpoint";
        private const long Checkpoint = 100;
        
        private readonly Mock<ICache> _cacheMock = new Mock<ICache>();
        private readonly ICheckpointManager _sut;
        
        public RedisCheckpointManagerTests()
        {
            _sut = new RedisCheckpointManager(_cacheMock.Object);
        }

        [Fact]
        public void ShouldThrowIfCancellationRequestedWhenSettingCheckpoint()
        {
            // Arrange
            using var cancellationSource = new CancellationTokenSource();
            var token = cancellationSource.Token;
            cancellationSource.Cancel();
            
            // Act/Assert
            Should.Throw<TaskCanceledException>(async () =>
                await _sut.SetCheckpoint(CheckpointName, Checkpoint, token).ConfigureAwait(false));
        }

        [Fact]
        public async Task ShouldSetCheckpointToGivenValue()
        {
            // Act
            await _sut.SetCheckpoint(CheckpointName, Checkpoint).ConfigureAwait(false);

            // Assert
            _cacheMock.Verify(
                cache => cache.Save(CheckpointName, Checkpoint, null, Array.Empty<string>()),
                Times.Once);
        }
        
        [Fact]
        public async Task ShouldSetCheckpointToGivenValueAndName()
        {
            // Act
            await _sut.SetCheckpoint(CheckpointName, Checkpoint).ConfigureAwait(false);

            // Assert
            _cacheMock.Verify(
                cache => cache.Save(CheckpointName, Checkpoint, null, Array.Empty<string>()),
                Times.Once);
        }
        
        [Fact]
        public void ShouldThrowIfCancellationRequestedWhenGettingCheckpoint()
        {
            // Arrange
            using var cancellationSource = new CancellationTokenSource();
            var token = cancellationSource.Token;
            cancellationSource.Cancel();
            
            // Act/Assert
            Should.Throw<TaskCanceledException>(async () =>
                await _sut.GetCheckpoint(CheckpointName, token).ConfigureAwait(false));
        }

        [Fact]
        public async Task ShouldCallForCheckpoint()
        {
            // Arrange
            _cacheMock.Setup(cache => cache.Exists<long>(CheckpointName)).ReturnsAsync(true);
            _cacheMock.Setup(cache => cache.Get<long>(CheckpointName))
                .ReturnsAsync(new CacheItem<long>{Value = 100})
                .Verifiable();
            
            // Act
            await _sut.GetCheckpoint(CheckpointName).ConfigureAwait(false);
            
            // Assert
            _cacheMock.Verify();
        }

        [Fact]
        public async Task ShouldReturnNullIfCheckpointNotFound()
        {
            // Arrange
            _cacheMock.Setup(cache => cache.Exists<long>(CheckpointName)).ReturnsAsync(false);
            
            // Act
            var result = await _sut.GetCheckpoint(CheckpointName).ConfigureAwait(false);
            
            // Assert
            result.ShouldBeNull();
        }
        
        [Fact]
        public void ShouldThrowIfCancellationRequestedWhenClearingCheckpoint()
        {
            // Arrange
            using var cancellationSource = new CancellationTokenSource();
            var token = cancellationSource.Token;
            cancellationSource.Cancel();
            
            // Act/Assert
            Should.Throw<TaskCanceledException>(async () =>
                await _sut.ClearCheckpoint(CheckpointName, token).ConfigureAwait(false));
        }
        
        [Fact]
        public async Task ShouldCallToClearCheckpoint()
        {
            // Act
            await _sut.ClearCheckpoint(CheckpointName).ConfigureAwait(false);
            
            // Assert
            _cacheMock.Verify(cache => cache.Delete<long>(CheckpointName), Times.Once);
        }
    }
}