using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MohitKhare
{
    /// <summary>
    /// Core metadata for MohitKhare developer utilities.
    /// Homepage: https://mohitkhare.me
    /// </summary>
    public static class Info
    {
        public const string Version = "0.1.0";
        public const string BaseUrl = "https://mohitkhare.me";
        public const string Author = "Mohit Khare";
        public const string Description = "Developer utilities for .NET: token counting, slug generation, text analysis, and common helpers.";
    }

    /// <summary>
    /// Estimates token counts for text strings.
    /// Useful for working with LLM APIs that charge per token.
    /// Uses the standard ~4 characters per token heuristic.
    /// </summary>
    public static class TokenCounter
    {
        private const double CharsPerToken = 4.0;

        /// <summary>
        /// Estimates the token count for a given text string.
        /// Uses a ~4 characters per token approximation.
        /// </summary>
        public static int Estimate(string text)
        {
            if (string.IsNullOrEmpty(text)) return 0;
            return (int)Math.Ceiling(text.Length / CharsPerToken);
        }

        /// <summary>
        /// Estimates cost in USD for a given text at a per-token rate.
        /// </summary>
        /// <param name="text">Input text</param>
        /// <param name="pricePerMillionTokens">Cost per 1M tokens in USD</param>
        public static decimal EstimateCost(string text, decimal pricePerMillionTokens)
        {
            var tokens = Estimate(text);
            return Math.Round(tokens * pricePerMillionTokens / 1_000_000m, 6);
        }

        /// <summary>
        /// Checks whether a text fits within a token budget.
        /// </summary>
        public static bool FitsInBudget(string text, int maxTokens)
        {
            return Estimate(text) <= maxTokens;
        }
    }

    /// <summary>
    /// Text analysis and statistics utilities.
    /// </summary>
    public static class TextAnalyzer
    {
        /// <summary>
        /// Counts words in a text string.
        /// </summary>
        public static int WordCount(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return 0;
            return text.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
        }

        /// <summary>
        /// Estimates reading time in minutes (average 238 words per minute).
        /// </summary>
        public static double ReadingTimeMinutes(string text)
        {
            var words = WordCount(text);
            return Math.Round(words / 238.0, 1);
        }

        /// <summary>
        /// Calculates sentence count by splitting on sentence-ending punctuation.
        /// </summary>
        public static int SentenceCount(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return 0;
            return Regex.Split(text.Trim(), @"[.!?]+\s*")
                .Count(s => !string.IsNullOrWhiteSpace(s));
        }

        /// <summary>
        /// Computes the Flesch-Kincaid reading ease score (0-100).
        /// Higher scores indicate easier readability.
        /// </summary>
        public static double FleschReadingEase(string text)
        {
            var words = WordCount(text);
            var sentences = SentenceCount(text);
            var syllables = EstimateSyllables(text);

            if (words == 0 || sentences == 0) return 0;

            return Math.Round(
                206.835
                - 1.015 * ((double)words / sentences)
                - 84.6 * ((double)syllables / words),
                1
            );
        }

        private static int EstimateSyllables(string text)
        {
            var words = text.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            int total = 0;
            foreach (var word in words)
            {
                var clean = Regex.Replace(word.ToLowerInvariant(), @"[^a-z]", "");
                if (clean.Length <= 3) { total += 1; continue; }
                var vowels = Regex.Matches(clean, @"[aeiouy]+").Count;
                if (clean.EndsWith("e")) vowels = Math.Max(1, vowels - 1);
                total += Math.Max(1, vowels);
            }
            return total;
        }
    }

    /// <summary>
    /// URL-safe slug generation with multiple format options.
    /// </summary>
    public static class SlugGenerator
    {
        /// <summary>
        /// Converts a string to a URL-safe slug.
        /// </summary>
        public static string Slugify(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;
            var slug = input.ToLowerInvariant().Trim();
            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
            slug = Regex.Replace(slug, @"\s+", "-");
            slug = Regex.Replace(slug, @"-+", "-");
            return slug.Trim('-');
        }

        /// <summary>
        /// Converts a string to a file-safe name (underscores instead of dashes).
        /// </summary>
        public static string ToFileName(string input)
        {
            return Slugify(input).Replace('-', '_');
        }

        /// <summary>
        /// Converts a string to camelCase.
        /// </summary>
        public static string ToCamelCase(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;
            var words = input.Split(new[] { ' ', '-', '_' }, StringSplitOptions.RemoveEmptyEntries);
            if (words.Length == 0) return string.Empty;
            var sb = new StringBuilder(words[0].ToLowerInvariant());
            for (int i = 1; i < words.Length; i++)
            {
                if (words[i].Length > 0)
                {
                    sb.Append(char.ToUpperInvariant(words[i][0]));
                    if (words[i].Length > 1)
                        sb.Append(words[i].Substring(1).ToLowerInvariant());
                }
            }
            return sb.ToString();
        }
    }

    /// <summary>
    /// Common string extension methods for .NET developers.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Truncates a string to a maximum length, appending an ellipsis if truncated.
        /// </summary>
        public static string Truncate(this string value, int maxLength, string suffix = "...")
        {
            if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
                return value;
            return value.Substring(0, maxLength - suffix.Length) + suffix;
        }

        /// <summary>
        /// Strips all HTML tags from a string, returning plain text.
        /// </summary>
        public static string StripHtml(this string html)
        {
            if (string.IsNullOrEmpty(html)) return string.Empty;
            return Regex.Replace(html, @"<[^>]+>", "").Trim();
        }

        /// <summary>
        /// Extracts the first N sentences from a text.
        /// </summary>
        public static string FirstSentences(this string text, int count)
        {
            if (string.IsNullOrWhiteSpace(text) || count <= 0) return string.Empty;
            var matches = Regex.Matches(text, @"[^.!?]*[.!?]");
            var sentences = matches.Take(count).Select(m => m.Value.Trim());
            return string.Join(" ", sentences);
        }
    }
}
