using System;
using System.Threading;

namespace Kesco.Lib.Win.SingleInstance
{
    public delegate void InstanceEventHandler(object sender, InstanceEventArgs e);

    [Serializable]
    public class InstanceEventArgs : EventArgs
    {
        public InstanceEventArgs(string[] strNewArgs, bool NewInst)
        {
            strArgs = strNewArgs;
            bNewInstance = NewInst;
        }

        public string[] strArgs;
        public bool bNewInstance;
    }

    public class SingleInstanceHandler : MarshalByRefObject
    {
        private Mutex m_Mutex;
        private string m_UniqueIdentifier;

        public event InstanceEventHandler InstanceEvent;

        public bool Test(string mutexName)
        {
            bool createdNew = false;
            m_UniqueIdentifier = mutexName.ToUpper() + " One Instance Mutex Name";
            /*System.Threading.Mutex */
            m_Mutex = new Mutex(false, m_UniqueIdentifier, out createdNew);
            if (createdNew)
                m_Mutex.Close();
            return !createdNew;
        }

        public bool WaitClose(string mutexName)
        {
            bool createdNew;
            m_UniqueIdentifier = mutexName.ToUpper() + " One Instance Mutex Name";
            m_Mutex = new Mutex(true, m_UniqueIdentifier, out createdNew);
            if (createdNew)
                m_Mutex.Close();
            else
                createdNew = !m_Mutex.WaitOne(3000, false);
            return createdNew;
        }

        public bool IsCreated(string mutexName)
        {
            bool createdNew = true;
            m_UniqueIdentifier = mutexName.ToUpper() + " One Instance Mutex Name";
            m_Mutex = new Mutex(true, m_UniqueIdentifier, out createdNew);
            return !createdNew;
        }

        public void RaiseStartUpEvent(InstanceEventArgs event_args)
        {
            InstanceEvent(this, event_args);
        }

        public void RaiseMutex()
        {
            m_Mutex.Close();
        }

        public void RaiseMutex(string mutexName)
        {
            m_UniqueIdentifier = mutexName.ToUpper() + " One Instance Mutex Name";
            m_Mutex = new Mutex(true, m_UniqueIdentifier);
            m_Mutex.ReleaseMutex();
            m_Mutex.Close();
        }
    }
}