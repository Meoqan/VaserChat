using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vaser;
using Global;
using System.Threading;

namespace VaserChatServerModule
{
    public class Server
    {
        static volatile bool online = true;
        static IDPool myPool = new IDPool(10000);

        public static void Stop()
        {
            online = false;
        }

        public static void Start()
        {
            new Thread(Run).Start();
        }

        public static void Run()
        {
            Console.WriteLine("VaserChat Server is starting...");
            
            //initialize the server
            OPTIONS.init();
            //start the server
            VaserServer ChatServer = new VaserServer(System.Net.IPAddress.Any, 3100, VaserOptions.ModeKerberos);

            //create connection managing lists
            List<Client> Removelist = new List<Client>();

            Console.WriteLine("VaserChat Server has started.");
            //run the server
            while (online)
            {
                //accept new client
                Link lnk1 = ChatServer.GetNewLink();
                if (lnk1 != null)
                {
                    Console.WriteLine("Client connected from IP: " + lnk1.IPv4Address);
                    
                    lnk1.Accept();


                    Client C1 = new Client();
                    C1.ID = myPool.GetFreeID();
                    C1.lnk = lnk1;
                    Client.ClientList.Add(C1);
                    Client.ClientArray[C1.ID] = C1;
                }
                
                //proceed incoming login data
                foreach (Packet_Recv pak in OPTIONS.Login.GetPakets())
                {
                    
                    //unpack the packet, true if the decode was successful
                    if (SEND_LOGIN.ContID == pak.ContainerID && SEND_LOGIN.slCont.UnpackContainer(pak, OPTIONS.Login))
                    {
                        foreach (Client c in Client.ClientList)
                        {
                            if (c.lnk == pak.link)
                            {
                                if (c.Username == "NONE" && SEND_LOGIN.slCont.Username != "NONE" && SEND_LOGIN.slCont.Username.Length > 2 && SEND_LOGIN.slCont.Username.Length < 25)
                                {
                                    c.Username = SEND_LOGIN.slCont.Username;
                                    
                                    //Send ID & OK
                                    OPTIONS.Login.SendContainer(c.lnk, null, STATUS.LOGIN_OK, c.ID);

                                    foreach(Client c2 in Client.ClientList)
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
                                    Client.BCastContainer(SEND_USER.suCont,SEND_USER.ContID);
                                }
                                else
                                {
                                    Console.WriteLine("Login error > Username invalid " + pak.link.IPv4Address);
                                    pak.link.Dispose();
                                }
                            }
                        }
                        
                    }
                    else
                    {
                        Console.WriteLine("Login error > Disconnecting "+pak.link.IPv4Address);
                        pak.link.Dispose();
                    }
                }

                //proceed incoming chat data
                foreach (Packet_Recv pak in OPTIONS.Chat.GetPakets())
                {
                    try
                    {
                        Client c = Client.ClientArray[pak.ObjectID];
                        if (c.lnk == pak.link)
                        {
                            switch(pak.ContainerID)
                            {
                                case SEND_MESSAGE.ContID:
                                    if(SEND_MESSAGE.smCont.UnpackContainer(pak,OPTIONS.Chat))
                                    {
                                        SEND_MESSAGE.smCont.UserID = c.ID;
                                        Client.BCastContainer(SEND_MESSAGE.smCont,SEND_MESSAGE.ContID);
                                    }
                                    else
                                    {
                                        Console.WriteLine("Chat error > packet decode error > " + c.Username + " > " + pak.link.IPv4Address);
                                        pak.link.Dispose();
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Chat error > packet (IP) dows not belong to the client (Username) > " + c.Username + " > " + pak.link.IPv4Address);
                            pak.link.Dispose();
                        }
                    }catch(Exception ex)
                    {
                        Console.WriteLine("Chat error > " + ex.ToString() + " > " + pak.link.IPv4Address);
                        pak.link.Dispose();
                    }
                }

                //send all bufferd data to the clients
                Portal.Finialize();
                
                //disconnet clients
                foreach (Client c in Client.ClientList)
                {
                    if (!c.lnk.IsConnected) Removelist.Add(c);
                }

                foreach (Client c in Removelist)
                {
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
                }
                Removelist.Clear();

                Thread.Sleep(1);
            }


            //Teardown
            foreach (Client c in Client.ClientList)
            {
                c.lnk.Dispose();
            }
            Client.ClientList.Clear();

            //close the server
            ChatServer.Stop();

            online = false;
        }
    }
}
