﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FIXApplication
{
    /// <summary>
    /// Interface to optionally provide hooks for some hacks on outgoing messages.
    /// </summary>
    public interface ICustomFixStrategy
    {
        QuickFix.SessionSettings SessionSettings { get; set; }

        Dictionary<int, string> DefaultNewOrderSingleCustomFields { get; }

        void ProcessToAdmin(QuickFix.Message msg, QuickFix.Session session);
        void ProcessToApp(QuickFix.Message msg, QuickFix.Session session);

        /// <summary>
        /// Modify a newly-created OrderCancelRequest in some way before it is sent out.  May throw exceptions.
        /// </summary>
        /// <param name="nos">the message that created the order being canceled</param>
        /// <param name="msg">the cancel message to be modified</param>
        void ProcessOrderCancelRequest(QuickFix.FIX44.NewOrderSingle nos, QuickFix.FIX44.OrderCancelRequest msg);

        /// <summary>
        /// Modify a newly-created OrderCancelReplaceRequest in some way before it is sent out.  May throw exceptions.
        /// </summary>
        /// <param name="nos">the message that created the order being canceled</param>
        /// <param name="msg">the cancel message to be modified</param>
        void ProcessOrderCancelReplaceRequest(QuickFix.FIX44.NewOrderSingle nos, QuickFix.FIX44.OrderCancelReplaceRequest msg);
    }
}
