using System.Collections.Generic;
using System.Threading.Tasks;
using Borg.Machine;
using System.Linq;
using System;

namespace Borg.Query
{
    public class WorkEnvelopeQueryHandler : I274QueryHandler
    {
        public Task<I274Result> ExecuteQuery(IQueryContext context, I274Query query)
        {
            return Task.Run(() =>
            {
                var bounds = context.Bounds();
                var boundsCalculator = new ProgramBoundsCalculator();

                var result = new RS274Result(new Dictionary<string, object>()
                {
                    {"bounds", bounds},
                    {"programBounds", boundsCalculator.ProgramBounds(context.InstructionSet)}
                });
                
                return result as I274Result;
            });
        }

    }

}