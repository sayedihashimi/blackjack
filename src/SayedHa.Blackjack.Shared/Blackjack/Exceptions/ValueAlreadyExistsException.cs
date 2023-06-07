using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SayedHa.Blackjack.Shared.Blackjack.Exceptions {

	[Serializable]
	public class ValueAlreadyExistsException : Exception {
		public ValueAlreadyExistsException() { }
		public ValueAlreadyExistsException(string message) : base(message) { }
		public ValueAlreadyExistsException(string message, Exception inner) : base(message, inner) { }
		protected ValueAlreadyExistsException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
