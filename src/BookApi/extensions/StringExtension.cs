using System.IO;

namespace BookApi.extensions
{
    public static class StringExtension
    {
        public static string GetFirstCharacterLowerCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            if (input.Length == 1)
            {
                return input.ToLower();
            }

            return char.ToLower(input[0]) + input.Substring(1);
        }

        public static string GetFileExtension(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            if (Path.HasExtension(input))
            {
                return Path.GetExtension(input).ToLower();
            }

            return string.Empty;
        }
    }
}