namespace Regexator.UI
{
    partial class ErrorInfoForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._lblMessage = new System.Windows.Forms.Label();
            this._btnOK = new System.Windows.Forms.Button();
            this._pnl = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // _lblMessage
            // 
            this._lblMessage.AutoSize = true;
            this._lblMessage.Location = new System.Drawing.Point(13, 10);
            this._lblMessage.Name = "_lblMessage";
            this._lblMessage.Padding = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this._lblMessage.Size = new System.Drawing.Size(195, 17);
            this._lblMessage.TabIndex = 0;
            this._lblMessage.Text = "Following files could not be loaded:";
            // 
            // _btnOK
            // 
            this._btnOK.Location = new System.Drawing.Point(321, 218);
            this._btnOK.Name = "_btnOK";
            this._btnOK.Size = new System.Drawing.Size(100, 30);
            this._btnOK.TabIndex = 2;
            this._btnOK.Text = "OK";
            this._btnOK.UseVisualStyleBackColor = true;
            // 
            // _pnl
            // 
            this._pnl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._pnl.Location = new System.Drawing.Point(16, 30);
            this._pnl.Name = "_pnl";
            this._pnl.Size = new System.Drawing.Size(405, 182);
            this._pnl.TabIndex = 3;
            // 
            // ErrorInfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 261);
            this.Controls.Add(this._pnl);
            this.Controls.Add(this._btnOK);
            this.Controls.Add(this._lblMessage);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.Name = "ErrorInfoForm";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _lblMessage;
        private System.Windows.Forms.Button _btnOK;
        private System.Windows.Forms.Panel _pnl;
    }
}