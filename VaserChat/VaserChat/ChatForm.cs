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

        public ChatForm()
        {
            InitializeComponent();

        }

        public void Reset()
        {
            this.Hide();

            rtb_output.Text = string.Empty;

            tv_userlist.BeginUpdate();
            tv_userlist.Nodes.Clear();
            tv_userlist.EndUpdate();

        }

        SEND_MESSAGE SendM2 = new SEND_MESSAGE();
        private void tb_input_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                //Send Chat Message
                SendM2.Message = tb_input.Text;
                OPTIONS.Chat.SendContainer(OPTIONS.Connection, SendM2, SEND_MESSAGE.ContID, OPTIONS.clientID);

                tb_input.Text = string.Empty;

                e.Handled = true;
            }
        }

        private void SetChatmessage(string Username, Message msg)
        {
            rtb_output.Text += Environment.NewLine + Username + " > " + msg.Messagedata;

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



        private void ChatForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            OPTIONS.Connection.Dispose();
        }

        private void ChatCron_Tick(object sender, EventArgs e)
        {
            if (Networking.IncommingChat.Count > 0)
            {
                Message msg = Networking.IncommingChat.Dequeue();

                switch (msg.command)
                {
                    case 10: // message
                        SetChatmessage(msg.Username, msg);
                        break;
                    case 11:// new
                        AddNode(msg.cli);
                        SetChatmessage("System", msg);
                        break;
                    case 12://delete
                        RemoveNode(msg.cli);
                        SetChatmessage("System", msg);
                        break;
                }
            }
        }
    }
}
