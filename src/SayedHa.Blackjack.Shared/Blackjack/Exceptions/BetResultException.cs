using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SayedHa.Blackjack.Shared.Blackjack.Exceptions {

	[Serializable]
	public class BetResultException : Exception {
		public BetResultException() { }
		public BetResultException(string message) : base(message) { }
		public BetResultException(string message, Exception inner) : base(message, inner) { }
		protected BetResultException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
