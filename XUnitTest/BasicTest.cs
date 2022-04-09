using System;
using System.IO;
using NewLife;
using NewLife.YuQue;

namespace XUnitTest
{
    public class BasicTest
    {
        public static YuqueClient CreateClient()
        {
            var client = new YuqueClient
            {
                Token = Environment.GetEnvironmentVariable("yuque_token")
            };
            if (client.Token.IsNullOrEmpty())
            {
                var file = @"config\yuque.config";
                if (File.Exists(file)) client.Token = File.ReadAllText(file.GetFullPath())?.Trim();
                if (client.Token.IsNullOrEmpty())
                {
                    file.GetFullPath().EnsureDirectory(true);
                    File.WriteAllText(file.GetFullPath(), "");
                }
            }

            return client;
        }
    }
}