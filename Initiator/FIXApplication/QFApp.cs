﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FIXApplication
{
    public class QFApp : QuickFix.MessageCracker, QuickFix.IApplication
    {
        public QuickFix.SessionID ActiveSessionID { get; set; }
        public QuickFix.SessionSettings MySessionSettings { get; set; }

        private ICustomFixStrategy _strategy = null;

        private QuickFix.IInitiator _initiator = null;
        public QuickFix.IInitiator Initiator
        {
            set
            {
                if (_initiator != null)
                    throw new Exception("You already set the initiator");
                _initiator = value;
            }
            get
            {
                if (_initiator == null)
                    throw new Exception("You didn't provide an initiator");
                return _initiator;
            }
        }

        public event Action LogonEvent;
        public event Action LogoutEvent;

        public event Action<QuickFix.FIX44.ExecutionReport> Fix44ExecReportEvent;

        /// <summary>
        /// Triggered on any message sent or received (arg1: isIncoming)
        /// </summary>
        public event Action<QuickFix.Message, bool> MessageEvent;


        public QFApp(QuickFix.SessionSettings settings)
            : this(settings, new NullFixStrategy())
        {}

        public QFApp(QuickFix.SessionSettings settings, ICustomFixStrategy strategy)
        {
            _strategy = strategy;
            ActiveSessionID = null;
            MySessionSettings = settings;
        }

        public void Start()
        {
            Trace.WriteLine("QFApp::Start() called");
            if(Initiator.IsStopped)
                Initiator.Start();
            else
                Trace.WriteLine("(already started)");
        }

        public void Stop()
        {
            Trace.WriteLine("QFApp::Stop() called");
            Initiator.Stop();
        }

        /// <summary>
        /// Tries to send the message; throws if not logged on.
        /// </summary>
        /// <param name="m"></param>
        public void Send(QuickFix.Message m)
        {            
            if (Initiator.IsLoggedOn == false)
                throw new Exception("Can't send a message.  We're not logged on.");
            if (ActiveSessionID == null)
                throw new Exception("Can't send a message.  ActiveSessionID is null (not logged on?).");

            QuickFix.Session.SendToTarget(m, this.ActiveSessionID);
        }


        #region Application Members

        public void FromAdmin(QuickFix.Message message, QuickFix.SessionID sessionID)
        {
            if (message.Header.GetString(35) == QuickFix.FIX44.Reject.MsgType)
                Console.WriteLine("REJECT RECEIVED: " + message.ToString());

            Trace.WriteLine("## FromAdmin: " + message.ToString());
            if (MessageEvent != null)
                MessageEvent(message, true);
        }

        public void FromApp(QuickFix.Message message, QuickFix.SessionID sessionID)
        {
            Trace.WriteLine("## FromApp: " + message.ToString());
            if (MessageEvent != null)
                MessageEvent(message, true);
            Crack(message, sessionID);
        }

        public void OnCreate(QuickFix.SessionID sessionID)
        {
            Trace.WriteLine("## OnCreate: " + sessionID.ToString());
        }

        public void OnLogon(QuickFix.SessionID sessionID)
        {
            this.ActiveSessionID = sessionID;
            Trace.WriteLine(String.Format("==OnLogon: {0}==", this.ActiveSessionID.ToString()));
            if (LogonEvent != null)
                LogonEvent();
        }

        public void OnLogout(QuickFix.SessionID sessionID)
        {
            // not sure how ActiveSessionID could ever be null, but it happened.
            string a = (this.ActiveSessionID == null) ? "null" : this.ActiveSessionID.ToString();
            Trace.WriteLine(String.Format("==OnLogout: {0}==", a));
            if (LogoutEvent != null)
                LogoutEvent();
        }

        public void ToAdmin(QuickFix.Message message, QuickFix.SessionID sessionID)
        {
            _strategy.ProcessToAdmin(message, QuickFix.Session.LookupSession(sessionID));
            Trace.WriteLine("## ToAdmin: " + message.ToString());
            if (MessageEvent != null)
                MessageEvent(message, false);
        }

        public void ToApp(QuickFix.Message message, QuickFix.SessionID sessionId)
        {
            _strategy.ProcessToApp(message, QuickFix.Session.LookupSession(sessionId));
            Trace.WriteLine("## ToApp: " + message.ToString());
            if (MessageEvent != null)
                MessageEvent(message, false);
        }

        #endregion


        public void OnMessage(QuickFix.FIX44.BusinessMessageReject msg, QuickFix.SessionID s) { }
        public void OnMessage(QuickFix.FIX44.OrderCancelReject msg, QuickFix.SessionID s) { }

        public void OnMessage(QuickFix.FIX44.ExecutionReport msg, QuickFix.SessionID s) 
        {
            if (Fix44ExecReportEvent != null)
                Fix44ExecReportEvent(msg);
        }

        public void OnMessage(QuickFix.FIX44.News msg, QuickFix.SessionID s)
        {
            if (msg.Headline.Obj == "TESTECHO")
                Send(MessageCreator44.News("ECHORESPONSE"));
        }


    }
}
