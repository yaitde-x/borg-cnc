namespace Borg.Design.Domain
{
    public class Design
    {
        public string Name { get; set; }
        public string[] Tags { get; set; }

        public string[] Keywords { get; set; }

        public DesignElement[] Elements { get; set; }
    }
}