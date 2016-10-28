namespace Model生成器
{
    partial class CreateModelForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnOpenXls = new System.Windows.Forms.Button();
            this.CreateModel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.labXlsPath = new System.Windows.Forms.Label();
            this.checkModel = new System.Windows.Forms.CheckBox();
            this.checkIRepository = new System.Windows.Forms.CheckBox();
            this.checkRepository = new System.Windows.Forms.CheckBox();
            this.checkFactory = new System.Windows.Forms.CheckBox();
            this.checkEvent = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnOpenXls
            // 
            this.btnOpenXls.Location = new System.Drawing.Point(14, 67);
            this.btnOpenXls.Name = "btnOpenXls";
            this.btnOpenXls.Size = new System.Drawing.Size(123, 23);
            this.btnOpenXls.TabIndex = 0;
            this.btnOpenXls.Text = "打开数据文件";
            this.btnOpenXls.UseVisualStyleBackColor = true;
            this.btnOpenXls.Click += new System.EventHandler(this.btnOpenXls_Click);
            // 
            // CreateModel
            // 
            this.CreateModel.Location = new System.Drawing.Point(14, 173);
            this.CreateModel.Name = "CreateModel";
            this.CreateModel.Size = new System.Drawing.Size(123, 23);
            this.CreateModel.TabIndex = 1;
            this.CreateModel.Text = "生成代码";
            this.CreateModel.UseVisualStyleBackColor = true;
            this.CreateModel.Click += new System.EventHandler(this.CreateModel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "文件路径：";
            // 
            // labXlsPath
            // 
            this.labXlsPath.AutoSize = true;
            this.labXlsPath.Location = new System.Drawing.Point(78, 23);
            this.labXlsPath.Name = "labXlsPath";
            this.labXlsPath.Size = new System.Drawing.Size(0, 12);
            this.labXlsPath.TabIndex = 3;
            // 
            // checkModel
            // 
            this.checkModel.AutoSize = true;
            this.checkModel.Checked = true;
            this.checkModel.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkModel.Location = new System.Drawing.Point(14, 111);
            this.checkModel.Name = "checkModel";
            this.checkModel.Size = new System.Drawing.Size(54, 16);
            this.checkModel.TabIndex = 4;
            this.checkModel.Text = "Model";
            this.checkModel.UseVisualStyleBackColor = true;
            // 
            // checkIRepository
            // 
            this.checkIRepository.AutoSize = true;
            this.checkIRepository.Checked = true;
            this.checkIRepository.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkIRepository.Location = new System.Drawing.Point(70, 111);
            this.checkIRepository.Name = "checkIRepository";
            this.checkIRepository.Size = new System.Drawing.Size(90, 16);
            this.checkIRepository.TabIndex = 5;
            this.checkIRepository.Text = "IRepository";
            this.checkIRepository.UseVisualStyleBackColor = true;
            // 
            // checkRepository
            // 
            this.checkRepository.AutoSize = true;
            this.checkRepository.Checked = true;
            this.checkRepository.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkRepository.Location = new System.Drawing.Point(162, 111);
            this.checkRepository.Name = "checkRepository";
            this.checkRepository.Size = new System.Drawing.Size(84, 16);
            this.checkRepository.TabIndex = 6;
            this.checkRepository.Text = "Repository";
            this.checkRepository.UseVisualStyleBackColor = true;
            // 
            // checkFactory
            // 
            this.checkFactory.AutoSize = true;
            this.checkFactory.Checked = true;
            this.checkFactory.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkFactory.Location = new System.Drawing.Point(13, 134);
            this.checkFactory.Name = "checkFactory";
            this.checkFactory.Size = new System.Drawing.Size(66, 16);
            this.checkFactory.TabIndex = 7;
            this.checkFactory.Text = "Factory";
            this.checkFactory.UseVisualStyleBackColor = true;
            // 
            // checkEvent
            // 
            this.checkEvent.AutoSize = true;
            this.checkEvent.Checked = true;
            this.checkEvent.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkEvent.Location = new System.Drawing.Point(86, 134);
            this.checkEvent.Name = "checkEvent";
            this.checkEvent.Size = new System.Drawing.Size(54, 16);
            this.checkEvent.TabIndex = 8;
            this.checkEvent.Text = "Event";
            this.checkEvent.UseVisualStyleBackColor = true;
            // 
            // CreateModelForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(345, 261);
            this.Controls.Add(this.checkEvent);
            this.Controls.Add(this.checkFactory);
            this.Controls.Add(this.checkRepository);
            this.Controls.Add(this.checkIRepository);
            this.Controls.Add(this.checkModel);
            this.Controls.Add(this.labXlsPath);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.CreateModel);
            this.Controls.Add(this.btnOpenXls);
            this.Name = "CreateModelForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "代码生成器";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOpenXls;
        private System.Windows.Forms.Button CreateModel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labXlsPath;
        private System.Windows.Forms.CheckBox checkModel;
        private System.Windows.Forms.CheckBox checkIRepository;
        private System.Windows.Forms.CheckBox checkRepository;
        private System.Windows.Forms.CheckBox checkFactory;
        private System.Windows.Forms.CheckBox checkEvent;
    }
}

