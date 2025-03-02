using Shared.Enums;

namespace Infrastructure.Helpers;

public static class PermissionHelper
{
    public static string GetPermission(FunctionCodeEnum functionCode, CommandCodeEnum commandCode)
        => string.Join(".", functionCode, commandCode);

    public static string GetPermission(string functionCode, string commandCode)
        => string.Join(".", functionCode, commandCode);
}