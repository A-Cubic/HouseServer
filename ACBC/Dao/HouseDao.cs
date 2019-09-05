using ACBC.Buss;
using Com.ACBC.Framework.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACBC.Dao
{
    public class HouseDao
    {

        public List<House> getHouseList()
        {
            List<House> list = new List<House>();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(ShipSqls.SELECT_HOUSELIST);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    House house = new House
                    {
                        houseId = dt.Rows[i]["house_id"].ToString(),
                        houseName = dt.Rows[i]["house_name"].ToString(),
                        houseImg = dt.Rows[i]["house_img"].ToString(),
                        peopleNum = dt.Rows[i]["people_num"].ToString(),
                        hourPrice = dt.Rows[i]["hour_price"].ToString(),
                        remark = dt.Rows[i]["remark"].ToString(),
                    };
                    list.Add(house);
                }

            }
            return list;
        }
        public HouseBookingDateInfo getHouseBookingDateInfo(HouseBookingDateParam param)
        {
            HouseBookingDateInfo houseBookingDateInfo = new HouseBookingDateInfo();

            houseBookingDateInfo.todoDayList = new List<TodoDay>();
            houseBookingDateInfo.disableDayList = new List<DisableDay>();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(ShipSqls.SELECT_HOUSELIST_DATA_BY_HOUSEID, param.houseId);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["BOOKINGTYPE"].ToString()=="1")
                    {
                        DisableDay disableDay = new DisableDay
                        {
                            year = dt.Rows[i]["BOOKINGYEAR"].ToString(),
                            month = dt.Rows[i]["BOOKINGMONTH"].ToString(),
                            day = dt.Rows[i]["BOOKINGDAY"].ToString(),
                        };
                        houseBookingDateInfo.disableDayList.Add(disableDay);
                    }
                    else
                    {
                        string todoText = "";
                        if (param.userPhone != "")
                        {
                            if (param.userPhone == dt.Rows[i]["BOOKINGYEAR"].ToString())
                            {
                                todoText = "预订";
                            }
                        }

                        TodoDay todoDay = new TodoDay
                        {
                            year = dt.Rows[i]["BOOKINGYEAR"].ToString(),
                            month = dt.Rows[i]["BOOKINGMONTH"].ToString(),
                            day = dt.Rows[i]["BOOKINGDAY"].ToString(),
                            todoText = todoText,
                        };
                        houseBookingDateInfo.todoDayList.Add(todoDay);
                    }
                }
            }
            StringBuilder builder1 = new StringBuilder();
            builder1.AppendFormat(ShipSqls.SELECT_HOUSELIST_BY_HOUSEID, param.houseId);
            string sql1 = builder1.ToString();
            DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "T").Tables[0];
            if (dt1.Rows.Count > 0)
            {
                House house = new House
                {
                    houseId = dt1.Rows[0]["house_id"].ToString(),
                    houseName = dt1.Rows[0]["house_name"].ToString(),
                    houseImg = dt1.Rows[0]["house_img"].ToString(),
                    peopleNum = dt1.Rows[0]["people_num"].ToString(),
                    hourPrice = dt1.Rows[0]["hour_price"].ToString(),
                    remark = dt1.Rows[0]["remark"].ToString(),
                };
                houseBookingDateInfo.house = house;
            }
            return houseBookingDateInfo;
        }
        public class ShipSqls
        {
            public const string SELECT_HOUSELIST = "" +
                "SELECT * " +
                "FROM T_BASE_HOUSE " +
                "WHERE FLAG = 1 ";
            public const string SELECT_HOUSELIST_BY_HOUSEID = "" +
                "SELECT * " +
                "FROM T_BASE_HOUSE " +
                "WHERE FLAG = 1 AND HOUSE_ID ={0}";
            public const string SELECT_HOUSELIST_DATA_BY_HOUSEID = "" +
                "SELECT BOOKINGTYPE,USER_PHONE,DAYOFMONTH(BOOKING_DATA) BOOKINGDAY," +
                       "MONTH(BOOKING_DATA) BOOKINGMONTH,YEAR(BOOKING_DATA) BOOKINGYEAR " +
                "FROM T_BOOKING_LIST " +
                "WHERE HOUSE_ID ='{0}' AND BOOKING_DATA>NOW() AND BOOKING_STATUS='1'";
        }
        
    }
}
