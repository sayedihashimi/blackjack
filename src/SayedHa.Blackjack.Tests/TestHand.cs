using SayedHa.Blackjack.Shared;
using System.Collections.Generic;
using Xunit;

namespace SayedHa.Blackjack.Tests {
    public class TestHand {
        [Fact]
        public void Test_Score_TwoCards_NoAce_NotOver21() {
            var hand1 = new Hand {
                DealtCards = new List<Card> {
                        new Card{Number=CardNumber.Nine, Suit=CardSuit.Diamond },
                        new Card{Number=CardNumber.Two, Suit=CardSuit.Heart }
                    }
            };
            Assert.Equal(11, hand1.GetScore());

            var hand2 = new Hand {
                DealtCards = new List<Card> {
                        new Card{Number=CardNumber.Eight, Suit=CardSuit.Club },
                        new Card{Number=CardNumber.Eight, Suit=CardSuit.Spade }
                    }
            };
            Assert.Equal(16, hand2.GetScore());

            var hand3 = new Hand {
                DealtCards = new List<Card> {
                        new Card{Number=CardNumber.King, Suit=CardSuit.Diamond },
                        new Card{Number=CardNumber.King, Suit=CardSuit.Heart }
                    }
            };
            Assert.Equal(20, hand3.GetScore());

            var hand4 = new Hand {
                DealtCards = new List<Card> {
                        new Card{Number=CardNumber.Queen, Suit=CardSuit.Diamond },
                        new Card{Number=CardNumber.Jack, Suit=CardSuit.Heart }
                    }
            };
            Assert.Equal(20, hand4.GetScore());
        }

        [Fact]
        public void Test_Score_ThreeCards_NoAce_21OrOver() {
            var hand1 = new Hand {
                DealtCards = new List<Card> {
                    new Card{Number=CardNumber.Nine, Suit=CardSuit.Diamond },
                    new Card{Number=CardNumber.Two, Suit=CardSuit.Heart },
                    new Card{Number=CardNumber.King, Suit=CardSuit.Heart }
                }
            };
            Assert.Equal(21, hand1.GetScore());

            var hand2 = new Hand {
                DealtCards = new List<Card> {
                    new Card{Number=CardNumber.Eight, Suit=CardSuit.Club },
                    new Card{Number=CardNumber.Eight, Suit=CardSuit.Spade },
                    new Card{Number=CardNumber.Jack, Suit=CardSuit.Heart }
                }
            };
            Assert.Equal(26, hand2.GetScore());

            var hand3 = new Hand {
                DealtCards = new List<Card> {
                    new Card{Number=CardNumber.Six, Suit=CardSuit.Diamond },
                    new Card{Number=CardNumber.King, Suit=CardSuit.Heart },
                    new Card{Number=CardNumber.Five, Suit=CardSuit.Club }
                }
            };
            Assert.Equal(21, hand3.GetScore());

            var hand4 = new Hand {
                DealtCards = new List<Card> {
                    new Card{Number=CardNumber.Queen, Suit=CardSuit.Diamond },
                    new Card{Number=CardNumber.Jack, Suit=CardSuit.Heart },
                    new Card{Number=CardNumber.Ten, Suit=CardSuit.Club }
                }
            };
            Assert.Equal(30, hand4.GetScore());
        }
        [Fact]
        public void Test_Score_WithAce_NotOver21() {
            var hand1 = new Hand {
                DealtCards = new List<Card> {
                        new Card{Number=CardNumber.Ace, Suit=CardSuit.Diamond },
                        new Card{Number=CardNumber.King, Suit=CardSuit.Heart }
                    }
            };
            Assert.Equal(21, hand1.GetScore());

            var hand2 = new Hand {
                DealtCards = new List<Card> {
                        new Card{Number=CardNumber.Two, Suit=CardSuit.Club },
                        new Card{Number=CardNumber.Three, Suit=CardSuit.Spade },
                        new Card{Number=CardNumber.Ace, Suit=CardSuit.Heart }
                    }
            };
            Assert.Equal(16, hand2.GetScore());

            var hand3 = new Hand {
                DealtCards = new List<Card> {
                        new Card{Number=CardNumber.Eight, Suit=CardSuit.Diamond },
                        new Card{Number=CardNumber.Ace, Suit=CardSuit.Heart },
                        new Card{Number=CardNumber.Two, Suit=CardSuit.Club }
                    }
            };
            Assert.Equal(21, hand3.GetScore());

            var hand4 = new Hand {
                DealtCards = new List<Card> {
                        new Card{Number=CardNumber.Queen, Suit=CardSuit.Diamond },
                        new Card{Number=CardNumber.Ace, Suit=CardSuit.Heart },
                        new Card{Number=CardNumber.Ten, Suit=CardSuit.Club }
                    }
            };
            Assert.Equal(21, hand4.GetScore());

            var hand5 = new Hand {
                DealtCards = new List<Card> {
                        new Card{Number=CardNumber.Queen, Suit=CardSuit.Diamond },
                        new Card{Number=CardNumber.Ace, Suit=CardSuit.Heart },
                        new Card{Number=CardNumber.Three, Suit=CardSuit.Club },
                    }
            };
            Assert.Equal(14, hand5.GetScore());

            var hand6 = new Hand {
                DealtCards = new List<Card> {
                    new Card{Number=CardNumber.Ace, Suit = CardSuit.Diamond },
                    new Card{Number=CardNumber.Ace, Suit = CardSuit.Heart },
                    new Card{Number=CardNumber.Ace, Suit = CardSuit.Club },
                    new Card{Number=CardNumber.Ace, Suit = CardSuit.Spade }
                }
            };
            Assert.Equal(14, hand6.GetScore());

            var hand7 = new Hand {
                DealtCards = new List<Card> {
                    new Card{Number=CardNumber.Three, Suit = CardSuit.Diamond },
                    new Card{Number=CardNumber.Ace, Suit = CardSuit.Spade },
                    new Card{Number=CardNumber.Two, Suit = CardSuit.Diamond },
                    new Card{Number=CardNumber.Ace, Suit = CardSuit.Club },
                    new Card{Number=CardNumber.Two, Suit = CardSuit.Heart },

                }
            };
            Assert.Equal(19, hand7.GetScore());

            var hand8 = new Hand {
                DealtCards = new List<Card> {
                    new Card{Number=CardNumber.Three, Suit = CardSuit.Diamond },
                    new Card{Number=CardNumber.Ace, Suit = CardSuit.Spade },
                    new Card{Number=CardNumber.Two, Suit = CardSuit.Diamond },
                    new Card{Number=CardNumber.Ace, Suit = CardSuit.Club },
                    new Card{Number=CardNumber.Two, Suit = CardSuit.Heart },
                    new Card{Number=CardNumber.Two, Suit = CardSuit.Spade },
                }
            };
            Assert.Equal(21, hand8.GetScore());
        }

