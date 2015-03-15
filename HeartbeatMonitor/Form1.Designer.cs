namespace HeartbeatMonitor
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;


        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblError = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblError
            // 
            this.lblError.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblError.Location = new System.Drawing.Point(12, 24);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(260, 20);
            this.lblError.TabIndex = 0;
            this.lblError.Text = "-";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 67);
            this.Controls.Add(this.lblError);
            this.Name = "Form1";
            this.Text = "Heartbeat monitor";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblError;
    }
}

