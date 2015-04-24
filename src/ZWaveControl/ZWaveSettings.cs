﻿using System.Configuration;

namespace ZWaveControl
{
    public interface IZWaveSettings
    {
        string ConfigurationPath { get; }
        string ControllerComPort { get; }
    }

    public class ZWaveSettings : IZWaveSettings
    {
        public string ConfigurationPath
        {
            get { return GetRequiredSetting("ZWaveConfigurationPath"); }
        }

        public string ControllerComPort
        {
            get { return @"\\.\" + GetRequiredSetting("ZWaveControllerComPort").ToUpperInvariant(); }
        }

        private string GetRequiredSetting(string key)
        {
            var value = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrEmpty(value))
                throw new ConfigurationErrorsException(string.Format("Configuration [{0}] is required.", key));
            return value;
        }
    }
}