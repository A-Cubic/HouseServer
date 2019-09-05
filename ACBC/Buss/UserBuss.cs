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
    public class UserBuss : IBuss
    {
        public ApiType GetApiType()
        {
            return ApiType.UserApi;
        } 

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="baseApi"></param>
        /// <returns></returns>
        public object Do_GetUserByPhone(BaseApi baseApi)
        {
            PhoneParam param = JsonConvert.DeserializeObject<PhoneParam>(baseApi.param.ToString());
            if (param == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (param.phone == null || param.phone == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            UserDao userDao = new UserDao(); 

            return userDao.getUserByPhone(param.phone);
        }


        /// <summary>
        /// 新增或者更新用户信息
        /// </summary>
        /// <param name="baseApi"></param>
        /// <returns></returns>
        public object Do_UpdateUser(BaseApi baseApi)
        {
            UserParam param = JsonConvert.DeserializeObject<UserParam>(baseApi.param.ToString());
            if (param == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (param.userName == null || param.userName == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (param.userPhone == null || param.userPhone == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (param.userType == null || param.userType == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            string openId = Utils.GetOpenID(baseApi.token);
            UserDao userDao = new UserDao();

            if (userDao.getUserByPhone(param.userPhone) == null)
            {
                userDao.addUser(param, openId);
            }
            else
            {
                userDao.updateUser(param);
            }
            return userDao.getUserByPhone(param.userPhone);
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="baseApi"></param>
        /// <returns></returns>
        public object Do_Recharge(BaseApi baseApi)
        {
            UserParam param = JsonConvert.DeserializeObject<UserParam>(baseApi.param.ToString());
            if (param == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (param.userPhone == null || param.userPhone == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (param.store == 0)
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            string openId = Utils.GetOpenID(baseApi.token);
            UserDao userDao = new UserDao();

            if (userDao.addRecharge(param, openId))
            {
                return userDao.getUserByPhone(param.userPhone);
            }
            else
            {
                throw new ApiException(CodeMessage.RechargeError, "RechargeError");
            }
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="baseApi"></param>
        /// <returns></returns>
        public object Do_GetRechargeList(BaseApi baseApi)
        {
            UserParam param = JsonConvert.DeserializeObject<UserParam>(baseApi.param.ToString());
            if (param == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (param.userPhone == null || param.userPhone == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            } 
            UserDao userDao = new UserDao();

            return userDao.getRechargeList(param.userPhone);
        }
    }
}
