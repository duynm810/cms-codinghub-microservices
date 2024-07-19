namespace Shared.Settings;

public class GrpcSettings
{
    public string CategoryUrl { get; set; } = default!;

    public string PostUrl { get; set; } = default!;

    public string SeriesUrl { get; set; } = default!;
    
    public string PostInSeriesUrl { get; set; } = default!;
        
    public string TagUrl { get; set; } = default!;

    public string PostInTagUrl { get; set; } = default!;
        
    public string IdentityUrl { get; set; } = default!;
}