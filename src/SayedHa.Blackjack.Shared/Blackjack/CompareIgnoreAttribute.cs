using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SayedHa.Blackjack.Shared.Blackjack {
	// to be used with CompareIgnoreAttribute 
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class CompareIgnoreAttribute : Attribute {
	}
}
