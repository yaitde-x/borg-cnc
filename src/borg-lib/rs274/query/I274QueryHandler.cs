using System.Threading.Tasks;

namespace Borg.Query
{
    public interface I274QueryHandler
    {
        Task<I274Result> ExecuteQuery(IQueryContext context, I274Query query);

    }

}