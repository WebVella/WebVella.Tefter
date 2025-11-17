namespace WebVella.Tefter.Utility;

public static class SearchParser
{
	/// <summary>
    /// Generates a PostgreSQL WHERE clause fragment from a user-supplied search string,
    /// translating search terms into case-insensitive ILIKE clauses.
    /// Supports logical operators AND/OR and parentheses for grouping.
    /// </summary>
    /// <param name="userInput">The user's search query (e.g., "(apple AND pears) OR plumb").</param>
    /// <param name="fieldName">The PostgreSQL field name to search against (e.g., "tf_search").</param>
    /// <returns>A valid PostgreSQL WHERE clause fragment.</returns>
    public static string GeneratePostgresSearchScript(this string userInput, string fieldName, List<NpgsqlParameter> parameters)
    {
        if (string.IsNullOrWhiteSpace(userInput))
        {
            return string.Empty;
        }

        // 1. Normalize and Standardize Input
        string normalizedInput = userInput.Trim();

        // Standardize operators and parentheses with spaces
        normalizedInput = normalizedInput.Replace("(", " ( ")
                                         .Replace(")", " ) ")
                                         .Replace("AND", " AND ")
                                         .Replace("OR", " OR ")
                                         .Replace("and", " AND ")
                                         .Replace("or", " OR ");
        
        // Collapse multiple spaces into a single space and make all operators uppercase
        normalizedInput = Regex.Replace(normalizedInput, @"\s+", " ").Trim().ToUpperInvariant();

        // 2. Define the Term Matching Pattern
        // This pattern looks for sequences of characters that are NOT:
        // - ' ' (space)
        // - '(' or ')' (parentheses)
        // - 'A', 'N', 'D', 'O', 'R' (used to avoid matching AND/OR if they are separate words)
        // It ensures we capture only the search *words* by looking for word boundaries,
        // but given the standardization, a simpler negative lookahead/lookbehind is better.

        // Simpler Pattern: Match any sequence of non-space characters that is NOT one of the operators/parentheses.
        // We ensure a term is not 'AND' or 'OR' explicitly, and is not a parenthesis.
        string searchTermsPattern = @"(?!\b(AND|OR)\b|\(|\))(\b[^ \(\)]+\b)";


        // 3. Replace Terms with ILIKE Clause
        string sqlResult = Regex.Replace(
            normalizedInput,
            searchTermsPattern,
            match =>
            {
                // The search term found by the regex
                string term = match.Groups[2].Value.Trim();

                // It's crucial to check if the captured 'term' is actually one of the reserved words 
                // that might have slipped through the regex boundaries (like 'OR' when it's part of a word).
                if (term.Equals("AND") || term.Equals("OR") || term.Equals("(") || term.Equals(")"))
                {
                    return term; // Return as is if it's an operator/parenthesis
                }

                // Escape single quotes for SQL safety
                string escapedTerm = term.Replace("'", "''").Trim();
                var paramName = $"@search_param_{parameters.Count + 1}";
                parameters.Add(new NpgsqlParameter(paramName, escapedTerm.ToLowerInvariant()));
                // Return the complete ILIKE clause
                return $"{fieldName} ILIKE CONCAT ('%', {paramName}, '%')";
            },
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant // Important for case-insensitive matching of operators/terms
        );

        // 4. Cleanup and Final Formatting
        // Remove spaces next to parentheses and collapse extra spaces
        sqlResult = Regex.Replace(sqlResult, @"\s+", " ").Trim();
        sqlResult = sqlResult.Replace("( ", "(").Replace(" )", ")");

        return sqlResult;
    }

	/// <summary>
	/// Joins two user queries from the search field so they can be processed from the SQL generator 
	/// </summary>
	/// <param name="query1"></param>
	/// <param name="query2"></param>
	/// <returns></returns>
	public static string? JoinUserSearchQueries(this string? query1, string? query2)
	{
		query1 = query1?.Trim();
		query2 = query2?.Trim();
		if (String.IsNullOrWhiteSpace(query1) && String.IsNullOrWhiteSpace(query2)) return null;
		if (String.IsNullOrWhiteSpace(query1)) return query2;
		if (String.IsNullOrWhiteSpace(query2)) return query1;
		if (!query1.ValidateOperatorParentheses())
			query1 = $"({query1})";
		
		if (!query2.ValidateOperatorParentheses())
			query2 = $"({query2})";		
		
		return $"{query1} AND {query2}";
	}
	
	 // Regex that matches the two operators (case‑insensitive) as whole words
        private static readonly Regex OperatorRegex = new(
            @"\b(AND|OR)\b",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Returns true if all logical operators are surrounded by parentheses.
        /// </summary>
        public static bool ValidateOperatorParentheses(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            // First, make sure the parentheses are balanced
            if (!AreParenthesesBalanced(input))
                return false;

            // Scan for operators and check that each one is inside a pair of brackets
            int depth = 0;
            foreach (var ch in input)
            {
                switch (ch)
                {
                    case '(':
                        depth++;
                        break;
                    case ')':
                        depth--;
                        break;
                }
            }

            // If we reached here the parentheses are balanced.
            // Now verify each operator is inside at least one pair of brackets
            var matches = OperatorRegex.Matches(input);
            foreach (Match m in matches)
            {
                int pos = m.Index;

                // Count how many '(' appear before this position
                int openBefore = 0;
                for (int i = 0; i < pos; i++)
                    if (input[i] == '(') openBefore++;

                // Count how many ')' appear before this position
                int closeBefore = 0;
                for (int i = 0; i < pos; i++)
                    if (input[i] == ')') closeBefore++;

                // If more opens than closes, the operator is inside parentheses
                if (openBefore <= closeBefore)
                    return false;   // operator not wrapped
            }

            // All operators are wrapped – expression considered true
            return true;
        }

        private static bool AreParenthesesBalanced(string input)
        {
            int balance = 0;
            foreach (var ch in input)
            {
                if (ch == '(') balance++;
                else if (ch == ')')
                {
                    balance--;
                    if (balance < 0) return false; // more ')' than '('
                }
            }
            return balance == 0;
        }
} 