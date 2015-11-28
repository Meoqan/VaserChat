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
        public static SEND_LOGIN slCont = new SEND_LOGIN();
        internal const int ContID = 1;


        public string Username = "NONE";
    }

    public class SEND_MESSAGE : Container
    {
        public static SEND_MESSAGE smCont = new SEND_MESSAGE();
        internal const int ContID = 2;

        public int UserID = 0;
        public string Message = "NONE";
    }

    public class SEND_USER : Container
    {
        public static SEND_USER suCont = new SEND_USER();
        internal const int ContID = 3;

        public int UserID = 0;
        public string Username = "NONE";
        public int StatusID = 0;
    }
}
