using CommunityToolkit.Mvvm.Messaging;
using ExtendedPomodoro.Entities;
using ExtendedPomodoro.Models.Domains;
using ExtendedPomodoro.ViewModels;
using NHotkey;
using NHotkey.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

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

        public void RegisterOrUpdateStartTimerHotkey(Hotkey? hotkeyDomain)
        {
            RegisterOrUpdate("StartTimerHotkey", hotkeyDomain, _timerViewModel.StartSessionFromHotkey);
        }

        public void RegisterOrUpdatePauseTimerHotkey(Hotkey? hotkeyDomain)
        {
            RegisterOrUpdate("PauseTimerHotkey", hotkeyDomain, _timerViewModel.PauseSessionFromHotkey);
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
            RegisterOrUpdateStartTimerHotkey(message.SettingsViewModel.StartHotkey);
            RegisterOrUpdatePauseTimerHotkey(message.SettingsViewModel.PauseHotkey);
        }

        ~HotkeyLoaderService()
        {
            _messenger.UnregisterAll(this);
        }
    }
}
