namespace UtahRealEstateWatcher.Models
{
    public class UreListing
    {
        public string Mls { get; set; }

        public string Html { get; set; }

        public string Url { get; set; }

        public string City { get; set; }

        public int Price { get; set; }

        public bool Equals(UreListing other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return string.Equals(Mls, other.Mls);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((UreListing)obj);
        }

        public override int GetHashCode()
        {
            return Mls.GetHashCode();
        }
    }
}
