using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SayedHa.Blackjack.Shared.Blackjack.Exceptions {

	[Serializable]
	public class UnexpectedNodeTypeException : Exception {
		public UnexpectedNodeTypeException() { }
		public UnexpectedNodeTypeException(string message) : base(message) { }
		public UnexpectedNodeTypeException(string message, Exception inner) : base(message, inner) { }
		protected UnexpectedNodeTypeException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
