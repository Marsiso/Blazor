namespace Blazor.Shared.Entities.RequestFeatures;

public sealed class ResetPasswordRequestParameters : RequestParameters, ICloneable
{
    public DateTime MinIssueDate { get; set; } = DateTime.MinValue;
    public DateTime MaxIssueDate { get; set; } = DateTime.MaxValue;
    public DateTime MinExpirationDate { get; set; } = DateTime.MinValue;

    public DateTime MaxExpirationDate { get; set; } = DateTime.MaxValue;

    public bool ValidExpirationDateRange => MinExpirationDate < MaxExpirationDate;
    public bool ValidIssueDateRange => MinIssueDate < MaxIssueDate;
    public object Clone()
    {
        return this.MemberwiseClone();
    }
}