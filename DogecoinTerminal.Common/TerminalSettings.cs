using DogecoinTerminal.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace DogecoinTerminal
{
	public class TerminalSettings : ITerminalSettings
    {
        private const string SETTINGS_FILE = "terminalsettings.json";
        private Dictionary<string, string> settings;

        public TerminalSettings()
        {
            LoadSettings();
        }


        public string GetString(string settingName, string valueIfEmpty = default)
        {
            if(settings.ContainsKey(settingName))
            {
                return settings[settingName];
            }

            return valueIfEmpty;
        }



		public bool GetBool(string settingName, bool valueIfEmpty = default)
        {
            if(settings.ContainsKey(settingName))
            {
                return bool.Parse(settings[settingName]);
            }

            return valueIfEmpty;
        }


        public decimal GetDecimal(string settingName, decimal valueIfEmpty = default)
        {
            if(settings.ContainsKey(settingName))
            {
                return decimal.Parse(settings[settingName]);
            }

            return valueIfEmpty;
        }


		public int GetInt(string settingName, int valueIfEmpty = default)
		{
			if (settings.ContainsKey(settingName))
			{
				return int.Parse(settings[settingName]);
			}

			return valueIfEmpty;
		}


		public bool IsSet(string settingName)
        {
            return settings.ContainsKey(settingName);
        }

        public void Set(string settingName, object value)
        {
            settings[settingName] = value.ToString();
            SaveSettings();
        }

        private void LoadSettings()
        {
            if (File.Exists(SETTINGS_FILE))
            {
                string json = File.ReadAllText(SETTINGS_FILE);
                settings = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            }
            else
            {
                settings = new Dictionary<string, string>();
            }
        }

        private void SaveSettings()
        {
            string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(SETTINGS_FILE, json);
        }


        private void SetIfMissing(string settingName, object value)
        {
            if (!IsSet(settingName))
            {
                Set(settingName, value);
            }
        }
    }


}
