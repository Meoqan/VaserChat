namespace VaserChat
{
    partial class LoginForm
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.tb_Username = new System.Windows.Forms.TextBox();
            this.b_Connect = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.tb_ServerAddress = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.Cronjob = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 18);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(131, 26);
            this.label1.TabIndex = 0;
            this.label1.Text = "Welcome to VaserChat!\r\nPlease set your username.";
            // 
            // tb_Username
            // 
            this.tb_Username.Location = new System.Drawing.Point(127, 86);
            this.tb_Username.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tb_Username.Name = "tb_Username";
            this.tb_Username.Size = new System.Drawing.Size(96, 20);
            this.tb_Username.TabIndex = 1;
            // 
            // b_Connect
            // 
            this.b_Connect.Location = new System.Drawing.Point(225, 80);
            this.b_Connect.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.b_Connect.Name = "b_Connect";
            this.b_Connect.Size = new System.Drawing.Size(101, 29);
            this.b_Connect.TabIndex = 2;
            this.b_Connect.Text = "Connect";
            this.b_Connect.UseVisualStyleBackColor = true;
            this.b_Connect.Click += new System.EventHandler(this.b_Connect_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(65, 86);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Username:";
            // 
            // tb_ServerAddress
            // 
            this.tb_ServerAddress.Location = new System.Drawing.Point(127, 55);
            this.tb_ServerAddress.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tb_ServerAddress.Name = "tb_ServerAddress";
            this.tb_ServerAddress.Size = new System.Drawing.Size(96, 20);
            this.tb_ServerAddress.TabIndex = 4;
            this.tb_ServerAddress.Text = "localhost";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(41, 57);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Server Address:";
            // 
            // Cronjob
            // 
            this.Cronjob.Enabled = true;
            this.Cronjob.Tick += new System.EventHandler(this.Cronjob_Tick);
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(397, 139);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tb_ServerAddress);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.b_Connect);
            this.Controls.Add(this.tb_Username);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MaximizeBox = false;
            this.Name = "LoginForm";
            this.Text = "VaserChat Login";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tb_Username;
        private System.Windows.Forms.Button b_Connect;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tb_ServerAddress;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Timer Cronjob;
    }
}

