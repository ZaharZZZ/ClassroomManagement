// Helpers/SessionHelper.cs
namespace ClassroomManagement.Helpers
{
    public static class SessionHelper
    {
        public const string UserId = "UserId";
        public const string UserLogin = "UserLogin";
        public const string UserRole = "UserRole";
        public const string TeacherId = "TeacherId";
        public const string UserFullName = "UserFullName";

        public static bool IsAuthenticated(ISession session)
        {
            return session.GetInt32(UserId).HasValue;
        }

        public static bool IsAdmin(ISession session)
        {
            return session.GetString(UserRole) == "Admin";
        }

        public static bool IsTeacher(ISession session)
        {
            var role = session.GetString(UserRole);
            return role == "Teacher";
        }

        public static bool IsDispatcher(ISession session)
        {
            var role = session.GetString(UserRole);
            return role == "Dispatcher";
        }

        public static int? GetUserId(ISession session)
        {
            return session.GetInt32(UserId);
        }

        public static int? GetTeacherId(ISession session)
        {
            return session.GetInt32(TeacherId);
        }
    }
}