using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace MalcolmCore.Utils
{
    /// <summary>
    /// redis链接帮助类 
    /// 基于程序集：StackExchange.Redis
    /// </summary>
    public class SERedisHelp
    {

        private string _connectionString; //连接字符串
        private int _defaultDB; //默认数据库
        private readonly ConnectionMultiplexer connectionMultiplexer;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="defaultDB">默认使用Redis的0库</param>
        public SERedisHelp(string connectionString, int defaultDB = 0)
        {
            _connectionString = connectionString;
            _defaultDB = defaultDB;
            connectionMultiplexer = ConnectionMultiplexer.Connect(_connectionString);
        }

        /// <summary>
        /// 获取数据库
        /// </summary>
        /// <returns></returns>
        public IDatabase GetDatabase()
        {
            return connectionMultiplexer.GetDatabase(_defaultDB);
        }
    }
}
