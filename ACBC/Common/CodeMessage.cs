﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ACBC.Common
{
    /// <summary>
    /// 返回信息对照
    /// </summary>
    public enum CodeMessage
    {
        OK = 0,
        PostNull = -1,

        AppIDError = 201,
        SignError = 202,

        NotFound = 404,
        InnerError = 500,

        SenparcCode = 1000,

        PaymentError = 3000,
        PaymentTotalPriceZero=3001,
        PaymentMsgError = 3002,
        PaymentStateError = 3003,
        PaymentBillError = 3004,

        InvalidToken = 4000,
        InvalidMethod = 4001,
        InvalidParam = 4002,
        InterfaceRole = 4003,//接口权限不足
        InterfaceValueError = 4004,//接口的参数不对
        InterfaceDBError=4005,//接口数据库操作失败
        NeedLogin = 4006,

        MemberExist = 10001,
        MemberRegError = 10002,
        NoLoginError = 10003,
        RechargeError = 10004,

        BookingInsertError = 20001,//预订房间失败
        CutPaymentError = 20002,//扣款失败
        BookingUserError = 20003,//预订用户未找到
        UserPriceError = 20004,//预订用户余额不足
        BookingRefundError = 20005,//退房间失败
        AddPaymentError = 20006,//退款失败

    }
}
