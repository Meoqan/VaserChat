using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vaser;
using Global;
using System.Threading;
using System.Diagnostics;

namespace VaserChat
{
    static class Networking
    {

        public static Queue<Message> IncommingLogin = new Queue<Message>();
        public static Queue<Message> IncommingChat = new Queue<Message>();

        static volatile bool ConnectingRunning = false;

        public static Client[] ClientArray = new Client[10000];
        public static List<Client> ClientList = new List<Client>();
        public static List<Message> MessageList = new List<Message>();

        static string Username = string.Empty;

        public static void Connect(string _Username)
        {
            if (!ConnectingRunning)
            {
                Username = _Username;
                ConnectingRunning = true;
                ThreadPool.QueueUserWorkItem(ConnectionThread);
            }
        }

        static SEND_LOGIN SendL1 = new SEND_LOGIN();
        public static void ConnectionThread(object state)
        {


            OPTIONS.init();
            OPTIONS.Login.IncomingPacket += OnLoginPacket;
            OPTIONS.Chat.IncomingPacket += OnChatPaket;

            try
            {
#if DEBUG
                OPTIONS.Connection = VaserClient.ConnectClient("localhost", 3100, OPTIONS.PColl);
                OPTIONS.Connection.Disconnecting += OnDisconnectingLink;
#else
                OPTIONS.Connection = VaserClient.ConnectClient(tb_ServerAddress.Text, 3100, OPTIONS.PColl);
                OPTIONS.Connection.Disconnecting += OnDisconnectingLink;
#endif
            }
            catch
            {
                Message msg = new Message();
                msg.command = 2; //close
                msg.Messagedata = "Can't connect.";
                IncommingLogin.Enqueue(msg);

                ConnectingRunning = false;

                return;
            }

            SendL1.Username = Username;
            OPTIONS.Login.SendContainer(OPTIONS.Connection, SendL1, SEND_LOGIN.ContID, 0);

            ConnectingRunning = false;
        }


        public static void Close()
        {
            OPTIONS.Connection.Dispose();

            Message msg = new Message();
            msg.command = 2; //close
            msg.Messagedata = "Closed.";
            IncommingLogin.Enqueue(msg);
        }

        public static void OnDisconnectingLink(object sender, LinkEventArgs e)
        {
            //MessageBox.Show("Lost Connection...");
            if (OPTIONS.clientID != 0)
            {
                OPTIONS.clientID = 0;

                Message msg = new Message();
                msg.command = 2; //close
                msg.Messagedata = "Lost connection.";
                IncommingLogin.Enqueue(msg);


            }
        }

        public static void OnLoginPacket(object sender, PacketEventArgs e)
        {

            if (e.pak.ContainerID == 1)
            {
                OPTIONS.clientID = e.pak.ObjectID;

                Message msg = new Message();
                msg.command = 1; //open

                IncommingLogin.Enqueue(msg);
            }

        }

        static SEND_MESSAGE SendM1 = new SEND_MESSAGE();
        static SEND_USER SendU1 = new SEND_USER();
        private static void OnChatPaket(object sender, PacketEventArgs e)
        {

            switch (e.pak.ContainerID)
            {
                case SEND_MESSAGE.ContID:
                    Debug.WriteLine("SEND_MESSAGE Packet");
                    if (SendM1.UnpackContainer(e.pak, OPTIONS.Chat))
                    {
                        Message msg = new Message();

                        msg.command = 10; // message

                        msg.UserID = SendM1.UserID;
                        msg.Messagedata = SendM1.Message;

                        msg.Username = msg.UserID.ToString();

                        try
                        {
                            msg.Username = ClientArray[msg.UserID].Username;
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.ToString());
                        }

                        IncommingChat.Enqueue(msg);
                    }
                    break;
                case SEND_USER.ContID:
                    Debug.WriteLine("SEND_USER Packet");
                    if (SendU1.UnpackContainer(e.pak, OPTIONS.Chat))
                    {
                        int statID = SendU1.StatusID;
                        if (statID == STATUS.USER_NEW)
                        {
                            uint ID = SendU1.UserID;
                            string Username = SendU1.Username;

                            Client c = new Client();
                            c.Username = Username;
                            c.ID = ID;


                            ClientArray[ID] = c;

                            Message msg = new Message();

                            msg.command = 11; // new

                            msg.cli = c;
                            msg.UserID = SendU1.UserID;
                            msg.Username = SendU1.Username;

                            msg.Messagedata = "new client '" + c.Username + "' connected.";

                            IncommingChat.Enqueue(msg);
                            Debug.WriteLine("New User: " + c.Username + " ID: " + c.ID);
                        }

                        if (statID == STATUS.USER_DISCONNECT)
                        {
                            uint ID = SendU1.UserID;

                            try
                            {
                                Client c = ClientArray[ID];

                                Message msg = new Message();

                                msg.command = 12; // Delete

                                msg.Messagedata = "Client '" + c.Username + "' disconnected.";

                                msg.cli = c;

                                IncommingChat.Enqueue(msg);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex.ToString());
                            }
                        }

                    }
                    break;
            }
        }
    }
}
