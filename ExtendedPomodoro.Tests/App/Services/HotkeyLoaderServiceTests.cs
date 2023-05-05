using System.Windows.Input;
using CommunityToolkit.Mvvm.Messaging;
using ExtendedPomodoro.Entities;
using ExtendedPomodoro.Messages;
using ExtendedPomodoro.Services;
using ExtendedPomodoro.Services.Interfaces;
using Moq;
using Moq.AutoMock;
using NHotkey;

namespace ExtendedPomodoro.Tests.App.Services
{

    public class HotkeyLoaderServiceTests
    {
        private readonly AutoMocker _mocker = new AutoMocker();
        private readonly HotkeyLoaderService _sut;
        private readonly Mock<IMessenger> _messengerMock;
        private readonly Mock<IHotkeyManager> _hotkeyManagerMock;

        public HotkeyLoaderServiceTests()
        {
            _messengerMock = _mocker.GetMock<IMessenger>();
            _hotkeyManagerMock = _mocker.GetMock<IHotkeyManager>();
            _sut = _mocker.CreateInstance<HotkeyLoaderService>();
        }

        [Fact]
        void RegisterOrUpdateStartTimerHotkey_WhenEventInvoked_SendMessengerNotification()
        {
            // Arrange
            _hotkeyManagerMock.Setup((x) =>
                x.AddOrReplace(It.IsAny<string>(), It.IsAny<Key>(), It.IsAny<ModifierKeys>(),
                    It.IsAny<EventHandler<HotkeyEventArgs>>()))
                .Callback((string name, Key _, ModifierKeys _, EventHandler <HotkeyEventArgs> handler) =>
                    handler?.Invoke(_hotkeyManagerMock.Object, new HotkeyEventArgs(name)));

            _messengerMock.Setup((x) =>
                x.Send(It.IsAny<StartHotkeyTriggeredMessage>(), It.IsAny<IsAnyToken>()));

            // Act
            _sut.RegisterOrUpdateStartTimerHotkey(new Hotkey(Key.A, ModifierKeys.Alt));

            // Assert
            _messengerMock.Verify((x) => 
                x.Send(It.IsAny<StartHotkeyTriggeredMessage>(), It.IsAny<IsAnyToken>()), Times.Once);
        }


        [Fact]
        void RegisterOrUpdatePauseTimerHotkey_WhenEventInvoked_SendMessengerNotification()
        {
            // Arrange
            _hotkeyManagerMock.Setup((x) =>
                    x.AddOrReplace(It.IsAny<string>(), It.IsAny<Key>(), It.IsAny<ModifierKeys>(),
                        It.IsAny<EventHandler<HotkeyEventArgs>>()))
                .Callback((string name, Key _, ModifierKeys _, EventHandler<HotkeyEventArgs> handler) =>
                    handler?.Invoke(_hotkeyManagerMock.Object, new HotkeyEventArgs(name)));

            _messengerMock.Setup((x) =>
                x.Send(It.IsAny<PauseHotkeyTriggeredMessage>(), It.IsAny<IsAnyToken>()));

            // Act
            _sut.RegisterOrUpdatePauseTimerHotkey(new Hotkey(Key.A, ModifierKeys.Alt));

            // Assert
            _messengerMock.Verify((x) =>
                x.Send(It.IsAny<PauseHotkeyTriggeredMessage>(), It.IsAny<IsAnyToken>()), Times.Once);
        }

    }
}
