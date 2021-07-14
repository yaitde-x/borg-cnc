using Borg.Machine;

namespace Borg.Query
{
    public interface IQueryContextProvider {
        IQueryContext GetContext(RS274Query query, RS274InstructionSet instructionSet);
    }

}