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
            ChatServer = new VaserServer(System.Net.IPAddress.Any, 3100, OPTIONS.PColl);
            ChatServer.NewLink += OnNewLink;
            ChatServer.DisconnectingLink += OnDisconnectingLink;
            ChatServer.Start();

            Console.WriteLine("VaserChat Server has started.");
        }

        static SEND_USER SendU2 = new SEND_USER();
        static void OnDisconnectingLink(object p, LinkEventArgs e)
        {
            Client c = (Client)e.lnk.AttachedObject;
            Client.ClientList.Remove(c);
            Client.ClientArray[c.ID] = null;

            //BCast Send disconnect
            SendU2.StatusID = STATUS.USER_DISCONNECT;
            SendU2.UserID = c.ID;
            SendU2.Username = c.Username;
            Client.BCastContainer(SendU2, SEND_USER.ContID);

            myPool.DisposeID(c.ID);

            //free all resources
            c.lnk.Dispose();
            Console.WriteLine("Client disconnected from IP: " + c.lnk.IPv4Address);
        }

        static void OnNewLink(object p, LinkEventArgs e)
        {
            Console.WriteLine("Client connected from IP: " + e.lnk.IPv4Address);

            Client C1 = new Client();
            C1.ID = myPool.GetFreeID();
            C1.lnk = e.lnk;
            Client.ClientList.Add(C1);
            Client.ClientArray[C1.ID] = C1;

            e.lnk.AttachedID = C1.ID;
            e.lnk.AttachedObject = C1;

            e.lnk.Accept();
        }

        static SEND_LOGIN SendL3 = new SEND_LOGIN();
        static SEND_USER SendU3 = new SEND_USER();
        static void OnLoginPacket(object p, PacketEventArgs e)
        {
            Debug.WriteLine("SEND_LOGIN Packet");
            //unpack the packet, true if the decode was successful
            if (SEND_LOGIN.ContID == e.pak.ContainerID && SendL3.UnpackContainer(e.pak, OPTIONS.Login))
            {
                Client c = (Client)e.lnk.AttachedObject;

                if (c.Username == "NONE" && SendL3.Username != "NONE" && SendL3.Username.Length > 2 && SendL3.Username.Length < 25)
                {
                    c.Username = SendL3.Username;

                    //Send ID & OK
                    OPTIONS.Login.SendContainer(c.lnk, null, STATUS.LOGIN_OK, c.ID);

                    //send new client all existing clients
                    foreach (Client c2 in Client.ClientList)
                    {
                        if (c != c2)
                        {
                            SendU3.StatusID = STATUS.USER_NEW;
                            SendU3.UserID = c2.ID;
                            SendU3.Username = c2.Username;
                            OPTIONS.Chat.SendContainer(c.lnk, SendU3, SEND_USER.ContID, c2.ID);
                        }
                    }

                    //BCast new client
                    SendU3.StatusID = STATUS.USER_NEW;
                    SendU3.UserID = c.ID;
                    SendU3.Username = c.Username;
                    Client.BCastContainer(SendU3, SEND_USER.ContID);
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

        static SEND_MESSAGE SendM3 = new SEND_MESSAGE();
        static void OnChatPacket(object p, PacketEventArgs e)
        {

            try
            {
                Client c = (Client)e.lnk.AttachedObject;
                switch (e.pak.ContainerID)
                {
                    case SEND_MESSAGE.ContID:
                        Debug.WriteLine("Server> SEND_MESSAGE Packet");
                        if (SendM3.UnpackContainer(e.pak, OPTIONS.Chat))
                        {
                            SendM3.UserID = c.ID;
                            Client.BCastContainer(SendM3, SEND_MESSAGE.ContID);
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
        }

    }
}
