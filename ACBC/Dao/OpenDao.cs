using ACBC.Buss;
using ACBC.Common;
using Com.ACBC.Framework.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ACBC.Dao
{
    public class OpenDao
    {
        public void writeLog(string posCode, string openId, string logType, string logTxt)
        {
            StringBuilder builder1 = new StringBuilder();
            builder1.AppendFormat(OpenSqls.INSERT_LOG, posCode, openId, logType, logTxt);
            string sql1 = builder1.ToString();
            DatabaseOperationWeb.ExecuteDML(sql1);
        }
        public Member GetMember(string openID)
        {
            Member member = null;

            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(OpenSqls.SELECT_MEMBER_BY_OPENID, openID);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null && dt.Rows.Count == 1)
            {
                member = new Member
                {
                    memberId = dt.Rows[0]["MEMBER_ID"].ToString(),
                    memberImg = dt.Rows[0]["MEMBER_IMG"].ToString(),
                    memberName = dt.Rows[0]["MEMBER_NAME"].ToString(),
                    memberPhone = dt.Rows[0]["MEMBER_PHONE"].ToString(),
                    memberSex = dt.Rows[0]["MEMBER_SEX"].ToString(),
                    openid = dt.Rows[0]["OPENID"].ToString(),
                    scanCode = "CHECK_" + dt.Rows[0]["SCAN_CODE"].ToString(),
                    status = dt.Rows[0]["STATUS"].ToString(),
                };
            }

            return member;
        }

        public bool MemberReg(MemberRegParam memberRegParam, string openID)
        {
            string scanCode = "";
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.UTF8.GetBytes(openID));
                var strResult = BitConverter.ToString(result);
                scanCode = strResult.Replace("-", "");
            }

            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(OpenSqls.INSERT_MEMBER,
                memberRegParam.nickName,
                memberRegParam.avatarUrl,
                memberRegParam.gender,
                openID,
                scanCode);
            string sqlInsert = builder.ToString();

            return DatabaseOperationWeb.ExecuteDML(sqlInsert);
        }
        public class OpenSqls
        {
            public const string INSERT_LOG = ""
                + "INSERT INTO T_BASE_LOG "
                + "(LOGTIME,POSCODE,OPENID,LOGTYPE,LOGTXT)"
                + "VALUES( "
                + "NOW(),'{0}','{1}','{2}','{3}')";
            public const string SELECT_MEMBER_BY_OPENID = ""
               + "SELECT * "
               + "FROM T_BASE_MEMBER "
               + "WHERE OPENID = '{0}'";
            public const string INSERT_MEMBER = ""
                + "INSERT INTO T_BASE_MEMBER "
                + "(MEMBER_NAME,MEMBER_IMG,MEMBER_SEX,OPENID,SCAN_CODE)"
                + "VALUES( "
                + "'{0}','{1}','{2}','{3}','{4}')";
           
        }
    }
}
