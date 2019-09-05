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
            HouseBookingDateParam param = JsonConvert.DeserializeObject<HouseBookingDateParam>(baseApi.param.ToString());
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

    }
}
