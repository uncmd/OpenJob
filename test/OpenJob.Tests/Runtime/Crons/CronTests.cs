namespace OpenJob.Runtime.Crons;

public class CronTests
{
    [Theory]
    [InlineData("* * * * *", "* * * * *")]
    [InlineData("0 0 31W * *", "0 0 31W * *")]
    [InlineData("0 23 ? * MON-FRI", "0 23 ? * 1-5")]
    [InlineData("*/5 * * * *", "*/5 * * * *")]
    [InlineData("30 11 * * 1-5", "30 11 * * 1-5")]
    [InlineData("23 12 * JAN *", "23 12 * 1 *")]
    [InlineData("* * * * MON#3", "* * * * 1#3")]
    [InlineData("*/5 * L JAN *", "*/5 * L 1 *")]
    [InlineData("0 0 ? 1 MON#1", "0 0 ? 1 1#1")]
    [InlineData("0 0 LW * *", "0 0 LW * *")]
    [InlineData("0 30 10-13 ? * WED,FRI", "0 30 10-13 ? * 3,5")]
    [InlineData("0 */5 * * * *", "0 */5 * * * *")]
    [InlineData("0 0/1 * * * ?", "0 */1 * * * ?")]
    [InlineData("5-10 30-35 10-12 * * *", "5-10 30-35 10-12 * * *")]
    public void TestParse(string expression, string outputString)
    {
        var output = Cron.Parse(expression).ToString();
        outputString.ShouldBe(output);
    }

    [Theory]
    [InlineData("* * * * *", "2021-01-01 00:01:00")]
    [InlineData("0 0 31W * *", "2021-01-29 00:00:00")]
    [InlineData("0 23 ? * MON-FRI", "2021-01-01 23:00:00")]
    [InlineData("*/5 * * * *", "2021-01-01 00:05:00")]
    [InlineData("30 11 * * 1-5", "2021-01-01 11:30:00")]
    [InlineData("23 12 * JAN *", "2021-01-01 12:23:00")]
    [InlineData("* * * * MON#3", "2021-01-18 00:00:00")]
    [InlineData("*/5 * L JAN *", "2021-01-31 00:00:00")]
    [InlineData("0 0 ? 1 MON#1", "2021-01-04 00:00:00")]
    [InlineData("0 0 LW * *", "2021-01-29 00:00:00")]
    [InlineData("0 30 10-13 ? * WED,FRI", "2021-01-01 10:30:00")]
    [InlineData("0 */5 * * * *", "2021-01-01 00:05:00")]
    [InlineData("0 0/1 * * * ?", "2021-01-01 00:01:00")]
    [InlineData("5-10 30-35 10-12 * * *", "2021-01-01 10:30:05")]
    public void TestGetNextOccurence(string expression, string nextOccurenceString)
    {
        var beginTime = new DateTime(2021, 1, 1, 0, 0, 0);
        var crontab = Cron.Parse(expression);
        var nextOccurence = crontab.GetNextOccurrence(beginTime);
        nextOccurenceString.ShouldBe(nextOccurence.ToString("yyyy-MM-dd HH:mm:ss"));
    }

    [Fact]
    public void EverySecondTest()
    {
        var beginTime = DateTime.Now.Date;
        var nextOccurence = Cron.EverySecond.GetNextOccurrence(beginTime);
        nextOccurence.ShouldBe(beginTime.AddSeconds(1));
    }

    [Fact]
    public void EveryFiveSecondsTest()
    {
        var beginTime = DateTime.Now.Date;
        var nextOccurence = Cron.EveryFiveSeconds.GetNextOccurrence(beginTime);
        nextOccurence.ShouldBe(beginTime.AddSeconds(5));
    }

    [Fact]
    public void EveryTenSecondsTest()
    {
        var beginTime = DateTime.Now.Date;
        var nextOccurence = Cron.EveryTenSeconds.GetNextOccurrence(beginTime);
        nextOccurence.ShouldBe(beginTime.AddSeconds(10));
    }

    [Fact]
    public void EveryFifteenSecondsTest()
    {
        var beginTime = DateTime.Now.Date;
        var nextOccurence = Cron.EveryFifteenSeconds.GetNextOccurrence(beginTime);
        nextOccurence.ShouldBe(beginTime.AddSeconds(15));
    }

    [Fact]
    public void EveryThirtySecondsTest()
    {
        var beginTime = DateTime.Now.Date;
        var nextOccurence = Cron.EveryThirtySeconds.GetNextOccurrence(beginTime);
        nextOccurence.ShouldBe(beginTime.AddSeconds(30));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(0)]
    [InlineData(59)]
    [InlineData(60)]
    [InlineData(-1)]
    public void EverySecondsTest(int seconds)
    {
        var beginTime = DateTime.Now.Date;
        if (seconds < 1 || seconds > 59)
        {
            Should.Throw<ArgumentException>(() =>
            {
                Cron.EverySeconds(seconds).GetNextOccurrence(beginTime);
            }, "When calling 'EverySeconds(int seconds)', 'seconds' must be between 1 and 59");
        }
        else
        {
            var nextOccurence = Cron.EverySeconds(seconds).GetNextOccurrence(beginTime);
            nextOccurence.ShouldBe(beginTime.AddSeconds(seconds));
        }
    }

