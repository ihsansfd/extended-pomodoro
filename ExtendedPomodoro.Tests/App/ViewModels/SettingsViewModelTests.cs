using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ExtendedPomodoro.Core.Timeout;
using ExtendedPomodoro.Entities;
using ExtendedPomodoro.Helpers;
using ExtendedPomodoro.Models.Domains;
using ExtendedPomodoro.Models.Services.Interfaces;
using ExtendedPomodoro.Services.Interfaces;
using ExtendedPomodoro.ViewModels;
using ExtendedPomodoro.ViewServices.Interfaces;
using Moq;
using Moq.AutoMock;

namespace ExtendedPomodoro.Tests.App.ViewModels
{
    public class SettingsViewModelTests
    {
        private readonly AutoMocker _mocker = new AutoMocker();
        private readonly Mock<ISettingsService> _settingsServiceMock;
        private readonly Mock<IMessageBoxService> _messageBoxMock;
        private readonly Mock<ISettingsViewService> _settingsViewServiceMock;
        private readonly Mock<IAppSettingsProvider> _appSettingsProviderMock;
        private readonly Mock<RegisterWaitTimeoutCallback> _registerWaitTimeoutCallbackMock;
        private readonly SettingsViewModel _sut;

        public SettingsViewModelTests()
        {
            _settingsServiceMock = _mocker.GetMock<ISettingsService>();
            _messageBoxMock = _mocker.GetMock<IMessageBoxService>();
            _settingsViewServiceMock = _mocker.GetMock<ISettingsViewService>();
            _appSettingsProviderMock = _mocker.GetMock<IAppSettingsProvider>();
            _registerWaitTimeoutCallbackMock = _mocker.GetMock<RegisterWaitTimeoutCallback>();
            _sut = _mocker.CreateInstance<SettingsViewModel>();
        }

        [Fact]
        void PlayAlarmSoundTesterCommand_PlaySuccessfully()
        {
            _sut.PlayAlarmSoundTesterCommand.Execute(null);
        }

        [Fact]
        void ResetToDefaultCommand_WhenUserConfirms_ResetSuccessfully()
        {
            // Arrange
            SettingsDomain returnedDomain = new SettingsDomain()
            {
                PomodoroDuration = TimeSpan.FromMinutes(30),
                ShortBreakDuration = TimeSpan.FromMinutes(5),
                LongBreakDuration = TimeSpan.FromMinutes(20),
                LongBreakInterval = 3,
                DailyPomodoroTarget = 5,
                IsAutostart = true,
                AlarmSound = AlarmSound.Mechanics,
                Volume = 20,
                IsRepeatForever = true,
                PushNotificationEnabled = false,
                DarkModeEnabled = true,
                StartHotkeyDomain = new HotkeyDomain((int)ModifierKeys.Alt, (int)Key.A),
                PauseHotkeyDomain = new HotkeyDomain((int)ModifierKeys.Control, (int)Key.B)
            };

            _messageBoxMock.Setup((x) => x.Show(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<MessageBoxButton>(),
                It.IsAny<MessageBoxImage>()
            )).Returns(() => MessageBoxResult.Yes);

            _settingsServiceMock.Setup((x) => x.ResetToDefaultSettings());
            _settingsServiceMock.Setup((x) => x.GetSettings())
                .ReturnsAsync(returnedDomain);

            // Act
            _sut.ResetToDefaultSettingsCommand.Execute(null);

            // Assert
            _settingsServiceMock.Verify((x) => x.ResetToDefaultSettings(), Times.Once);
            _settingsServiceMock.Verify((x) => x.GetSettings(), Times.Once);
            Assert.Equal(_sut.PomodoroDurationInMinutes, returnedDomain.PomodoroDuration.TotalMinutes);
            Assert.Equal(_sut.ShortBreakDurationInMinutes, returnedDomain.ShortBreakDuration.TotalMinutes);
            Assert.Equal(_sut.LongBreakDurationInMinutes, returnedDomain.LongBreakDuration.TotalMinutes);
            Assert.Equal(_sut.LongBreakInterval, returnedDomain.LongBreakInterval);
            Assert.Equal(_sut.DailyPomodoroTarget, returnedDomain.DailyPomodoroTarget);
            Assert.Equal(_sut.IsAutostart, returnedDomain.IsAutostart);
            Assert.Equal(_sut.AlarmSound, (int)returnedDomain.AlarmSound);
            Assert.Equal(_sut.Volume, returnedDomain.Volume);
            Assert.Equal(_sut.IsRepeatForever, returnedDomain.IsRepeatForever);
            Assert.Equal(_sut.PushNotificationEnabled, returnedDomain.PushNotificationEnabled);
            Assert.Equal(_sut.DarkModeEnabled, returnedDomain.DarkModeEnabled);
            Assert.Equal(_sut.StartHotkey, returnedDomain.StartHotkeyDomain.ConvertToHotkey());
            Assert.Equal(_sut.PauseHotkey, returnedDomain.PauseHotkeyDomain.ConvertToHotkey());

        }

