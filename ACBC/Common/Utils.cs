using ACBC.Buss;
using Newtonsoft.Json;
using Senparc.Weixin.Cache.Redis;
using Senparc.Weixin.WxOpen.Containers;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ACBC.Common
{
    public class Utils
    {
        /// <summary>
        /// 获取系统已登录用户OPENID
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static string GetOpenID(string token)
        {

            //return "123456";

            SessionBag sessionBag = SessionContainer.GetSession(token);
            if (sessionBag == null)
            {
                return null;
            }
            return sessionBag.OpenId;
        }

        public static bool SetCache(string key, object value, int hours, int minutes, int seconds)
        {
            key = Global.NAMESPACE + "." + key;
            var db = RedisManager.Manager.GetDatabase(Global.REDIS_NO);
            var expiry = new TimeSpan(hours, minutes, seconds);
            string valueStr = JsonConvert.SerializeObject(value);
            return db.StringSet(key, valueStr, expiry);
        }

        public static bool SetCache(BussCache value, int hours, int minutes, int seconds)
        {
            string key = value.GetType().FullName + value.Unique;
            return SetCache(key, value, hours, minutes, seconds);
        }

        public static bool SetCache(BussCache value)
        {
            return SetCache(value, Global.REDIS_EXPIRY_H, Global.REDIS_EXPIRY_M, Global.REDIS_EXPIRY_S);
        }

        public static dynamic GetCache<T>(string key)
        {
            key = Global.NAMESPACE + "." + key;
            var db = RedisManager.Manager.GetDatabase(Global.REDIS_NO);
            if (db.StringGet(key).HasValue)
            {
                return JsonConvert.DeserializeObject<T>(db.StringGet(key));
            }
            return null;
        }

        public static dynamic GetCache<T>()
        {
            string key = typeof(T).FullName;
            return GetCache<T>(key);
        }

        public static dynamic GetCache<T>(BussParam bussParam)
        {
            string key = typeof(T).FullName + bussParam.GetUnique();
            return GetCache<T>(key);
        }

        public static void DeleteCache(string key)
        {
            key = Global.NAMESPACE + "." + key;
            var db = RedisManager.Manager.GetDatabase(Global.REDIS_NO);
            if (db.StringGet(key).HasValue)
            {
                db.KeyDelete(key);
            }
        }

        public static void DeleteCacheAll(string key)
        {
            key = Global.NAMESPACE + "." + key + "*";
            var db = RedisManager.Manager.GetDatabase(Global.REDIS_NO);
            var server = RedisManager.Manager.GetServer(Global.REDIS);
            var keys = server.Keys(database: db.Database, pattern: key);
            db.KeyDelete(keys.ToArray());
        }

        public static void DeleteCache<T>(bool delChild = false)
        {
            string key = typeof(T).FullName;
            if (delChild)
            {
                DeleteCacheAll(key);
            }
            else
            {
                DeleteCache(key);
            }
        }

        public static void ClearCache()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                  .SelectMany(a => a.GetTypes()
                  .Where(t => typeof(BussCache).Equals(t.BaseType)))
                  .ToArray();
            foreach (var v in types)
            {
                if (v.IsClass)
                {
                    DeleteCacheAll(v.FullName);
                }
            }
        }

        public static string PostHttp(string url, string body, string contentType)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

            httpWebRequest.ContentType = contentType;
            httpWebRequest.Method = "POST";
            httpWebRequest.Timeout = 20000;

            byte[] btBodys = Encoding.UTF8.GetBytes(body);
            httpWebRequest.ContentLength = btBodys.Length;
            httpWebRequest.GetRequestStream().Write(btBodys, 0, btBodys.Length);

            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
            string responseContent = streamReader.ReadToEnd();

            httpWebResponse.Close();
            streamReader.Close();
            httpWebRequest.Abort();
            httpWebResponse.Close();

            return responseContent;
        }

        public static string GetHttp(string url)
        {

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";
            httpWebRequest.Timeout = 20000;

            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
            string responseContent = streamReader.ReadToEnd();

            httpWebResponse.Close();
            streamReader.Close();

            return responseContent;
        }

        public static double GetExchange(string name)
        {
            ExchangeRes exchangeRes = GetCache<ExchangeRes>("EXCHANGE");
            if (exchangeRes == null)
            {
                string exchange = GetHttp(Global.EXCHANGE_URL);
                exchangeRes = JsonConvert.DeserializeObject<ExchangeRes>(exchange);
                SetCache("EXCHANGE", exchangeRes, 1, 0, 0);
            }

            double exRate = 0;
            if (exchangeRes.error_code == 0)
            {
                var list = exchangeRes.result.list;
                foreach (string[] item in list)
                {
                    foreach (string s in item)
                    {
                        if (s != name)
                            break;
                        exRate = Convert.ToDouble(item[3]);
                        break;
                    }
                    if (exRate > 0)
                        break;
                }
            }

            return exRate / 100;
        }

        private static string USER_AGENT = string.Format("WXPaySDK/{3} ({0}) .net/{1} {2}", Environment.OSVersion, Environment.Version, Global.MCHID, typeof(Utils).Assembly.GetName().Version);

        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            //直接确认，否则打不开    
            return true;
        }

        public static string Post(string xml, string url, bool isUseCert, int timeout)
        {
            System.GC.Collect();//垃圾回收，回收没有正常关闭的http连接

            string result = "";//返回结果

            HttpWebRequest request = null;
            HttpWebResponse response = null;
            Stream reqStream = null;

            try
            {
                //设置最大连接数
                ServicePointManager.DefaultConnectionLimit = 200;
                //设置https验证方式
                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback =
                            new RemoteCertificateValidationCallback(CheckValidationResult);
                }

                /***************************************************************
                * 下面设置HttpWebRequest的相关属性
                * ************************************************************/
                request = (HttpWebRequest)WebRequest.Create(url);
                request.UserAgent = USER_AGENT;
                request.Method = "POST";
                request.Timeout = timeout * 1000;

                //设置代理服务器
                //WebProxy proxy = new WebProxy();                          //定义一个网关对象
                //proxy.Address = new Uri(WxPayConfig.PROXY_URL);              //网关服务器端口:端口
                //request.Proxy = proxy;

                //设置POST的数据类型和长度
                request.ContentType = "text/xml";
                byte[] data = System.Text.Encoding.UTF8.GetBytes(xml);
                request.ContentLength = data.Length;

                //是否使用证书
                if (isUseCert)
                {
                    X509Certificate2 cert = new X509Certificate2( Global.SSlCertPath, Global.SSlCertPassword);
                    request.ClientCertificates.Add(cert);
                    Log.Debug("WxPayApi", "PostXml used cert");
                }

                //往服务器写入数据
                reqStream = request.GetRequestStream();
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();

                //获取服务端返回
                response = (HttpWebResponse)request.GetResponse();

                //获取服务端返回数据
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                result = sr.ReadToEnd().Trim();
                sr.Close();
            }
            catch (System.Threading.ThreadAbortException e)
            {
                Log.Error("HttpService", "Thread - caught ThreadAbortException - resetting.");
                Log.Error("Exception message: {0}", e.Message);
                System.Threading.Thread.ResetAbort();
            }
            catch (WebException e)
            {
                Log.Error("HttpService", e.ToString());
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    Log.Error("HttpService", "StatusCode : " + ((HttpWebResponse)e.Response).StatusCode);
                    Log.Error("HttpService", "StatusDescription : " + ((HttpWebResponse)e.Response).StatusDescription);
                }
                throw new WxPayException(e.ToString());
            }
            catch (Exception e)
            {
                Log.Error("HttpService", e.ToString());
                throw new WxPayException(e.ToString());
            }
            finally
            {
                //关闭连接和流
                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
            }
            return result;
        }

        /// <summary>
        /// 处理http GET请求，返回数据
        /// </summary>
        /// <param name="url">请求的url地址</param>
        /// <returns>http GET成功后返回的数据，失败抛WebException异常</returns>
        public static string Get(string url)
        {
            System.GC.Collect();
            string result = "";

            HttpWebRequest request = null;
            HttpWebResponse response = null;

            //请求url以获取数据
            try
            {
                //设置最大连接数
                ServicePointManager.DefaultConnectionLimit = 200;
                //设置https验证方式
                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback =
                            new RemoteCertificateValidationCallback(CheckValidationResult);
                }

                /***************************************************************
                * 下面设置HttpWebRequest的相关属性
                * ************************************************************/
                request = (HttpWebRequest)WebRequest.Create(url);
                request.UserAgent = USER_AGENT;
                request.Method = "GET";

                //获取服务器返回
                response = (HttpWebResponse)request.GetResponse();

                //获取HTTP返回数据
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                result = sr.ReadToEnd().Trim();
                sr.Close();
            }
            catch (System.Threading.ThreadAbortException e)
            {
                Log.Error("HttpService", "Thread - caught ThreadAbortException - resetting.");
                Log.Error("Exception message: {0}", e.Message);
                System.Threading.Thread.ResetAbort();
            }
            catch (WebException e)
            {
                Log.Error("HttpService", e.ToString());
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    Log.Error("HttpService", "StatusCode : " + ((HttpWebResponse)e.Response).StatusCode);
                    Log.Error("HttpService", "StatusDescription : " + ((HttpWebResponse)e.Response).StatusDescription);
                }
                throw new WxPayException(e.ToString());
            }
            catch (Exception e)
            {
                Log.Error("HttpService", e.ToString());
                throw new WxPayException(e.ToString());
            }
            finally
            {
                //关闭连接和流
                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
            }
            return result;
        }
    }
}
