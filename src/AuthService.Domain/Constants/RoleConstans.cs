namespace AuthService.Domain.Constants;

public class RoleConstants
{
    public const string PLATFORM_ADMIN_ROLE = "PLATFORM_ADMIN";
    public const string BRANCH_ADMIN_ROLE = "BRANCH_ADMIN";
    public const string EMPLOYEE_ROLE = "EMPLOYEE";
    public const string CLIENT_ROLE = "CLIENT";

    public static readonly string[] AllowedRoles = 
    [
        PLATFORM_ADMIN_ROLE, 
        BRANCH_ADMIN_ROLE, 
        EMPLOYEE_ROLE, 
        CLIENT_ROLE
    ];
}