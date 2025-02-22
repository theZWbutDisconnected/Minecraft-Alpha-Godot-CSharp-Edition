using System.Diagnostics;

public class Timer
{
    private static readonly long NS_PER_SECOND = 1000000000L;
    private static readonly long MAX_NS_PER_UPDATE = 1000000000L;
    private static readonly int MAX_TICKS_PER_UPDATE = 100;
    
    private float ticksPerSecond;
    private long lastTime;
    public int ticks;
    public float a;
    public float timeScale = 1.0f;
    public float fps;
    public float passedTime;
    
    private readonly double nanosecondsPerTick;

    public Timer(float ticksPerSecond)
    {
        this.ticksPerSecond = ticksPerSecond;
        nanosecondsPerTick = 1e9 / Stopwatch.Frequency;
        lastTime = Stopwatch.GetTimestamp();
    }

    public void advanceTime()
    {
        long currentTime = Stopwatch.GetTimestamp();
        long elapsedTicks = currentTime - lastTime;
        lastTime = currentTime;

        long passedNs = (long)(elapsedTicks * nanosecondsPerTick);
        
        if (passedNs < 0) passedNs = 0;
        if (passedNs > MAX_NS_PER_UPDATE) passedNs = MAX_NS_PER_UPDATE;

        fps = passedNs > 0 
            ? (float)(NS_PER_SECOND / (double)passedNs) 
            : 0;

        passedTime += (float)passedNs * timeScale * ticksPerSecond / 1e9f;
        
        ticks = (int)passedTime;
        if (ticks > MAX_TICKS_PER_UPDATE)
            ticks = MAX_TICKS_PER_UPDATE;
        
        passedTime -= ticks;
        a = passedTime;
    }
}