using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client.Payloads;
using SayedHa.Blackjack.Shared;
using SayedHa.Blackjack.Shared.Blackjack.Strategy;
using SayedHa.Blackjack.Shared.Blackjack.Strategy.Tree;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
// TODO: Get rid of this
using SB = SayedHa.Blackjack.Shared.Blackjack.Strategy.StrategyBuilder;

namespace SayedHa.Blackjack.Tests.StrategyBuilder
{
    public class StrategyBuilderTests
    {
        // don't want this running as a test, takes too long and it's not a test.
        // [Fact]
        public void Test_StrategyBuilderSetup()
        {
            var strategy = new SB();

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var result = strategy.FindBestStrategies(5);
            stopwatch.Stop();

            var sb = new StringBuilder();
            var sWriter = new StringWriter(sb);

            sWriter.WriteLine($"elapsed time: {stopwatch.Elapsed.ToString(@"mm\:ss")}");

            sWriter.WriteLine($"Num generations: {new StrategyBuilderSettings().MaxNumberOfGenerations}");
            sWriter.WriteLine("Top strategies found");
            for (int i = 0; i < result.Count; i++)
            {
                sWriter.WriteLine($" ------------- {i} -------------");
                result[i].WriteTreeStringTo(sWriter);
            }

            sWriter.Flush();
            sWriter.Close();

            Console.WriteLine(sb.ToString());
            Assert.NotNull(strategy);
        }
        [Fact]
        public void TestProduceOffspring() {
            var factory = BlackjackStrategyTreeFactory.GetInstance(true);
            var parent1 = factory.GetAllStands(false);
            var parent2 = factory.GetAllHits(true);

            var sb = new SB();
            (var child1, var child2) = sb.ProduceOffspring(parent1, parent2);

            Assert.NotNull(child1);
            Assert.NotNull(child2);

            var allCardNumbers = CardDeckFactory.GetAllCardNumbers();
            // hard totals
            foreach (var dealerCard in allCardNumbers) {
                for (int score = 3; score <= 11; score++) {
                    Assert.Equal(HandAction.Stand, child1.GetFromHardTotalTree(dealerCard, score));
                    Assert.Equal(HandAction.Hit, child2.GetFromHardTotalTree(dealerCard, score));
                }
                for(int score = 12;score <= 20; score++) {
                    Assert.Equal(HandAction.Hit, child1.GetFromHardTotalTree(dealerCard, score));
                    Assert.Equal(HandAction.Stand, child2.GetFromHardTotalTree(dealerCard, score));
                }
            }
            // soft totals
            foreach (var dealerCard in allCardNumbers) {
                for (int i = 2; i <= 5; i++) {
                    Assert.Equal(HandAction.Stand, child1.GetFromAceTree(dealerCard, i));
                    Assert.Equal(HandAction.Hit, child2.GetFromAceTree(dealerCard, i));
                }
                for (int i = 6; i <= 9; i++) {
                    Assert.Equal(HandAction.Hit, child1.GetFromAceTree(dealerCard, i));
                    Assert.Equal(HandAction.Stand, child2.GetFromAceTree(dealerCard, i));
                }
            }
            // pairs
            foreach(var dealerCard in allCardNumbers) {
                for (int i = 0; i < allCardNumbers.Length; i++) {
                    var pairCard = allCardNumbers[i];
                    if(pairCard == CardNumber.Ace ||
                        pairCard == CardNumber.Two ||
                        pairCard == CardNumber.Three ||
                        pairCard == CardNumber.Four ||
                        pairCard == CardNumber.Five) {
                        Assert.False(child1.DoesPairTreeContainSplit(dealerCard, pairCard));
                        Assert.True(child2.DoesPairTreeContainSplit(dealerCard, pairCard));
                    }
                    else {
                        Assert.True(child1.DoesPairTreeContainSplit(dealerCard, pairCard));
                        Assert.False(child2.DoesPairTreeContainSplit(dealerCard, pairCard));
                    }
                }
            }
        }
    }
}
