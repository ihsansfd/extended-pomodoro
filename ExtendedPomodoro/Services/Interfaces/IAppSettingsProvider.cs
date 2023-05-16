using ExtendedPomodoro.Entities;
using ExtendedPomodoro.Models.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedPomodoro.Services.Interfaces
{
    public interface IAppSettingsProvider
    {
        AppSettings AppSettings { get; }
        Task Initialize();
        Task<AppSettings> LoadSettings();
    }
}
