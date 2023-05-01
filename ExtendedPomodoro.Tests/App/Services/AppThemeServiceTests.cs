using CommunityToolkit.Mvvm.Messaging;
using ExtendedPomodoro.Converters;
using ExtendedPomodoro.Services;
using ExtendedPomodoro.Tests.App.Converters;
using Moq;
using Moq.AutoMock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ExtendedPomodoro.Tests.App.Services
{
    public class AppThemeServiceTests : IClassFixture<AppThemeServiceTestsFixture>
    {
        private readonly AppThemeServiceTestsFixture _fixture;
        private readonly AutoMocker _mocker = new();
        private readonly Mock<IMessenger> _messenger;
        private readonly AppThemeService _appThemeService;

        public AppThemeServiceTests(AppThemeServiceTestsFixture fixture)
        {
            _fixture = fixture;
            _messenger = _mocker.GetMock<IMessenger>();
            _appThemeService = new(AppThemeServiceTestsFixture.LIGHT_THEME_RESOURCE, 
                AppThemeServiceTestsFixture.DARK_THEME_RESOURCE, _messenger.Object);
        }

        [Fact]
        public void SwitchThemeTo_WhenDark_AddDarkThemeInTheAppResource()
        {
            _appThemeService.SwitchThemeTo(AppTheme.Dark);

            Assert.Equal(AppThemeServiceTestsFixture.DARK_THEME_RESOURCE,
                Application.Current.Resources.MergedDictionaries[0]);
        }

        [Fact]
        public void SwitchThemeTo_WhenLight_AddLightThemeInTheAppResource()
        {
            _appThemeService.SwitchThemeTo(AppTheme.Light);

            Assert.Equal(AppThemeServiceTestsFixture.LIGHT_THEME_RESOURCE,
                Application.Current.Resources.MergedDictionaries[0]);
        }
    }

    public class AppThemeServiceTestsFixture : IDisposable
    {
        public Application Application { get; set; }

        public static readonly ResourceDictionary LIGHT_THEME_RESOURCE =
            new ResourceDictionary();

        public static readonly ResourceDictionary DARK_THEME_RESOURCE = new ResourceDictionary();

        public AppThemeServiceTestsFixture()
        {
            LIGHT_THEME_RESOURCE.Add("LIGHT", "LIGHT");
            DARK_THEME_RESOURCE.Add("DARK", "DARK");
            if (System.Windows.Application.Current == null)
            { Application = new System.Windows.Application { ShutdownMode = ShutdownMode.OnExplicitShutdown }; }

            else Application = System.Windows.Application.Current;

            Application.Current.Resources.MergedDictionaries.Add(LIGHT_THEME_RESOURCE);

        }

        public void Dispose()
        {
        }
    }
}
