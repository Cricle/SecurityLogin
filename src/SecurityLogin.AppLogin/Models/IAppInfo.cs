﻿using System;

namespace SecurityLogin.AppLogin.Models
{
    public interface IAppInfo
    {
        string? AppKey { get; set; }

        string? AppSecret { get; set; }

        DateTime? EndTime { get; set; }

        bool Enable { get; set; }
    }
}
