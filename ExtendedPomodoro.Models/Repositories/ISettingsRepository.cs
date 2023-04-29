﻿using ExtendedPomodoro.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedPomodoro.Models.Repositories
{
    public interface ISettingsRepository
    {
        Task<SettingsDTO> GetMainSettings();
        Task<SettingsDTO> GetDefaultSettings();
        Task UpdateSettings(SettingsDTO dto);
    }
}
