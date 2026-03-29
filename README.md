# MohitKhare .NET Utilities

Developer utilities for token estimation, text analysis, and slug generation. Built for .NET 8+ with idiomatic C# patterns including records, pattern matching, and LINQ.

## Installation

```bash
dotnet add package MohitKhare
```

## Token Estimation

Estimate token counts and API costs before sending text to LLM APIs. The estimator uses a word-boundary heuristic calibrated against BPE tokenizers (GPT, Claude):

```csharp
using MohitKhare;

var article = File.ReadAllText("article.md");
var estimate = TokenEstimator.Estimate(article);

Console.WriteLine($"Tokens: ~{estimate.Tokens}");
Console.WriteLine($"Words: {estimate.Words}");
Console.WriteLine($"Estimated cost: ${estimate.EstimateCost(3.00m):F4}");
// Tokens: ~1729
// Words: 1300
// Estimated cost: $0.0052
```

## Text Analysis

Analyze text for word count, reading time, sentence structure, and readability. Readability uses the Flesch Reading Ease formula:

```csharp
var analysis = TextAnalyzer.Analyze(article);

Console.WriteLine($"Words: {analysis.WordCount}");
Console.WriteLine($"Sentences: {analysis.SentenceCount}");
Console.WriteLine($"Reading time: {analysis.ReadingTime.Minutes}m {analysis.ReadingTime.Seconds}s");
Console.WriteLine($"Readability: {analysis.FleschReadingEase} ({analysis.ReadingLevel})");
// Words: 1300
// Sentences: 68
// Reading time: 5m 27s
// Readability: 62.3 (Standard)
```

## Slug Generation

Convert arbitrary text to clean, URL-safe slugs with full Unicode support. Handles diacritics, special characters, and optional length limits:

```csharp
Slugify.ToSlug("C# Best Practices 2025");    // "c-best-practices-2025"
Slugify.ToSlug("Resume: A Developer's Path"); // "resume-a-developers-path"
Slugify.ToSlug("Long Title Here", maxLength: 10); // "long-title"
```

## Reading Levels

The Flesch Reading Ease score maps to human-readable levels:

| Score | Level | Typical Audience |
|-------|-------|-----------------|
| 90-100 | Very Easy | 5th grader |
| 80-89 | Easy | 6th grader |
| 70-79 | Fairly Easy | 7th grader |
| 60-69 | Standard | 8th-9th grader |
| 50-59 | Fairly Difficult | High school |
| 30-49 | Difficult | College |
| 0-29 | Very Difficult | Graduate |

## API Surface

| Type | Description |
|------|-------------|
| `TokenEstimator` | BPE-calibrated token count and cost estimation |
| `TextAnalyzer` | Word count, reading time, Flesch readability |
| `Slugify` | Unicode-aware slug generation with optional length limit |

## Links

- [MohitKhare](https://mohitkhare.me)
- [Source Code](https://github.com/arnaudleroy-studio/mohitkhare-dotnet)
- [NuGet Package](https://www.nuget.org/packages/MohitKhare)

## License

MIT License. See [LICENSE](LICENSE) for details.
