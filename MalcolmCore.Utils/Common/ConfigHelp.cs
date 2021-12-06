using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MalcolmCore.Utils.Common
{
    public static class ConfigHelp
    {
        /// <summary>
        /// 读取Json类型的配置文件
        /// </summary>
        /// <param name="key">键名</param>
        /// <param name="FilePath">文件路径，默认为：appsettings.json</param>
        /// <returns></returns>
        public static string GetString(string key, string FilePath = "appsettings.json")
        {
            var configurationBuilder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile(FilePath, optional: true, reloadOnChange: true);
            var configuration = configurationBuilder.Build();
            return configuration[key];
        }


        /// <summary>
        /// 读取Xml类型的配置文件
        /// </summary>
        /// <param name="key">键名</param>
        /// <param name="FilePath">文件路径，默认为：myXml.json</param>
        /// <returns></returns>
        public static string GetXmlString(string key, string FilePath = "myXml.json")
        {
            var configurationBuilder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddXmlFile(FilePath, optional: true, reloadOnChange: true);
            var configuration = configurationBuilder.Build();
            return configuration[key];
        }
    }
}
