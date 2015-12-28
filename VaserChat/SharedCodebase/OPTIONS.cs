using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vaser;

namespace Global
{
    public class OPTIONS
    {
        public static PortalCollection PColl = null;
        public static Portal Login;
        public static Portal Chat;

        private static object _ThreadLock = new object();
        private static uint _clientID = 0;
        public static uint clientID
        {
            get
            {
                lock (_ThreadLock)
                {
                    return _clientID;
                }
            }
            set
            {
                lock (_ThreadLock)
                {
                    _clientID = value;
                }
            }
        }

        private static Link _Connection;
        public static Link Connection
        {
            get
            {
                lock (_ThreadLock)
                {
                    return _Connection;
                }
            }
            set
            {
                lock (_ThreadLock)
                {
                    _Connection = value;
                }
            }
        }

        public static void init()
        {
            PColl = new PortalCollection();
            Login = PColl.CreatePortal(100);
            Chat = PColl.CreatePortal(101);
        }
    }
}
