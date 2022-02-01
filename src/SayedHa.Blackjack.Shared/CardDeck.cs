using System.Text;

namespace SayedHa.Blackjack.Shared {
    public class CardDeck {
        private ILogger _logger;
        internal LinkedList<Card>? Cards { get; set; }
        internal LinkedListNode<Card>? CurrentCard { get; set; }
        public List<Card> DiscardedCards { get; internal set; } = new List<Card>();

        public CardDeck(ILogger logger,LinkedList<Card>? cards) {
            _logger = logger;
            Cards = cards;
            CurrentCard = Cards != null ? Cards.First : null;
        }

        public Card? GetCardAndMoveNext() {
            var retValue = CurrentCard?.Value;
            if(retValue != null) {
                DiscardedCards.Add(retValue);
            }
            CurrentCard = CurrentCard?.Next;
            return retValue;
        }

        public Card? GetCurrentCard() {
            return CurrentCard?.Value;
        }

        public bool Contains(Card card) {
            if (card == null) {
                throw new ArgumentNullException(nameof(card));
            }

            return Cards != null ? Cards.Contains(card) : throw new ApplicationException("CardDeck hasn't been initalized, Cards is null");
        }

        override public string ToString() {
            if(Cards == null) {
                return "(empty)";
            }
            var sb = new StringBuilder();
            foreach(var card in Cards) {
                sb.AppendLine(card.ToString());
            }
            return sb.ToString();
        }
    }
}
