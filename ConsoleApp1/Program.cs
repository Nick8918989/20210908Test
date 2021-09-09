using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FileService;
using FileService.Models;
using LogService;
using LogService.Models;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start ...");
            Console.ReadLine();
            //SetFile().Wait();
        }

        public static async Task SetFile()
        {
            //資料庫連線
            string CONNECTION_STRING = "Server=192.168.5.162;Database=HOME_CARE_TEST;User ID=sa;Pwd=24drs;MultipleActiveResultSets=True;Max Pool Size=256;";
            //Ser放檔案的資料夾
            string BASE_PATH = "C:\\Users\\Admin\\Desktop\\date";
            //建立Log實例
            var logHelp = LogHelper.GetInstance(LogProvider.SQL, CONNECTION_STRING);
            //建立檔案實例
            var fileHelp = FileService.StorageHelper.GetInstance(StorageProvider.NativeFileWithSQL, CONNECTION_STRING, logHelp, BASE_PATH);
            
            //測試資料來源
            string base64 = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAUAAAAFCAYAAACNbyblAAAAHElEQVQI12P4//8/w38GIAXDIBKE0DHxgljNBAAO9TXL0Y4OHwAAAABJRU5ErkJggg==";
            //正則表達式-抓取Base64碼
            string Rule = @"data:image/(?<type>[^;]+)[^,]+,(?<data>[A-Za-z0-9\+/=]+)";
            //比對資料
            MatchCollection base64Img = Regex.Matches(base64, Rule, RegexOptions.IgnoreCase);
            foreach (Match m in base64Img)
            {
                string type = m.Groups["type"].Value;
                string value = m.Groups["data"].Value;
                byte[] bytes = Convert.FromBase64String(value);
                MemoryStream stream = new MemoryStream(bytes);
                StorageInfo saveInfo = await fileHelp.SaveFileAsync("1",
                                                                    "2",
                                                                    "3",
                                                                    stream,
                                                                    "image/" + type).ConfigureAwait(true);

                if (saveInfo.Status == StorageStatus.Success)
                {
                    Console.WriteLine("檔案上傳完成...");
                }
            }
        }
    }
}
