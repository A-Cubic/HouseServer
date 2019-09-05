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
        public House getHouseByHouseId(string houseId)
        {
            House house = null;
            StringBuilder builder1 = new StringBuilder();
            builder1.AppendFormat(ShipSqls.SELECT_HOUSELIST_BY_HOUSEID, houseId);
            string sql1 = builder1.ToString();
            DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "T").Tables[0];
            if (dt1.Rows.Count > 0)
            {
                house = new House
                {
                    houseId = dt1.Rows[0]["house_id"].ToString(),
                    houseName = dt1.Rows[0]["house_name"].ToString(),
                    houseImg = dt1.Rows[0]["house_img"].ToString(),
                    peopleNum = dt1.Rows[0]["people_num"].ToString(),
                    hourPrice = dt1.Rows[0]["hour_price"].ToString(),
                    remark = dt1.Rows[0]["remark"].ToString(),
                };
            }
            return house;
        }
        public HouseBookingDateInfo getHouseBookingDateInfo(HouseBookingParam param)
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
                    if (dt.Rows[i]["BOOKINGTYPE"].ToString() == "1")
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
            houseBookingDateInfo.house = getHouseByHouseId(param.houseId);
            return houseBookingDateInfo;
        }

        public HouseBookingDateTimeInfo getHouseDataTimeInfoList(HouseBookingParam param)
        {
            HouseBookingDateTimeInfo houseBookingDateTimeInfo = new HouseBookingDateTimeInfo();

            houseBookingDateTimeInfo.beginTimes = new List<string>();
            houseBookingDateTimeInfo.endTimes = new List<string>();

            House house = getHouseByHouseId(param.houseId);

            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(ShipSqls.SELECT_HOUSELIST_TIME_BY_HOUSEID_AND_DATA, param.houseId, param.checkDate);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];

            DateTime begin = Convert.ToDateTime(param.checkDate + " 09:00:00");
            DateTime end = Convert.ToDateTime(param.checkDate + " 19:00:00");
            List<string> beginTimes = new List<string>();
            List<string> endTimes = new List<string>();
            List<string> bookingPrices = new List<string>();

            DateTime dateTime = begin;
            int i = 0;
            int count = 0;
            bool ifEndClose = false;
            while (dateTime < end)
            {
                if (i == dt.Rows.Count || dateTime.AddMinutes(30) <= Convert.ToDateTime(dt.Rows[i]["BOOKING_TIME_FROM"]))
                {
                    beginTimes.Add(dateTime.ToString("HH:mm"));
                    dateTime = dateTime.AddMinutes(15);
                    count++;
                    if (!ifEndClose)
                    {
                        endTimes.Add(dateTime.ToString("HH:mm"));
                        bookingPrices.Add(Convert.ToString(Convert.ToDouble( house.hourPrice)/4*count));
                    }
                }
                else
                {
                    dateTime = Convert.ToDateTime(dt.Rows[i]["BOOKING_TIME_END"]).AddMinutes(15);
                    i++;
                    if (endTimes.Count>0)
                    {
                        ifEndClose = true;
                    }
                    
                }
            }
            houseBookingDateTimeInfo.beginTimes = beginTimes;
            houseBookingDateTimeInfo.endTimes = endTimes;
            houseBookingDateTimeInfo.bookingPrices = bookingPrices;

            return houseBookingDateTimeInfo;
        }

        public HouseBookingDateTimeInfo getHouseDataTimeInfoListByBeginTime(HouseBookingParam param)
        {
            HouseBookingDateTimeInfo houseBookingDateTimeInfo = new HouseBookingDateTimeInfo();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(ShipSqls.SELECT_HOUSELIST_TIME_BY_HOUSEID_AND_DATA_AND_TIME,
                                    param.houseId, param.checkDate, param.checkDate + " " + param.beginTime);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];

            House house = getHouseByHouseId(param.houseId);
            DateTime begin = Convert.ToDateTime(param.checkDate + " " + param.beginTime);
            DateTime end = Convert.ToDateTime(param.checkDate + " 19:00:00");
            List<string> endTimes = new List<string>();
            List<string> bookingPrices = new List<string>();

            DateTime dateTime = begin;
            int i = 0;
            int count = 0;
            while (dateTime < end)
            {
                if (i == dt.Rows.Count || dateTime.AddMinutes(30) <= Convert.ToDateTime(dt.Rows[i]["BOOKING_TIME_FROM"]))
                {
                    dateTime = dateTime.AddMinutes(15);
                    count++;
                    endTimes.Add(dateTime.ToString("HH:mm"));
                    bookingPrices.Add(Convert.ToString(Convert.ToDouble(house.hourPrice) / 4 * count));
                }
                else
                {
                    dateTime = Convert.ToDateTime(dt.Rows[i]["BOOKING_TIME_END"]).AddMinutes(15);
                    i++;
                    break;
                }
            }
            houseBookingDateTimeInfo.endTimes = endTimes;
            houseBookingDateTimeInfo.bookingPrices = bookingPrices;
            return houseBookingDateTimeInfo;
        }
        public bool bookingHouse(BookingHouseParam param, string openId)
        {
            StringBuilder builder1 = new StringBuilder();
            builder1.AppendFormat(ShipSqls.INSERT_BOOKINGLIST,openId, param.userPhone, param.houseId,
                param.checkDate, param.checkDate+" "+param.beginTime+":00",
                param.checkDate + " " + param.endTime + ":00", param.bookingPrice);
            string sql1 = builder1.ToString();
            return DatabaseOperationWeb.ExecuteDML(sql1);
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
            public const string SELECT_HOUSELIST_TIME_BY_HOUSEID_AND_DATA = "" +
                "SELECT BOOKING_TIME_FROM,BOOKING_TIME_END " +
                "FROM T_BOOKING_LIST " +
                "WHERE HOUSE_ID ='{0}' AND BOOKING_DATA='{1}' " +
                "AND BOOKING_STATUS='1' AND BOOKINGTYPE='2'";
            public const string SELECT_HOUSELIST_TIME_BY_HOUSEID_AND_DATA_AND_TIME = "" +
                "SELECT BOOKING_TIME_FROM,BOOKING_TIME_END " +
                "FROM T_BOOKING_LIST " +
                "WHERE HOUSE_ID ='{0}' AND BOOKING_DATA='{1}' " +
                "AND BOOKING_STATUS='1' AND BOOKINGTYPE='2' " +
                "AND BOOKING_TIME_FROM >'{2}'";
            public const string INSERT_BOOKINGLIST = "" +
                "INSERT INTO T_BOOKING_LIST(OPENID,USER_PHONE,HOUSE_ID,BOOKINGTYPE,BOOKING_STATUS," +
                                 "CREATETIME,BOOKING_DATA,BOOKING_TIME_FROM,BOOKING_TIME_END,PRICE) " +
                "VALUES('{0}','{1}',{2},'2','1'," +
                        "NOW(),'{3}','{4}','{5}','{6}')";
        }

    }
}
