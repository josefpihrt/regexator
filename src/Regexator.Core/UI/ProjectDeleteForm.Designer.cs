namespace Regexator.UI
{
    partial class ProjectDeleteForm
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
            this._btnOk = new System.Windows.Forms.Button();
            this._btnCancel = new System.Windows.Forms.Button();
            this._cbxDeleteAllFileInputs = new System.Windows.Forms.CheckBox();
            this._lblMessage = new System.Windows.Forms.Label();
            this._pbxIcon = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this._pbxIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // _btnOk
            // 
            this._btnOk.Location = new System.Drawing.Point(215, 118);
            this._btnOk.Name = "_btnOk";
            this._btnOk.Size = new System.Drawing.Size(100, 30);
            this._btnOk.TabIndex = 2;
            this._btnOk.Text = "OK";
            this._btnOk.UseVisualStyleBackColor = true;
            // 
            // _btnCancel
            // 
            this._btnCancel.Location = new System.Drawing.Point(321, 118);
            this._btnCancel.Name = "_btnCancel";
            this._btnCancel.Size = new System.Drawing.Size(100, 30);
            this._btnCancel.TabIndex = 3;
            this._btnCancel.Text = "Cancel";
            this._btnCancel.UseVisualStyleBackColor = true;
            // 
            // _cbxDeleteAllFileInputs
            // 
            this._cbxDeleteAllFileInputs.AutoSize = true;
            this._cbxDeleteAllFileInputs.Location = new System.Drawing.Point(20, 118);
            this._cbxDeleteAllFileInputs.Margin = new System.Windows.Forms.Padding(3, 3, 3, 10);
            this._cbxDeleteAllFileInputs.Name = "_cbxDeleteAllFileInputs";
            this._cbxDeleteAllFileInputs.Size = new System.Drawing.Size(129, 19);
            this._cbxDeleteAllFileInputs.TabIndex = 1;
            this._cbxDeleteAllFileInputs.Text = "Delete all file inputs";
            this._cbxDeleteAllFileInputs.UseVisualStyleBackColor = true;
            this._cbxDeleteAllFileInputs.Visible = false;
            // 
            // _lblMessage
            // 
            this._lblMessage.AutoEllipsis = true;
            this._lblMessage.Location = new System.Drawing.Point(72, 20);
            this._lblMessage.Margin = new System.Windows.Forms.Padding(10, 10, 10, 3);
            this._lblMessage.Name = "_lblMessage";
            this._lblMessage.Size = new System.Drawing.Size(349, 92);
            this._lblMessage.TabIndex = 0;
            this._lblMessage.Text = "A project will be deleted permanently.";
            // 
            // _pbxIcon
            // 
            this._pbxIcon.Location = new System.Drawing.Point(20, 20);
            this._pbxIcon.Margin = new System.Windows.Forms.Padding(10);
            this._pbxIcon.Name = "_pbxIcon";
            this._pbxIcon.Size = new System.Drawing.Size(32, 32);
            this._pbxIcon.TabIndex = 5;
            this._pbxIcon.TabStop = false;
            // 
            // ProjectDeleteForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 161);
            this.Controls.Add(this._pbxIcon);
            this.Controls.Add(this._lblMessage);
            this.Controls.Add(this._cbxDeleteAllFileInputs);
            this.Controls.Add(this._btnCancel);
            this.Controls.Add(this._btnOk);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.Name = "ProjectDeleteForm";
            this.Padding = new System.Windows.Forms.Padding(10);
            ((System.ComponentModel.ISupportInitialize)(this._pbxIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _btnOk;
        private System.Windows.Forms.Button _btnCancel;
        private System.Windows.Forms.CheckBox _cbxDeleteAllFileInputs;
        private System.Windows.Forms.Label _lblMessage;
        private System.Windows.Forms.PictureBox _pbxIcon;
    }
}