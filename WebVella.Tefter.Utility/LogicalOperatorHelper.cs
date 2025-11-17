	/// <summary>
	/// Checks if AND/OR operators are properly parenthesized
	/// </summary>
	/// <param name="input">Logical expression string</param>
	/// <returns>Detailed validation result</returns>
	public static ParenValidationResult ValidateOperatorParentheses(this string input)
	{
		var result = new ParenValidationResult();

		if (string.IsNullOrWhiteSpace(input))
			return result;
		// Count parentheses
		result.OpenParenthesesCount = CountChar(input, '(');
		result.CloseParenthesesCount = CountChar(input, ')');

		// Track nesting level to handle nested parentheses properly
		int nestingLevel = 0;
		var parenPositions = new Dictionary<int, char>(); // position -> parenthesis type

		for (int i = 0; i < input.Length; i++)
		{
			if (input[i] == '(')
				parenPositions[i] = '(';
			else if (input[i] == ')')
				parenPositions[i] = ')';
		}

		// Regex to find AND/OR as whole words (case-insensitive)
		var regex = new Regex(@"\b(AND|OR)\b",
			RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);

		foreach (Match match in regex.Matches(input))
		{
			var info = new OperatorParensInfo { Operator = match.Value.ToUpper(), Position = match.Index };

			// Find the nearest opening parenthesis before this operator
			int openParenPos = -1;
			for (int i = match.Index - 1; i >= 0; i--)
			{
				if (input[i] == '(')
				{
					openParenPos = i;
					break;
				}
			}

			// Find the nearest closing parenthesis after this operator
			int closeParenPos = -1;
			for (int i = match.Index + match.Length; i < input.Length; i++)
			{
				if (input[i] == ')')
				{
					closeParenPos = i;
					break;
				}
			}

			// Check if operator is properly parenthesized
			info.HasOpenParenBefore = openParenPos != -1;
			info.HasCloseParenAfter = closeParenPos != -1;

			// Validate that the parentheses actually surround the operator
			if (info.HasOpenParenBefore && info.HasCloseParenAfter)
			{
				// Check if there's a matching closing parenthesis for the opening one found
				int openCount = 0;
				bool valid = true;
				for (int i = openParenPos; i <= closeParenPos; i++)
				{
					if (input[i] == '(') openCount++;
					else if (input[i] == ')') openCount--;
					// If we close more than we opened, this is not a valid pairing
					if (openCount < 0)
					{
						valid = false;
						break;
					}
				}
				info.HasOpenParenBefore = valid && openCount == 0;
			}

			// Record positions if found
			info.OpenParenPosition = openParenPos;
			info.CloseParenPosition = closeParenPos;

			// Update overall result
			if (!info.IsParenthesizedCorrectly)
				result.AllOperatorsParenthesized = false;

			result.OperatorsInfo.Add(info);
		}

		return result;
	}