        [Fact]
        public void Test_Score_WithAce_Over21() {
            var hand1 = new Hand {
                DealtCards = new List<Card> {
                    new Card{Number=CardNumber.Ace, Suit=CardSuit.Diamond },
                    new Card{Number=CardNumber.Ace, Suit=CardSuit.Diamond },
                    new Card{Number=CardNumber.King, Suit=CardSuit.Heart }
                }
            };
            Assert.Equal(22, hand1.GetScore());

            var hand2 = new Hand {
                DealtCards = new List<Card> {
                    new Card{Number=CardNumber.Ten, Suit=CardSuit.Diamond },
                    new Card{Number=CardNumber.Six, Suit=CardSuit.Club },
                    new Card{Number=CardNumber.Eight, Suit=CardSuit.Spade },
                    new Card{Number=CardNumber.Ace, Suit=CardSuit.Heart }
                }
            };
            Assert.Equal(25, hand2.GetScore());

            var hand3 = new Hand {
                DealtCards = new List<Card> {
                    new Card{Number=CardNumber.Eight, Suit=CardSuit.Diamond },
                    new Card{Number=CardNumber.Ace, Suit=CardSuit.Heart },
                    new Card{Number=CardNumber.Ten, Suit=CardSuit.Club },
                    new Card{Number=CardNumber.Ace, Suit=CardSuit.Heart },
                    new Card{Number=CardNumber.Ace, Suit=CardSuit.Heart },
                    new Card{Number=CardNumber.Ace, Suit=CardSuit.Heart },
                }
            };
            Assert.Equal(22, hand3.GetScore());

            var hand4 = new Hand {
                DealtCards = new List<Card> {
                    new Card{Number=CardNumber.Queen, Suit=CardSuit.Diamond },
                    new Card{Number=CardNumber.Ace, Suit=CardSuit.Heart },
                    new Card{Number=CardNumber.Ten, Suit=CardSuit.Club },
                    new Card{Number=CardNumber.Ace, Suit=CardSuit.Heart },
                }
            };
            Assert.Equal(22, hand4.GetScore());

            var hand5 = new Hand {
                DealtCards = new List<Card> {
                    new Card{Number=CardNumber.Ace, Suit=CardSuit.Heart },
                    new Card{Number=CardNumber.Jack, Suit=CardSuit.Heart },
                    new Card{Number=CardNumber.Queen, Suit=CardSuit.Diamond },
                    new Card{Number=CardNumber.Ace, Suit=CardSuit.Heart },
                    new Card{Number=CardNumber.Three, Suit=CardSuit.Club },
                }
            };
            Assert.Equal(25, hand5.GetScore());

            var hand6 = new Hand {
                DealtCards = new List<Card> {
                    new Card{Number=CardNumber.Ace, Suit = CardSuit.Diamond },
                    new Card{Number=CardNumber.Ace, Suit = CardSuit.Heart },
                    new Card{Number=CardNumber.Ace, Suit = CardSuit.Club },
                    new Card{Number=CardNumber.Ace, Suit = CardSuit.Spade },
                    new Card{Number=CardNumber.Queen, Suit=CardSuit.Heart },
                    new Card{Number=CardNumber.Nine, Suit=CardSuit.Heart },
                }
            };
            Assert.Equal(23, hand6.GetScore());
        }
    }
}
