using System.Reflection;

namespace Oboete.API.Entities.Users;

public static class UserRoles
{
    public static IEnumerable<string> AllRoles => 
        typeof(UserRoles)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(fi => fi is { IsLiteral: true, IsInitOnly: false } && fi.FieldType == typeof(string))
            .Select(x => (string)x.GetRawConstantValue()!)
            .ToList();
    public const string Admin = "Admin";
    public const string User = "User";
}