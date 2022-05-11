using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using slnFarmAttractions.Models;
using PagedList;

namespace slnFarmAttractions.Controllers
{
    public class HomeController : Controller
    {
        int pageSize = 10; // 決定一頁要呈現多少筆資料

        // GET: Home
        public async Task<ActionResult> Index(int page = 1)
        {
            int currentPage = page < 1 ? 1 : page;

            // 下載全國休閒農業區旅遊資訊JSON開放資料
            string url = "https://data.coa.gov.tw/Service/OpenData/ODwsv/ODwsvAttractions.aspx";
            HttpClient httpClient = new HttpClient();
            httpClient.MaxResponseContentBufferSize = Int32.MaxValue;

            // 將 GET 要求傳送至指定的 URI，並透過非同步作業，以字串形式傳回回應內容。
            var response = await httpClient.GetStringAsync(url);

            string path = $"{Server.MapPath("JSON").Replace("Home\\", "")}\\ODwsvAttraction.json"; // C:\Users\user\Desktop\prjFarmAttractions\slnFarmAttractions\JSON\ODwsvAttraction.json
            //FileInfo fileInfo = new FileInfo(path); // 提供建立、複製、刪除、移動和開啟檔案的屬性和執行個體方法，並協助建立 FileStream 物件。

            StreamWriter streamWriter = new StreamWriter(path, false, System.Text.Encoding.UTF8);
            streamWriter.WriteLine(response); // 檔案此時才下載到JSON資料夾
            streamWriter.Close();

            // 將全國休閒農業區旅遊資訊JSON資料反序列化成FarmAttractions陣列物件
            StreamReader streamReader = new StreamReader(path);
            FarmAttractions[] farmAttractions = JsonConvert.DeserializeObject<FarmAttractions[]>(streamReader.ReadToEnd());
            streamReader.Close();

            // 將FarmAttractions陣列物件先依City縣市遞增排序，接著再依Town鄉鎮遞增排序
            // 將排序後的FarmAttractions陣列物件傳送至View檢視頁面
            var attractions = farmAttractions.OrderBy(x => x.City).ThenBy(x => x.Town).ToList();
            var result = attractions.ToPagedList(currentPage, pageSize);
            return View(result);
        }
    }
}