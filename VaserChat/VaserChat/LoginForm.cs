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


namespace VaserChat
{
    public partial class LoginForm : Form
    {
        ChatForm form2 = null;

        public LoginForm()
        {
            InitializeComponent();

            OPTIONS.init();
            OPTIONS.Login.IncomingPacket += OnLoginPacket;

            form2 = new ChatForm();
        }

        private void b_Connect_Click(object sender, EventArgs e)
        {
            if (tb_Username.Text.Length < 3)
            {
                MessageBox.Show("Username needs at least three chars.");
                return;
            }

            try
            {
#if DEBUG
                OPTIONS.Connection = VaserClient.ConnectClient("localhost", 3100, VaserOptions.ModeNotEncrypted, OPTIONS.PColl);
                OPTIONS.Connection.Disconnecting += OnDisconnectingLink;
#else
                OPTIONS.Connection = VaserClient.ConnectClient(tb_ServerAddress.Text, 3100, VaserOptions.ModeNotEncrypted, OPTIONS.PColl);
                OPTIONS.Connection.Disconnecting += OnDisconnectingLink;
#endif
            }
            catch
            {
                MessageBox.Show("can't connect to " + tb_ServerAddress.Text);
                return;
            }

            SEND_LOGIN.slCont.Username = tb_Username.Text;
            OPTIONS.Login.SendContainer(OPTIONS.Connection, SEND_LOGIN.slCont, SEND_LOGIN.ContID, 0);
            Portal.Finialize();

        }


        private void OpenChatwindow()
        {
            form2.Show();

            this.Hide();
            form2.Closed += (s, args) => this.Close();
        }

        private void ResetChatwindow()
        {
            /*if (!form2.)
            {
                //Reset chat
                form2.Invoke(new Action(() => form2.Reset()));
            }
            if(!IsDisposed) this.Show();

            OPTIONS.Connection = null;*/
        }

        public void OnDisconnectingLink(object sender, LinkEventArgs e)
        {
            MessageBox.Show("Lost Connection...");
            if (OPTIONS.clientID != 0)
            {
                OPTIONS.clientID = 0;
                //Reset chat
                if (!this.IsDisposed)
                {
                    if (this.InvokeRequired)
                    {
                        this.Invoke(new Action(() => this.Close()));
                    }
                    else
                    {
                        this.Close();
                    }
                }
            }
        }

        public void OnLoginPacket(object sender, PacketEventArgs e)
        {

            if (e.pak.ContainerID == 1)
            {
                OPTIONS.clientID = e.pak.ObjectID;

                //open chat
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() => OpenChatwindow()));
                }
                else
                {
                    OpenChatwindow();
                }
            }

            Portal.Finialize();

        }
    }
}
