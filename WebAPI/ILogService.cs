namespace MiniOglasnikZaBesplatneStvari
{
    public interface ILogService
    {
        void Log(DateTime timestamp, string level, string message);

    }
}
