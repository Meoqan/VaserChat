using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vaser;

namespace Global
{
    public class SEND_LOGIN : Container
    {
        internal const ushort ContID = 1;

        public string Username = "NONE";
    }

    public class SEND_MESSAGE : Container
    {
        internal const ushort ContID = 2;

        public uint UserID = 0;
        public string Message = "NONE";
    }

    public class SEND_USER : Container
    {
        internal const ushort ContID = 3;

        public uint UserID = 0;
        public string Username = "NONE";
        public int StatusID = 0;
    }
}
