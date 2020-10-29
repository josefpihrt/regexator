namespace Regexator.UI
{
    partial class NewItemForm
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
            this._btnCancel = new System.Windows.Forms.Button();
            this._btnOk = new System.Windows.Forms.Button();
            this._lsv = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // _btnCancel
            // 
            this._btnCancel.Location = new System.Drawing.Point(218, 165);
            this._btnCancel.Margin = new System.Windows.Forms.Padding(6);
            this._btnCancel.Name = "_btnCancel";
            this._btnCancel.Size = new System.Drawing.Size(100, 30);
            this._btnCancel.TabIndex = 4;
            this._btnCancel.Text = "Cancel";
            this._btnCancel.UseVisualStyleBackColor = true;
            // 
            // _btnOk
            // 
            this._btnOk.Location = new System.Drawing.Point(106, 165);
            this._btnOk.Margin = new System.Windows.Forms.Padding(6);
            this._btnOk.Name = "_btnOk";
            this._btnOk.Size = new System.Drawing.Size(100, 30);
            this._btnOk.TabIndex = 3;
            this._btnOk.Text = "OK";
            this._btnOk.UseVisualStyleBackColor = true;
            // 
            // _lsv
            // 
            this._lsv.Location = new System.Drawing.Point(13, 13);
            this._lsv.Name = "_lsv";
            this._lsv.Size = new System.Drawing.Size(305, 143);
            this._lsv.TabIndex = 5;
            this._lsv.UseCompatibleStateImageBehavior = false;
            // 
            // NewProjectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 211);
            this.Controls.Add(this._lsv);
            this.Controls.Add(this._btnCancel);
            this.Controls.Add(this._btnOk);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.Name = "NewProjectForm";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.Text = "New Item";
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.Button _btnCancel;
        internal System.Windows.Forms.Button _btnOk;
        internal System.Windows.Forms.ListView _lsv;
    }
}