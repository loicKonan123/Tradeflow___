namespace TradeFlow.Domain.Projects;

public sealed record CustomProjectRequestId(Guid Value)
{
    public static CustomProjectRequestId New() => new(Guid.NewGuid());
    public static CustomProjectRequestId From(Guid value) => new(value);
}
