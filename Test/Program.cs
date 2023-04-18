using System;
using System.Net.Http;
using NewLife.Http;
using NewLife.Log;

namespace Test
{
    class Program
    {
        static void Main(String[] args)
        {
            XTrace.UseConsole();

            try
            {
                Test1();
            }
            catch (Exception ex)
            {
                XTrace.WriteException(ex);
            }

            Console.WriteLine("OK!");
            Console.ReadKey();
        }

        static async void Test1()
        {
            var url = "https://cdn.nlark.com/yuque/0/2022/png/1144030/1668661752961-83340534-f73c-4cf0-9938-3868bcfa4acb.png";
            var client = new HttpClient();
            client.SetUserAgent();

            //var html = await client.GetStringAsync(url);
            //XTrace.WriteLine(html);

            var rs = await client.GetAsync(url);
            XTrace.WriteLine(rs.Content.Headers.ContentType + "");
            var html = await rs.Content.ReadAsStringAsync();
            //XTrace.WriteLine(html);

            await client.DownloadFileAsync(url, "aa.png");
        }
    }
}