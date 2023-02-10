using SayedHa.Blackjack.Shared;
using SayedHa.Blackjack.Shared.Roulette;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SayedHa.Roulette.Cli {
    public interface IDefaultGameSettingsFile {
        string SettingsFilePath { get; set; }

        Task<GameSettings> GetOrCreateGameSettingsFileAsync();
        Task SaveGameSettingsConfigAsync(GameSettings configSettings);
    }

    public class DefaultGameSettingsFile : IDefaultGameSettingsFile {
        private readonly IReporter _reporter;
        private readonly IGameSettingsFactory _gameSettingsFactory;

        public DefaultGameSettingsFile(IReporter reporter, IGameSettingsFactory gameSettingsFactory) {
            _reporter = reporter;
            _gameSettingsFactory = gameSettingsFactory;
        }

        // should work xplat, see: https://developers.redhat.com/blog/2018/11/07/dotnet-special-folder-api-linux#environment_getfolderpath
        public string SettingsFilePath { get; set; } = Path.Combine(
            Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData,
                    Environment.SpecialFolderOption.DoNotVerify), 
                "sayedha.roulette"), 
            "roulette.settings.json");

        public async Task SaveGameSettingsConfigAsync(GameSettings configSettings) {
            Debug.Assert(configSettings != null);
            // ensure that the directory exists
            
            Directory.CreateDirectory(Path.GetDirectoryName(SettingsFilePath));
            await _gameSettingsFactory.SaveSettingsToJsonFileAsync(SettingsFilePath, configSettings);
        }

        public async Task<GameSettings> GetOrCreateGameSettingsFileAsync() {
            Debug.Assert(!string.IsNullOrEmpty(SettingsFilePath));
            // ensure that the directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(SettingsFilePath));

            var settings = new GameSettings();
            if (File.Exists(SettingsFilePath)) {
                try {
                    settings = await _gameSettingsFactory.ReadFromJsonFileAsync(SettingsFilePath);
                }
                catch (JsonException je) {
                    _reporter.WriteLine($"unable to read settings from file, loading default settings. filepath='{SettingsFilePath}'.\njson Error:{je}");
                    settings = new GameSettings();
                }
                catch (Exception ex) {
                    _reporter.WriteLine($"unable to read settings from file, loading default settings. filepath='{SettingsFilePath}'.\nError:{ex}");
                    settings = new GameSettings();
                }
                _reporter.WriteLine($"Settings file loaded from '{SettingsFilePath}'");
            }
            else {
                _reporter.WriteLine($"No settings file found at '{SettingsFilePath}' using default settings.");
            }

            return settings;
        }
    }
}
