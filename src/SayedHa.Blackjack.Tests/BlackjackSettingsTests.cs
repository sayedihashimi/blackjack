using SayedHa.Blackjack.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SayedHa.Blackjack.Shared.Extensions;
using Xunit;

namespace SayedHa.Blackjack.Tests {
    public class BlackjackSettingsTests {
        [Fact]
        public async Task Test_LoadFromJsonfile_01Async() {
            var text = await GetTestFile01ContentsAsync();
            var result = BlackjackSettings.LoadFromJson(text);

            Assert.NotNull(result);
            Assert.Equal(1, result.BetAmount);
            Assert.Equal(0, result.BankrollAmount);
            Assert.Equal(6, result.NumberOfDecks);
            Assert.Equal(20, result.ShuffleThresholdPercent);
            Assert.True(result.DoubleDownEnabled);
            Assert.True(result.SplitEnabled);
            Assert.Equal(3, result.StrategiesToPlay.Count);
            Assert.NotNull(result.CreateBettingStrategy);
        }

        private string _testFile01;
        private async Task<string> GetTestFile01ContentsAsync() {
            if (!string.IsNullOrEmpty(_testFile01)) {
                return _testFile01;
            }

            var ass = Assembly.GetExecutingAssembly();
            var info = ass.GetManifestResourceNames();

            using var stream = Assembly.GetExecutingAssembly().GetEmbeddedResourceStream(@"assets.blackjack.settings.test01.json");
            TextReader reader = new StreamReader(stream);
            string text = await reader.ReadToEndAsync();
            return text;
        }
        //public static Stream GetEmbeddedResourceStream(this Assembly assembly, string relativeResourcePath) {
        //    if (string.IsNullOrEmpty(relativeResourcePath))
        //        throw new ArgumentNullException("relativeResourcePath");

        //    var resourcePath = String.Format("{0}.{1}",
        //        Regex.Replace(assembly.ManifestModule.Name, @"\.(exe|dll)$",
        //              string.Empty, RegexOptions.IgnoreCase), relativeResourcePath);

        //    var stream = assembly.GetManifestResourceStream(resourcePath);
        //    if (stream == null)
        //        throw new ArgumentException(String.Format("The specified embedded resource \"{0}\" is not found.", relativeResourcePath));
        //    return stream;
        //}
    }
}
