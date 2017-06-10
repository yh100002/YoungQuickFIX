using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickFix;

namespace UnitTests.Util
{
    public class MockInitiator : QuickFix.IInitiator
    {
        public bool IsLoggedOnValue { get; set; }
        private bool _started = false;


        #region IInitiator Members

        public bool IsStopped { get { return !_started; } }

        bool IInitiator.IsLoggedOn => throw new NotImplementedException();

        public bool IsLoggedOn() { return IsLoggedOnValue; }

        public void Start()
        {
            _started = true;
        }

        public void Stop(bool force)
        {
            Stop();
        }

        public void Stop()
        {
            _started = false;
        }

        public HashSet<SessionID> GetSessionIDs()
        {
            throw new NotImplementedException();
        }

        public bool AddSession(SessionID sessionID, Dictionary dict)
        {
            throw new NotImplementedException();
        }

        public bool RemoveSession(SessionID sessionID, bool terminateActiveSession)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
