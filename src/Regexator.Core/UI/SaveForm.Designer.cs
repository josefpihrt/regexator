namespace Regexator.UI
{
    partial class SaveForm
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
            this._lblQuestion = new System.Windows.Forms.Label();
            this._btnYes = new System.Windows.Forms.Button();
            this._btnCancel = new System.Windows.Forms.Button();
            this._btnNo = new System.Windows.Forms.Button();
            this._trv = new Regexator.UI.SaveFormTreeView();
            this.SuspendLayout();
            // 
            // _lblQuestion
            // 
            this._lblQuestion.AutoSize = true;
            this._lblQuestion.Location = new System.Drawing.Point(13, 10);
            this._lblQuestion.Name = "_lblQuestion";
            this._lblQuestion.Padding = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this._lblQuestion.Size = new System.Drawing.Size(202, 17);
            this._lblQuestion.TabIndex = 0;
            this._lblQuestion.Text = "Save changes to the following items?";
            // 
            // _btnYes
            // 
            this._btnYes.Location = new System.Drawing.Point(59, 218);
            this._btnYes.Name = "_btnYes";
            this._btnYes.Size = new System.Drawing.Size(100, 30);
            this._btnYes.TabIndex = 2;
            this._btnYes.Text = "Yes";
            this._btnYes.UseVisualStyleBackColor = true;
            // 
            // _btnCancel
            // 
            this._btnCancel.Location = new System.Drawing.Point(271, 218);
            this._btnCancel.Name = "_btnCancel";
            this._btnCancel.Size = new System.Drawing.Size(100, 30);
            this._btnCancel.TabIndex = 4;
            this._btnCancel.Text = "Cancel";
            this._btnCancel.UseVisualStyleBackColor = true;
            // 
            // _btnNo
            // 
            this._btnNo.Location = new System.Drawing.Point(165, 218);
            this._btnNo.Name = "_btnNo";
            this._btnNo.Size = new System.Drawing.Size(100, 30);
            this._btnNo.TabIndex = 3;
            this._btnNo.Text = "No";
            this._btnNo.UseVisualStyleBackColor = true;
            // 
            // _trv
            // 
            this._trv.FullRowSelect = true;
            this._trv.HideSelection = false;
            this._trv.Location = new System.Drawing.Point(13, 30);
            this._trv.Name = "_trv";
            this._trv.ShowLines = false;
            this._trv.ShowPlusMinus = false;
            this._trv.Size = new System.Drawing.Size(358, 182);
            this._trv.TabIndex = 1;
            // 
            // SaveForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 261);
            this.Controls.Add(this._trv);
            this.Controls.Add(this._btnNo);
            this.Controls.Add(this._btnCancel);
            this.Controls.Add(this._btnYes);
            this.Controls.Add(this._lblQuestion);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.Name = "SaveForm";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _lblQuestion;
        private System.Windows.Forms.Button _btnYes;
        private System.Windows.Forms.Button _btnCancel;
        private System.Windows.Forms.Button _btnNo;
        private SaveFormTreeView _trv;
    }
}