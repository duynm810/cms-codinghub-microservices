namespace Post.Domain.Exceptions;

public class InvalidEntityTypeException(string entity, string type)
    : ApplicationException($"Entity \"{entity}\" not supported type: {type}");