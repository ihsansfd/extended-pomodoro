﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using ExtendedPomodoro.Messages;
using ExtendedPomodoro.ViewModels;
using Moq;
using Moq.AutoMock;

namespace ExtendedPomodoro.Tests.App.ViewModels
{
    public class MainWindowViewModelTests
    {
        private readonly AutoMocker _mocker = new AutoMocker();
        private readonly Mock<object> _timerViewModelMock;
        private readonly Mock<object> _tasksViewModelMock;
        private readonly Mock<object> _statsViewModelMock;
        private readonly Mock<object> _settingsViewModelMock;
        private readonly Mock<IMessenger> _messengerMock;
        private readonly MainWindowViewModel _sut;

        public MainWindowViewModelTests()
        {
            _timerViewModelMock = new Mock<object>();
            _tasksViewModelMock = new Mock<object>();
            _settingsViewModelMock = new Mock<object>();
            _statsViewModelMock = new Mock<object>();
            _messengerMock = new Mock<IMessenger>();
            _sut = new MainWindowViewModel(
                _timerViewModelMock.Object,
                _tasksViewModelMock.Object,
                _statsViewModelMock.Object,
                _settingsViewModelMock.Object,
                _messengerMock.Object
            );
        }

        [Fact]
        void NavigateToStats_Successfully()
        {
            _sut.NavigateToStatsCommand.Execute(null);
            Assert.Equal(_statsViewModelMock.Object, _sut.CurrentViewModel);
        }

        [Fact]
        void NavigateToSettings_Successfully()
        {
            _sut.NavigateToSettingsCommand.Execute(null);
            Assert.Equal(_settingsViewModelMock.Object, _sut.CurrentViewModel);
        }

        [Fact]
        void NavigateToTasks_Successfully()
        {
            _sut.NavigateToTasksCommand.Execute(null);
            Assert.Equal(_tasksViewModelMock.Object, _sut.CurrentViewModel);
        }

        [Fact]
        void NavigateToTimer_Successfully()
        {
            _sut.NavigateToTimerCommand.Execute(null);
            Assert.Equal(_timerViewModelMock.Object, _sut.CurrentViewModel);
        }

        [Fact]
        void HandleWindowClosedCommand_NotificationSendSuccessfully()
        {
            _messengerMock.Setup((x) => 
                x.Send(It.IsAny<MainWindowIsClosingMessage>(), It.IsAny<IsAnyToken>()));

            _sut.HandleWindowClosedCommand.Execute(null);

            _messengerMock.Verify((x) =>
                    x.Send(It.IsAny<MainWindowIsClosingMessage>(), It.IsAny<IsAnyToken>()), 
                Times.Once());
        }

    }
}