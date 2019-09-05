using ACBC.Buss;
using ACBC.Dao;
using Com.ACBC.Framework.Database;
using System;
using StackExchange.Redis;
using Senparc.Weixin.Cache.Redis;
using Senparc.Weixin.Cache;
using Senparc.Weixin.WxOpen.Containers;
using System.Collections.Generic;

namespace ACBC.Common
{
    public class Global
    {
        public const string ROUTE_PX = "/api/analysis";
        public const string NAMESPACE = "net.llwell.analysis";
        public const int REDIS_NO = 1;
        public const int REDIS_EXPIRY_H = 1;
        public const int REDIS_EXPIRY_M = 0;
        public const int REDIS_EXPIRY_S = 0;

        public const string SMS_CODE_URL = "http://v.juhe.cn/sms/send?mobile={0}&tpl_id=68600&tpl_value=%23code%23%3D{1}&dtype=&key=7c21d791256af1ffdd85375c64846358";
        public const string EXCHANGE_URL = "http://op.juhe.cn/onebox/exchange/query?key=08940f90d07501ace3f535e32968cf94";

        /// <summary>
        /// 基础业务处理类对象
        /// </summary>
        public static BaseBuss BUSS = new BaseBuss();

        /// <summary>
        /// 初始化启动预加载
        /// </summary>
        public static void StartUp()
        {
            if (DatabaseOperationWeb.TYPE == null)
            {
                DatabaseOperationWeb.TYPE = new DBManager();
            }

            try
            {
                RedisManager.ConfigurationOption = REDIS;
                CacheStrategyFactory.RegisterObjectCacheStrategy(() => RedisContainerCacheStrategy.Instance);
            }
            catch
            {
                Console.WriteLine("Redis Error, Change Local");
            }
        }


        public static string REDIS
        {
            get
            {
                var redis = System.Environment.GetEnvironmentVariable("redis");
                return redis;
            }
        }
        /// <summary>
        /// 商户posCode
        /// </summary>
        public static string POSCODE
        {
            get
            {
                var posCode = System.Environment.GetEnvironmentVariable("PosCode");
                return posCode;
            }
        }
        /// <summary>
        /// 商户posCode
        /// </summary>
        public static string ALLOTTYPE
        {
            get
            {
                var allotType = System.Environment.GetEnvironmentVariable("AllotType");
                return allotType;
            }
        }
        #region 小程序相关

        /// <summary>
        /// 小程序APPID
        /// </summary>
        public static string APPID
        {
            get
            {
                var appId = System.Environment.GetEnvironmentVariable("WxAppId");
                return appId;
            }
        }

        /// <summary>
        /// 小程序APPSECRET
        /// </summary>
        public static string APPSECRET
        {
            get
            {
                var appSecret = System.Environment.GetEnvironmentVariable("WxAppSecret");
                return appSecret;
            }
        }


        /// <summary>
        /// 微信支付MchId
        /// </summary>
        public static string MCHID
        {
            get
            {
                var mchId = System.Environment.GetEnvironmentVariable("WxMchId");
                return mchId;
            }
        }


        /// <summary>
        /// 微信支付Key
        /// </summary>
        public static string PaymentKey
        {
            get
            {
                var paymentKey = System.Environment.GetEnvironmentVariable("WxPaymentKey");
                return paymentKey;
            }
        }

        /// <summary>
        /// 微信支付回调地址
        /// </summary>
        public static string CallBackUrl
        {
            get
            {
                var callBackUrl = System.Environment.GetEnvironmentVariable("CallBackUrl");
                return callBackUrl;
            }
        }

        /// <summary>
        /// 公众号ID
        /// </summary>
        public static string WxPaymentAppID
        {
            get
            {
                var wxPaymentAppID = System.Environment.GetEnvironmentVariable("WxPaymentAppID");
                return wxPaymentAppID;
            }
        }

        /// <summary>
        /// 开船前可以售票时间
        /// </summary>
        public static string IfBookingTime
        {
            get
            {
                var ifBookingTime = System.Environment.GetEnvironmentVariable("IfBookingTime");
                return ifBookingTime;
            }
        }
        /// <summary>
        /// 证书路径
        /// </summary>
        public static string SSlCertPath
        {
            get
            {
                var sSlCertPath = System.Environment.GetEnvironmentVariable("SSlCertPath");
                return sSlCertPath;
            }
        }

