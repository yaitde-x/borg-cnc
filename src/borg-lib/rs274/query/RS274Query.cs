namespace Borg.Query
{
    public class RS274QueryBuilder
    {

        private RS274Query _query;
        public RS274QueryBuilder(string query)
        {
            _query = new RS274Query() { RawQuery = query };
        }

        public RS274Query Build()
        {
            return _query;
        }
    
    }
    public class RS274Query : I274Query
    {
        public string RawQuery { get; internal set; }
    }

}