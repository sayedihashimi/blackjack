// This file is part of SayedHa.Blackjack.
//
// SayedHa.Blackjack is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// SayedHa.Blackjack is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License
// along with SayedHa.Blackjack.  If not, see <https://www.gnu.org/licenses/>.
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
