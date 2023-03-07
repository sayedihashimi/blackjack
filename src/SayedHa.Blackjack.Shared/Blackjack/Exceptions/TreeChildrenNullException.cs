using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SayedHa.Blackjack.Shared.Blackjack.Exceptions {

	[Serializable]
	public class TreeChildrenNullException : Exception {
		public TreeChildrenNullException() { }
		public TreeChildrenNullException(string message) : base(message) { }
		public TreeChildrenNullException(string message, Exception inner) : base(message, inner) { }
		protected TreeChildrenNullException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
