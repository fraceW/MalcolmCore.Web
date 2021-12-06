using JWT;
using JWT.Algorithms;
using JWT.Exceptions;
using JWT.Serializers;
using System;
using System.Collections.Generic;
using System.Text;

namespace MalcolmCore.Utils.Common
{
    public class JWTHelp
    {
        /// <summary>
        /// JWT加密算法
        /// </summary>
        /// <param name="payload">负荷部分，存储使用的信息</param>
        /// <param name="secret">密钥</param>
        /// <param name="extraHeaders">存放表头额外的信息,不需要的话可以不传</param>
        /// <returns></returns>
        public static string JWTJiaM(IDictionary<string, object> payload, string secret, IDictionary<string, object> extraHeaders = null)
        {
            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);
            var token = encoder.Encode(payload, secret);
            return token;
        }

        /// <summary>
        /// JWT解密算法(新的7.x版本写法）
        /// </summary>
        /// <param name="token">需要解密的token串</param>
        /// <param name="secret">密钥</param>
        /// <returns></returns>
        public static string JWTJieM(string token, string secret)
        {
            try
            {
                IJsonSerializer serializer = new JsonNetSerializer();
                IDateTimeProvider provider = new UtcDateTimeProvider();
                IJwtValidator validator = new JwtValidator(serializer, provider);
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtAlgorithm algorithm = new HMACSHA256Algorithm(); // symmetric
                IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder, algorithm);

                var json = decoder.Decode(token, secret, true);
                //校验通过，返回解密后的字符串
                return json;
            }
            catch (TokenExpiredException)
            {
                //表示过期
                return "expired";
            }
            catch (SignatureVerificationException)
            {
                //表示验证不通过
                return "invalid";
            }
            catch (Exception)
            {
                return "error";
            }
        }

    }
}
