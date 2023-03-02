namespace OpenJob.Runtime.Crons;

/// <summary>
/// TimeCrontab ģ�鳣��
/// </summary>
internal static class Constants
{
    /// <summary>
    /// Cron �ֶ��������ֵ
    /// </summary>
    internal static readonly Dictionary<CrontabFieldKind, int> MaximumDateTimeValues = new()
    {
        { CrontabFieldKind.Second, 59 },
        { CrontabFieldKind.Minute, 59 },
        { CrontabFieldKind.Hour, 23 },
        { CrontabFieldKind.DayOfWeek, 7 },
        { CrontabFieldKind.Day, 31 },
        { CrontabFieldKind.Month, 12 },
    };

    /// <summary>
    /// Cron �ֶ�������Сֵ
    /// </summary>
    internal static readonly Dictionary<CrontabFieldKind, int> MinimumDateTimeValues = new()
    {
        { CrontabFieldKind.Second, 0 },
        { CrontabFieldKind.Minute, 0 },
        { CrontabFieldKind.Hour, 0 },
        { CrontabFieldKind.DayOfWeek, 0 },
        { CrontabFieldKind.Day, 1 },
        { CrontabFieldKind.Month, 1 },
    };

    /// <summary>
    /// ���� C# �� <see cref="DayOfWeek"/> ö��Ԫ��ֵ
    /// </summary>
    /// <remarks>��Ҫ��� C# �и����ͺ� Cron �����ֶ��򲻶�Ӧ����</remarks>
    internal static readonly Dictionary<DayOfWeek, int> CronDays = new()
    {
        { DayOfWeek.Sunday, 0 },
        { DayOfWeek.Monday, 1 },
        { DayOfWeek.Tuesday, 2 },
        { DayOfWeek.Wednesday, 3 },
        { DayOfWeek.Thursday, 4 },
        { DayOfWeek.Friday, 5 },
        { DayOfWeek.Saturday, 6 },
    };

    /// <summary>
    /// ���� Cron �����ֶ���ֵ֧�ֵ�����Ӣ����д
    /// </summary>
    internal static readonly Dictionary<string, int> Days = new()
    {
        { "SUN", 0 },
        { "MON", 1 },
        { "TUE", 2 },
        { "WED", 3 },
        { "THU", 4 },
        { "FRI", 5 },
        { "SAT", 6 },
    };

    /// <summary>
    /// ���� Cron ���ֶ���ֵ֧�ֵ��·�Ӣ����д
    /// </summary>
    internal static readonly Dictionary<string, int> Months = new()
    {
        { "JAN", 1 },
        { "FEB", 2 },
        { "MAR", 3 },
        { "APR", 4 },
        { "MAY", 5 },
        { "JUN", 6 },
        { "JUL", 7 },
        { "AUG", 8 },
        { "SEP", 9 },
        { "OCT", 10 },
        { "NOV", 11 },
        { "DEC", 12 },
    };
}
