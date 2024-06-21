namespace Media.Api.Dtos;

public class SingleFileDto
{
    public IFormFile File { get; set; }
    
    public string Type { get; set; }
}