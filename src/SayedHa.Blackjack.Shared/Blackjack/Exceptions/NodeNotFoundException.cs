using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SayedHa.Blackjack.Shared.Blackjack.Exceptions {

	[Serializable]
	public class NodeNotFoundException : Exception {
		public NodeNotFoundException() { }
		public NodeNotFoundException(string message) : base(message) { }
		public NodeNotFoundException(string message, Exception inner) : base(message, inner) { }
		protected NodeNotFoundException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
