using CommunityToolkit.Mvvm.Messaging;
using ExtendedPomodoro.Entities;
using ExtendedPomodoro.Messages;
using ExtendedPomodoro.ViewModels;
using NHotkey;
using NHotkey.Wpf;
using System;
using System.Windows.Input;
using ExtendedPomodoro.Services.Interfaces;

namespace ExtendedPomodoro.Services
{

    public class HotkeyManagerAdapter : IHotkeyManager
    {
        private readonly HotkeyManager _hotkeyManager = HotkeyManager.Current;

        public void AddOrReplace(string name, Key key, ModifierKeys modifiers, 
            EventHandler<HotkeyEventArgs> handler) =>
            _hotkeyManager.AddOrReplace(name, key, modifiers, handler);

        public void Remove(string name) =>
            _hotkeyManager.Remove(name);
    }

    public class HotkeyLoaderService : IRecipient<SettingsUpdateInfoMessage>
    {
        private readonly IMessenger _messenger;
        private readonly IHotkeyManager _hotkeyManager;

        public HotkeyLoaderService(
            IHotkeyManager hotkeyManager, 
            IMessenger messenger
            )
        {
            _hotkeyManager = hotkeyManager;
            _messenger = messenger;

            _messenger.RegisterAll(this);
        }

        public void RegisterOrUpdateStartTimerHotkey(Hotkey? hotkey)
        {
            RegisterOrUpdate("StartTimerHotkey", hotkey, 
                (_, _) => _messenger.Send(new StartHotkeyTriggeredMessage()));
        }

        public void RegisterOrUpdatePauseTimerHotkey(Hotkey? hotkey)
        {
            RegisterOrUpdate("PauseTimerHotkey", hotkey,
                (_, _) => _messenger.Send(new PauseHotkeyTriggeredMessage()));
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
