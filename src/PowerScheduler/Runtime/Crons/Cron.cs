﻿using PowerScheduler.Runtime.Crons.Parsers;

namespace PowerScheduler.Runtime.Crons;

/// <summary>
/// Cron 表达式, 支持(分 时 天 月 周)和(秒 分 时 天 月 周)两种格式
/// <para>┌───────────── second                 0-59              * , - /        </para>
/// <para>│ ┌───────────── minute               0-59              * , - /        </para>
/// <para>│ │ ┌───────────── hour               0-23              * , - /        </para>
/// <para>│ │ │ ┌───────────── day of month     1-31              * , - / L W ?  </para>
/// <para>│ │ │ │ ┌───────────── month          1-12 or JAN-DEC   * , - /        </para>
/// <para>│ │ │ │ │ ┌───────────── day of week  0-6  or SUN-SAT   * , - / # L ?  </para>
/// </summary>
public class Cron
{
    /// <summary>
    /// Cron 字段解析器字典集合
    /// </summary>
    private Dictionary<CrontabFieldKind, List<ICronParser>> Parsers { get; set; }

    public bool IsSeconds => Parsers.Count == 6;

    private Cron()
    {
        Parsers = new Dictionary<CrontabFieldKind, List<ICronParser>>();
    }

    /// <summary>
    /// 解析 Cron 表达式并转换成 <see cref="Cron"/> 对象
    /// </summary>
    /// <param name="expression">Cron 表达式</param>
    /// <returns><see cref="Cron"/></returns>
    /// <exception cref="CronExpressionException"></exception>
    public static Cron Parse(string expression)
    {
        return new Cron
        {
            Parsers = ParseToDictionary(expression)
        };
    }

