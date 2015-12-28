using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vaser;
using Global;
using System.Diagnostics;

namespace VaserChat
{
    public partial class ChatForm : Form
    {
        Client[] ClientArray = new Client[10000];
        List<Client> ClientList = new List<Client>();
        public class Client
        {
            public uint ID;
            public string Username;

            public TreeNode tn;
        }

        List<Message> MessageList = new List<Message>();
        public class Message
        {
            public uint UserID;
            public string message;
        }

        public ChatForm()
        {
            InitializeComponent();
            OPTIONS.Chat.IncomingPacket += OnChatPaket;
        }

        public void Reset()
        {
            this.Hide();

            rtb_output.Text = string.Empty;

            tv_userlist.BeginUpdate();
            tv_userlist.Nodes.Clear();
            tv_userlist.EndUpdate();

            ClientArray = new Client[10000];
            ClientList = new List<Client>();
            MessageList = new List<Message>();
        }

        private void tb_input_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                //Send Chat Message
                SEND_MESSAGE.smCont.Message = tb_input.Text;
                OPTIONS.Chat.SendContainer(OPTIONS.Connection, SEND_MESSAGE.smCont, SEND_MESSAGE.ContID, OPTIONS.clientID);
                Portal.Finialize();

                tb_input.Text = string.Empty;

                e.Handled = true;
            }
        }

        private void SetChatmessage(string Username, Message msg)
        {
            rtb_output.Text += Environment.NewLine + Username + " > " + msg.message;

            rtb_output.SelectionStart = rtb_output.Text.Length; //Set the current caret position at the end
            rtb_output.ScrollToCaret(); //Now scroll it automatically
        }

        private void AddNode(Client c)
        {
            tv_userlist.BeginUpdate();
            c.tn = new TreeNode(c.Username);
            tv_userlist.Nodes.Add(c.tn);
            tv_userlist.EndUpdate();
        }

        private void RemoveNode(Client c)
        {
            tv_userlist.BeginUpdate();
            tv_userlist.Nodes.Remove(c.tn);
            tv_userlist.EndUpdate();
        }

        private void OnChatPaket(object sender, PacketEventArgs e)
        {

            switch (e.pak.ContainerID)
            {
                case SEND_MESSAGE.ContID:
                    Debug.WriteLine("SEND_MESSAGE Packet");
                    if (SEND_MESSAGE.smCont.UnpackContainer(e.pak, OPTIONS.Chat))
                    {
                        Message msg = new Message();
                        msg.UserID = SEND_MESSAGE.smCont.UserID;
                        msg.message = SEND_MESSAGE.smCont.Message;

                        MessageList.Add(msg);
                        string Username = msg.UserID.ToString();

                        try
                        {
                            Username = ClientArray[msg.UserID].Username;
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.ToString());
                        }

                        if (this.InvokeRequired)
                        {
                            this.Invoke(new Action(() => SetChatmessage(Username, msg)));
                        }
                        else
                        {
                            SetChatmessage(Username, msg);
                        }

                    }
                    break;
                case SEND_USER.ContID:
                    Debug.WriteLine("SEND_USER Packet");
                    if (SEND_USER.suCont.UnpackContainer(e.pak, OPTIONS.Chat))
                    {
                        int statID = SEND_USER.suCont.StatusID;
                        if (statID == STATUS.USER_NEW)
                        {
                            uint ID = SEND_USER.suCont.UserID;
                            string Username = SEND_USER.suCont.Username;

                            Client c = new Client();
                            c.Username = Username;
                            c.ID = ID;


                            ClientArray[ID] = c;

                            if (this.InvokeRequired)
                            {
                                this.Invoke(new Action(() => AddNode(c)));
                            }
                            else
                            {
                                AddNode(c);
                            }
                        }

                        if (statID == STATUS.USER_DISCONNECT)
                        {
                            uint ID = SEND_USER.suCont.UserID;

                            try
                            {
                                Client c = ClientArray[ID];

                                if (this.InvokeRequired)
                                {
                                    this.Invoke(new Action(() => RemoveNode(c)));
                                }
                                else
                                {
                                    RemoveNode(c);
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex.ToString());
                            }
                        }

                    }
                    break;
            }

            Portal.Finialize();

        }

        private void ChatForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            OPTIONS.Connection.Dispose();
        }
    }
}
