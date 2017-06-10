﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FIXApplication.Enums;

namespace FIXApplication
{
    public static class MessageCreator44
    {
        /// <summary>
        /// Create a new ClOrdID based on the time
        /// </summary>
        /// <returns></returns>
        static public QuickFix.Fields.ClOrdID GenerateClOrdID()
        {
            return new QuickFix.Fields.ClOrdID(DateTime.Now.ToString("HHmmssfff"));
        }

        /// <summary>
        /// Create a News message that has 0 lines.  Nothing unexpected here.
        /// </summary>
        /// <param name="headline_str"></param>
        /// <returns></returns>
        static public QuickFix.FIX44.News News(string headline_str)
        {
            return News(headline_str, new List<string>());
        }

        /// <summary>
        /// Create a News message.  Nothing unexpected here.
        /// </summary>
        /// <param name="headline_str"></param>
        /// <param name="lines"></param>
        /// <returns></returns>
        static public QuickFix.FIX44.News News(string headline_str, IList<string> lines)
        {
            QuickFix.Fields.Headline headline = new QuickFix.Fields.Headline(headline_str);

            QuickFix.FIX44.News news = new QuickFix.FIX44.News(headline);
            QuickFix.FIX44.News.LinesOfTextGroup group = new QuickFix.FIX44.News.LinesOfTextGroup();
            foreach (string s in lines)
            {
                group.Text = new QuickFix.Fields.Text(s);
                news.AddGroup(group);
            }

            if (lines.Count == 0)
            {
                QuickFix.Fields.LinesOfText noLines = new QuickFix.Fields.LinesOfText(0);
                news.SetField(noLines, true);
            }

            return news;
        }

        /// <summary>
        /// Create a NewOrderSingle message.
        /// </summary>
        /// <param name="customFields"></param>
        /// <param name="orderType"></param>
        /// <param name="side"></param>
        /// <param name="symbol"></param>
        /// <param name="orderQty"></param>
        /// <param name="tif"></param>
        /// <param name="price">ignored if orderType=Market</param>
        /// <returns></returns>
        static public QuickFix.FIX44.NewOrderSingle NewOrderSingle(
            Dictionary<int, string> customFields,
            OrderType orderType, Side side, string symbol,
            int orderQty, TimeInForce tif, decimal price)
        {
            // hard-coded fields
            QuickFix.Fields.HandlInst fHandlInst = new QuickFix.Fields.HandlInst(QuickFix.Fields.HandlInst.AUTOMATED_EXECUTION_ORDER_PRIVATE);

            // from params
            QuickFix.Fields.OrdType fOrdType = FixEnumTranslator.ToField(orderType);
            QuickFix.Fields.Side fSide = FixEnumTranslator.ToField(side);
            QuickFix.Fields.Symbol fSymbol = new QuickFix.Fields.Symbol(symbol);
            QuickFix.Fields.TransactTime fTransactTime = new QuickFix.Fields.TransactTime(DateTime.Now);
            QuickFix.Fields.ClOrdID fClOrdID = GenerateClOrdID();
                        
            QuickFix.FIX44.NewOrderSingle nos = new QuickFix.FIX44.NewOrderSingle(
                fClOrdID, fSymbol, fSide, fTransactTime, fOrdType);
            nos.OrderQty = new QuickFix.Fields.OrderQty(orderQty);
            nos.TimeInForce = FixEnumTranslator.ToField(tif);

            if (orderType == OrderType.Limit)
                nos.Price = new QuickFix.Fields.Price(price);

            // add custom fields
            foreach (KeyValuePair<int, string> p in customFields)
                nos.SetField(new QuickFix.Fields.StringField(p.Key, p.Value));

            return nos;
        }

        public static QuickFix.FIX44.OrderCancelReplaceRequest OrderCancelReplaceRequest(
            Dictionary<int, string> customFields,
            QuickFix.FIX44.NewOrderSingle nos, int newQty, decimal newPrice)
        {
            QuickFix.FIX44.OrderCancelReplaceRequest ocrq = new QuickFix.FIX44.OrderCancelReplaceRequest(
                new QuickFix.Fields.OrigClOrdID(nos.ClOrdID.Obj),
                GenerateClOrdID(),                
                nos.Symbol,
                nos.Side,
                new QuickFix.Fields.TransactTime(DateTime.Now),
                nos.OrdType);

            ocrq.OrderQty = new QuickFix.Fields.OrderQty(newQty);

            if (nos.OrdType.Obj != QuickFix.Fields.OrdType.MARKET)
                ocrq.Price = new QuickFix.Fields.Price(newPrice);

            // other fields to relay
            ocrq.TimeInForce = nos.TimeInForce;

            // add custom fields
            foreach (KeyValuePair<int, string> p in customFields)
                ocrq.SetField(new QuickFix.Fields.StringField(p.Key, p.Value));

            return ocrq;
        }
    }
}
