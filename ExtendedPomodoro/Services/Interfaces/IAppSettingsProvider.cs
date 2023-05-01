using ExtendedPomodoro.Models.Domains;
using ExtendedPomodoro.Services.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedPomodoro.Services.Interfaces
{
    public interface IAppSettingsProvider
    {
        AppSettings AppSettings { get; set; }
        Task Initialize();
        Task<AppSettings> LoadSettings();
    }
}
