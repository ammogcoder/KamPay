using System.Data;

namespace Kpakam.Interface
{
    public interface IdataAccess
    {
        DataTable GetCurrentIntegrator(string IntKey);
        int ModifyProcess(Guid ProcID, Guid IntegratorID, DateTime datestamp, string Inflow, string Outflow);
    }
}