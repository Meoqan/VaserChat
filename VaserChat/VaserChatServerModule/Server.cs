using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vaser;
using Global;
using System.Threading;
using System.Diagnostics;

namespace VaserChatServerModule
{
    public class Server
    {
        static IDPool myPool = new IDPool(10000);
        static VaserServer ChatServer = null;

        public static void Stop()
        {
            if (ChatServer != null)
            {
                Console.WriteLine("VaserChat Server is closing...");

                //Teardown
                myPool = new IDPool(10000);
                Client.ClientList.Clear();

                //close the server
                ChatServer.Stop();

                ChatServer = null;
            }
        }

        public static void Start()
        {
            Console.WriteLine("VaserChat Server is starting...");

            //initialize the server
            OPTIONS.init();

            OPTIONS.Login.IncomingPacket += OnLoginPacket;
            OPTIONS.Chat.IncomingPacket += OnChatPacket;

            //start the server
            ChatServer = new VaserServer(System.Net.IPAddress.Any, 3100, VaserOptions.ModeNotEncrypted, OPTIONS.PColl);
            ChatServer.NewLink += OnNewLink;
            ChatServer.DisconnectingLink += OnDisconnectingLink;

            Console.WriteLine("VaserChat Server has started.");
        }

        static void OnDisconnectingLink(object p, LinkEventArgs e)
        {
            Client c = (Client)e.lnk.AttachedObject;
            Client.ClientList.Remove(c);
            Client.ClientArray[c.ID] = null;

            //BCast Send disconnect
            SEND_USER.suCont.StatusID = STATUS.USER_DISCONNECT;
            SEND_USER.suCont.UserID = c.ID;
            SEND_USER.suCont.Username = c.Username;
            Client.BCastContainer(SEND_USER.suCont, SEND_USER.ContID);

            myPool.DisposeID(c.ID);

            //free all resources
            c.lnk.Dispose();
            Console.WriteLine("Client disconnected from IP: " + c.lnk.IPv4Address);

            //send all bufferd data to the clients
            Portal.Finialize();
        }

        static void OnNewLink(object p, LinkEventArgs e)
        {
            Console.WriteLine("Client connected from IP: " + e.lnk.IPv4Address);

            e.lnk.Accept();


            Client C1 = new Client();
            C1.ID = myPool.GetFreeID();
            C1.lnk = e.lnk;
            Client.ClientList.Add(C1);
            Client.ClientArray[C1.ID] = C1;

            e.lnk.AttachedID = C1.ID;
            e.lnk.AttachedObject = C1;

            //send all bufferd data to the clients
            Portal.Finialize();
        }

        static void OnLoginPacket(object p, PacketEventArgs e)
        {
            Debug.WriteLine("SEND_LOGIN Packet");
            //unpack the packet, true if the decode was successful
            if (SEND_LOGIN.ContID == e.pak.ContainerID && SEND_LOGIN.slCont.UnpackContainer(e.pak, OPTIONS.Login))
            {
                Client c = (Client)e.lnk.AttachedObject;

                if (c.Username == "NONE" && SEND_LOGIN.slCont.Username != "NONE" && SEND_LOGIN.slCont.Username.Length > 2 && SEND_LOGIN.slCont.Username.Length < 25)
                {
                    c.Username = SEND_LOGIN.slCont.Username;

                    //Send ID & OK
                    OPTIONS.Login.SendContainer(c.lnk, null, STATUS.LOGIN_OK, c.ID);

                    foreach (Client c2 in Client.ClientList)
                    {
                        if (c != c2)
                        {
                            SEND_USER.suCont.StatusID = STATUS.USER_NEW;
                            SEND_USER.suCont.UserID = c2.ID;
                            SEND_USER.suCont.Username = c2.Username;
                            OPTIONS.Chat.SendContainer(c.lnk, SEND_USER.suCont, SEND_USER.ContID, c2.ID);
                        }
                    }

                    //BCast new client
                    SEND_USER.suCont.StatusID = STATUS.USER_NEW;
                    SEND_USER.suCont.UserID = c.ID;
                    SEND_USER.suCont.Username = c.Username;
                    Client.BCastContainer(SEND_USER.suCont, SEND_USER.ContID);

                    Portal.Finialize();
                }
                else
                {
                    Console.WriteLine("Login error > Username invalid " + e.pak.link.IPv4Address);
                    e.pak.link.Dispose();
                }


            }
            else
            {
                Console.WriteLine("Login error > Disconnecting " + e.pak.link.IPv4Address);
                e.pak.link.Dispose();
            }
        }

        static void OnChatPacket(object p, PacketEventArgs e)
        {

            try
            {
                Client c = (Client)e.lnk.AttachedObject;
                switch (e.pak.ContainerID)
                {
                    case SEND_MESSAGE.ContID:
                        Debug.WriteLine("SEND_MESSAGE Packet");
                        if (SEND_MESSAGE.smCont.UnpackContainer(e.pak, OPTIONS.Chat))
                        {
                            SEND_MESSAGE.smCont.UserID = c.ID;
                            Client.BCastContainer(SEND_MESSAGE.smCont, SEND_MESSAGE.ContID);
                        }
                        else
                        {
                            Console.WriteLine("Chat error > packet decode error > " + c.Username + " > " + e.pak.link.IPv4Address);
                            e.pak.link.Dispose();
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Chat error > " + ex.ToString() + " > " + e.pak.link.IPv4Address);
                e.pak.link.Dispose();
            }

            //send all bufferd data to the clients
            Portal.Finialize();
        }

    }
}
