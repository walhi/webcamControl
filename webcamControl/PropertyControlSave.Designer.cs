namespace webcamControl
{
    partial class PropertyControlSave
    {
        /// <summary> 
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.label = new System.Windows.Forms.Label();
            this.cbAuto = new System.Windows.Forms.CheckBox();
            this.trackBar = new System.Windows.Forms.TrackBar();
            this.cbAll = new System.Windows.Forms.CheckBox();
            this.cbUSB = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // label
            // 
            this.label.AutoSize = true;
            this.label.Enabled = false;
            this.label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label.Location = new System.Drawing.Point(3, 3);
            this.label.MaximumSize = new System.Drawing.Size(89, 13);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(46, 13);
            this.label.TabIndex = 17;
            this.label.Text = "Property";
            // 
            // cbAuto
            // 
            this.cbAuto.AutoSize = true;
            this.cbAuto.Enabled = false;
            this.cbAuto.Location = new System.Drawing.Point(92, 3);
            this.cbAuto.Name = "cbAuto";
            this.cbAuto.Size = new System.Drawing.Size(15, 14);
            this.cbAuto.TabIndex = 16;
            this.cbAuto.UseVisualStyleBackColor = true;
            this.cbAuto.CheckedChanged += new System.EventHandler(this.cbAuto_CheckedChanged);
            // 
            // trackBar
            // 
            this.trackBar.AutoSize = false;
            this.trackBar.Enabled = false;
            this.trackBar.Location = new System.Drawing.Point(113, 3);
            this.trackBar.Name = "trackBar";
            this.trackBar.Size = new System.Drawing.Size(145, 17);
            this.trackBar.TabIndex = 15;
            this.trackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar.Scroll += new System.EventHandler(this.trackBar_Scroll);
            // 
            // cbAll
            // 
            this.cbAll.AutoSize = true;
            this.cbAll.Location = new System.Drawing.Point(264, 3);
            this.cbAll.Name = "cbAll";
            this.cbAll.Size = new System.Drawing.Size(15, 14);
            this.cbAll.TabIndex = 18;
            this.cbAll.UseVisualStyleBackColor = true;
            // 
            // cbUSB
            // 
            this.cbUSB.AutoSize = true;
            this.cbUSB.Location = new System.Drawing.Point(285, 3);
            this.cbUSB.Name = "cbUSB";
            this.cbUSB.Size = new System.Drawing.Size(15, 14);
            this.cbUSB.TabIndex = 19;
            this.cbUSB.UseVisualStyleBackColor = true;
            // 
            // PropertyControlSave
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cbUSB);
            this.Controls.Add(this.cbAll);
            this.Controls.Add(this.label);
            this.Controls.Add(this.cbAuto);
            this.Controls.Add(this.trackBar);
            this.Name = "PropertyControlSave";
            this.Size = new System.Drawing.Size(303, 19);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label;
        private System.Windows.Forms.CheckBox cbAuto;
        private System.Windows.Forms.TrackBar trackBar;
        private System.Windows.Forms.CheckBox cbAll;
        private System.Windows.Forms.CheckBox cbUSB;
    }
}
