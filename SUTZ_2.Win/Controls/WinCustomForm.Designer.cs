namespace SUTZ_2.Win
{
    partial class WinCustomForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        
        public WinCustomUserControl CustomUserControl;
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.CustomUserControl = new SUTZ_2.Win.WinCustomUserControl();
            this.SuspendLayout();
            // 
            // CustomUserControl
            // 
            this.CustomUserControl.AutoScroll = true;
            this.CustomUserControl.AutoSize = true;
            this.CustomUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CustomUserControl.Location = new System.Drawing.Point(0, 0);
            this.CustomUserControl.Margin = new System.Windows.Forms.Padding(0);
            this.CustomUserControl.Name = "CustomUserControl";
            this.CustomUserControl.Size = new System.Drawing.Size(472, 613);
            this.CustomUserControl.TabIndex = 0;
            // 
            // WinCustomForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(472, 613);
            this.Controls.Add(this.CustomUserControl);
            this.Name = "WinCustomForm";
            this.Text = "WinCustomForm";
            this.Load += new System.EventHandler(this.WinCustomForm_Load);
            this.Shown += new System.EventHandler(this.WinCustomForm_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion        
    }
    
}