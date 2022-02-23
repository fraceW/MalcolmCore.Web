using MalcolmCore.Data;
using MalcolmCore.IService.Login;
using MalcolmCore.Utils.Model;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using MalcolmCore.Utils.Common;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using MalcolmCore.Utils.Tokens;
using MalcolmCore.Utils.Logs;
using System.Drawing;

namespace MalcolmCore.Service.Login
{
    public class LoginService : BaseService, ILoginService
    {
        public LoginService(CoreFrameDBContext db) : base(db)
        {
            this.db = db;
        }

        private static List<RefreshToken> rTokenList = new List<RefreshToken>();
        public RetuenResult<Token> Login(string userAccount, string userPwd)
        {
            useinfo useinfo = Entities<useinfo>().FirstOrDefault(t => t.usename.Equals(userAccount) && t.pwd.Equals(userPwd));
            if (useinfo != null)
            {
                Token token = TokenHelp.GetToken(userAccount, userPwd);
                LogUtils.Info(JsonConvert.SerializeObject(token), "ApiLog");
                return new RetuenResult<Token>
                {
                    code = 200,
                    msg = "登录成功",
                    data = token
                };
            }
            else 
            {
                return new RetuenResult<Token>
                {
                    code = 201,
                    msg = "登录失败，账号或密码错误",
                    data = new Token
                    {
                    }
                };
            }
        }

        public RetuenResult<Token> UpdateAccessToken(string accessToken, string refreshToken)
        {
            string AccessTokenKey = ConfigHelp.GetString("AccessTokenKey");
            string RefreshTokenKey = ConfigHelp.GetString("RefreshTokenKey");

            //1.先通过纯代码校验refreshToken的物理合法性
            var result = JWTHelp.JWTJieM(refreshToken, ConfigHelp.GetString("RefreshTokenKey"));
            if (result == "expired" || result == "invalid" || result == "error")
            {
                return new RetuenResult<Token> { code = 201, msg = "error", data = new Token() { } };
            }

            //2.从accessToken中解析出来userId等其它数据(即使accessToken已经过期，依旧可以解析出来)
            JwtData myJwtData = JsonConvert.DeserializeObject<JwtData>(this.Base64UrlDecode(accessToken.Split('.')[1]));

            //3. 拿着userId、refreshToken、当前时间去RefreshToken表中查数据
            var rTokenItem = rTokenList.Where(u => u.userId == myJwtData.userId && u.Token == refreshToken && u.expire > DateTime.Now).FirstOrDefault();
            if (rTokenItem == null)
            {
                return new RetuenResult<Token> { code = 201, msg = "error", data = new Token() { } };
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

            ////4.2.生成refreshToken
            ////过期时间(可以不设置，下面表示签名后 2天过期)
            //var expireTime = DateTime.Now.AddDays(2);
            //double exp2 = (expireTime - new DateTime(1970, 1, 1)).TotalSeconds;
            //var payload2 = new Dictionary<string, object>
            //         {
            //              {"userId", myJwtData.userId },
            //              { "userAccount", myJwtData.userAccount },
            //              { "exp",exp2 }
            //         };
            //var MyRefreshToken = JWTHelp.JWTJiaM(payload2, RefreshTokenKey);

            ////4.3 更新refreshToken表
            //rTokenItem.Token = MyRefreshToken;
            //rTokenItem.expire = expireTime;


            //5. 返回双Token
            return new RetuenResult<Token>
            {
                code = 200,
                msg = "成功",
                data = new Token
                {
                    accessToken = MyAccessToken,
                    refreshToken = refreshToken
                }
            };
        }

        /// <summary>
        /// Base64解码
        /// </summary>
        /// <param name="base64UrlStr"></param>
        /// <returns></returns>

        public string Base64UrlDecode(string base64UrlStr)
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

        public void test() 
        {
            using (var dataContext = new CoreFrameDBContext()) 
            {
                var q1 = from u in dataContext.useinfo
                        from d in dataContext.useDetails
                        where u.id == d.id && u.creatdate.HasValue
                        select u;

                var q2 = from u in dataContext.useinfo
                         join d in dataContext.useDetails on u.id equals d.id
                         select u;
                //join时必须将join后的表into到一个新的变量XX中，然后要用XX.DefaultIfEmpty()表示外连接。
                //DefaultIfEmpty使用了泛型中的default关键字。default关键字对于引用类型将返回null,而对于值类型则返回0。对于结构体类型，则会根据其成员类型将它们相应地初始化为null(引用类型)或0(值类型)
                var q3 = from u in dataContext.useinfo
                         join d in dataContext.useDetails on u.id equals d.id into f
                         from c in f.DefaultIfEmpty()
                         select c;

                var q4 = from u in dataContext.useinfo
                         orderby u.id descending
                         select u;

                var q5 = from u in dataContext.useinfo
                         group u by u.id into g
                         select g;

                q1.ToDictionary(t => t.id);
            }
        }
    }
}
