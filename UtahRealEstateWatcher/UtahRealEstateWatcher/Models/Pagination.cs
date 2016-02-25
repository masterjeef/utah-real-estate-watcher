namespace UtahRealEstateWatcher.Models
{
    public class Pagination
    {
        public Pagination()
        {
            Page = 1;
            Limit = 50;
        }

        public int Page { get; set; }

        public int Limit { get; set; }
    }
}
