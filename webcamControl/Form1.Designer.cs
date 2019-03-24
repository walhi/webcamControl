namespace webcamControl
{
    partial class Form1
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

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.updateButton = new System.Windows.Forms.Button();
            this.overWindowToggleButton = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // updateButton
            // 
            this.updateButton.Location = new System.Drawing.Point(162, 12);
            this.updateButton.Name = "updateButton";
            this.updateButton.Size = new System.Drawing.Size(146, 23);
            this.updateButton.TabIndex = 1;
            this.updateButton.Text = "Update";
            this.updateButton.UseVisualStyleBackColor = true;
            this.updateButton.Click += new System.EventHandler(this.updateButton_Click);
            // 
            // overWindowToggleButton
            // 
            this.overWindowToggleButton.Location = new System.Drawing.Point(12, 12);
            this.overWindowToggleButton.Name = "overWindowToggleButton";
            this.overWindowToggleButton.Size = new System.Drawing.Size(144, 23);
            this.overWindowToggleButton.TabIndex = 2;
            this.overWindowToggleButton.Text = "Over all windows";
            this.overWindowToggleButton.UseVisualStyleBackColor = true;
            this.overWindowToggleButton.CheckedChanged += new System.EventHandler(this.overWindowToggleButton_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(320, 49);
            this.Controls.Add(this.overWindowToggleButton);
            this.Controls.Add(this.updateButton);
            this.Name = "Form1";
            this.Text = "Webcam Focus control";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button updateButton;
        private System.Windows.Forms.CheckBox overWindowToggleButton;
    }
}

