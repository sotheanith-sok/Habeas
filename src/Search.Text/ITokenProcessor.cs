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
		/// <param name="token">Preprocess token</param>
		/// <returns>A list of terms</returns>
		List<string> ProcessToken(string token);
	}
}
