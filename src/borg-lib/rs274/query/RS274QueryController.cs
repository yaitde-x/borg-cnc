using System.Collections.Concurrent;
using System.Threading.Tasks;
using Borg.Machine;

namespace Borg.Query
{

    public class RS274QueryController
    {
        private readonly I274QueryResolver _resolver;
        private readonly IQueryContextProvider _contextProvider;

        public  RS274QueryController(I274QueryResolver resolver, IQueryContextProvider contextProvider)
        {
            _resolver = resolver;
            _contextProvider = contextProvider;
        }
        public Task<I274Result> Query(string query, RS274InstructionSet instructionSet)
        {
            var handler = _resolver.GetHandler(query);

            var queryModel = new RS274Query()
            {
                RawQuery = query
            };

            var context = _contextProvider.GetContext(queryModel, instructionSet);
            var result = handler.ExecuteQuery(context, queryModel);
            return result;
        }
    }

}