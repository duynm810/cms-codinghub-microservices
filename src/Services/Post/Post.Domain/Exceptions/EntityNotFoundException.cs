namespace Post.Domain.Exceptions;

public class EntityNotFoundException(string entity, object key) 
    : ApplicationException($"Entity \"{entity}\" ({key}) was not found.");