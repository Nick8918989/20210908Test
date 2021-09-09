using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dapper;
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
            
            //資料庫連線
            string cnStr = "Server=192.168.5.162;Database=Test;User ID=sa;Pwd=24drs;MultipleActiveResultSets=True;Max Pool Size=256;";

            //測試-Dapper連線資料庫實作
            //QueryUser(cnStr);

            //測試-Dapper新增資料
            //InsertUser(cnStr);

            //測試-Dapper更新資料
            //UpdateUser(cnStr);

            //測試-Dapper刪除資料
            //DeleteUser(cnStr);

            //測試-檔案上傳功能
            //SetFile(cnStr).Wait();

            Console.ReadLine();
        }

        public static async Task SetFile(string _cnStr)
        {
            //Ser放檔案的資料夾
            string BASE_PATH = "C:\\Users\\Admin\\Desktop\\date";
            //建立Log實例
            var logHelp = LogHelper.GetInstance(LogProvider.SQL, _cnStr);
            //建立檔案實例
            var fileHelp = FileService.StorageHelper.GetInstance(StorageProvider.NativeFileWithSQL, _cnStr, logHelp, BASE_PATH);

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

        public static void QueryUser(string _cnStr)
        {
            using (var cn = new SqlConnection(_cnStr))
            {
                var userList = cn.Query("SELECT * FROM MVC_DEMO_USER");
                foreach (var user in userList)
                {
                    Console.WriteLine("資料是...{0}", user.ID);
                }
            }
        }

        public static void InsertUser(string _cnStr)
        {
            using (var cn = new SqlConnection(_cnStr))
            {
                string strSql =
                    "INSERT INTO MVC_DEMO_USER(GROUP_ID, NAME, MAIL, PASSWORD, USERNAME) VALUES (@GROUP_ID,@NAME,@MAIL,@PASSWORD,@USERNAME);";
                dynamic[] datas = new[] {
                    new { GROUP_ID = 4, NAME = "上班做蝦皮5", MAIL = "test5@gmail.com", PASSWORD = "005", USERNAME = "沒有5" },
                    new { GROUP_ID = 4, NAME = "上班做蝦皮6", MAIL = "test6@gmail.com", PASSWORD = "006", USERNAME = "沒有6" },
                };
                cn.Execute(strSql, datas);
                Console.WriteLine("資料新增成功");
            }
        }

        public static void UpdateUser(string _cnStr)
        {
            using (var cn = new SqlConnection(_cnStr))
            {
                string strSql =
                    "UPDATE  MVC_DEMO_USER SET NAME = @NAME WHERE ID = @ID;";
                dynamic[] updateData = new[] { 
                    new { ID = 93, NAME = "還是上班做蝦皮1" },
                    new { ID = 94, NAME = "還是上班做蝦皮2" },
                };
                cn.Execute(strSql, updateData);
                Console.WriteLine("資料更新成功");
            }
        }

        public static void DeleteUser(string _cnStr)
        {
            using (var cn = new SqlConnection(_cnStr))
            {
                string strSql = "DELETE  MVC_DEMO_USER WHERE MAIL LIKE @MAILSTRING";
                dynamic[] updateData = new[] {
                    new { MAILSTRING = "%" + "test" + "%" },
                };
                cn.Execute(strSql, updateData);
                Console.WriteLine("資料刪除成功");
            }
        }
    }
}
