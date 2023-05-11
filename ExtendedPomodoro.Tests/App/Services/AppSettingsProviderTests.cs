using CommunityToolkit.Mvvm.Messaging;
using ExtendedPomodoro.Messages;
using ExtendedPomodoro.Models.Domains;
using ExtendedPomodoro.Models.Services;
using ExtendedPomodoro.Models.Services.Interfaces;
using ExtendedPomodoro.Proxies;
using Moq;
using Moq.AutoMock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedPomodoro.Tests.App.Services
{
    public class AppSettingsProviderTests
    {
        private readonly AppSettingsProvider _appSettingsProvider;
        private readonly AutoMocker _mocker = new();
        private readonly Mock<IMessenger> _messenger;
        private readonly Mock<ISettingsService> _settingsService;


        public AppSettingsProviderTests()
        {
            _messenger = _mocker.GetMock<IMessenger>();
            _settingsService = _mocker.GetMock<ISettingsService>();
            _appSettingsProvider = _mocker.CreateInstance<AppSettingsProvider>();
        }

        [Fact]
        public async Task Initialize_ExecutedProperly()
        {
            _settingsService.Setup((x) => x.GetSettings()).ReturnsAsync(
                new SettingsDomain()
                {
                    PomodoroDuration = TimeSpan.FromMinutes(25),
                    ShortBreakDuration = TimeSpan.FromMinutes(10),
                    LongBreakDuration = TimeSpan.FromMinutes(10),
                    DailyPomodoroTarget = 10,
                    IsAutostart = true,
                });

            await _appSettingsProvider.Initialize();
        }

        [Fact]
        public async Task LoadSettings_ExecutedProperly()
        {
            // Arrange
            _messenger.Setup((x) => 
            x.Send(It.IsAny<SettingsUpdateInfoMessage>(), It.IsAny<IsAnyToken>()));

            _settingsService.Setup((x) => x.GetSettings()).ReturnsAsync(
                new SettingsDomain()
                {
                    PomodoroDuration = TimeSpan.FromMinutes(25),
                    ShortBreakDuration = TimeSpan.FromMinutes(10),
                    LongBreakDuration = TimeSpan.FromMinutes(10),
                    DailyPomodoroTarget = 10,
                    IsAutostart = true,
                });


            // Act
            var res = await _appSettingsProvider.LoadSettings();

            // Assert
            Assert.Equal(_appSettingsProvider.AppSettings, res);
            _messenger.Verify((x) => x.Send(
                new SettingsUpdateInfoMessage(_appSettingsProvider.AppSettings), 
                It.IsAny<IsAnyToken>()), Times.Once);
        }

    }
}
