using Newtonsoft.Json;
using Shared.Dtos.Category;
using Shared.Dtos.Tag;
using Shared.Responses;

namespace Shared.Dtos.Aggregator;

public class FooterDto
{
    [JsonProperty("categories")] 
    public ApiResult<List<CategoryDto>> Categories { get; set; } = new();

    [JsonProperty("tags")] 
    public ApiResult<List<TagDto>> Tags { get; set; } = new();
}