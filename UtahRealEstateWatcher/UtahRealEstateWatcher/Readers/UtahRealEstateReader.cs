using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using UtahRealEstateWatcher.Models;

namespace UtahRealEstateWatcher.Readers
{
    public class UtahRealEstateReader
    {

        private const string url = "http://www.utahrealestate.com";


        public SearchCriteria Criteria { get; set; }

        public List<string> GetListings()
        {
            var listings = new List<string>();

            var pagination = new Pagination();

            var pageList = new List<string>();

            do {
                
                pageList = GetListings(pagination).ToList();

                Thread.Sleep(100);

                pagination.Page++;

                listings.AddRange(pageList);

            } while (pageList.Count > 0) ;

                return listings;
        }

        private IEnumerable<string> GetListings(Pagination pagination)
        {
            var content = GetContent(pagination);

            if(content == null)
            {
                yield break;
            }

            dynamic json = JsonConvert.DeserializeObject(content);
            
            foreach (dynamic listing in json.listing_data)
            {
                yield return listing.listno;
            }
        }

        private string GetContent(Pagination pagination)
        {
            var client = new RestClient(url);

            var resource = string.Format("search/chained.update/count/true/criteria/false/pg/{0}/limit/{1}", pagination.Page, pagination.Limit);

            var request = new RestRequest(resource, Method.POST);
            request.AddHeader("Accept", "application/json, text/javascript, */*; q=0.01");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
            
            var body = string.Format("param=city&value={0}&param_reset=county_code,o_county_code,city,o_city,zip,o_zip,geometry,o_geometry&chain=saveLocation,criteriaAndCountAction,mapInlineResultsAction&tx=true&all=1&accuracy=4&geocoded={0}&state=UT&box=%257B%2522north%2522%253A40.471736%252C%2522south%2522%253A40.356205%252C%2522east%2522%253A-111.81881799999996%252C%2522west%2522%253A-111.91877299999999%257D&htype=city&lat=40.3916172&lng=-111.85076620000001&selected_listno=&type=1&geolocation={0}&listprice1=&listprice2=&tot_bed1=&tot_bath1=&proptype=&style=&o_style=4&tot_sqf1=&dim_acres1=&yearblt1=&cap_garage1=&opens=&accessibility=&o_accessibility=32&htype=city&hval={0}&loc={0}&accr=4&op=4&advanced_search=0&param_reset=housenum,dir_pre,street,streettype,dir_post,city,county_code,zip,area,subdivision,quadrant,unitnbr1,unitnbr2,geometry,coord_ns1,coord_ns2,coord_ew1,coord_ew2,housenum,o_dir_pre,o_street,o_streettype,o_dir_post,o_city,o_county_code,o_zip,o_area,o_subdivision,o_quadrant,o_unitnbr1,o_unitnbr2,o_geometry,o_coord_ns1,o_coord_ns2,o_coord_ew1,o_coord_ew2", Criteria.City);

            request.AddParameter("application/x-www-form-urlencoded", body, ParameterType.RequestBody);

            var response = client.Execute(request);

            if(response.ResponseStatus != ResponseStatus.Completed && response.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            return response.Content;
        }
    }
}
