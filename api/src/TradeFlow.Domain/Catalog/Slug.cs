using System.Text.RegularExpressions;
using TradeFlow.Domain.Common;

namespace TradeFlow.Domain.Catalog;

public sealed record Slug(string Value)
{
    public static Result<Slug> FromTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure<Slug>("Title cannot be empty.");

        var slug = Regex.Replace(title.ToLowerInvariant().Trim(), @"[^a-z0-9\s-]", "");
        slug = Regex.Replace(slug, @"\s+", "-");
        slug = slug.Trim('-');

        if (slug.Length < 3)
            return Result.Failure<Slug>("Slug is too short.");

        return Result.Success(new Slug(slug));
    }
}
