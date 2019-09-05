using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ACBC.Buss
{
    #region Sys
    public class BussCache
    {
        private string unique = "";
        public string Unique
        {
            get
            {
                return unique;
            }
            set
            {
                unique = value;
            }
        }
    }

    public class BussParam
    {
        public string GetUnique()
        {
            string needMd5 = "";
            string md5S = "";
            foreach (FieldInfo f in this.GetType().GetFields())
            {
                needMd5 += f.Name;
                needMd5 += f.GetValue(this).ToString();
            }
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.UTF8.GetBytes(needMd5));
                var strResult = BitConverter.ToString(result);
                md5S = strResult.Replace("-", "");
            }
            return md5S;
        }
    }

    public class SessionUser
    {
        public string openid;
        public string checkPhone;
        public string checkCode;
        public string userType;
        public string memberId;
    } 
    public class SmsCodeRes
    {
        public int error_code;
        public string reason;
    }

    public class WsPayState
    {
        public string userId;
        public string scanCode;
    }

    public class ExchangeRes
    {
        public string reason;
        public ExchangeResult result;
        public int error_code;
    }
    public class ExchangeResult
    {
        public string update;
        public List<string[]> list;
    }

    public enum ScanCodeType
    {
        Shop,
        User,
        Null,
    }

    #endregion

    #region Params

    public class LoginParam
    {
        public string code;
    }
    public class MemberRegParam
    {
        public string avatarUrl;
        public string city;
        public string country;
        public string gender;
        public string language;
        public string nickName;
        public string province;
    }
    public class PhoneParam
    {
        public string phone;
    }
    public class UserParam
    {
        public string userPhone;
        public string userName;
        public string userType;
        public string remark;
        public double store;
    }
    public class HouseBookingParam
    {
        public string houseId;
        public string userPhone;
        public string checkDate;
        public string beginTime;
    }
    public class BookingHouseParam
    {
        public string houseId;
        public string checkDate;
        public string userPhone;
        public string beginTime;
        public string endTime;
        public string bookingPrice;
    }
    public class BookingIdParam
    {
        public string bookingId;
    }
    #endregion

    #region DaoObjs

    public class Member
    {
        public string memberName;
        public string memberId;
        public string openid;
        public string memberImg;
        public string memberPhone;
        public string memberSex;
        public string scanCode;
        public string status;
    }
    public class User
    {
        public string userId;
        public string userName;
        public string userPhone;
        public string userType;
        public string userTypeName;
        public string openId;
        public double store;
        public double price;
        public double balance;
        public string remark;
        public string status;
    }
    public class RechargeLog
    {
        public string id;
        public string createTime;
        public string userName;
        public string userPhone;
        public string store;
        public string beforeStore;
        public string afterStore;
        public string beforePrice;
        public string afterPrice;
        public string inputMember;
    }
    public class House
    {
        public string houseId;
        public string houseName;
        public string houseImg;
        public string peopleNum;
        public string hourPrice;
        public string remark;
    }
    public class HouseBookingDateInfo
    {
        public House house;
        public List<TodoDay> todoDayList;
        public List<DisableDay> disableDayList;
    }
    public class TodoDay
    {
        public string year;
        public string month;
        public string day;
        public string todoText;
    }
    public class DisableDay
    {
        public string year;
        public string month;
        public string day;
    }
    public class HouseBookingDateTimeInfo
    {
        public List<string> beginTimes;
        public List<string> endTimes;
        public List<string> bookingPrices;
    }
    public class Booking
    {
        public string bookingId;
        public string bookingCode;
        public string houseName;
        public string userPhone;
        public string bookingStatus;
        public string createTime;
        public string bookingTime;
        public string price;
        public bool ifReturn;
    }
    #endregion
}
