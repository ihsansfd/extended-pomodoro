using ExtendedPomodoro.Models.Repositories;
using ExtendedPomodoro.Models.Services;
using Moq.AutoMock;
using Moq;
using ExtendedPomodoro.Models.DTOs;
using ExtendedPomodoro.Models.Domains;
using System.Windows.Input;
using System.Text.Json;

namespace ExtendedPomodoro.Tests.Models.Services
{
    public class SettingsServiceTests
    {
        private readonly AutoMocker _mocker;
        private readonly Mock<ISettingsRepository> _repositoryMock;
        private readonly SettingsService _settingsService;

        public SettingsServiceTests()
        {
            _mocker = new AutoMocker();
            _repositoryMock = _mocker.GetMock<ISettingsRepository>();
            _settingsService = _mocker.CreateInstance<SettingsService>();
        }

        [Fact]
        public async Task GetSettings_ReturnMainSettings()
        {
            // Arrange
            var dbRows = new List<SettingsDTO>
            {
                new SettingsDTO()
                {
                    SettingsType = "MAIN",
                    DailyPomodoroTarget = 5
                },

                new SettingsDTO()
                {
                    SettingsType = "DEFAULT",
                    DailyPomodoroTarget = 10
                }
            };

            _repositoryMock.Setup((x) => x.GetMainSettings())
                .ReturnsAsync(dbRows.Where((row) => row.SettingsType == "MAIN").First());

            // Act
            var res = await _settingsService.GetSettings();

            var dailyPomodoroTargetExpectation =
                dbRows.Where((row) => row.SettingsType == "MAIN").First().DailyPomodoroTarget;

            // Assert
            Assert.Equal(dailyPomodoroTargetExpectation, res.DailyPomodoroTarget);
        }

        [Fact]
        public async Task UpdateSettings_MainSettingsUpdatedProperly()
        {
            // Arrange
            var dbRows = new List<SettingsDTO>()
            {
                new()
                {
                    SettingsType = "MAIN",
                    PomodoroDurationInSeconds = (int)TimeSpan.FromMinutes(25).TotalSeconds,
                    ShortBreakDurationInSeconds = (int)TimeSpan.FromMinutes(5).TotalSeconds,
                    LongBreakDurationInSeconds = (int)TimeSpan.FromMinutes(15).TotalSeconds,
                    DailyPomodoroTarget = 10,
                    IsAutostart = false,
                    AlarmSound = (int)AlarmSound.Chimes,
                    Volume = 50,
                    IsRepeatForever = false,
                    PushNotificationEnabled = true,
                    DarkModeEnabled = false,
                    StartHotkey = "{\"ModifierKeys\":5,\"Key\":62}",
                    PauseHotkey = "{\"ModifierKeys\":5,\"Key\":59}"
                },
                new()
                {
                    SettingsType = "DEFAULT"
                }
            };

            _repositoryMock.Setup((x) => x.UpdateSettings(It.IsAny<SettingsDTO>()))
                .Callback((SettingsDTO dto) =>
                {
                    var main = dbRows.Where((row) => row.SettingsType == "MAIN").First();
                    main.PomodoroDurationInSeconds = dto.PomodoroDurationInSeconds;
                    main.ShortBreakDurationInSeconds = dto.ShortBreakDurationInSeconds;
                    main.LongBreakDurationInSeconds = dto.LongBreakDurationInSeconds;
                    main.DailyPomodoroTarget = dto.DailyPomodoroTarget;
                    main.IsAutostart = dto.IsAutostart;
                    main.AlarmSound = dto.AlarmSound;
                    main.Volume = dto.Volume;
                    main.IsRepeatForever = dto.IsRepeatForever;
                    main.PushNotificationEnabled = dto.PushNotificationEnabled;
                    main.DarkModeEnabled = dto.DarkModeEnabled;
                    main.StartHotkey = dto.StartHotkey;
                    main.PauseHotkey = dto.PauseHotkey;

                });

            // Act
            var param = new SettingsDomain()
            {
                PomodoroDuration = TimeSpan.FromMinutes(30),
                ShortBreakDuration = TimeSpan.FromMinutes(10),
                LongBreakDuration = TimeSpan.FromMinutes(20),
                DailyPomodoroTarget = 5,
                IsAutostart = true,
                AlarmSound = AlarmSound.Retro,
                Volume = 70,
                IsRepeatForever = true,
                PushNotificationEnabled = false,
                DarkModeEnabled = true,
                StartHotkeyDomain = new HotkeyDomain((int)ModifierKeys.Shift, (int)Key.A),
                PauseHotkeyDomain = new HotkeyDomain((int)ModifierKeys.Windows, (int)Key.B)
            };

            await _settingsService.UpdateSettings(param);

            // Assert
            var main = dbRows.Where((row) => row.SettingsType == "MAIN").First();

            Assert.Equal(param.PomodoroDuration.TotalSeconds, main.PomodoroDurationInSeconds);
            Assert.Equal(param.ShortBreakDuration.TotalSeconds, main.ShortBreakDurationInSeconds);
            Assert.Equal(param.LongBreakDuration.TotalSeconds, main.LongBreakDurationInSeconds);
            Assert.Equal(param.DailyPomodoroTarget, main.DailyPomodoroTarget);
            Assert.Equal(param.IsAutostart, main.IsAutostart);
            Assert.Equal((int)param.AlarmSound, (int)main.AlarmSound);
            Assert.Equal(param.Volume, main.Volume);
            Assert.Equal(param.IsRepeatForever, main.IsRepeatForever);
            Assert.Equal(param.PushNotificationEnabled, main.PushNotificationEnabled);
            Assert.Equal(param.DarkModeEnabled, main.DarkModeEnabled);
            Assert.Equal(JsonSerializer.Serialize(param.StartHotkeyDomain), 
                main.StartHotkey?.Replace(" ", ""));
            Assert.Equal(JsonSerializer.Serialize(param.PauseHotkeyDomain),
                main.PauseHotkey?.Replace(" ", ""));
        }

        [Fact]
        public async Task ResetToDefaultSettings_MainSettingsUpdatedToDefaultSettings()
        {
            // Arrange
            var dbRows = new List<SettingsDTO>()
            {
                new()
                {
                    SettingsType = "MAIN",
                    DailyPomodoroTarget = 5,
                },
                new()
                {
                    SettingsType = "DEFAULT",
                    DailyPomodoroTarget = 10
                }
            };

            var main = dbRows.Where((row) => row.SettingsType == "MAIN").First();
            var def = dbRows.Where((row) => row.SettingsType == "DEFAULT").First();

            _repositoryMock.Setup((x) => x.GetDefaultSettings())
                .ReturnsAsync(def);

            _repositoryMock.Setup((x) => x.UpdateSettings(It.IsAny<SettingsDTO>()))
                .Callback((SettingsDTO dto) => main.DailyPomodoroTarget = dto.DailyPomodoroTarget);

            // Act
            await _settingsService.ResetToDefaultSettings();

            // Assert
            Assert.Equal(def.DailyPomodoroTarget, main.DailyPomodoroTarget);
        }
    }
}
