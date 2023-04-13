using Xunit;

namespace OpenJob.TimeWheel;

public class TimingWheelTest
{
    /// <summary>
    /// 测试时间轮
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task TestTimingWheel()
    {
        var outputs = new Dictionary<string, DateTime>();
        var timer = new TimingWheelTimer();

        outputs.Add("start time", DateTime.Now);

        timer.Schedule(TimeSpan.FromMilliseconds(5000), () => { outputs.Add("5000ms", DateTime.Now); });
        timer.Schedule(TimeSpan.FromMilliseconds(2000), () => { outputs.Add("2000ms", DateTime.Now); });

        timer.Schedule(TimeSpan.FromSeconds(3), () => { outputs.Add("3s", DateTime.Now); });
        timer.Schedule(TimeSpan.FromSeconds(2), () => { outputs.Add("2s", DateTime.Now); });

        await Task.Delay(TimeSpan.FromSeconds(6));
        timer.Stop();

        outputs.Add("stop time", DateTime.Now);

        Console.WriteLine(string.Join(Environment.NewLine, outputs.Select(o => $"{o.Key}, {o.Value:HH:mm:ss.ffff}")));

        Assert.Equal(6, outputs.Count);
        Assert.Equal(2, Calc(outputs.Skip(1).First().Value, outputs.First().Value));
        Assert.Equal(2, Calc(outputs.Skip(2).First().Value, outputs.First().Value));
        Assert.Equal(3, Calc(outputs.Skip(3).First().Value, outputs.First().Value));
        Assert.Equal(5, Calc(outputs.Skip(4).First().Value, outputs.First().Value));
    }

    /// <summary>
    /// 测试任务状态
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task TestTaskStatus()
    {
        var timer = new TimingWheelTimer();

        var task1 = timer.Schedule(TimeSpan.FromSeconds(5), () => { Thread.Sleep(3000); });
        var task2 = timer.Schedule(TimeSpan.FromSeconds(5), () => { throw new Exception(); });
        var task3 = timer.Schedule(TimeSpan.FromSeconds(5), () => { throw new Exception(); });

        Assert.Equal(TimeTaskStatus.Waiting, task1.TaskStatus);
        Assert.Equal(TimeTaskStatus.Waiting, task2.TaskStatus);
        Assert.Equal(TimeTaskStatus.Waiting, task3.TaskStatus);

        task3.Cancel();
        await Task.Delay(TimeSpan.FromSeconds(6));

        Assert.Equal(TimeTaskStatus.Running, task1.TaskStatus);
        Assert.Equal(TimeTaskStatus.Fail, task2.TaskStatus);
        Assert.Equal(TimeTaskStatus.Cancel, task3.TaskStatus);

        await Task.Delay(TimeSpan.FromSeconds(4));
        Assert.Equal(TimeTaskStatus.Success, task1.TaskStatus);

        timer.Stop();
    }

    private static int Calc(DateTime dt1, DateTime dt2)
    {
        return (int)(CutOffMillisecond(dt1) - CutOffMillisecond(dt2)).TotalSeconds;
    }

    /// <summary>
    /// 截掉毫秒部分
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    private static DateTime CutOffMillisecond(DateTime dt)
    {
        return new DateTime(dt.Ticks - (dt.Ticks % TimeSpan.TicksPerSecond), dt.Kind);
    }
}
