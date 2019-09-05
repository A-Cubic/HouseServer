using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ACBC.Common
{
    public class WxPayApi
    {

        /**
        * 
        * 申请退款
        * @param WxPayData inputObj 提交给申请退款API的参数
        * @param int timeOut 超时时间
        * @throws WxPayException
        * @return 成功时返回接口调用结果，其他抛异常
        */
        public static string Refund(WxPayData inputObj, int timeOut = 6)
        {
            string url = "https://api.mch.weixin.qq.com/secapi/pay/refund";
            //检测必填参数
            if (!inputObj.IsSet("out_trade_no") && !inputObj.IsSet("transaction_id"))
            {
                throw new WxPayException("退款申请接口中，out_trade_no、transaction_id至少填一个！");
            }
            else if (!inputObj.IsSet("out_refund_no"))
            {
                throw new WxPayException("退款申请接口中，缺少必填参数out_refund_no！");
            }
            else if (!inputObj.IsSet("total_fee"))
            {
                throw new WxPayException("退款申请接口中，缺少必填参数total_fee！");
            }
            else if (!inputObj.IsSet("refund_fee"))
            {
                throw new WxPayException("退款申请接口中，缺少必填参数refund_fee！");
            }
            else if (!inputObj.IsSet("op_user_id"))
            {
                throw new WxPayException("退款申请接口中，缺少必填参数op_user_id！");
            }

            inputObj.SetValue("appid", Global.WxPaymentAppID);//公众账号ID
            inputObj.SetValue("mch_id", Global.MCHID);//商户号
            inputObj.SetValue("nonce_str", Guid.NewGuid().ToString().Replace("-", ""));//随机字符串
            inputObj.SetValue("sign_type", WxPayData.SIGN_TYPE_MD5);//签名类型
            inputObj.SetValue("sign", inputObj.MakeSign(WxPayData.SIGN_TYPE_MD5));//签名

            string xml = inputObj.ToXml();
            var start = DateTime.Now;

            string response = Utils.Post(xml, url, true, timeOut);//调用HTTP通信接口提交数据到API
            return response;
        }


        //   /**
        //* 
        //* 查询退款
        //* 提交退款申请后，通过该接口查询退款状态。退款有一定延时，
        //* 用零钱支付的退款20分钟内到账，银行卡支付的退款3个工作日后重新查询退款状态。
        //* out_refund_no、out_trade_no、transaction_id、refund_id四个参数必填一个
        //* @param WxPayData inputObj 提交给查询退款API的参数
        //* @param int timeOut 接口超时时间
        //* @throws WxPayException
        //* @return 成功时返回，其他抛异常
        //*/
        //   public static WxPayData RefundQuery(WxPayData inputObj, int timeOut = 6)
        //   {
        //       string url = "https://api.mch.weixin.qq.com/pay/refundquery";
        //       //检测必填参数
        //       if (!inputObj.IsSet("out_refund_no") && !inputObj.IsSet("out_trade_no") &&
        //           !inputObj.IsSet("transaction_id") && !inputObj.IsSet("refund_id"))
        //       {
        //           throw new WxPayException("退款查询接口中，out_refund_no、out_trade_no、transaction_id、refund_id四个参数必填一个！");
        //       }

        //       inputObj.SetValue("appid", WxPayConfig.GetConfig().GetAppID());//公众账号ID
        //       inputObj.SetValue("mch_id", WxPayConfig.GetConfig().GetMchID());//商户号
        //       inputObj.SetValue("nonce_str", GenerateNonceStr());//随机字符串
        //       inputObj.SetValue("sign_type", WxPayData.SIGN_TYPE_HMAC_SHA256);//签名类型
        //       inputObj.SetValue("sign", inputObj.MakeSign());//签名

        //       string xml = inputObj.ToXml();

        //       var start = DateTime.Now;//请求开始时间

        //       Log.Debug("WxPayApi", "RefundQuery request : " + xml);
        //       string response = HttpService.Post(xml, url, false, timeOut);//调用HTTP通信接口以提交数据到API
        //       Log.Debug("WxPayApi", "RefundQuery response : " + response);

        //       var end = DateTime.Now;
        //       int timeCost = (int)((end - start).TotalMilliseconds);//获得接口耗时

        //       //将xml格式的结果转换为对象以返回
        //       WxPayData result = new WxPayData();
        //       result.FromXml(response);

        //       ReportCostTime(url, timeCost, result);//测速上报

        //       return result;
        //   }


        /**
        * 根据当前系统时间加随机序列来生成订单号
         * @return 订单号
        */
        public static string GenerateOutTradeNo()
        {
            var ran = new Random();
            return string.Format("{0}{1}{2}", Global.MCHID, DateTime.Now.ToString("yyyyMMddHHmmss"), ran.Next(999));
        }

        /**
        * 生成时间戳，标准北京时间，时区为东八区，自1970年1月1日 0点0分0秒以来的秒数
         * @return 时间戳
        */
        public static string GenerateTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }


    }
}
