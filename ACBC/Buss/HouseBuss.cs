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
    public class HouseBuss : IBuss
    {
        public ApiType GetApiType()
        {
            return ApiType.HouseApi;
        }
        /// <summary>
        /// 获取房间信息
        /// </summary>
        /// <param name="baseApi"></param>
        /// <returns></returns>
        public object Do_GetHouseList(BaseApi baseApi)
        {
            HouseDao houseDao = new HouseDao();

            return houseDao.getHouseList();
        }

        /// <summary>
        /// 获取房间预订信息
        /// </summary>
        /// <param name="baseApi"></param>
        /// <returns></returns>
        public object Do_GetBookingDateInfo(BaseApi baseApi)
        {
            HouseBookingParam param = JsonConvert.DeserializeObject<HouseBookingParam>(baseApi.param.ToString());
            if (param == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (param.houseId == null || param.houseId == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            string openId = Utils.GetOpenID(baseApi.token);
            HouseDao houseDao = new HouseDao();

            return houseDao.getHouseBookingDateInfo(param);
        }

        /// <summary>
        /// 获取房间预订信息
        /// </summary>
        /// <param name="baseApi"></param>
        /// <returns></returns>
        public object Do_GetHouseDataTimeInfoList(BaseApi baseApi)
        {
            HouseBookingParam param = JsonConvert.DeserializeObject<HouseBookingParam>(baseApi.param.ToString());
            if (param == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (param.houseId == null || param.houseId == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (param.checkDate == null || param.checkDate == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            string openId = Utils.GetOpenID(baseApi.token);
            HouseDao houseDao = new HouseDao();

            return houseDao.getHouseDataTimeInfoList(param);
        }

        /// <summary>
        /// 获取房间预订信息
        /// </summary>
        /// <param name="baseApi"></param>
        /// <returns></returns>
        public object Do_GetHouseDataTimeInfoListByBeginTime(BaseApi baseApi)
        {
            HouseBookingParam param = JsonConvert.DeserializeObject<HouseBookingParam>(baseApi.param.ToString());
            if (param == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (param.houseId == null || param.houseId == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (param.checkDate == null || param.checkDate == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (param.beginTime == null || param.beginTime == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            string openId = Utils.GetOpenID(baseApi.token);
            HouseDao houseDao = new HouseDao();

            return houseDao.getHouseDataTimeInfoListByBeginTime(param);
        }

        /// <summary>
        /// 预订房间
        /// </summary>
        /// <param name="baseApi"></param>
        /// <returns></returns>
        public object Do_BookingHouse(BaseApi baseApi)
        {
            BookingHouseParam param = JsonConvert.DeserializeObject<BookingHouseParam>(baseApi.param.ToString());
            if (param == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (param.houseId == null || param.houseId == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (param.checkDate == null || param.checkDate == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (param.beginTime == null || param.beginTime == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (param.userPhone == null || param.userPhone == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (param.endTime == null || param.endTime == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (param.bookingPrice == null || param.bookingPrice == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            string openId = Utils.GetOpenID(baseApi.token);
            HouseDao houseDao = new HouseDao();
            if (houseDao.bookingHouse(param, openId))
            {
                UserDao userDao = new UserDao();
                if(userDao.updateUserPrice(param.userPhone, param.bookingPrice))
                {
                    return userDao.getUserByPhone(param.userPhone);
                }
                else
                {
                    throw new ApiException(CodeMessage.CutPaymentError, "CutPaymentError");
                }
            }
            else
            {
                throw new ApiException(CodeMessage.BookingInsertError, "BookingInsertError");
            }
        }

    }
}
