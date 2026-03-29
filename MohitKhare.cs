using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace MohitKhare;

// -- Token Estimation --------------------------------------------------------

/// <summary>
/// Result of a token estimation including the raw count and cost projection.
/// </summary>
public sealed record TokenEstimate(
    int Tokens,
    int Words,
    int Characters)
{
    /// <summary>
    /// Estimates the API cost at a given price per million tokens.
    /// Default rate ($3.00/M) approximates GPT-4-class input pricing.
    /// </summary>
    public decimal EstimateCost(decimal pricePerMillionTokens = 3.00m) =>
        Tokens * pricePerMillionTokens / 1_000_000m;
}

/// <summary>
/// Estimates token counts for text using a word-boundary heuristic.
/// The ratio of ~1.33 tokens per word is calibrated against BPE tokenizers
/// used by GPT-class and Claude-class models.
/// </summary>
public static class TokenEstimator
{
    private const double TokensPerWord = 1.33;

    private static readonly Regex WordBoundary =
        new(@"\S+", RegexOptions.Compiled);

    /// <summary>
    /// Estimates the number of tokens in the given text.
    /// </summary>
    public static TokenEstimate Estimate(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return new TokenEstimate(0, 0, 0);

        var words = WordBoundary.Matches(text).Count;
        var tokens = (int)Math.Ceiling(words * TokensPerWord);
        return new TokenEstimate(tokens, words, text.Length);
    }
}

// -- Text Analysis -----------------------------------------------------------

/// <summary>
/// Result of a full text analysis.
/// </summary>
public sealed record TextAnalysis(
    int WordCount,
    int SentenceCount,
    int ParagraphCount,
    TimeSpan ReadingTime,
    double FleschReadingEase,
    string ReadingLevel);

/// <summary>
/// Analyzes text for word count, reading time, and readability.
/// Reading time assumes 238 words per minute (average adult reading speed).
/// Readability is computed using the Flesch Reading Ease formula.
/// </summary>
public static class TextAnalyzer
{
    private const double WordsPerMinute = 238.0;

    private static readonly Regex SentenceEnd =
        new(@"[.!?]+", RegexOptions.Compiled);

    private static readonly Regex WordPattern =
        new(@"\b[a-zA-Z]+\b", RegexOptions.Compiled);

    private static readonly Regex ParagraphSplit =
        new(@"\n\s*\n", RegexOptions.Compiled);

    /// <summary>
    /// Performs a full analysis of the given text, returning word count,
    /// sentence count, paragraph count, estimated reading time, and a
    /// Flesch Reading Ease score with a human-readable level label.
    /// </summary>
    public static TextAnalysis Analyze(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return new TextAnalysis(0, 0, 0, TimeSpan.Zero, 0, "N/A");

        var words = WordPattern.Matches(text);
        int wordCount = words.Count;
        int sentenceCount = Math.Max(1, SentenceEnd.Matches(text).Count);
        int paragraphCount = Math.Max(1, ParagraphSplit.Split(text).Length);

        var readingTime = TimeSpan.FromMinutes(wordCount / WordsPerMinute);

        // Flesch Reading Ease
        int totalSyllables = words.Sum(w => CountSyllables(w.Value));
        double fre = 206.835
            - 1.015 * ((double)wordCount / sentenceCount)
            - 84.6 * ((double)totalSyllables / wordCount);
        fre = Math.Clamp(fre, 0, 100);

        string level = fre switch
        {
            >= 90 => "Very Easy",
            >= 80 => "Easy",
            >= 70 => "Fairly Easy",
            >= 60 => "Standard",
            >= 50 => "Fairly Difficult",
            >= 30 => "Difficult",
            _     => "Very Difficult"
        };

        return new TextAnalysis(wordCount, sentenceCount, paragraphCount,
            readingTime, Math.Round(fre, 1), level);
    }

    /// <summary>
    /// Returns the estimated reading time for a given word count.
    /// </summary>
    public static TimeSpan ReadingTime(int wordCount) =>
        TimeSpan.FromMinutes(wordCount / WordsPerMinute);

    private static int CountSyllables(string word)
    {
        word = word.ToLowerInvariant();
        if (word.Length <= 2) return 1;

        int count = 0;
        bool prevVowel = false;
        foreach (var c in word)
        {
            bool isVowel = "aeiouy".Contains(c);
            if (isVowel && !prevVowel)
                count++;
            prevVowel = isVowel;
        }

        // Silent 'e' adjustment
        if (word.EndsWith('e') && count > 1)
            count--;

        return Math.Max(1, count);
    }
}

// -- Slug Helper -------------------------------------------------------------

/// <summary>
/// Converts text to URL-safe slugs with Unicode normalization and
/// diacritics removal.
/// </summary>
public static class Slugify
{
    private static readonly Regex NonAlphaNum = new(@"[^a-z0-9\s-]", RegexOptions.Compiled);
    private static readonly Regex Whitespace  = new(@"[\s-]+",       RegexOptions.Compiled);

    /// <summary>
    /// Converts a string to a URL-safe, lowercase slug.
    /// </summary>
    /// <example>
    /// Slugify.ToSlug("Hello World!")            // "hello-world"
    /// Slugify.ToSlug("C# Best Practices 2025") // "c-best-practices-2025"
    /// </example>
    public static string ToSlug(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        var normalized = text.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(normalized.Length);

        foreach (var c in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }

        var result = sb.ToString().Normalize(NormalizationForm.FormC);
        result = result.ToLowerInvariant();
        result = NonAlphaNum.Replace(result, "");
        result = Whitespace.Replace(result, "-");
        return result.Trim('-');
    }

    /// <summary>
    /// Generates a slug with an optional maximum length, truncating at a
    /// word boundary to avoid broken words.
    /// </summary>
    public static string ToSlug(string text, int maxLength)
    {
        var slug = ToSlug(text);
        if (slug.Length <= maxLength)
            return slug;

        slug = slug[..maxLength];
        int lastHyphen = slug.LastIndexOf('-');
        return lastHyphen > 0 ? slug[..lastHyphen] : slug;
    }
}

/// <summary>
/// Package metadata and version information.
/// </summary>
public static class Info
{
    public const string Version = "0.1.0";
    public const string BaseUrl = "https://mohitkhare.me";
}
