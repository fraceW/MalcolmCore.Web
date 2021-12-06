using MalcolmCore.Utils.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MalcolmCore.IService.Login
{
    public interface ILoginService
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="userAccount"></param>
        /// <param name="userPwd"></param>
        /// <returns></returns>
        RetuenResult<Token> Login(string userAccount, string userPwd);
        /// <summary>
        /// 刷新Token
        /// </summary>
        /// <param name="userAccount"></param>
        /// <param name="userPwd"></param>
        /// <returns></returns>
        RetuenResult<Token> UpdateAccessToken(string accessToken, string refreshToken);
    }
}
