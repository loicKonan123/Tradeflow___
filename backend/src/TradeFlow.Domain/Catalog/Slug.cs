using System.Text.RegularExpressions;
using TradeFlow.Domain.Common;

namespace TradeFlow.Domain.Catalog;

public record Slug
{
    public string Value { get; }

    private Slug(string value) => Value = value;

    public static Result<Slug> FromTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure<Slug>("Title cannot be empty.");

        var slug = title.ToLowerInvariant().Trim();
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = Regex.Replace(slug, @"\s+", "-");
        slug = Regex.Replace(slug, @"-+", "-").Trim('-');

        if (string.IsNullOrEmpty(slug))
            return Result.Failure<Slug>("Title produces an empty slug.");

        return Result.Success(new Slug(slug));
    }

    public static Slug From(string value) => new(value);
}
