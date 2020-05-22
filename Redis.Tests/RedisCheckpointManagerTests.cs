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
            Should.Throw<TaskCanceledException>(async () => await _sut.SetCheckpoint(Checkpoint, cancellationToken: token).ConfigureAwait(false));
        }

        [Fact]
        public async Task ShouldSetCheckpointToGivenValue()
        {
            // Act
            await _sut.SetCheckpoint(Checkpoint).ConfigureAwait(false);

            // Assert
            _cacheMock.Verify(
                cache => cache.Save(nameof(ShouldSetCheckpointToGivenValue), Checkpoint, null, Array.Empty<string>()),
                Times.Once);
        }
        
        [Fact]
        public async Task ShouldSetCheckpointToGivenValueAndName()
        {
            // Arrange
            const string name = "Test";
            
            // Act
            await _sut.SetCheckpoint(Checkpoint, name).ConfigureAwait(false);

            // Assert
            _cacheMock.Verify(
                cache => cache.Save(name, Checkpoint, null, Array.Empty<string>()),
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
            Should.Throw<TaskCanceledException>(async () => await _sut.GetCheckpoint<object>(cancellationToken: token).ConfigureAwait(false));
        }

        [Fact]
        public void ShouldThrowIfNameIsNullWhenGettingCheckpoint()
        {
            // Act/Assert
            Should.Throw<ArgumentNullException>(async () => await _sut.GetCheckpoint<long>(null).ConfigureAwait(false));
        }

        [Fact]
        public async Task ShouldCallForCheckpoint()
        {
            // Arrange
            _cacheMock.Setup(cache => cache.Get<long>(nameof(ShouldCallForCheckpoint)))
                .ReturnsAsync(100)
                .Verifiable();
            
            // Act
            await _sut.GetCheckpoint<long>().ConfigureAwait(false);
            
            // Assert
            _cacheMock.Verify();
        }
        
        [Fact]
        public void ShouldThrowIfCancellationRequestedWhenClearingCheckpoint()
        {
            // Arrange
            using var cancellationSource = new CancellationTokenSource();
            var token = cancellationSource.Token;
            cancellationSource.Cancel();
            
            // Act/Assert
            Should.Throw<TaskCanceledException>(async () => await _sut.ClearCheckpoint(cancellationToken: token).ConfigureAwait(false));
        }
        
        [Fact]
        public void ShouldThrowIfNameIsNullWhenClearingCheckpoint()
        {
            // Act/Assert
            Should.Throw<ArgumentNullException>(async () => await _sut.ClearCheckpoint(null).ConfigureAwait(false));
        }
        
        [Fact]
        public async Task ShouldCallToClearCheckpoint()
        {
            // Act
            await _sut.ClearCheckpoint().ConfigureAwait(false);
            
            // Assert
            _cacheMock.Verify(cache => cache.Delete<object>(nameof(ShouldCallToClearCheckpoint)), Times.Once);
        }
    }
}