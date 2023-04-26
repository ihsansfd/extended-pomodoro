﻿using CommunityToolkit.Mvvm.Messaging;
using ExtendedPomodoro.Entities;
using ExtendedPomodoro.Services;
using ExtendedPomodoro.ViewModels;
using ExtendedPomodoro.Views.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedPomodoro.ViewServices
{
    public class StatsViewService
    {
        private readonly DialogWindowService _dialogWindowService;
        private readonly IMessenger _messenger;

        public StatsViewService(
            DialogWindowService dialogWindowService,
            IMessenger messenger
            )
        {
            _dialogWindowService = dialogWindowService;
            _messenger = messenger;

        }

        public void OpenScatterPlotStats(StatAxesDomainViewModel axes)
        {
            var userControl = new ScatterPlotStatsUserControl
            {
                Axes = axes
            };
            userControl.Initialize();

            var dialog = _dialogWindowService.GenerateMaximizibleDialogWindow(
                new(800, 450, "Fullscreen Stats", userControl));
            dialog.Show();
            dialog.Focus();
            dialog.Activate();
        }

    }
}