    /// <summary>
    /// 解析 Cron 表达式并转换成 <see cref="Cron"/> 对象
    /// </summary>
    /// <remarks>解析失败返回 default</remarks>
    /// <param name="expression">Cron 表达式</param>
    /// <returns><see cref="Cron"/></returns>
    public static Cron TryParse(string expression)
    {
        try
        {
            return Parse(expression);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 获取当前时间<see cref="DateTime.Now"/>开始的下一个发生时间
    /// </summary>
    /// <returns></returns>
    public DateTime GetNextOccurrence()
    {
        return GetNextOccurrence(DateTime.Now);
    }

    /// <summary>
    /// 获取起始时间下一个发生时间
    /// </summary>
    /// <param name="baseTime">起始时间</param>
    /// <returns><see cref="DateTime"/></returns>
    public DateTime GetNextOccurrence(DateTime baseTime)
    {
        return GetNextOccurrence(baseTime, DateTime.MaxValue);
    }

    /// <summary>
    /// 获取特定时间范围下一个发生时间
    /// </summary>
    /// <param name="baseTime">起始时间</param>
    /// <param name="endTime">结束时间</param>
    /// <returns><see cref="DateTime"/></returns>
    public DateTime GetNextOccurrence(DateTime baseTime, DateTime endTime)
    {
        return InternalGetNextOccurence(baseTime, endTime);
    }

    /// <summary>
    /// 获取当前时间<see cref="DateTime.Now"/>到指定结束时间范围所有发生时间
    /// </summary>
    /// <param name="endTime">结束时间</param>
    /// <returns></returns>
    public IEnumerable<DateTime> GetNextOccurrences(DateTime endTime)
    {
        return GetNextOccurrences(DateTime.Now, endTime);
    }

    /// <summary>
    /// 获取特定时间范围所有发生时间
    /// </summary>
    /// <param name="baseTime">起始时间</param>
    /// <param name="endTime">结束时间</param>
    /// <returns><see cref="IEnumerable{T}"/></returns>
    public IEnumerable<DateTime> GetNextOccurrences(DateTime baseTime, DateTime endTime)
    {
        for (var occurrence = GetNextOccurrence(baseTime, endTime);
             occurrence < endTime;
             occurrence = GetNextOccurrence(occurrence, endTime))
        {
            yield return occurrence;
        }
    }

    /// <summary>
    /// 将 <see cref="Crontab"/> 对象转换成 Cron 表达式字符串
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var paramList = new List<string>();

        // 判断当前 Cron 格式化类型是否包含秒字段域
        if (IsSeconds)
        {
            JoinParsers(paramList, CrontabFieldKind.Second);
        }

        // Cron 常规字段域
        JoinParsers(paramList, CrontabFieldKind.Minute);
        JoinParsers(paramList, CrontabFieldKind.Hour);
        JoinParsers(paramList, CrontabFieldKind.Day);
        JoinParsers(paramList, CrontabFieldKind.Month);
        JoinParsers(paramList, CrontabFieldKind.DayOfWeek);

        // 空格分割并输出
        return string.Join(" ", paramList.ToArray());
    }

    /// <summary>
    /// 解析 Cron 表达式字段并存储其 所有发生值 字符解析器
    /// </summary>
    /// <param name="expression">Cron 表达式</param>
    /// <returns><see cref="Dictionary{TKey, TValue}"/></returns>
    /// <exception cref="CronExpressionException"></exception>
    private static Dictionary<CrontabFieldKind, List<ICronParser>> ParseToDictionary(string expression)
    {
        // Cron 表达式空检查
        if (string.IsNullOrWhiteSpace(expression))
        {
            throw new CronExpressionException("The provided cron string is null, empty or contains only whitespace.");
        }

        // 通过空白符切割 Cron 表达式每个字段域
        var instructions = expression.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        if (instructions.Length > 6)
        {
            throw new CronExpressionException(string.Format("The provided cron string <{0}> has too many parameters.", expression));
        }
        if (instructions.Length < 5)
        {
            throw new CronExpressionException(string.Format("The provided cron string <{0}> has too few parameters.", expression));
        }

        // 初始化字段偏移量和字段字符解析器
        var defaultFieldOffset = 0;
        var fieldParsers = new Dictionary<CrontabFieldKind, List<ICronParser>>();

        // 判断当前 Cron 格式化类型是否包含秒字段域，如果包含则优先解析秒字段域字符解析器
        if (instructions.Length == 6)
        {
            fieldParsers.Add(CrontabFieldKind.Second, ParseField(instructions[0], CrontabFieldKind.Second));
            defaultFieldOffset = 1;
        }

        // Cron 常规字段域
        fieldParsers.Add(CrontabFieldKind.Minute, ParseField(instructions[defaultFieldOffset + 0], CrontabFieldKind.Minute));   // 偏移量 1
        fieldParsers.Add(CrontabFieldKind.Hour, ParseField(instructions[defaultFieldOffset + 1], CrontabFieldKind.Hour));   // 偏移量 2
        fieldParsers.Add(CrontabFieldKind.Day, ParseField(instructions[defaultFieldOffset + 2], CrontabFieldKind.Day)); // 偏移量 3
        fieldParsers.Add(CrontabFieldKind.Month, ParseField(instructions[defaultFieldOffset + 3], CrontabFieldKind.Month)); // 偏移量 4
        fieldParsers.Add(CrontabFieldKind.DayOfWeek, ParseField(instructions[defaultFieldOffset + 4], CrontabFieldKind.DayOfWeek)); // 偏移量 5

        // 检查非法字符解析器，如 2 月没有 30 和 31 号
        CheckForIllegalParsers(fieldParsers);

        return fieldParsers;
    }

    /// <summary>
    /// 解析 Cron 单个字段域所有发生值 字符解析器
    /// </summary>
    /// <param name="field">字段值</param>
    /// <param name="kind">Cron 表达式格式化类型</param>
    /// <returns><see cref="List{T}"/></returns>
    /// <exception cref="CronExpressionException"></exception>
    private static List<ICronParser> ParseField(string field, CrontabFieldKind kind)
    {
        /*
         * 在 Cron 表达式中，单个字段域值也支持定义多个值（我们称为值中值），如 1,2,3 或 SUN,FRI,SAT
         * 所以，这里需要将字段域值通过 , 进行切割后独立处理
         */

        try
        {
            return field.Split(',').Select(parser => ParseParser(parser, kind)).ToList();
        }
        catch (Exception ex)
        {
            throw new CronExpressionException(
                string.Format("There was an error parsing '{0}' for the {1} field.", field, Enum.GetName(typeof(CrontabFieldKind), kind))
                , ex);
        }
    }

    /// <summary>
    /// 解析 Cron 字段域值中值
    /// </summary>
    /// <param name="parser">字段值中值</param>
    /// <param name="kind">Cron 表达式格式化类型</param>
    /// <returns><see cref="ICronParser"/></returns>
    /// <exception cref="CronExpressionException"></exception>
    private static ICronParser ParseParser(string parser, CrontabFieldKind kind)
    {
        // Cron 字段中所有字母均采用大写方式，所以需要转换所有为大写再操作
        var newParser = parser.ToUpper();

        try
        {
            // 判断值是否以 * 字符开头
            if (newParser.StartsWith("*", StringComparison.OrdinalIgnoreCase))
            {
                // 继续往后解析
                newParser = newParser[1..];

                // 判断是否以 / 字符开头，如果是，则该值为带步长的 Cron 值
                if (newParser.StartsWith("/", StringComparison.OrdinalIgnoreCase))
                {
                    // 继续往后解析
                    newParser = newParser[1..];

                    // 解析 Cron 值步长并创建 StepParser 解析器
                    var steps = GetValue(ref newParser, kind);
                    return new StepParser(0, steps, kind);
                }

                // 否则，创建 AnyParser 解析器
                return new AnyParser(kind);
            }

            // 判断值是否以 L 字符开头
            if (newParser.StartsWith("L") && kind == CrontabFieldKind.Day)
            {
                // 继续往后解析
                newParser = newParser[1..];

                // 是否是 LW 字符，如果是，创建 LastWeekdayOfMonthParser 解析器
                if (newParser == "W")
                {
                    return new LastWeekdayOfMonthParser(kind);
                }
                // 否则创建 LastDayOfMonthParser 解析器
                else
                {
                    return new LastDayOfMonthParser(kind);
                }
            }

            // 判断值是否等于 ?
            if (newParser == "?")
            {
                // 创建 BlankDayOfMonthOrWeekParser 解析器
                return new BlankDayOfMonthOrWeekParser(kind);
            }

            /*
             * 如果上面均不匹配，那么该值类似取值有：2，1/2，1-10，1-10/2，SUN，SUNDAY，SUNL，JAN，3W，3L，2#5 等
             */

            // 继续推进解析
            var firstValue = GetValue(ref newParser, kind);

            // 如果没有返回新的待解析字符，则认为这是一个具体值
            if (string.IsNullOrEmpty(newParser))
            {
                // 创建 SpecificParser 解析器
                return new SpecificParser(firstValue, kind);
            }

            // 如果存在待解析字符，如 - / 值，则进一步解析
            switch (newParser[0])
            {
                // 判断值是否以 / 字符开头
                case '/':
                    {
                        // 继续往后解析
                        newParser = newParser[1..];

                        // 解析 Cron 值步长并创建 StepParser 解析器
                        var steps = GetValue(ref newParser, kind);
                        return new StepParser(firstValue, steps, kind);
                    }
                // 判断值是否以 - 字符开头
                case '-':
                    {
                        // 继续往后解析
                        newParser = newParser[1..];

                        // 获取范围结束值
                        var endValue = GetValue(ref newParser, kind);
                        int? steps = null;

                        // 继续推进解析，判断是否以 / 开头，如果是，则获取步长
                        if (newParser.StartsWith("/"))
                        {
                            newParser = newParser[1..];
                            steps = GetValue(ref newParser, kind);
                        }

                        // 创建 RangeParser 解析器
                        return new RangeParser(firstValue, endValue, steps, kind);
                    }
                // 判断值是否以 # 字符开头
                case '#':
                    {
                        // 继续往后解析
                        newParser = newParser[1..];

                        // 获取第几个
                        var weekNumber = GetValue(ref newParser, kind);

                        // 继续推进解析，如果存在其他字符，则抛异常
                        if (!string.IsNullOrEmpty(newParser))
                        {
                            throw new CronExpressionException(string.Format("Invalid parser '{0}.'", parser));
                        }

                        // 创建 SpecificDayOfWeekInMonthParser 解析器
                        return new SpecificDayOfWeekInMonthParser(firstValue, weekNumber, kind);
                    }
                // 判断解析值是否等于 L 或 W
                default:
                    // 创建 LastDayOfWeekInMonthParser 解析器
                    if (newParser == "L" && kind == CrontabFieldKind.DayOfWeek)
                    {
                        return new LastDayOfWeekInMonthParser(firstValue, kind);
                    }
                    // 创建 NearestWeekdayParser 解析器
                    else if (newParser == "W" && kind == CrontabFieldKind.Day)
                    {
                        return new NearestWeekdayParser(firstValue, kind);
                    }
                    break;
            }

            throw new CronExpressionException(string.Format("Invalid parser '{0}'.", parser));
        }
        catch (Exception ex)
        {
            throw new CronExpressionException(string.Format("Invalid parser '{0}'. See inner exception for details.", parser), ex);
        }
    }

    /// <summary>
    /// 将 Cron 字段值中值进一步解析
    /// </summary>
    /// <param name="parser">当前解析值</param>
    /// <param name="kind">Cron 表达式格式化类型</param>
    /// <returns><see cref="int"/></returns>
    /// <exception cref="CronExpressionException"></exception>
    private static int GetValue(ref string parser, CrontabFieldKind kind)
    {
        // 值空检查
        if (string.IsNullOrEmpty(parser))
        {
            throw new CronExpressionException("Expected number, but parser was empty.");
        }

        // 字符偏移量
        int offset;

        // 判断首个字符是数字还是字符串
        var isDigit = char.IsDigit(parser[0]);
        var isLetter = char.IsLetter(parser[0]);

        // 推进式遍历值并检查每一个字符，一旦出现类型不连贯则停止检查
        for (offset = 0; offset < parser.Length; offset++)
        {
            // 如果存在不连贯数字或字母则跳出循环
            if ((isDigit && !char.IsDigit(parser[offset])) || (isLetter && !char.IsLetter(parser[offset])))
            {
                break;
            }
        }

        var maximum = Constants.MaximumDateTimeValues[kind];

        // 前面连贯类型的值
        var valueToParse = parser[..offset];

        // 处理数字开头的连贯类型值
        if (int.TryParse(valueToParse, out var value))
        {
            // 导出下一轮待解析的值（依旧采用推进式）
            parser = parser[offset..];

            var returnValue = value;

            // 验证值范围
            if (returnValue > maximum)
            {
                throw new CronExpressionException(string.Format("Value for {0} parser exceeded maximum value of {1}.", Enum.GetName(typeof(CrontabFieldKind), kind), maximum));
            }

            return returnValue;
        }
        // 处理字母开头的连贯类型值，通常认为这是一个单词，如SUN，JAN
        else
        {
            List<KeyValuePair<string, int>> replaceVal = null;

            // 判断当前 Cron 字段类型是否是星期，如果是，则查找该单词是否在 Constants.Days 定义之中
            if (kind == CrontabFieldKind.DayOfWeek)
            {
                replaceVal = Constants.Days.Where(x => valueToParse.StartsWith(x.Key)).ToList();
            }
            // 判断当前 Cron 字段类型是否是月份，如果是，则查找该单词是否在 Constants.Months 定义之中
            else if (kind == CrontabFieldKind.Month)
            {
                replaceVal = Constants.Months.Where(x => valueToParse.StartsWith(x.Key)).ToList();
            }

            // 如果存在且唯一，则进入下一轮判断
            // 接下来的判断是处理 SUN + L 的情况，如 SUNL == 0L == SUNDAY，它们都是合法的 Cron 值
            if (replaceVal != null && replaceVal.Count == 1)
            {
                var missingParser = "";

                // 处理带 L 和不带 L 的单词问题
                if (parser.Length == offset
                    && parser.EndsWith("L")
                    && kind == CrontabFieldKind.DayOfWeek)
                {
                    missingParser = "L";
                }
                parser = parser[offset..] + missingParser;

                // 转换成 int 值返回（SUN，JAN.....）
                var returnValue = replaceVal.First().Value;

                // 验证值范围
                if (returnValue > maximum)
                {
                    throw new CronExpressionException(string.Format("Value for {0} parser exceeded maximum value of {1}.", Enum.GetName(typeof(CrontabFieldKind), kind), maximum));
                }

                return returnValue;
            }
        }

        throw new CronExpressionException("Parser does not contain expected number.");
    }

    /// <summary>
    /// 检查非法字符解析器，如 2 月没有 30 和 31 号
    /// </summary>
    /// <remarks>检查 2 月份是否存在 30 和 31 天的非法数值解析器</remarks>
    /// <param name="parsers">Cron 字段解析器字典集合</param>
    /// <exception cref="CronExpressionException"></exception>
    private static void CheckForIllegalParsers(Dictionary<CrontabFieldKind, List<ICronParser>> parsers)
    {
        // 获取当前 Cron 表达式月字段和天字段所有数值
        var monthSingle = GetSpecificParsers(parsers, CrontabFieldKind.Month);
        var daySingle = GetSpecificParsers(parsers, CrontabFieldKind.Day);

        // 如果月份为 2 月单天数出现 30 和 31 天，则是无效数值
        if (monthSingle.Any() && monthSingle.All(x => x.SpecificValue == 2))
        {
            if (daySingle.Any() && daySingle.All(x => (x.SpecificValue == 30) || (x.SpecificValue == 31)))
            {
                throw new CronExpressionException("The February 30 and 31 don't exist.");
            }
        }
    }

    /// <summary>
    /// 查找 Cron 字段类型所有具体值解析器
    /// </summary>
    /// <param name="parsers">Cron 字段解析器字典集合</param>
    /// <param name="kind">Cron 字段种类</param>
    /// <returns><see cref="List{T}"/></returns>
    private static List<SpecificParser> GetSpecificParsers(Dictionary<CrontabFieldKind, List<ICronParser>> parsers, CrontabFieldKind kind)
    {
        var kindParsers = parsers[kind];

        // 查找 Cron 字段类型所有具体值解析器
        return kindParsers.Where(x => x.GetType() == typeof(SpecificParser)).Cast<SpecificParser>()
            .Union(
            kindParsers.Where(x => x.GetType() == typeof(RangeParser)).SelectMany(x => ((RangeParser)x).SpecificParsers)
            ).Union(
                kindParsers.Where(x => x.GetType() == typeof(StepParser)).SelectMany(x => ((StepParser)x).SpecificParsers)
            ).ToList();
    }

    /// <summary>
    /// 获取特定时间范围下一个发生时间
    /// </summary>
    /// <param name="baseTime">起始时间</param>
    /// <param name="endTime">结束时间</param>
    /// <returns><see cref="DateTime"/></returns>
    private DateTime InternalGetNextOccurence(DateTime baseTime, DateTime endTime)
    {
        // 由于 Cron 格式化类型不包含毫秒，则裁剪掉毫秒部分
        var newValue = baseTime;
        newValue = newValue.AddMilliseconds(-newValue.Millisecond);

        // 如果当前 Cron 格式化类型不支持秒，则裁剪掉秒部分
        if (!IsSeconds)
        {
            newValue = newValue.AddSeconds(-newValue.Second);
        }

        // 获取分钟、小时所有字符解析器
        var minuteParsers = Parsers[CrontabFieldKind.Minute].Where(x => x is ITimeParser).Cast<ITimeParser>().ToList();
        var hourParsers = Parsers[CrontabFieldKind.Hour].Where(x => x is ITimeParser).Cast<ITimeParser>().ToList();

        // 获取秒、分钟、小时解析器中最小起始值
        // 该值主要用来获取下一个发生值的输入参数
        var firstSecondValue = newValue.Second;
        var firstMinuteValue = minuteParsers.Select(x => x.First()).Min();
        var firstHourValue = hourParsers.Select(x => x.First()).Min();

        // 定义一个标识，标识当前时间下一个发生时间值是否进入新一轮循环
        // 如：如果当前时间的秒数为 59，那么下一个秒数应该为 00，那么当前时间分钟就应该 +1
        // 以此类推，如果 +1 后分钟数为 59，那么下一个分钟数也应该为 00，那么当前时间小时数就应该 +1
        // ....
        var overflow = true;

        // 处理 Cron 格式化类型包含秒的情况 =================================================================
        var newSeconds = newValue.Second;
        if (IsSeconds)
        {
            // 获取秒所有字符解析器
            var secondParsers = Parsers[CrontabFieldKind.Second].Where(x => x is ITimeParser).Cast<ITimeParser>().ToList();

            // 获取秒解析器最小起始值
            firstSecondValue = secondParsers.Select(x => x.First()).Min();

            // 获取秒下一个发生值
            newSeconds = Increment(secondParsers, newValue.Second, firstSecondValue, out overflow);

            // 设置起始时间为下一个秒时间
            newValue = new DateTime(newValue.Year, newValue.Month, newValue.Day, newValue.Hour, newValue.Minute, newSeconds);

            // 如果当前秒并没有进入下一轮循环但存在不匹配的字符过滤器
            if (!overflow && !IsMatch(newValue))
            {
                // 重置秒为起始值并标记 overflow 为 true 进入新一轮循环
                newSeconds = firstSecondValue;

                // 此时计算时间秒部分应该为起始值
                // 如 22:10:59 -> 22:10:00
                newValue = new DateTime(newValue.Year, newValue.Month, newValue.Day, newValue.Hour, newValue.Minute, newSeconds);

                // 标记进入下一轮循环
                overflow = true;
            }

            // 如果程序到达这里，说明并没有进入上面分支，则直接返回下一秒时间
            if (!overflow)
            {
                return MinDate(newValue, endTime);
            }
        }

        // 程序到达这里，说明秒部分已经标识进入新一轮循环，那么分支就应该获取下一个分钟发生值 =================================================================
        var newMinutes = Increment(minuteParsers, newValue.Minute + (overflow ? 0 : -1), firstMinuteValue, out overflow);

        // 设置起始时间为下一个分钟时间
        newValue = new DateTime(newValue.Year, newValue.Month, newValue.Day, newValue.Hour, newMinutes, overflow ? firstSecondValue : newSeconds);

        // 如果当前分钟并没有进入下一轮循环但存在不匹配的字符过滤器
        if (!overflow && !IsMatch(newValue))
        {
            // 重置秒，分钟为起始值并标记 overflow 为 true 进入新一轮循环
            newSeconds = firstSecondValue;
            newMinutes = firstMinuteValue;

            // 此时计算时间秒和分钟部分应该为起始值
            // 如 22:59:59 -> 22:00:00
            newValue = new DateTime(newValue.Year, newValue.Month, newValue.Day, newValue.Hour, newMinutes, firstSecondValue);

            // 标记进入下一轮循环
            overflow = true;
        }

        // 如果程序到达这里，说明并没有进入上面分支，则直接返回下一分钟时间
        if (!overflow)
        {
            return MinDate(newValue, endTime);
        }

        // 程序到达这里，说明分钟部分已经标识进入新一轮循环，那么分支就应该获取下一个小时发生值 =================================================================
        var newHours = Increment(hourParsers, newValue.Hour + (overflow ? 0 : -1), firstHourValue, out overflow);

        // 设置起始时间为下一个小时时间
        newValue = new DateTime(newValue.Year, newValue.Month, newValue.Day, newHours,
            overflow ? firstMinuteValue : newMinutes,
            overflow ? firstSecondValue : newSeconds);

        // 如果当前小时并没有进入下一轮循环但存在不匹配的字符过滤器
        if (!overflow && !IsMatch(newValue))
        {
            // 此时计算时间秒，分钟和小时部分应该为起始值
            // 如 23:59:59 -> 23:00:00
            newValue = new DateTime(newValue.Year, newValue.Month, newValue.Day, firstHourValue, firstMinuteValue, firstSecondValue);

            // 标记进入下一轮循环
            overflow = true;
        }

        // 如果程序到达这里，说明并没有进入上面分支，则直接返回下一小时时间
        if (!overflow)
        {
            return MinDate(newValue, endTime);
        }

        // 程序能够执行到这里，那么说明时间已经是 23:59:59，所以起始时间追加 1 天
        // 这里的代码看起来很奇怪，但是是为了处理终止时间为 12/31/9999 23:59:59.999 的情况，也就是世界末日了~~~
        try
        {
            newValue = newValue.AddDays(1);
        }
        catch
        {
            return endTime;
        }

        // 在有效的年份时间内死循环至天、周、月、年全部匹配才终止循环
        while (!(IsMatch(newValue, CrontabFieldKind.Day)
            && IsMatch(newValue, CrontabFieldKind.DayOfWeek)
            && IsMatch(newValue, CrontabFieldKind.Month)))
        {
            // 如果当前匹配到的时间已经大于或等于终止时间，则直接返回
            if (newValue >= endTime)
            {
                return MinDate(newValue, endTime);
            }

            // 同样防止终止时间为 12/31/9999 23:59:59.999 的情况
            try
            {
                // 不断增加 1 天直至匹配成功
                newValue = newValue.AddDays(1);
            }
            catch
            {
                return endTime;
            }
        }

        return MinDate(newValue, endTime);
    }

    /// <summary>
    /// 获取当前时间解析器下一个发生值
    /// </summary>
    /// <param name="parsers">解析器</param>
    /// <param name="value">当前值</param>
    /// <param name="defaultValue">默认值</param>
    /// <param name="overflow">控制秒、分钟、小时到达59秒/分和23小时开关</param>
    /// <returns><see cref="int"/></returns>
    private static int Increment(IEnumerable<ITimeParser> parsers, int value, int defaultValue, out bool overflow)
    {
        var nextValue = parsers.Select(x => x.Next(value))
            .Where(x => x > value)
            .Min()
            ?? defaultValue;

        // 如果此时秒或分钟或23到达最大值，则应该返回起始值
        overflow = nextValue <= value;

        return nextValue;
    }

    /// <summary>
    /// 判断 Cron 所有字段字符解析器是否都能匹配当前时间各个部分
    /// </summary>
    /// <param name="datetime">当前时间</param>
    /// <returns><see cref="bool"/></returns>
    private bool IsMatch(DateTime datetime)
    {
        return Parsers.All(fieldKind =>
            fieldKind.Value.Any(parser => parser.IsMatch(datetime))
        );
    }

    /// <summary>
    /// 判断当前 Cron 字段类型字符解析器和当前时间至少存在一种匹配
    /// </summary>
    /// <param name="datetime">当前时间</param>
    /// <param name="kind">Cron 字段种类</param>
    /// <returns></returns>
    private bool IsMatch(DateTime datetime, CrontabFieldKind kind)
    {
        return Parsers.Where(x => x.Key == kind)
            .SelectMany(x => x.Value)
            .Any(parser => parser.IsMatch(datetime));
    }

    /// <summary>
    /// 处理下一个发生时间边界值
    /// </summary>
    /// <remarks>如果发生时间大于终止时间，则返回终止时间，否则返回发生时间</remarks>
    /// <param name="newTime">下一个发生时间</param>
    /// <param name="endTime">终止时间</param>
    /// <returns><see cref="DateTime"/></returns>
    private static DateTime MinDate(DateTime newTime, DateTime endTime)
    {
        return newTime >= endTime ? endTime : newTime;
    }

    /// <summary>
    /// 将 Cron 字段解析器转换成字符串
    /// </summary>
    /// <param name="paramList">Cron 字段字符串集合</param>
    /// <param name="kind">Cron 字段种类</param>
    private void JoinParsers(List<string> paramList, CrontabFieldKind kind)
    {
        paramList.Add(
            string.Join(",", Parsers
                .Where(x => x.Key == kind)
                .SelectMany(x => x.Value.Select(y => y.ToString())).ToArray()
            )
        );
    }

    #region 常用表达式

    /// <summary>
    /// 表示每秒的 <see cref="Cron"/> 对象
    /// </summary>
    public static readonly Cron EverySecond = Parse("* * * * * *");

    /// <summary>
    /// 表示每5秒的 <see cref="Cron"/> 对象
    /// </summary>
    public static readonly Cron EveryFiveSeconds = Parse("*/5 * * * * *");

    /// <summary>
    /// 表示每10秒的 <see cref="Cron"/> 对象
    /// </summary>
    public static readonly Cron EveryTenSeconds = Parse("*/10 * * * * *");

    /// <summary>
    /// 表示每15秒的 <see cref="Cron"/> 对象
    /// </summary>
    public static readonly Cron EveryFifteenSeconds = Parse("*/15 * * * * *");

    /// <summary>
    /// 表示每30秒的 <see cref="Cron"/> 对象
    /// </summary>
    public static readonly Cron EveryThirtySeconds = Parse("*/30 * * * * *");

    /// <summary>
    /// 表示每 <paramref name="seconds"/>(1-59) 秒的 <see cref="Cron"/> 对象
    /// </summary>
    public static Cron EverySeconds(int seconds)
    {
        if (seconds < 1 || seconds > 59)
        {
            throw new ArgumentException("When calling 'EverySeconds(int seconds)', 'seconds' must be between 1 and 59");
        }

        return Parse($"*/{seconds} * * * * *");
    }

    /// <summary>
    /// 表示每分钟的 <see cref="Cron"/> 对象
    /// </summary>
    public static readonly Cron EveryMinute = Parse("* * * * *");

    /// <summary>
    /// 表示每5分钟的 <see cref="Cron"/> 对象
    /// </summary>
    public static readonly Cron EveryFiveMinutes = Parse("*/5 * * * *");

    /// <summary>
    /// 表示每10分钟的 <see cref="Cron"/> 对象
    /// </summary>
    public static readonly Cron EveryTenMinutes = Parse("*/10 * * * *");

    /// <summary>
    /// 表示每15分钟的 <see cref="Cron"/> 对象
    /// </summary>
    public static readonly Cron EveryFifteenMinutes = Parse("*/15 * * * *");

    /// <summary>
    /// 表示每30分钟的 <see cref="Cron"/> 对象
    /// </summary>
    public static readonly Cron EveryThirtyMinutes = Parse("*/30 * * * *");

    /// <summary>
    /// 表示每 <paramref name="minutes"/>(1-59) 分钟的 <see cref="Cron"/> 对象
    /// </summary>
    public static Cron EveryMinutes(int minutes)
    {
        if (minutes < 1 || minutes > 59)
        {
            throw new ArgumentException("When calling 'EveryMinutes(int minutes)', 'minutes' must be between 1 and 59");
        }

        return Parse($"*/{minutes} * * * *");
    }

    /// <summary>
    /// 表示每小时的 <see cref="Cron"/> 对象
    /// </summary>
    public static readonly Cron Hourly = Parse("0 * * * *");

    /// <summary>
    /// 表示每小时的 <see cref="Cron"/> 对象，并且在指定的分钟执行
    /// </summary>
    /// <param name="minute">指定要执行的分钟数</param>
    public static Cron HourlyAt(int minute)
    {
        if (minute < 0 || minute > 59)
        {
            throw new ArgumentException("When calling 'HourlyAt(int minute)', 'minute' must be between 1 and 59");
        }

        return Parse($"{minute} * * * *");
    }

    /// <summary>
    /// 表示每天的 <see cref="Cron"/> 对象
    /// </summary>
    public static readonly Cron Daily = Parse("0 0 * * *");

    /// <summary>
    /// 表示每天的 <see cref="Cron"/> 对象，并且在指定的小时执行
    /// </summary>
    /// <param name="hour">指定要执行的小时数</param>
    public static Cron DailyAtHour(int hour)
    {
        if (hour < 1 || hour > 23)
        {
            throw new ArgumentException("When calling 'DailyAtHour(int hour)', 'hour' must be between 1 and 23");
        }

        return Parse($"* {hour} * * * ");
    }

    /// <summary>
    /// 表示每小时的 <see cref="Cron"/> 对象，并且在指定的分钟执行
    /// </summary>
    public static Cron DailyAt(int hour, int minute)
    {
        if (hour < 0 || hour > 23)
        {
            throw new ArgumentException("When calling 'DailyAt(int hour, int minute)', 'hour' must be between 0 and 23");
        }
        if (minute < 1 || minute > 59)
        {
            throw new ArgumentException("When calling 'DailyAt(int hour, int minute)', 'minute' must be between 1 and 59");
        }

        return Parse($"{minute} {hour} * * *");
    }

    /// <summary>
    /// 表示每周星期一00:00 UTC触发的 <see cref="Cron"/> 对象
    /// </summary>
    public static Cron Weekly()
    {
        return Weekly(DayOfWeek.Monday);
    }

    public static Cron Weekly(DayOfWeek dayOfWeek)
    {
        return Weekly(dayOfWeek, hour: 0);
    }

    public static Cron Weekly(DayOfWeek dayOfWeek, int hour)
    {
        return Weekly(dayOfWeek, hour, minute: 0);
    }

    public static Cron Weekly(DayOfWeek dayOfWeek, int hour, int minute)
    {
        return Parse($"{minute} {hour} * * {(int)dayOfWeek}");
    }

    /// <summary>
    /// 表示每月1号（午夜）开始的 <see cref="Cron"/> 对象
    /// </summary>
    public static Cron Monthly()
    {
        return Monthly(day: 1);
    }

    public static Cron Monthly(int day)
    {
        return Monthly(day, hour: 0);
    }

    public static Cron Monthly(int day, int hour)
    {
        return Monthly(day, hour, minute: 0);
    }

    public static Cron Monthly(int day, int hour, int minute)
    {
        return Parse($"{minute} {hour} {day} * *");
    }

    /// <summary>
    /// 表示每年1月1号（午夜）开始的 <see cref="Cron"/> 对象
    /// </summary>
    public static readonly Cron Yearly = Parse("0 0 1 1 *");

    #endregion
}
