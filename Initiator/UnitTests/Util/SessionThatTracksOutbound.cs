using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickFix;

namespace UnitTests.Util
{
    public class SessionThatTracksOutbound : QuickFix.Session
    {
        private Dictionary<string, IList<QuickFix.Message>> _msgLookup = new Dictionary<string, IList<QuickFix.Message>>();
        public Dictionary<string, IList<QuickFix.Message>> MsgLookup { get { return _msgLookup; } }

        public SessionThatTracksOutbound(
            QuickFix.IApplication app,
            QuickFix.IMessageStoreFactory storeFactory,
            QuickFix.SessionID sessID,
            QuickFix.DataDictionaryProvider dataDictProvider,
            QuickFix.SessionSchedule sessionSchedule,
            int heartBtInt,
            QuickFix.ILogFactory logFactory,
            QuickFix.IMessageFactory msgFactory,
            string senderDefaultApplVerID)
            : base(app, storeFactory, sessID, dataDictProvider, sessionSchedule, heartBtInt, logFactory, msgFactory, senderDefaultApplVerID)
        {
        }

        public override bool Send(QuickFix.Message message)
        {
            string type = message.Header.GetField(QuickFix.Fields.Tags.MsgType);
            
            if(_msgLookup.ContainsKey(type)==false)
                _msgLookup[type] = new List<QuickFix.Message>();

            _msgLookup[type].Add(message);

            return true;
        }
    }
}
