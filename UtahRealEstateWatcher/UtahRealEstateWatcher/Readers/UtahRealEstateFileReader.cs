using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UtahRealEstateWatcher.Models;

namespace UtahRealEstateWatcher.Readers
{
    public class UtahRealEstateFileReader
    {
        public string FileName { get; set; }

        public UtahRealEstateFileReader(string fileName)
        {
            FileName = fileName;
        }

        public List<UreListing> GetListings()
        {
            var listings = new List<UreListing>();

            if (File.Exists(FileName))
            {
                var jsonListings = File.ReadAllText(FileName);

                listings = JsonConvert.DeserializeObject<List<UreListing>>(jsonListings);
            }

            return listings;
        }

    }
}
