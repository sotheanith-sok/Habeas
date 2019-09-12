using System;
using System.Collections.Generic;
using System.Text;

namespace Search.Text {
	/// <summary>
	/// A BasicTokenProcessor creates terms from tokens by removing all non-alphanumeric characters from the token, and
	/// converting it to all lowercase.
	/// </summary>
	public class BasicTokenProcessor : ITokenProcessor {
		public string ProcessToken(string token) {
			// Regular expressions are generally slower than doing this by hand.
			char[] arr = token.ToLower().ToCharArray();
			char[] stripped = Array.FindAll(arr, c => char.IsLetterOrDigit(c));
			return new string(stripped);
		}
	}
}
