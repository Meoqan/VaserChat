using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vaser;
using Global;

namespace VaserChatServerModule
{
    class Client
    {
        public static Client[] ClientArray = new Client[10000];
        public static List<Client> ClientList = new List<Client>();

        public Link lnk;
        public uint ID = 0;
        public string Username = "NONE";

        public static void BCastContainer(Container con, ushort ContainerID)
        {
            foreach (Client c in ClientList)
            {
                if (c.Username != "NONE") OPTIONS.Chat.SendContainer(c.lnk, con, ContainerID, c.ID);
            }
        }
    }
}
