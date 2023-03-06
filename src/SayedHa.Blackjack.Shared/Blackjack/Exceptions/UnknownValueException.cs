using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SayedHa.Blackjack.Shared.Blackjack.Exceptions {

	[Serializable]
	public class UnknownValueException : Exception {
		public UnknownValueException() { }
		public UnknownValueException(string message) : base(message) { }
		public UnknownValueException(string message, Exception inner) : base(message, inner) { }
		protected UnknownValueException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