    [Fact]
    public void EveryMinuteTest()
    {
        var beginTime = DateTime.Now.Date;
        var nextOccurence = Cron.EveryMinute.GetNextOccurrence(beginTime);
        nextOccurence.ShouldBe(beginTime.AddMinutes(1));
    }

    [Fact]
    public void EveryFiveMinutesTest()
    {
        var beginTime = DateTime.Now.Date;
        var nextOccurence = Cron.EveryFiveMinutes.GetNextOccurrence(beginTime);
        nextOccurence.ShouldBe(beginTime.AddMinutes(5));
    }

    [Fact]
    public void EveryTenMinutesTest()
    {
        var beginTime = DateTime.Now.Date;
        var nextOccurence = Cron.EveryTenMinutes.GetNextOccurrence(beginTime);
        nextOccurence.ShouldBe(beginTime.AddMinutes(10));
    }

    [Fact]
    public void EveryFifteenMinutesTest()
    {
        var beginTime = DateTime.Now.Date;
        var nextOccurence = Cron.EveryFifteenMinutes.GetNextOccurrence(beginTime);
        nextOccurence.ShouldBe(beginTime.AddMinutes(15));
    }

    [Fact]
    public void EveryThirtyMinutesTest()
    {
        var beginTime = DateTime.Now.Date;
        var nextOccurence = Cron.EveryThirtyMinutes.GetNextOccurrence(beginTime);
        nextOccurence.ShouldBe(beginTime.AddMinutes(30));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(0)]
    [InlineData(59)]
    [InlineData(60)]
    [InlineData(-1)]
    public void EveryMinutesTest(int minutes)
    {
        var beginTime = DateTime.Now.Date;
        if (minutes < 1 || minutes > 59)
        {
            Should.Throw<ArgumentException>(() =>
            {
                Cron.EveryMinutes(minutes).GetNextOccurrence(beginTime);
            }, "When calling 'EveryMinutes(int minutes)', 'minutes' must be between 1 and 59");
        }
        else
        {
            var nextOccurence = Cron.EveryMinutes(minutes).GetNextOccurrence(beginTime);
            nextOccurence.ShouldBe(beginTime.AddMinutes(minutes));
        }
    }

    [Fact]
    public void HourlyTest()
    {
        var beginTime = DateTime.Now.Date;
        var nextOccurence = Cron.Hourly.GetNextOccurrence(beginTime);
        nextOccurence.ShouldBe(beginTime.AddHours(1));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(0)]
    [InlineData(59)]
    [InlineData(60)]
    [InlineData(-1)]
    public void HourlyAtTest(int minutes)
    {
        var beginTime = DateTime.Now.Date;
        if (minutes < 1 || minutes > 59)
        {
            Should.Throw<ArgumentException>(() =>
            {
                Cron.EveryMinutes(minutes).GetNextOccurrence(beginTime);
            }, "When calling 'HourlyAt(int minute)', 'minute' must be between 1 and 59");
        }
        else
        {
            var nextOccurence = Cron.HourlyAt(minutes).GetNextOccurrence(beginTime);
            nextOccurence.ShouldBe(beginTime.AddMinutes(minutes));
        }
    }

    [Fact]
    public void DailyTest()
    {
        var beginTime = DateTime.Now.Date;
        var nextOccurence = Cron.Daily.GetNextOccurrence(beginTime);
        nextOccurence.ShouldBe(beginTime.AddDays(1));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(0)]
    [InlineData(23)]
    [InlineData(24)]
    [InlineData(-1)]
    public void DailyAtHourTest(int hour)
    {
        var beginTime = DateTime.Now.Date;
        if (hour < 1 || hour > 23)
        {
            Should.Throw<ArgumentException>(() =>
            {
                Cron.DailyAtHour(hour).GetNextOccurrence(beginTime);
            }, "When calling 'DailyAtHour(int hour)', 'hour' must be between 1 and 23");
        }
        else
        {
            var nextOccurence = Cron.DailyAtHour(hour).GetNextOccurrence(beginTime);
            nextOccurence.ShouldBe(beginTime.AddHours(hour));
        }
    }

    [Theory]
    [InlineData(1, 30)]
    [InlineData(8, 11)]
    [InlineData(0, 30)]
    [InlineData(23, 59)]
    [InlineData(24, 30)]
    [InlineData(9, 0)]
    public void DailyAtTest(int hour, int minute)
    {
        var beginTime = DateTime.Now.Date;
        if (hour < 0 || hour > 23)
        {
            Should.Throw<ArgumentException>(() =>
            {
                Cron.DailyAt(hour, minute).GetNextOccurrence(beginTime);
            }, "When calling 'DailyAtHour(int hour)', 'hour' must be between 0 and 23");
        }
        else if (minute < 1 || minute > 59)
        {
            Should.Throw<ArgumentException>(() =>
            {
                Cron.DailyAt(hour, minute).GetNextOccurrence(beginTime);
            }, "When calling 'EveryMinutes(int minutes)', 'minutes' must be between 1 and 59");
        }
        else
        {
            var nextOccurence = Cron.DailyAt(hour, minute).GetNextOccurrence(beginTime);
            nextOccurence.ShouldBe(beginTime.AddHours(hour).AddMinutes(minute));
        }
    }
}
