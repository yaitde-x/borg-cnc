using System.Threading.Tasks;

namespace Borg.Machine
{
    public interface IRS274Controller
    {
        Task<RS274State> GetState();
        Task<GrblSettings> GetSettings();

        Task<RS274Job> Run(RS274Instruction instruction);

        Task<RS274EngineStatus> GetEngineStatus();
        Task<RS274Job> GetJob(string jobId);
        Task<RS274Job> Run(string gCodeBlock);
        Task<RS274State> File(string fileName);
    }
}