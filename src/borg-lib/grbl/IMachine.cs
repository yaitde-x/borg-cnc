using System.Threading.Tasks;
using Yuni.CoordinateSpace;

namespace Borg.Machine
{
    public interface IMachine
    {
        Task<CoordinateBounds> Bounds();

        Task<RS274State> UnLock();
        Task<RS274State> Lock();
        Task<RS274Job> Home();
        Task<RS274State> GetState();
        Task<RS274EngineStatus> GetEngineStatus();
        Task<GrblSettings> GetSettings();

        Task<RS274Job> GetJob(string jobId);
        Task<RS274Job> ExecuteBlock(string gCodeBlock);
        Task<RS274State> ExecuteFile(string fileName);
        Task<RS274State> Stop();
    }
}