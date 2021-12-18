namespace BookApi.extensions
{
    public static class StringExtension
    {
        public static string ToLowerFirstCharacter(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            else if (input.Length == 1)
            {
                return input.ToLower();
            }

            return char.ToLower(input[0]) + input.Substring(1);
        }
    }
}