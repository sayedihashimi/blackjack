using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SayedHa.Blackjack.Shared.Blackjack.Exceptions {

	[Serializable]
	public class UnexpectedValueException : Exception {
		public UnexpectedValueException() { }
		public UnexpectedValueException(string message) : base(message) { }
		public UnexpectedValueException(string message, Exception inner) : base(message, inner) { }
		protected UnexpectedValueException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
