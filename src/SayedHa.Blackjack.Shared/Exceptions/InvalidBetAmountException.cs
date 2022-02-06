using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SayedHa.Blackjack.Shared.Exceptions {

    [Serializable]
    public class InvalidBetAmountException : Exception {
        public InvalidBetAmountException() { }
        public InvalidBetAmountException(string message) : base(message) { }
        public InvalidBetAmountException(string message, Exception inner) : base(message, inner) { }
        protected InvalidBetAmountException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