        /// <summary>
        /// 证书密码
        /// </summary>
        public static string SSlCertPassword
        {
            get
            {
                var sSlCertPassword = System.Environment.GetEnvironmentVariable("SSlCertPassword");
                return sSlCertPassword;
            }
        }

        /// <summary>
        /// 支付成功消息模板
        /// </summary>
        public static string PaySuccessTemplate
        {
            get
            {
                return "Ik_3GVasaJuTg9dvSztd3ByvgpzaY5u4LRz6_2x5ZeM";
            }
        }

        /// <summary>
        /// 退票成功消息模板
        /// </summary>
        public static string ReturnSuccessTemplate
        {
            get
            {
                return "6nJyAQEA1LsSD9YU-v-h_TEhLp_U_dO2AGiBkpJqRX0";
            }
        }

        /// <summary>
        /// 退票申请消息模板
        /// </summary>
        public static string ReturnTicketTemplate
        {
            get
            {
                return "5xmN4AkeICotjXQB9Vy36h9-MTWWne9z_Q0Qjr0YskY";
            }
        }

        /// <summary>
        /// 行程提醒消息模板
        /// </summary>
        public static string TravelReminderTemplate
        {
            get
            {
                return "GvfDFC1GpKFEBWBVM6tZK24EwNaPNzhUAEHuwgGXVos";
            }
        }

        /// <summary>
        /// 待支付消息模板
        /// </summary>
        public static string ToBePayTemplate
        {
            get
            {
                return "gBqJ4ilt5RWm0bQIsT-j1E_5m-aUXLx0z0-UWxMe4vs";
            }
        }

        #endregion

        #region OSS相关

        /// <summary>
        /// AccessId
        /// </summary>
        public static string AccessId
        {
            get
            {
                var accessId = System.Environment.GetEnvironmentVariable("ossAccessId");
                return accessId;
            }
        }
        /// <summary>
        /// AccessKey
        /// </summary>
        public static string AccessKey
        {
            get
            {
                var accessKey = System.Environment.GetEnvironmentVariable("ossAccessKey");
                return accessKey;
            }
        }
        /// <summary>
        /// OssHttp
        /// </summary>
        public static string OssHttp
        {
            get
            {
                var ossHttp = System.Environment.GetEnvironmentVariable("ossHttp");
                return ossHttp;
            }
        }
        /// <summary>
        /// OssBucket
        /// </summary>
        public static string OssBucket
        {
            get
            {
                var ossBucket = System.Environment.GetEnvironmentVariable("ossBucket");
                return ossBucket;
            }
        }
        /// <summary>
        /// ossUrl
        /// </summary>
        public static string OssUrl
        {
            get
            {
                var ossUrl = System.Environment.GetEnvironmentVariable("ossUrl");
                return ossUrl;
            }
        }
        /// <summary>
        /// OssDir
        /// </summary>
        public static string OssDir
        {
            get
            {
                var ossDir = System.Environment.GetEnvironmentVariable("ossDir");
                return ossDir;
            }
        }
        #endregion
        #region 携程相关

        /// <summary>
        /// 携程posCode
        /// </summary>
        public static string XCPOSCODE
        {
            get
            {
                var XCPosCode = System.Environment.GetEnvironmentVariable("XCPosCode");
                return XCPosCode;
            }
        }

        /// <summary>
        /// 携程OpenId
        /// </summary>
        public static string XCOPENID
        {
            get
            {
                var XCOpenId = System.Environment.GetEnvironmentVariable("XCOpenId");
                return XCOpenId;
            }
        }

        /// <summary>
        /// 携程Secret
        /// </summary>
        public static string XCSECRET
        {
            get
            {
                var XCSecret = System.Environment.GetEnvironmentVariable("XCSecret");
                return XCSecret;
            }
        }
        /// <summary>
        /// 商户posCode
        /// </summary>
        public static string XCALLOTTYPE
        {
            get
            {
                var xcallotType = System.Environment.GetEnvironmentVariable("XCAllotType");
                return xcallotType;
            }
        }
        /// <summary>
        /// 开船前可以售票时间
        /// </summary>
        public static string XCIfBookingTime
        {
            get
            {
                var xCIfBookingTime = System.Environment.GetEnvironmentVariable("XCIfBookingTime");
                return xCIfBookingTime;
            }
        }
        #endregion
    }
}
