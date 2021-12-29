using Cloudcrate.AspNetCore.Blazor.Browser.Storage;
using Microsoft.JSInterop;
using Moq;
using WelcomeTo.Client.Services;
using Xunit;

namespace WelcomeTo.Test.Client.Services
{
    public class GameStorageTest
    {
        private readonly Mock<IJsRuntimeWrapper> _mockJSRuntime = new();
        private readonly GameStorage _gameStorage;
        private const string GameId = "GameId";
        private const string PlayerName = "PlayerName";

        public GameStorageTest()
        {
            _mockJSRuntime.Setup(s => s.Invoke<string>(It.IsAny<string>(), GameId)).Returns(GameId);
            _mockJSRuntime.Setup(s => s.Invoke<string>(It.IsAny<string>(), PlayerName)).Returns(PlayerName);
            _gameStorage = new GameStorage(new LocalStorage(_mockJSRuntime.Object));
        }

        [Fact]
        public void GameStorage_Caches_GameId()
        {
            // Arrange
            var newGameId = "NewGameId";

            // Act
            var callOne = _gameStorage.GameId;
            _gameStorage.GameId = newGameId;
            var callTwo = _gameStorage.GameId;

            // Assert
            _mockJSRuntime.Verify(s => s.Invoke<string>(It.IsAny<string>(), GameId), Times.Once);
            _mockJSRuntime.Verify(s => s.Invoke<object>(It.IsAny<string>(), GameId, newGameId), Times.Once);
            Assert.Equal(GameId, callOne);
            Assert.Equal(newGameId, callTwo);
        }


        [Fact]
        public void GameStorage_Caches_PlayerName()
        {
            // Arrange
            var newPlayerName = "NewPlayerName";

            // Act
            var callOne = _gameStorage.PlayerName;
            _gameStorage.PlayerName = newPlayerName;
            var callTwo = _gameStorage.PlayerName;

            // Assert
            _mockJSRuntime.Verify(s => s.Invoke<string>(It.IsAny<string>(), PlayerName), Times.Once);
            _mockJSRuntime.Verify(s => s.Invoke<object>(It.IsAny<string>(), PlayerName, newPlayerName), Times.Once);
            Assert.Equal(PlayerName, callOne);
            Assert.Equal(newPlayerName, callTwo);
        }

        [Fact]
        public void GameStorage_SettingGameIdToNull_RemovesGameId()
        {
            // Act
            _gameStorage.GameId = null;

            // Assert
            _mockJSRuntime.Verify(s => s.Invoke<object>(It.IsAny<string>(), GameId), Times.Once);
        }

        [Fact]
        public void GameStorage_SettingPlayerNameToNull_RemovesPlayerName()
        {
            // Act
            _gameStorage.PlayerName = null;

            // Assert
            _mockJSRuntime.Verify(s => s.Invoke<object>(It.IsAny<string>(), PlayerName), Times.Once);
        }

        public interface IJsRuntimeWrapper : IJSRuntime, IJSInProcessRuntime
        {

        }
    }
}