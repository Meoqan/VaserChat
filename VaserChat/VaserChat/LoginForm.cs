using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VaserChat
{
    public partial class LoginForm : Form
    {
        ChatForm form2 = null;

        public LoginForm()
        {
            InitializeComponent();
            form2 = new ChatForm();
        }

        private void b_Connect_Click(object sender, EventArgs e)
        {
            if (tb_Username.Text.Length < 3)
            {
                MessageBox.Show("Username needs at least three chars.");
                return;
            }

            Networking.Connect(tb_Username.Text);


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

        private void Cronjob_Tick(object sender, EventArgs e)
        {
            if (Networking.IncommingLogin.Count > 0)
            {
                Message msg = Networking.IncommingLogin.Dequeue();

                if (msg.command == 1)
                {
                    OpenChatwindow();
                }

                if (msg.command == 2)
                {
                    MessageBox.Show(msg.Messagedata);
                }
            }
        }
    }
}