        [Fact]
        void ResetToDefaultCommand_WhenUserNotConfirm_NotReset()
        {
            // Arrange
            _messageBoxMock.Setup((x) => x.Show(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<MessageBoxButton>(),
                It.IsAny<MessageBoxImage>()
            )).Returns(() => MessageBoxResult.No);

            _settingsServiceMock.Setup((x) => x.ResetToDefaultSettings());
            _settingsServiceMock.Setup((x) => x.GetSettings());

            // Act
            _sut.ResetToDefaultSettingsCommand.Execute(null);

            // Assert
            _settingsServiceMock.Verify((x) => x.ResetToDefaultSettings(), Times.Never);
            _settingsServiceMock.Verify((x) => x.GetSettings(), Times.Never);

        }

        [Fact]
        async Task UpdateSettings_WhenAllDataValid_UpdateSuccessfully()
        {
            // Arrange
            _sut.PomodoroDurationInMinutes = 25;
            _sut.ShortBreakDurationInMinutes = 10;
            _sut.LongBreakDurationInMinutes = 15;
            _sut.LongBreakInterval = 4;
            _sut.IsAutostart = true;
            _sut.AlarmSound = (int)AlarmSound.Chimes;
            _sut.Volume = 100;
            _sut.IsRepeatForever = false;
            _sut.PushNotificationEnabled = true;
            _sut.DarkModeEnabled = false;
            _sut.StartHotkey = new Hotkey(Key.A, ModifierKeys.Alt);
            _sut.PauseHotkey = new Hotkey(Key.B, ModifierKeys.Control);

            bool? isOpenNotificationBeforeClosing = null;

            _settingsServiceMock.Setup((x) => 
                    x.UpdateSettings(It.IsAny<SettingsDomain>()));
            _appSettingsProviderMock.Setup((x) => x.LoadSettings());
            _registerWaitTimeoutCallbackMock.
                Setup((x) => x.Invoke(It.IsAny<Action>(), It.IsAny<TimeSpan>()))
                .Callback((Action action, TimeSpan _) => {
                    isOpenNotificationBeforeClosing = _sut.IsSuccessChangingNotificationOpen;
                    action.Invoke();
                });

            // Act
            _sut.PomodoroDurationInMinutes = 55;
            _sut.ShortBreakDurationInMinutes = 20;
            _sut.LongBreakDurationInMinutes = 25;
            _sut.LongBreakInterval = 10;
            _sut.IsAutostart = false;
            _sut.AlarmSound = (int)AlarmSound.Echo;
            _sut.Volume = 50;
            _sut.IsRepeatForever = true;
            _sut.PushNotificationEnabled = false;
            _sut.DarkModeEnabled = true;
            _sut.StartHotkey = new Hotkey(Key.Z, ModifierKeys.Control);
            _sut.PauseHotkey = new Hotkey(Key.G, ModifierKeys.Alt);

            await _sut.UpdateSettingsCommand.ExecuteAsync(null);

            // Assert
            _settingsServiceMock.Verify((x) => 
                x.UpdateSettings(It.IsAny<SettingsDomain>()), Times.Once);

            _appSettingsProviderMock.Verify((x) => x.LoadSettings(), Times.Once);
            Assert.True(isOpenNotificationBeforeClosing);
            Assert.False(_sut.IsSuccessChangingNotificationOpen);
            _registerWaitTimeoutCallbackMock.Verify(
                (x) => x.Invoke(It.IsAny<Action>(),
                    It.IsAny<TimeSpan>()), Times.Once);
        }

        [Fact]
        async Task UpdateSettings_WhenAnyDataInvalid_NotUpdating()
        {
            // Arrange
            _sut.PomodoroDurationInMinutes = 25;
            _sut.ShortBreakDurationInMinutes = 10;
            _sut.LongBreakDurationInMinutes = 15;
            _sut.LongBreakInterval = 4;
            _sut.IsAutostart = true;
            _sut.AlarmSound = (int)AlarmSound.Chimes;
            _sut.Volume = 100;
            _sut.IsRepeatForever = false;
            _sut.PushNotificationEnabled = true;
            _sut.DarkModeEnabled = false;
            _sut.StartHotkey = new Hotkey(Key.A, ModifierKeys.Alt);
            _sut.PauseHotkey = new Hotkey(Key.B, ModifierKeys.Control);

            _settingsServiceMock.Setup((x) =>
                x.UpdateSettings(It.IsAny<SettingsDomain>()));
            _appSettingsProviderMock.Setup((x) => x.LoadSettings());


            // Act
            _sut.PomodoroDurationInMinutes = 0;
            _sut.ShortBreakDurationInMinutes = 0;
            _sut.LongBreakDurationInMinutes = 0;
            _sut.LongBreakInterval = 10;
            _sut.IsAutostart = false;
            _sut.AlarmSound = (int)AlarmSound.Echo;
            _sut.Volume = 50;
            _sut.IsRepeatForever = true;
            _sut.PushNotificationEnabled = false;
            _sut.DarkModeEnabled = true;
            _sut.StartHotkey = new Hotkey(Key.Z, ModifierKeys.Control);
            _sut.PauseHotkey = new Hotkey(Key.G, ModifierKeys.Alt);

            await _sut.UpdateSettingsCommand.ExecuteAsync(null);

            // Assert
            _settingsServiceMock.Verify((x) =>
                x.UpdateSettings(It.IsAny<SettingsDomain>()), Times.Never);

            _appSettingsProviderMock.Verify((x) => x.LoadSettings(), Times.Never);
        }
    }
}
