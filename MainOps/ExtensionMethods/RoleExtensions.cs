public enum Role
{
    Admin,
    DivisionAdmin,
    StorageManager,
    Manager,
    Member,
    MemberGuest,
    Guest,
    ProjectMember,
    International,
    Supervisor,
    AlarmTeam,
    ExternalDriller,
    Board
}

public static class RoleExtensions
{
    public static string GetRoleName(this Role role) // convenience method
    {
        return role.ToString();
    }
}