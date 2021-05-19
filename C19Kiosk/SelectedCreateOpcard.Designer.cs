
namespace C19Kiosk
{
    partial class SelectedCreateOpcard
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectedCreateOpcard));
            this.label1 = new System.Windows.Forms.Label();
            this.confirmBtn = new System.Windows.Forms.Button();
            this.cancelBtn = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("TH K2D July8", 32.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.OliveDrab;
            this.label1.Location = new System.Drawing.Point(-2, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(1056, 56);
            this.label1.TabIndex = 0;
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // confirmBtn
            // 
            this.confirmBtn.Font = new System.Drawing.Font("TH K2D July8", 36F, System.Drawing.FontStyle.Bold);
            this.confirmBtn.Location = new System.Drawing.Point(372, 176);
            this.confirmBtn.Name = "confirmBtn";
            this.confirmBtn.Size = new System.Drawing.Size(256, 72);
            this.confirmBtn.TabIndex = 1;
            this.confirmBtn.Text = "ลงทะเบียนใหม่";
            this.confirmBtn.UseVisualStyleBackColor = true;
            this.confirmBtn.Click += new System.EventHandler(this.confirmBtn_Click);
            // 
            // cancelBtn
            // 
            this.cancelBtn.Font = new System.Drawing.Font("TH K2D July8", 36F, System.Drawing.FontStyle.Bold);
            this.cancelBtn.Location = new System.Drawing.Point(636, 176);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(256, 72);
            this.cancelBtn.TabIndex = 2;
            this.cancelBtn.Text = "ยกเลิก";
            this.cancelBtn.UseVisualStyleBackColor = true;
            this.cancelBtn.Click += new System.EventHandler(this.cancelBtn_Click);
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("TH K2D July8", 32.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.OliveDrab;
            this.label2.Location = new System.Drawing.Point(10, 80);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(1056, 56);
            this.label2.TabIndex = 3;
            this.label2.Text = "กดที่ปุ่ม \"ลงทะเบียนใหม่\" เพื่อทำการลงทะเบียนในระบบของโรงพยาบาล";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SelectedCreateOpcard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1064, 264);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cancelBtn);
            this.Controls.Add(this.confirmBtn);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SelectedCreateOpcard";
            this.Text = "ออก VN ";
            this.Load += new System.EventHandler(this.SelectedCreateOpcard_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button confirmBtn;
        private System.Windows.Forms.Button cancelBtn;
        private System.Windows.Forms.Label label2;
    }
}