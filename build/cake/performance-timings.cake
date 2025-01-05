#addin nuget:?package=ZString&version=2.6.0
#addin nuget:?package=ZString&version=2.6.0

using Cysharp.Text;

System.Diagnostics.Stopwatch stopwatch;
List<(DateTime timestamp, string task, TimeSpan duration)> TimingDataStopwatch;
List<(DateTime timestamp, string task, TimeSpan duration)> TimingDataCake;

Setup
    (
        context =>
        {
            // Executed BEFORE the first task.
            stopwatch = new System.Diagnostics.Stopwatch();
            TimingDataStopwatch = new List<(DateTime timestamp, string task, TimeSpan duration)>();
            TimingDataCake      = new List<(DateTime timestamp, string task, TimeSpan duration)>();

            return;
        }
    );

Teardown
    (
        context =>
        {
            // Executed AFTER the last task.

            using(var sb1 = ZString.CreateStringBuilder())
            using(var sb2 = ZString.CreateStringBuilder())
            {
                string line_1_1 = ZString.Format
                                            (
                                                "{0,25},{1,20},{2,40},{3,25}",
                                                "#TimingDataStopwatch",
                                                "data.timestamp",
                                                "data.task",
                                                "data.duration"
                                            );
                sb1.AppendLine(line_1_1);
                foreach(var data in TimingDataStopwatch)
                {                
                    string line = ZString.Format
                                            (
                                                "{0,25},{1,20},{2,40},{3,25}",
                                                "#TimingDataStopwatch",
                                                data.timestamp.ToString("yyyyMMdd-HHmmss.FF"),
                                                data.task,
                                                data.duration
                                            );
                    
                    Information(line);
                    sb1.AppendLine(line);
                }

                string line_2_1 = ZString.Format
                                            (
                                                "{0,25},{1,20},{2,40},{3,25}",
                                                "#TimingDataCake",
                                                "data.timestamp",
                                                "data.task",
                                                "data.duration"
                                            );
                sb2.AppendLine(line_2_1);
                foreach(var data in TimingDataCake)
                {
                   string line = ZString.Format
                                            (
                                                "{0,25},{1,20},{2,40},{3,25}",
                                                "#TimingDataCake",
                                                data.timestamp.ToString("yyyyMMdd-HHmmss.FF"),
                                                data.task,
                                                data.duration
                                            );
 
                    Information(line);
                    sb2.AppendLine(line);
                }


                EnsureDirectoryExists("./output/timing-data/");
                string timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
                string filename = context.GetCallerInfo().SourceFilePath.GetFilename().ToString();

                System.IO.File.WriteAllText
                                    (
                                        $"./output/timing-data/{filename}.stopwatch.{timestamp}.csv", 
                                        sb1.ToString()
                                    );
                System.IO.File.WriteAllText
                                    (
                                        $"./output/timing-data/{filename}.cake-timer.{timestamp}.csv", 
                                        sb2.ToString()
                                    );
                            
                EnsureDirectoryExists($"./data/timings/{timestamp}");
                System.IO.File.WriteAllText
                                    (
                                        $"./data/timings/{timestamp}/{filename}.stopwatch.csv", 
                                        sb1.ToString()
                                    );
                System.IO.File.WriteAllText
                                    (
                                        $"./data/timings/{timestamp}/{filename}.cake-timer.csv", 
                                        sb2.ToString()
                                    );
            }

            return;
        }
    );

TaskSetup
    (
        context =>
        {
            // Executed BEFORE the each task.
            stopwatch.Start();

            return;
        }
    );

TaskTeardown
    (
        context =>
        {
            stopwatch.Stop();
            TimingDataStopwatch.Add( (DateTime.Now, context.Task.Name, stopwatch.Elapsed) );
            TimingDataCake.Add( (DateTime.Now, context.Task.Name, context.Duration) );
        }
    );

