// See https://aka.ms/new-console-template for more information
using SnowflakeID;
using System.Diagnostics;




//performance test
while (true)
{
    var stopwatch = new Stopwatch();
    stopwatch.Start();

    long counter = 0;
    while (true)
    {
        Snowflake snow = SnowflakeGuidGenerator.GetSnowflake();

        Guid guid = snow.Guid;

        snow.ToString();

        //var depois = snow.FromGuid(guid)
        Snowflake depois = new Snowflake(guid);

        counter++;
        if (stopwatch.ElapsedMilliseconds >= 1000)
        {
            Console.WriteLine($"{guid} - Iterations: {counter}");
            //Console.WriteLine($"{antes} - {guid} - {depois} - Iterations: {counter}");
            counter = 0;
            stopwatch.Restart();
        }
    }
}


