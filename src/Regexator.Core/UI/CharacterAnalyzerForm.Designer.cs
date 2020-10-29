using Regexator.Windows.Forms;
namespace Regexator.UI
{
    partial class CharacterAnalyzerForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this._pnlChar = new System.Windows.Forms.Panel();
            this._rtbChar = new Regexator.Windows.Forms.ExtendedRichTextBox();
            this._lblChar = new System.Windows.Forms.Label();
            this._pnlCode = new System.Windows.Forms.Panel();
            this._rtbCode = new Regexator.Windows.Forms.ExtendedRichTextBox();
            this._lblHex = new System.Windows.Forms.Label();
            this._pnlHex = new System.Windows.Forms.Panel();
            this._rtbHex = new Regexator.Windows.Forms.ExtendedRichTextBox();
            this._gbxRegexOptions = new System.Windows.Forms.GroupBox();
            this._cbxECMAScript = new System.Windows.Forms.CheckBox();
            this._cbxIgnoreCase = new System.Windows.Forms.CheckBox();
            this._pnlGrid = new System.Windows.Forms.Panel();
            this._dgv = new Regexator.Windows.Forms.ExtendedDataGridView();
            this._cbxCharGroup = new System.Windows.Forms.CheckBox();
            this._lblComment = new System.Windows.Forms.Label();
            this._lblCode = new System.Windows.Forms.Label();
            this._pnlChar.SuspendLayout();
            this._pnlCode.SuspendLayout();
            this._pnlHex.SuspendLayout();
            this._gbxRegexOptions.SuspendLayout();
            this._pnlGrid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dgv)).BeginInit();
            this.SuspendLayout();
            // 
            // _pnlChar
            // 
            this._pnlChar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._pnlChar.Controls.Add(this._rtbChar);
            this._pnlChar.Location = new System.Drawing.Point(147, 15);
            this._pnlChar.Margin = new System.Windows.Forms.Padding(5);
            this._pnlChar.Name = "_pnlChar";
            this._pnlChar.Size = new System.Drawing.Size(266, 20);
            this._pnlChar.TabIndex = 7;
            // 
            // _rtbChar
            // 
            this._rtbChar.AcceptsTab = true;
            this._rtbChar.AllowDrop = true;
            this._rtbChar.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._rtbChar.Dock = System.Windows.Forms.DockStyle.Fill;
            this._rtbChar.Location = new System.Drawing.Point(0, 0);
            this._rtbChar.Multiline = false;
            this._rtbChar.Name = "_rtbChar";
            this._rtbChar.NoTab = false;
            this._rtbChar.Size = new System.Drawing.Size(264, 18);
            this._rtbChar.SpaceCount = 4;
            this._rtbChar.TabIndex = 0;
            this._rtbChar.Text = "";
            this._rtbChar.WordWrap = false;
            // 
            // _lblChar
            // 
            this._lblChar.AutoSize = true;
            this._lblChar.Location = new System.Drawing.Point(13, 16);
            this._lblChar.Margin = new System.Windows.Forms.Padding(3, 0, 20, 0);
            this._lblChar.Name = "_lblChar";
            this._lblChar.Size = new System.Drawing.Size(61, 15);
            this._lblChar.TabIndex = 0;
            this._lblChar.Text = "Character:";
            // 
            // _pnlCode
            // 
            this._pnlCode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._pnlCode.Controls.Add(this._rtbCode);
            this._pnlCode.Location = new System.Drawing.Point(147, 45);
            this._pnlCode.Margin = new System.Windows.Forms.Padding(5);
            this._pnlCode.Name = "_pnlCode";
            this._pnlCode.Size = new System.Drawing.Size(266, 20);
            this._pnlCode.TabIndex = 14;
            // 
            // _rtbCode
            // 
            this._rtbCode.AcceptsTab = true;
            this._rtbCode.AllowDrop = true;
            this._rtbCode.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._rtbCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this._rtbCode.Location = new System.Drawing.Point(0, 0);
            this._rtbCode.Multiline = false;
            this._rtbCode.Name = "_rtbCode";
            this._rtbCode.NoTab = false;
            this._rtbCode.Size = new System.Drawing.Size(264, 18);
            this._rtbCode.SpaceCount = 4;
            this._rtbCode.TabIndex = 0;
            this._rtbCode.Text = "";
            this._rtbCode.WordWrap = false;
            // 
            // _lblHex
            // 
            this._lblHex.AutoSize = true;
            this._lblHex.Location = new System.Drawing.Point(13, 76);
            this._lblHex.Margin = new System.Windows.Forms.Padding(3, 0, 20, 0);
            this._lblHex.Name = "_lblHex";
            this._lblHex.Size = new System.Drawing.Size(109, 15);
            this._lblHex.TabIndex = 2;
            this._lblHex.Text = "Hexadecimal value:";
            // 
            // _pnlHex
            // 
            this._pnlHex.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._pnlHex.Controls.Add(this._rtbHex);
            this._pnlHex.Location = new System.Drawing.Point(147, 75);
            this._pnlHex.Margin = new System.Windows.Forms.Padding(5);
            this._pnlHex.Name = "_pnlHex";
            this._pnlHex.Size = new System.Drawing.Size(266, 20);
            this._pnlHex.TabIndex = 16;
            // 
            // _rtbHex
            // 
            this._rtbHex.AcceptsTab = true;
            this._rtbHex.AllowDrop = true;
            this._rtbHex.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._rtbHex.Dock = System.Windows.Forms.DockStyle.Fill;
            this._rtbHex.Location = new System.Drawing.Point(0, 0);
            this._rtbHex.Multiline = false;
            this._rtbHex.Name = "_rtbHex";
            this._rtbHex.NoTab = false;
            this._rtbHex.Size = new System.Drawing.Size(264, 18);
            this._rtbHex.SpaceCount = 4;
            this._rtbHex.TabIndex = 0;
            this._rtbHex.Text = "";
            this._rtbHex.WordWrap = false;
            // 
            // _gbxRegexOptions
            // 
            this._gbxRegexOptions.Controls.Add(this._cbxECMAScript);
            this._gbxRegexOptions.Controls.Add(this._cbxIgnoreCase);
            this._gbxRegexOptions.Location = new System.Drawing.Point(421, 13);
            this._gbxRegexOptions.Name = "_gbxRegexOptions";
            this._gbxRegexOptions.Padding = new System.Windows.Forms.Padding(10, 5, 5, 5);
            this._gbxRegexOptions.Size = new System.Drawing.Size(150, 82);
            this._gbxRegexOptions.TabIndex = 3;
            this._gbxRegexOptions.TabStop = false;
            this._gbxRegexOptions.Text = "Regex Options";
            // 
            // _cbxECMAScript
            // 
            this._cbxECMAScript.AutoSize = true;
            this._cbxECMAScript.Location = new System.Drawing.Point(13, 49);
            this._cbxECMAScript.Name = "_cbxECMAScript";
            this._cbxECMAScript.Size = new System.Drawing.Size(92, 19);
            this._cbxECMAScript.TabIndex = 2;
            this._cbxECMAScript.Text = "ECMA Script";
            this._cbxECMAScript.UseVisualStyleBackColor = true;
            // 
            // _cbxIgnoreCase
            // 
            this._cbxIgnoreCase.AutoSize = true;
            this._cbxIgnoreCase.Location = new System.Drawing.Point(13, 24);
            this._cbxIgnoreCase.Name = "_cbxIgnoreCase";
            this._cbxIgnoreCase.Size = new System.Drawing.Size(88, 19);
            this._cbxIgnoreCase.TabIndex = 1;
            this._cbxIgnoreCase.Text = "Ignore Case";
            this._cbxIgnoreCase.UseVisualStyleBackColor = true;
            // 
            // _pnlGrid
            // 
            this._pnlGrid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._pnlGrid.Controls.Add(this._dgv);
            this._pnlGrid.Location = new System.Drawing.Point(13, 130);
            this._pnlGrid.Name = "_pnlGrid";
            this._pnlGrid.Size = new System.Drawing.Size(558, 268);
            this._pnlGrid.TabIndex = 19;
            // 
            // _dgv
            // 
            this._dgv.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.WhiteSmoke;
            this._dgv.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this._dgv.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this._dgv.BackgroundColor = System.Drawing.SystemColors.Control;
            this._dgv.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._dgv.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this._dgv.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this._dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dgv.Dock = System.Windows.Forms.DockStyle.Fill;
            this._dgv.EnableHeadersVisualStyles = false;
            this._dgv.Location = new System.Drawing.Point(0, 0);
            this._dgv.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
            this._dgv.Name = "_dgv";
            this._dgv.ShowCellToolTips = false;
            this._dgv.Size = new System.Drawing.Size(556, 266);
            this._dgv.StandardTab = true;
            this._dgv.TabIndex = 0;
            // 
            // _cbxCharGroup
            // 
            this._cbxCharGroup.AutoSize = true;
            this._cbxCharGroup.Location = new System.Drawing.Point(434, 103);
            this._cbxCharGroup.Margin = new System.Windows.Forms.Padding(5);
            this._cbxCharGroup.Name = "_cbxCharGroup";
            this._cbxCharGroup.Size = new System.Drawing.Size(126, 19);
            this._cbxCharGroup.TabIndex = 4;
            this._cbxCharGroup.Text = "In Character Group";
            this._cbxCharGroup.UseVisualStyleBackColor = true;
            // 
            // _lblComment
            // 
            this._lblComment.AutoSize = true;
            this._lblComment.Location = new System.Drawing.Point(145, 104);
            this._lblComment.Margin = new System.Windows.Forms.Padding(5);
            this._lblComment.Name = "_lblComment";
            this._lblComment.Size = new System.Drawing.Size(45, 15);
            this._lblComment.TabIndex = 20;
            this._lblComment.Text = "...";
            // 
            // _lblCode
            // 
            this._lblCode.AutoSize = true;
            this._lblCode.Location = new System.Drawing.Point(13, 46);
            this._lblCode.Margin = new System.Windows.Forms.Padding(3, 0, 20, 0);
            this._lblCode.Name = "_lblCode";
            this._lblCode.Size = new System.Drawing.Size(84, 15);
            this._lblCode.TabIndex = 21;
            this._lblCode.Text = "Decimal value:";
            // 
            // CharacterAnalyzerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 411);
            this.Controls.Add(this._lblCode);
            this.Controls.Add(this._lblComment);
            this.Controls.Add(this._cbxCharGroup);
            this.Controls.Add(this._pnlGrid);
            this.Controls.Add(this._gbxRegexOptions);
            this.Controls.Add(this._lblHex);
            this.Controls.Add(this._pnlHex);
            this.Controls.Add(this._pnlCode);
            this.Controls.Add(this._lblChar);
            this.Controls.Add(this._pnlChar);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.Name = "CharacterAnalyzerForm";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.Text = "Character Analyzer";
            this._pnlChar.ResumeLayout(false);
            this._pnlCode.ResumeLayout(false);
            this._pnlHex.ResumeLayout(false);
            this._gbxRegexOptions.ResumeLayout(false);
            this._gbxRegexOptions.PerformLayout();
            this._pnlGrid.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._dgv)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ExtendedRichTextBox _rtbChar;
        private System.Windows.Forms.Panel _pnlChar;
        private Regexator.Windows.Forms.ExtendedDataGridView _dgv;
        private System.Windows.Forms.Label _lblChar;
        private System.Windows.Forms.Panel _pnlCode;
        private ExtendedRichTextBox _rtbCode;
        private System.Windows.Forms.Label _lblHex;
        private System.Windows.Forms.Panel _pnlHex;
        private ExtendedRichTextBox _rtbHex;
        private System.Windows.Forms.GroupBox _gbxRegexOptions;
        private System.Windows.Forms.CheckBox _cbxECMAScript;
        private System.Windows.Forms.CheckBox _cbxIgnoreCase;
        private System.Windows.Forms.Panel _pnlGrid;
        private System.Windows.Forms.CheckBox _cbxCharGroup;
        private System.Windows.Forms.Label _lblComment;
        private System.Windows.Forms.Label _lblCode;
    }
}