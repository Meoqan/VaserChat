using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaserChat
{
    public class Message
    {
        public int command = 0;

        public uint UserID = 0;
        public string Username = "NONE";
        public int StatusID = 0;
        public Client cli = null;

        public string Messagedata = string.Empty;

    }
}
