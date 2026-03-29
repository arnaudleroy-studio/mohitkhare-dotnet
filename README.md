# MohitKhare .NET Utilities

Developer utilities for .NET projects: token counting, text analysis, slug generation, and common string helpers. Built by [Mohit Khare](https://mohitkhare.me).

## Installation

```bash
dotnet add package MohitKhare
```

Or via the NuGet Package Manager:

```
Install-Package MohitKhare
```

## Quick Start

```csharp
using MohitKhare;

// Estimate LLM token count
var tokens = TokenCounter.Estimate("Hello, how are you doing today?");
Console.WriteLine($"Estimated tokens: {tokens}"); // 8

// Generate URL-safe slugs
var slug = SlugGenerator.Slugify("My Blog Post Title!");
Console.WriteLine(slug); // "my-blog-post-title"
```

## Features

### Token Counter

Estimate token counts for LLM API calls. Uses the standard ~4 characters per token heuristic, suitable for planning API budgets:

```csharp
using MohitKhare;

var text = "The quick brown fox jumps over the lazy dog.";

// Estimate tokens
int tokens = TokenCounter.Estimate(text);
Console.WriteLine($"Tokens: {tokens}"); // 12

// Estimate cost at a given rate
decimal cost = TokenCounter.EstimateCost(text, pricePerMillionTokens: 3.0m);
Console.WriteLine($"Cost: ${cost}"); // $0.000036

// Check if text fits within a context window
bool fits = TokenCounter.FitsInBudget(text, maxTokens: 4096);
Console.WriteLine($"Fits in 4K context: {fits}"); // true
```

### Text Analyzer

Compute readability metrics and text statistics:

```csharp
using MohitKhare;

var article = "Artificial intelligence is transforming software development. " +
              "New tools help developers write better code faster. " +
              "The industry is evolving rapidly.";

Console.WriteLine($"Words: {TextAnalyzer.WordCount(article)}");           // 18
Console.WriteLine($"Sentences: {TextAnalyzer.SentenceCount(article)}");   // 3
Console.WriteLine($"Reading time: {TextAnalyzer.ReadingTimeMinutes(article)} min"); // 0.1
Console.WriteLine($"Flesch score: {TextAnalyzer.FleschReadingEase(article)}");      // ~60-70
```

### Slug Generator

Generate URL-safe slugs with multiple format options:

```csharp
using MohitKhare;

// URL slug
SlugGenerator.Slugify("Hello World! 2024");    // "hello-world-2024"

// File-safe name
SlugGenerator.ToFileName("My Report.pdf");      // "my_reportpdf"

// camelCase
SlugGenerator.ToCamelCase("user profile page"); // "userProfilePage"
```

### String Extensions

Convenient extension methods for everyday string manipulation:

```csharp
using MohitKhare;

// Truncate with ellipsis
"A very long sentence that needs truncation".Truncate(20);
// "A very long sente..."

// Strip HTML tags
"<p>Hello <strong>world</strong></p>".StripHtml();
// "Hello world"

// Extract first N sentences
var text = "First sentence. Second sentence. Third sentence.";
text.FirstSentences(2);
// "First sentence. Second sentence."
```

## Use Cases

- **LLM cost estimation** -- Budget API calls before sending requests to OpenAI, Anthropic, or other providers.
- **Content analysis** -- Compute readability scores for blog posts and documentation.
- **SEO tools** -- Generate consistent URL slugs for web applications.
- **Text processing** -- Strip HTML, truncate strings, and extract summaries in content pipelines.

## Links

- [Mohit Khare Homepage](https://mohitkhare.me)
- [Source Code](https://github.com/arnaudleroy-studio/mohitkhare-dotnet)
- [Report Issues](https://github.com/arnaudleroy-studio/mohitkhare-dotnet/issues)

## License

MIT License. See [LICENSE](LICENSE) for details.
