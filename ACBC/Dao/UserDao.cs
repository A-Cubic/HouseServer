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
    public class UserDao
    {

        public User getUserByPhone(string phone)
        {
            User user = null;
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(ShipSqls.SELECT_USER_BY_PHONE, phone);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt.Rows.Count > 0)
            {
                string userTypeName = "机构";
                if (dt.Rows[0]["user_type"].ToString()=="2")
                {
                    userTypeName = "个人";
                }
                user = new User
                {
                    userId = dt.Rows[0]["user_id"].ToString(),
                    userName = dt.Rows[0]["user_name"].ToString(),
                    userPhone = dt.Rows[0]["user_phone"].ToString(),
                    userType = dt.Rows[0]["user_type"].ToString(),
                    userTypeName = userTypeName,
                    openId = dt.Rows[0]["openId"].ToString(),
                    store = Convert.ToDouble( dt.Rows[0]["store"]),
                    price = Convert.ToDouble(dt.Rows[0]["price"]),
                    balance = Convert.ToDouble(dt.Rows[0]["store"])- Convert.ToDouble(dt.Rows[0]["price"]),
                    remark = dt.Rows[0]["remark"].ToString(),
                    status = dt.Rows[0]["status"].ToString(),
                };
            }
            return user;
        }
        public bool addUser(UserParam param, string openId)
        {
            StringBuilder builder1 = new StringBuilder();
            builder1.AppendFormat(ShipSqls.INSERT_USER, param.userPhone, param.userType, param.userName, param.remark, openId);
            string sql1 = builder1.ToString();
            return DatabaseOperationWeb.ExecuteDML(sql1);
        }
        public bool updateUser(UserParam param)
        {
            StringBuilder builder1 = new StringBuilder();
            builder1.AppendFormat(ShipSqls.UPDATE_USER, param.userType, param.userName, param.remark, param.userPhone);
            string sql1 = builder1.ToString();
            return DatabaseOperationWeb.ExecuteDML(sql1);
        }
        public bool addRecharge(UserParam param, string openId)
        {
            User user = getUserByPhone(param.userPhone);
            StringBuilder builder1 = new StringBuilder();
            builder1.AppendFormat(ShipSqls.UPDATE_STORE, param.userPhone, param.store);
            string sql1 = builder1.ToString();
            if (DatabaseOperationWeb.ExecuteDML(sql1))
            {
                StringBuilder builder2 = new StringBuilder();
                builder2.AppendFormat(ShipSqls.INSERT_LOG_RECHARGE, param.userPhone, param.store,
                    user.store, user.store + param.store, user.price, user.price + param.store, openId);
                string sql2 = builder2.ToString();
                DatabaseOperationWeb.ExecuteDML(sql2);
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<RechargeLog> getRechargeList(string userPhone)
        {
            List<RechargeLog> rechargeLogList = new List<RechargeLog>();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(ShipSqls.SELECT_RECHARGE_BY_PHONE, userPhone);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    RechargeLog rechargeLog = new RechargeLog
                    {
                        id = dt.Rows[i]["id"].ToString(),
                        createTime = dt.Rows[i]["createTime"].ToString(),
                        userName = dt.Rows[i]["user_name"].ToString(),
                        userPhone = dt.Rows[i]["user_phone"].ToString(),
                        store = dt.Rows[i]["store"].ToString(),
                        beforeStore = dt.Rows[i]["before_store"].ToString(),
                        afterStore = dt.Rows[i]["after_store"].ToString(),
                        beforePrice = dt.Rows[i]["before_price"].ToString(),
                        afterPrice = dt.Rows[i]["after_price"].ToString(),
                        inputMember = dt.Rows[i]["inputMember"].ToString(),
                    };
                    rechargeLogList.Add(rechargeLog);
                }
                
            }
            return rechargeLogList;
        }
        public bool updateUserPrice(string userPhone,string price)
        {
            StringBuilder builder1 = new StringBuilder();
            builder1.AppendFormat(ShipSqls.UPDATE_PRICE, userPhone, price);
            string sql1 = builder1.ToString();
            return DatabaseOperationWeb.ExecuteDML(sql1);
        }
        public class ShipSqls
        {
            public const string SELECT_USER_BY_PHONE = "" +
                "SELECT * " +
                "FROM T_USER_LIST " +
                "WHERE USER_PHONE = '{0}' ";
            public const string INSERT_USER = "" +
                "INSERT INTO T_USER_LIST(USER_PHONE,USER_TYPE,USER_NAME,REMARK,OPENID) " +
                "VALUES('{0}','{1}','{2}','{3}','{4}') ";
            public const string UPDATE_USER = "" +
                "UPDATE T_USER_LIST SET USER_TYPE='{0}',USER_NAME='{1}',REMARK='{2}'  " +
                "WHERE USER_PHONE='{3}' ";
            public const string UPDATE_STORE = "" +
                "UPDATE T_USER_LIST SET STORE=STORE+{1},PRICE=PRICE+{1} " +
                "WHERE USER_PHONE='{0}' ";
            public const string INSERT_LOG_RECHARGE = "" +
                "INSERT INTO T_LOG_RECHARGE(CREATETIME,USER_PHONE,STORE,BEFORE_STORE," +
                "AFTER_STORE,BEFORE_PRICE,AFTER_PRICE,INPUTMEMBER) " +
                "VALUES(NOW(),'{0}',{1},{2},{3},{4},{5},'{6}')";
            public const string SELECT_RECHARGE_BY_PHONE = "" +
                "SELECT R.*,U.USER_NAME " +
                "FROM T_LOG_RECHARGE R,T_USER_LIST U " +
                "WHERE R.USER_PHONE =U.USER_PHONE  AND U.USER_PHONE = '{0}' ";
            public const string UPDATE_PRICE = "" +
                "UPDATE T_USER_LIST SET PRICE=PRICE-{1} " +
                "WHERE USER_PHONE='{0}' ";
        }
        
    }
}
