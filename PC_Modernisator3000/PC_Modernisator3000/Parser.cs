using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace PC_Modernisator3000
{
    class Parser
    {
        //Some constants
        const string url = "http://www.citilink.ru/catalog/computers_and_notebooks/";
        public static Dictionary<string, string>areas = new Dictionary<string, string>() {
            {  "hdd/hdd_in/", "hdd" },
            { "parts/cpu/", "cpu" },
            { "parts/motherboards/", "mb" },
            { "parts/videocards/", "gpu" },
            { "parts/memory/", "ram" },
            { "parts/powersupply/", "power" },
            { "parts/odds/", "rom" }
        };
        public static Encoding encode = Encoding.GetEncoding("utf-8");

        //lists of items
        public List<MB> mb = new List<MB>();
        public List<HDD> hdd = new List<HDD>();
        public List<ROM> rom = new List<ROM>();
        public List<GPU> gpu = new List<GPU>();
        public List<POWER> power = new List<POWER>();
        public List<CPU> cpu = new List<CPU>();
        public List<RAM> ram = new List<RAM>();

        /// <summary>
        /// Main class
        /// </summary>
        public Parser()
        {
            //Trying open file with info
            try
            {
                openShopItems();
            }
            catch(Exception e)
            {
                throw new Exception("Ошибка во время парсинга или открытия файла ShopItems.json");
            }
        }

        /// <summary>
        /// Open and parser a file
        /// </summary>
        private void openShopItems()
        {
            string json = System.IO.File.ReadAllText(@"ShopItems.json");
            //Deseralize
            hdd = JsonConvert.DeserializeObject<Dictionary<string, List<HDD>>>(json)["hdd"];
            
            cpu = JsonConvert.DeserializeObject<Dictionary<string, List<CPU>>>(json)["cpu"];
            
            mb = JsonConvert.DeserializeObject<Dictionary<string, List<MB>>>(json)["mb"];
            
            gpu = JsonConvert.DeserializeObject<Dictionary<string, List<GPU>>>(json)["gpu"];
           
            ram = JsonConvert.DeserializeObject<Dictionary<string, List<RAM>>>(json)["ram"];
            
            power = JsonConvert.DeserializeObject<Dictionary<string, List<POWER>>>(json)["power"];

            rom = JsonConvert.DeserializeObject<Dictionary<string, List<ROM>>>(json)["rom"];

        }

        /// <summary>
        /// Generates a file with info from citilink
        /// </summary>
        /// TODO: nake for n pages
        public static void generateFile(int pages)
        {
            //Setting up webparser
            var wClient = new WebClient();
            wClient.Proxy = null;
            wClient.Encoding = encode;
            HtmlDocument html = new HtmlDocument();

            var shop = new Dictionary<string, List<Item>>();

            foreach (var area in areas.Keys)
            {
                //Load items and parse characteristics
                var itemObjects = new List<Item>();
                for (var i = 1; i <= pages; i++)
                {
                    //Open page
                    var page = url + area;
                    html.LoadHtml(wClient.DownloadString(page + @"?available=1&p=" + i));
                    //get table of contents
                    //*Search a tag 
                    var tmp = html.GetElementbyId("subcategoryList").ChildNodes.Where(x => x.Name == "div").Where(x => x.GetAttributeValue("class", "none") == "product_category_list").First();
                    var elements = tmp.FirstChild.ChildNodes.Where(x => x.GetAttributeValue("class", "") == "product_data__gtm-js product_data__pageevents-js");
                    //*Find id's
                    var elements_id = new Dictionary<string, string>();
                    foreach (var elem in elements)
                    {
                        var data_params = elem.GetAttributeValue("data-params", "").Replace("&quot;", "\"");
                        dynamic stuff = JObject.Parse(data_params);
                        elements_id.Add(stuff["id"].Value, stuff["shortName"].Value);
                    }

                    
                    foreach (var id in elements_id.Keys)
                    {
                        //Loading new item
                        html.LoadHtml(wClient.DownloadString(page + id + "/"));
                        //Finding parametrs
                        var tmp2 = html.GetElementbyId("content").ChildNodes[0].ChildNodes.Where(x => x.GetAttributeValue("class", "") == "specification_view product_view").First();
                        var tableOfParams = tmp2.ChildNodes.Where(x => x.GetAttributeValue("class", "") == "product_features").First().ChildNodes;
                        //Creating object and loading params
                        Item item;
                        switch (area)
                        {
                            case "hdd/hdd_in/":
                                item = new HDD();
                                break;
                            case "parts/cpu/":
                                item = new CPU();
                                break;
                            case "parts/motherboards/":
                                item = new MB();
                                break;
                            case "parts/videocards/":
                                item = new GPU();
                                break;
                            case "parts/memory/":
                                item = new RAM();
                                break;
                            case "parts/powersupply/":
                                item = new POWER();
                                break;
                            case "parts/odds/":
                                item = new ROM();
                                break;
                            default:
                                //never be reached
                                item = new HDD();
                                break;
                        }
                        //Adding to category array
                        item.add("Имя", elements_id[id]);
                        item.webAddress = page + id + "/";
                        item.fillItem(tableOfParams);
                        itemObjects.Add(item);
                    }
                }
                //Adding to database
                shop.Add(areas[area], itemObjects);
            }
            //Creating a file
            var json = JsonConvert.SerializeObject(shop);
            System.IO.File.WriteAllText(@"ShopItems.json", json);
        }
        
        /// <summary>
        /// Ищет среди элементов магазина совпадения полей и значения
        /// </summary>
        /// <param name="category">Весь массив</param>
        /// <param name="field">Поле категории</param>
        /// <param name="value">Искомое значение</param>
        /// <returns></returns>
        public static List<Item> find(List<Item> category, string field, string value)
        {
            return category.Where(x => x.get(field) == value).ToList();
        }
    }
}
