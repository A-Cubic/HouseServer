using ACBC.Common;
using ACBC.Dao;
using Newtonsoft.Json;
using Senparc.Weixin.MP.Containers;
using Senparc.Weixin.WxOpen.AdvancedAPIs.Sns;
using Senparc.Weixin.WxOpen.Containers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;

namespace ACBC.Buss
{
    public class OpenBuss : IBuss
    {
        public ApiType GetApiType()
        {
            return ApiType.OpenApi;
        }
        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="baseApi"></param>
        /// <returns></returns>
        public object Do_Login(BaseApi baseApi)
        {
            LoginParam loginParam = JsonConvert.DeserializeObject<LoginParam>(baseApi.param.ToString());
            if (loginParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }

            var jsonResult = SnsApi.JsCode2Json(Global.APPID, Global.APPSECRET, loginParam.code);
            if (jsonResult.errcode == Senparc.Weixin.ReturnCode.请求成功)
            {
                AccessTokenContainer.Register(Global.APPID, Global.APPSECRET);
                var sessionBag = SessionContainer.UpdateSession(null, jsonResult.openid, jsonResult.session_key);

                OpenDao openDao = new OpenDao();
                SessionUser sessionUser = new SessionUser();

                Member member = openDao.GetMember(Utils.GetOpenID(sessionBag.Key));
                if (member == null)
                {
                    sessionUser.userType = "GUEST";
                    sessionBag.Name = JsonConvert.SerializeObject(sessionUser);
                    SessionContainer.Update(sessionBag.Key, sessionBag);
                    return new { token = sessionBag.Key, isReg = false };
                }
                else
                {
                    sessionUser.userType = "MEMBER";
                    sessionUser.openid = sessionBag.OpenId;
                    sessionUser.memberId = member.memberId;
                    sessionBag.Name = JsonConvert.SerializeObject(sessionUser);
                    SessionContainer.Update(sessionBag.Key, sessionBag);
                    return new
                    {
                        token = sessionBag.Key,
                        isReg = true,
                        member.memberId,
                        member.memberName,
                        member.memberImg,
                        member.memberPhone,
                        member.memberSex,
                        member.scanCode
                    };
                }
            }
            else
            {
                throw new ApiException(CodeMessage.SenparcCode, jsonResult.errmsg);
            }
        }
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="baseApi"></param>
        /// <returns></returns>
        public object Do_MemberReg(BaseApi baseApi)
        {
            MemberRegParam memberRegParam = JsonConvert.DeserializeObject<MemberRegParam>(baseApi.param.ToString());
            if (memberRegParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }

            SessionBag sessionBag = SessionContainer.GetSession(baseApi.token);
            if (sessionBag == null)
            {
                throw new ApiException(CodeMessage.InvalidToken, "InvalidToken");
            }

            OpenDao openDao = new OpenDao();
            string openID = Utils.GetOpenID(baseApi.token);
            var member = openDao.GetMember(openID);

            if (member != null)
            {
                throw new ApiException(CodeMessage.MemberExist, "MemberExist");
            }

            if (!openDao.MemberReg(memberRegParam, openID))
            {
                throw new ApiException(CodeMessage.MemberRegError, "MemberRegError");
            }
            member = openDao.GetMember(openID);
            SessionUser sessionUser = JsonConvert.DeserializeObject<SessionUser>(sessionBag.Name);
            sessionUser.openid = sessionBag.OpenId;
            sessionUser.memberId = member.memberId;
            sessionUser.userType = "MEMBER";
            sessionBag.Name = JsonConvert.SerializeObject(sessionUser);
            SessionContainer.Update(sessionBag.Key, sessionBag);

            return "";
        }
        
    }
}
