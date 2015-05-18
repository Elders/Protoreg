namespace Elders.Protoreg.Tests
{
    public static class MeasureExecutionTime
    {
        public static string Start(System.Action action)
        {
            string result = string.Empty;

            var stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();
            action();
            stopWatch.Stop();
            System.TimeSpan ts = stopWatch.Elapsed;
            result = System.String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            return result;
        }

        public static string Start(System.Action action, int repeat, bool showTicksInfo = false)
        {
            var stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();
            for (int i = 0; i < repeat; i++)
            {
                action();
            }
            stopWatch.Stop();
            System.TimeSpan total = stopWatch.Elapsed;
            System.TimeSpan average = new System.TimeSpan(stopWatch.Elapsed.Ticks / repeat);

            System.Text.StringBuilder perfResultsBuilder = new System.Text.StringBuilder();
            perfResultsBuilder.AppendLine("--------------------------------------------------------------");
            perfResultsBuilder.AppendFormat("  Total Time => {0}\r\nAverage Time => {1}", Align(total), Align(average));
            perfResultsBuilder.AppendLine();
            perfResultsBuilder.AppendLine("--------------------------------------------------------------");
            if (showTicksInfo)
                perfResultsBuilder.AppendLine(TicksInfo());
            return perfResultsBuilder.ToString();
        }

        static string Align(System.TimeSpan interval)
        {
            string intervalStr = interval.ToString();
            int pointIndex = intervalStr.IndexOf(':');

            pointIndex = intervalStr.IndexOf('.', pointIndex);
            if (pointIndex < 0) intervalStr += "        ";
            return intervalStr;
        }

        static string TicksInfo()
        {
            System.Text.StringBuilder ticksInfoBuilder = new System.Text.StringBuilder("\r\n\r\n");
            ticksInfoBuilder.AppendLine("Ticks Info");
            ticksInfoBuilder.AppendLine("--------------------------------------------------------------");
            const string numberFmt = "{0,-22}{1,18:N0}";
            const string timeFmt = "{0,-22}{1,26}";

            ticksInfoBuilder.AppendLine(System.String.Format(numberFmt, "Field", "Value"));
            ticksInfoBuilder.AppendLine(System.String.Format(numberFmt, "-----", "-----"));

            // Display the maximum, minimum, and zero TimeSpan values.
            ticksInfoBuilder.AppendLine(System.String.Format(timeFmt, "Maximum TimeSpan", Align(System.TimeSpan.MaxValue)));
            ticksInfoBuilder.AppendLine(System.String.Format(timeFmt, "Minimum TimeSpan", Align(System.TimeSpan.MinValue)));
            ticksInfoBuilder.AppendLine(System.String.Format(timeFmt, "Zero TimeSpan", Align(System.TimeSpan.Zero)));
            ticksInfoBuilder.AppendLine();

            // Display the ticks-per-time-unit fields.
            ticksInfoBuilder.AppendLine(System.String.Format(numberFmt, "Ticks per day", System.TimeSpan.TicksPerDay));
            ticksInfoBuilder.AppendLine(System.String.Format(numberFmt, "Ticks per hour", System.TimeSpan.TicksPerHour));
            ticksInfoBuilder.AppendLine(System.String.Format(numberFmt, "Ticks per minute", System.TimeSpan.TicksPerMinute));
            ticksInfoBuilder.AppendLine(System.String.Format(numberFmt, "Ticks per second", System.TimeSpan.TicksPerSecond));
            ticksInfoBuilder.AppendLine(System.String.Format(numberFmt, "Ticks per millisecond", System.TimeSpan.TicksPerMillisecond));
            ticksInfoBuilder.AppendLine("--------------------------------------------------------------");
            return ticksInfoBuilder.ToString();
        }
    }
}