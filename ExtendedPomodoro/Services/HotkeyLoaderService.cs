using CommunityToolkit.Mvvm.Messaging;
using ExtendedPomodoro.Entities;
using ExtendedPomodoro.Messages;
using ExtendedPomodoro.ViewModels;
using NHotkey;
using NHotkey.Wpf;
using System;

namespace ExtendedPomodoro.Services
{
    public class HotkeyLoaderService : IRecipient<SettingsUpdateInfoMessage>
    {
        private readonly IMessenger _messenger;
        private readonly HotkeyManager _hotkeyManager;
        private readonly TimerViewModel _timerViewModel;

        public HotkeyLoaderService(
            HotkeyManager hotkeyManager, 
            TimerViewModel timerViewModel,
            IMessenger messenger
            )
        {
            _hotkeyManager = hotkeyManager;
            _timerViewModel = timerViewModel;
            _messenger = messenger;

            _messenger.RegisterAll(this);
        }

        public void RegisterOrUpdateStartTimerHotkey(Hotkey? hotkey)
        {
            RegisterOrUpdate("StartTimerHotkey", hotkey, _timerViewModel.StartSessionFromHotkey);
        }

        public void RegisterOrUpdatePauseTimerHotkey(Hotkey? hotkey)
        {
            RegisterOrUpdate("PauseTimerHotkey", hotkey, _timerViewModel.PauseSessionFromHotkey);
        }

        private void RegisterOrUpdate(string identifier, 
            Hotkey? hotkey, EventHandler<HotkeyEventArgs> handler)
        {
            if(hotkey == null)
            {
                _hotkeyManager.Remove(identifier);
                return;
            }

            _hotkeyManager.AddOrReplace(identifier,
                hotkey.Key, hotkey.Modifiers, handler);
        }

        public void Receive(SettingsUpdateInfoMessage message)
        {
            RegisterOrUpdateStartTimerHotkey(message.AppSettings.StartHotkey);
            RegisterOrUpdatePauseTimerHotkey(message.AppSettings.PauseHotkey);
        }

        ~HotkeyLoaderService()
        {
            _messenger.UnregisterAll(this);
        }
    }
}
