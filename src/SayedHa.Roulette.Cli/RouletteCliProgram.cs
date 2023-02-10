using Microsoft.Extensions.DependencyInjection;
using SayedHa.Blackjack.Shared.Roulette;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;

namespace SayedHa.Roulette.Cli {
    public class RouletteCliProgram {
        private Parser _parser;
        private ServiceCollection _services;
        private ServiceProvider _serviceProvider;

        public RouletteCliProgram() {
            RegisterServices();
        }
        public async Task<int> Execute(string[] args) {
            _parser = new CommandLineBuilder()
                        .AddCommand(
                            new SimulateCommand(
                                GetFromServices<IReporter>(), 
                                GetFromServices<IDefaultGameSettingsFile>()).CreateCommand())
                        .AddCommand(
                            new ConfigCommand(
                                GetFromServices<IReporter>(),
                                GetFromServices<IDefaultGameSettingsFile>()).CreateCommand())
                        .UseDefaults()
                        .Build();

            return await _parser.InvokeAsync(args);
        }
        private void RegisterServices() {
            _services = new ServiceCollection();
            _serviceProvider = _services
                                .AddSingleton<IReporter, Reporter>()
                                .AddSingleton<IDefaultGameSettingsFile,DefaultGameSettingsFile>()
                                .AddScoped<IGameSettingsFactory, GameSettingsFactory>()
                                .BuildServiceProvider();
        }
        private TType GetFromServices<TType>() {
            return _serviceProvider.GetRequiredService<TType>();
        }
    }
}
