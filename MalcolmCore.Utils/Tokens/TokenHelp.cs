using MalcolmCore.Utils.Common;
using MalcolmCore.Utils.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Newtonsoft.Json;

namespace MalcolmCore.Utils.Tokens
{
    public class TokenHelp
    {
        private static List<RefreshToken> rTokenList = new List<RefreshToken>();
        public static Token GetToken(string userAccount, string userPwd)
        {
            string AccessTokenKey = ConfigHelp.GetString("AccessTokenKey");
            string RefreshTokenKey = ConfigHelp.GetString("RefreshTokenKey");

            string userId = Guid.NewGuid().ToString();

            //2. 生成accessToken
            //过期时间(下面表示签名后 5分钟过期，这里设置20s为了演示)
            double exp = (DateTime.UtcNow.AddMinutes(5) - new DateTime(1970, 1, 1)).TotalSeconds;
            var payload = new Dictionary<string, object>
                     {
                   { "userId", userId },
                   { "userAccount", userAccount },
                   { "exp",exp }
                };
            var accessToken = JWTHelp.JWTJiaM(payload, AccessTokenKey);

            //3.生成refreshToken
            //过期时间(可以不设置，下面表示 2天过期)
            var expireTime = DateTime.Now.AddDays(2);
            double exp2 = (expireTime - new DateTime(1970, 1, 1)).TotalSeconds;
            var payload2 = new Dictionary<string, object>
                {
                    { "userId", userId },
                    { "userAccount", userAccount },
                    { "exp",exp2 }
                };
            var refreshToken = JWTHelp.JWTJiaM(payload2, RefreshTokenKey);

            //4.将生成refreshToken的原始信息存到数据库/Redis中 （这里暂时存到一个全局变量中）
            //先查询有没有，有则更新，没有则添加
            var RefreshTokenItem = rTokenList.Where(u => u.userId == userId).FirstOrDefault();
            if (RefreshTokenItem == null)
            {
                RefreshToken rItem = new RefreshToken()
                {
                    id = Guid.NewGuid().ToString("N"),
                    userId = userId,
                    expire = expireTime,
                    Token = refreshToken
                };
                rTokenList.Add(rItem);
            }
            else
            {
                RefreshTokenItem.Token = refreshToken;
                RefreshTokenItem.expire = expireTime;   //要和前面生成的过期时间相匹配
            }

            return new Token
            {
                accessToken = accessToken,
                refreshToken = refreshToken
            };
        }

        public static Token UpdateAccessToken(string accessToken, string refreshToken)
        {
            string AccessTokenKey = ConfigHelp.GetString("AccessTokenKey");
            string RefreshTokenKey = ConfigHelp.GetString("RefreshTokenKey");

            //1.先通过纯代码校验refreshToken的物理合法性
            var result = JWTHelp.JWTJieM(refreshToken, ConfigHelp.GetString("RefreshTokenKey"));
            if (result == "expired" || result == "invalid" || result == "error")
            {
                return new Token() { };
            }

            //2.从accessToken中解析出来userId等其它数据(即使accessToken已经过期，依旧可以解析出来)
            JwtData myJwtData = JsonConvert.DeserializeObject<JwtData>(Base64UrlDecode(accessToken.Split('.')[1]));

            //3. 拿着userId、refreshToken、当前时间去RefreshToken表中查数据
            var rTokenItem = rTokenList.Where(u => u.userId == myJwtData.userId && u.Token == refreshToken && u.expire > DateTime.Now).FirstOrDefault();
            if (rTokenItem == null)
            {
                return new Token() { };
            }

            //4.重新生成 accessToken和refreshToken，并写入RefreshToken表
            //4.1. 生成accessToken
            //过期时间(下面表示签名后 5分钟过期，这里设置20s为了演示)
            double exp = (DateTime.UtcNow.AddSeconds(20) - new DateTime(1970, 1, 1)).TotalSeconds;
            var payload = new Dictionary<string, object>
                     {
                          {"userId", myJwtData.userId },
                          { "userAccount", myJwtData.userAccount },
                          { "exp",exp }
                     };
            var MyAccessToken = JWTHelp.JWTJiaM(payload, AccessTokenKey);

            
            //5. 返回双Token
            return new Token
            {
                accessToken = MyAccessToken,
                refreshToken = refreshToken
            };
        }

        /// <summary>
        /// Base64解码
        /// </summary>
        /// <param name="base64UrlStr"></param>
        /// <returns></returns>

        public static string Base64UrlDecode(string base64UrlStr)
        {
            base64UrlStr = base64UrlStr.Replace('-', '+').Replace('_', '/');
            switch (base64UrlStr.Length % 4)
            {
                case 2:
                    base64UrlStr += "==";
                    break;
                case 3:
                    base64UrlStr += "=";
                    break;
            }
            var bytes = Convert.FromBase64String(base64UrlStr);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
