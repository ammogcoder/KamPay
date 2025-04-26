namespace Kpakam.Interface
{
    public interface IErrorLogger
    {
        void AddMatchIDs(string sourceID, string TargetID);
        void WriteActivity(string Activity);
        void WriteClientLog(string ex);
        void WriteLog(Exception ex);
    }
}