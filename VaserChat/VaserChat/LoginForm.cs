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
        public LoginForm()
        {
            InitializeComponent();

            OPTIONS.init();
            timer.Interval = 1;
            timer.Tick += OnTick;
        }

        private Timer timer = new Timer();

        private void b_Connect_Click(object sender, EventArgs e)
        {
            if (tb_Username.Text.Length < 3)
            {
                MessageBox.Show("username needs at least three chars.");
                return;
            }

            try
            {
                OPTIONS.Connection = VaserClient.ConnectClient(tb_ServerAddress.Text, 3100, VaserOptions.ModeKerberos);
            }
            catch
            {
                MessageBox.Show("can't connect to " + tb_ServerAddress.Text);
                return;
            }

            SEND_LOGIN.slCont.Username = tb_Username.Text;
            OPTIONS.Login.SendContainer(OPTIONS.Connection, SEND_LOGIN.slCont, SEND_LOGIN.ContID, 0);
            Portal.Finialize();

            timer.Enabled = true;
        }

        public void OnTick(object sender, EventArgs e)
        {
            foreach (Packet_Recv pak in OPTIONS.Login.GetPakets())
            {
                if (pak.ContainerID == 1)
                {
                    OPTIONS.clientID = pak.ObjectID;

                    timer.Enabled = false;

                    //open chat
                    
                    this.Hide();
                    var form2 = new ChatForm();
                    form2.Closed += (s, args) => this.Close();
                    form2.Show();
                }
            }

            Portal.Finialize();

            if (!OPTIONS.Connection.IsConnected)
            {
                timer.Enabled = false;

                OPTIONS.Connection.Dispose();

                OPTIONS.Connection = null;
            }
        }
    }
}
