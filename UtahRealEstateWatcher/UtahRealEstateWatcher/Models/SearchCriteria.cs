namespace UtahRealEstateWatcher.Models
{
    public class SearchCriteria
    {
        public SearchCriteria()
        {
            MinPrice = 0;
            MaxPrice = int.MaxValue;
        }

        public string City { get; set; }

        public int MinPrice { get; set; }

        public int MaxPrice { get; set; }
    }
}
