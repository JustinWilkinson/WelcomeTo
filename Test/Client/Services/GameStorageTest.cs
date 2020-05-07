using Cloudcrate.AspNetCore.Blazor.Browser.Storage;
using WelcomeTo.Client.Services;
using Microsoft.JSInterop;
using Moq;
using NUnit.Framework;

namespace WelcomeTo.Test.Client.Services
{
    [TestFixture]
    public class GameStorageTest
    {
        private readonly Mock<IJsRuntimeWrapper> _mockJSRuntime = new Mock<IJsRuntimeWrapper>();
        private GameStorage _gameStorage;
        private const string GameId = "GameId";
        private const string PlayerName = "PlayerName";

        [SetUp]
        public void SetUp()
        {
            _mockJSRuntime.Reset();
            _mockJSRuntime.Setup(s => s.Invoke<string>(It.IsAny<string>(), GameId)).Returns(GameId);
            _mockJSRuntime.Setup(s => s.Invoke<string>(It.IsAny<string>(), PlayerName)).Returns(PlayerName);
            _gameStorage = new GameStorage(new LocalStorage(_mockJSRuntime.Object));
        }

        [Test]
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
            Assert.AreEqual(GameId, callOne);
            Assert.AreEqual(newGameId, callTwo);
        }


        [Test]
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
            Assert.AreEqual(PlayerName, callOne);
            Assert.AreEqual(newPlayerName, callTwo);
        }

        [Test]
        public void GameStorage_SettingGameIdToNull_RemovesGameId()
        {
            // Act
            _gameStorage.GameId = null;

            // Assert
            _mockJSRuntime.Verify(s => s.Invoke<object>(It.IsAny<string>(), GameId), Times.Once);
        }

        [Test]
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