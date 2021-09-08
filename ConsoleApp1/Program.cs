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
            List<DateTime> dateTimeList = new List<DateTime>();
            DateTime _startDate = DateTime.Now;
            DateTime _endDate = new DateTime(2099, 5, 31);

            SetFile().Wait();
            //while (_startDate.AddDays(10) <= _endDate)
            //{
            //    _startDate = _startDate.AddDays(10);
            //    dateTimeList.Add(_startDate);
            //}
            //if (dateTimeList != null && dateTimeList.Count > 0)
            //{
            //    foreach (DateTime d in dateTimeList)
            //    {
            //        Console.WriteLine(d);
            //    }
            //}

            //DateTime thisMonthDate = new DateTime();
            //DateTime? _oneceEveryYearsDate = new DateTime(2022, 2, 1);
            ////二月專用的日
            //int setFebruaryDay = 35;
            //if(_oneceEveryYearsDate.HasValue)
            //{
            //    if (_oneceEveryYearsDate.Value.Date.Month == 2)
            //    {
            //        thisMonthDate = GetDateTime(_oneceEveryYearsDate.Value, setFebruaryDay);
            //        dateTimeList.Add(thisMonthDate);
            //        while (thisMonthDate.AddMonths(12) <= _endDate)
            //        {
            //            thisMonthDate = thisMonthDate.AddMonths(12);
            //            _startDate = GetDateTime(thisMonthDate, setFebruaryDay);
            //            dateTimeList.Add(_startDate);
            //        }
            //    }
            //    else
            //    {
            //        thisMonthDate = GetDateTime(_oneceEveryYearsDate.Value, _oneceEveryYearsDate.Value.Day);
            //        dateTimeList.Add(thisMonthDate);
            //        while (thisMonthDate.AddMonths(12) <= _endDate)
            //        {
            //            thisMonthDate = thisMonthDate.AddMonths(12);
            //            _startDate = GetDateTime(thisMonthDate, _oneceEveryYearsDate.Value.Day);
            //            dateTimeList.Add(_startDate);
            //        }
            //    }
            //}
        }

        public static async Task SetFile()
        {
            //資料庫連線
            string CONNECTION_STRING = "Server=192.168.5.162;Database=HOME_CARE_TEST;User ID=sa;Pwd=24drs;MultipleActiveResultSets=True;Max Pool Size=256;";
            //Ser放檔案的資料夾
            string BASE_PATH = "\\\\192.168.5.162\\Temp\\HomeCareFile";
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

        public static DateTime GetDateTime(DateTime _dateTime, int _dayReapet)
        {
            DateTime returnDateTime = new DateTime();
            //只有30天的月份
            List<int> thirtyMoths = new List<int>() { 4, 6, 9, 11 };

            if (_dateTime.Month != 2)
            {
                if (_dayReapet == 31)
                {
                    if (thirtyMoths.Contains(_dateTime.Month))
                    {
                        returnDateTime = new DateTime(_dateTime.Year, _dateTime.Month, 30);
                    }
                    else
                    {
                        returnDateTime = new DateTime(_dateTime.Year, _dateTime.Month, 31);
                    }
                }
                else
                {
                    returnDateTime = new DateTime(_dateTime.Year, _dateTime.Month, _dayReapet);
                }
            }
            else if (_dateTime.Month == 2)
            {
                if (_dayReapet > 28)
                {
                    int year = _dateTime.Year;
                    if (year % 400 == 0 || (year % 4 == 0 && year % 100 != 0))
                    {
                        //是閏年只有29天
                        returnDateTime = new DateTime(_dateTime.Year, _dateTime.Month, 29);
                    }
                    else
                    {
                        //不是閏年只有28天
                        returnDateTime = new DateTime(_dateTime.Year, _dateTime.Month, 28);
                    }
                }
                else
                {
                    returnDateTime = new DateTime(_dateTime.Year, _dateTime.Month, _dayReapet);
                }
            }
            return returnDateTime;
        }
    }
}
