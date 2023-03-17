using Volo.Abp.Reflection;

namespace OpenJob.Permissions;

public static class OpenJobPermissions
{
    public const string GroupName = "OpenJob";

    public static class Apps
    {
        public const string Default = GroupName + ".Apps";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public static class Jobs
    {
        public const string Default = GroupName + ".Jobs";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public static class Tasks
    {
        public const string Default = GroupName + ".Tasks";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public static class Workers
    {
        public const string Default = GroupName + ".Workers";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(OpenJobPermissions));
    }
}
