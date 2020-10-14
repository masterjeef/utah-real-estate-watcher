using HtmlAgilityPack;
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

        private const int getNextPageDelay = 200;

        private const string host = "www.utahrealestate.com";

        private const string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.121 Safari/537.36";

        private const string language = "en-US,en;q=0.9";

        private readonly Uri _uri = new Uri(string.Format("https://{0}", host));

        private readonly RestClient _restClient;

        private SearchCriteria _criteria;

        public UtahRealEstateReader()
        {
            _restClient = new RestClient(_uri);
            _restClient.CookieContainer = new CookieContainer();
            _restClient.UserAgent = userAgent;
            _restClient.AddDefaultHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            _restClient.AddDefaultHeader("Accept-Language", language);

            var request = new RestRequest("index/public.index", Method.GET);

            var response = _restClient.Execute(request);

            var browserSession = new Cookie("ureBrowserSession", DateTime.Now.Ticks.ToString(), "/", host);
            _restClient.CookieContainer.Add(browserSession);
        }

        public SearchCriteria Criteria
        {
            get
            {
                return _criteria;
            }
            set
            {
                var postSuccess = PostSearchCriteria(value);

                if (postSuccess)
                {
                    _criteria = value;
                }
            }
        }

        private bool PostSearchCriteria(SearchCriteria criteria)
        {
            var request = new RestRequest("search/save.search.criteria", Method.POST);
            request.AddHeader("Accept", "*/*");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");

            var body = string.Format("type=1&" +
                "geolocation={0}&" +
                "accuracy=4&" +
                "geocoded={0}&" +
                "state=UT&" +
                "box=&" +
                "htype=city&" +
                "lat=&" +
                "lng=&" +
                "selected_listno=&" +
                "listnoSearch=&" +
                "proptype=&" +
                "listprice1={1}&" +
                "listprice2={2}&" +
                "tot_bed1=&" +
                "tot_bath1=&" +
                "tot_sqf1=",
                criteria.City, criteria.MinPrice, criteria.MaxPrice);

            request.AddParameter("application/x-www-form-urlencoded", body, ParameterType.RequestBody);

            var response = _restClient.Execute(request);

            return response.ResponseStatus == ResponseStatus.Completed && response.StatusCode == HttpStatusCode.OK;
        }

        public List<UreListing> GetListings()
        {
            var listings = new List<UreListing>();
            var pagination = new Pagination();
            var pageList = new List<UreListing>();

            do
            {

                pageList = GetListingsFromPage(pagination).ToList();

                Thread.Sleep(getNextPageDelay);

                pagination.Page++;

                listings.AddRange(pageList);

            } while (pageList.Count > 0);

            return listings;
        }

        private IEnumerable<UreListing> GetListingsFromPage(Pagination pagination)
        {
            var content = GetContent(pagination);

            if (content == null)
            {
                yield break;
            }

            dynamic json = JsonConvert.DeserializeObject(content);

            var document = new HtmlDocument();
            document.LoadHtml(json.html.Value);

            foreach (dynamic listing in json.listing_data)
            {
                var mls = listing.listno;
                var url = string.Format("http://{0}/{1}", host, mls);
                var seeMore = string.Format("<div class=\"see-more\"><a href=\"{0}\" class=\"btn\" target=\"_blank\">see more</a></div>", url);
                var node = document.DocumentNode.SelectSingleNode(string.Format("//li[@id='mls-inline-{0}']", mls));

                var ureListing = new UreListing
                {
                    Mls = mls,
                    Url = url,
                    City = Criteria.City,
                    Html = string.Format("<div class=\"ure-listing\">\n{0}\n{1}\n</div>", node.InnerHtml, seeMore)
                };

                yield return ureListing;
            }
        }

        private string GetContent(Pagination pagination)
        {
            var resource = $"search/chained.update/param_reset/county_code,o_county_code,city,o_city,zip,o_zip,geometry,o_geometry/count/false/criteria/false/pg/{pagination.Page}/limit/{pagination.Limit}/dh/500";
            var request = new RestRequest(resource, Method.POST);
            request.AddHeader("Accept", "application/json, text/javascript, */*; q=0.01");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");

            var body = "param=city&" +
                $"value={Criteria.City}&" +
                "chain=saveLocation,criteriaAndCountAction,mapInlineResultsAction&" +
                "tx=true&" +
                "all=1&" +
                "accuracy=100&" +
                $"geocoded={Criteria.City.Replace(" ", " + ")}&" +
                "state=UT&" +
                "box=%257B%2522east%2522%253A-111.8952693%252C%2522south%2522%253A40.536858%252C%2522north%2522%253A40.5821002%252C%2522west%2522%253A-112.0707875%257D&" +
                "htype=city&" +
                "lat=40.5621704&" +
                "lng=-111.929658&" +
                "selected_listno=&" +
                "type=1&" +
                $"geolocation={Criteria.City.Replace(" ", "+")}%2C+UT&" +
                $"listprice1={Criteria.MinPrice}&" +
                $"listprice2={Criteria.MaxPrice}&" +
                "tot_bed1=&" +
                "tot_bath1=&" +
                "stat=1&" +
                "stat=7&" +
                "status=1%2C7&" +
                "opens=&" +
                "o_env_certification=32&" +
                "proptype=&" +
                "style=&" +
                "o_style=4&" +
                "tot_sqf1=&" +
                "dim_acres1=&" +
                "yearblt1=&" +
                "cap_garage1=&" +
                "o_has_solar=1&" +
                "o_seniorcommunity=1&" +
                "o_has_hoa=1&" +
                "o_accessibility=32&" +
                "htype=city&" +
                $"hval={Criteria.City}&" +
                $"loc={Criteria.City.Replace(" ", "+")},%20UT&" +
                "accr=100&" +
                "advanced_search=0&" +
                "param_reset=housenum,dir_pre,street,streettype,dir_post,city,county_code,zip,area,subdivision,quadrant,unitnbr1,unitnbr2,geometry,coord_ns1,coord_ns2,coord_ew1,coord_ew2,housenum,o_dir_pre,o_street,o_streettype,o_dir_post,o_city,o_county_code,o_zip,o_area,o_subdivision,o_quadrant,o_unitnbr1,o_unitnbr2,o_geometry,o_coord_ns1,o_coord_ns2,o_coord_ew1,o_coord_ew2";
            
            request.AddParameter("application/x-www-form-urlencoded", body, ParameterType.RequestBody);

            var response = _restClient.Execute(request);

            if (response.ResponseStatus != ResponseStatus.Completed && response.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            return response.Content;
        }
    }
}
