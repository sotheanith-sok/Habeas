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
		/// <param name="enableKGram">Should token be k-gram</param>
		/// <param name="enableSteam">Should token be steam</param>
		/// <returns></returns>
		List<string> ProcessToken(string token, bool enableKGram = false, bool enableSteam = false);
	}
}
