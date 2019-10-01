using System;
using System.Collections.Generic;
using System.Text;

namespace Search.Text {
	/// <summary>
	/// A TokenProcessor applies some rules of normalization to a token from a document and returns a term for that 
	/// token.
	/// </summary>
	public interface ITokenProcessor {
		/// <summary>
		/// Normalize a token into a term.
		/// </summary>
		/// <param name="token">a token to process</param>
		/// <returns></returns>
		List<string> ProcessToken(string token);
		//TODO: Change the return type to string
		//NOTE: return type as a list of string is only for the case of a hyphenated token.. for storing them in index.
	}
}
