namespace MedicalEconomicStandardsParser {
	partial class FormMain {
		/// <summary>
		/// Обязательная переменная конструктора.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Освободить все используемые ресурсы.
		/// </summary>
		/// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Код, автоматически созданный конструктором форм Windows

		/// <summary>
		/// Требуемый метод для поддержки конструктора — не изменяйте 
		/// содержимое этого метода с помощью редактора кода.
		/// </summary>
		private void InitializeComponent() {
			this.listViewFiles = new System.Windows.Forms.ListView();
			this.columnHeaderName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.label1 = new System.Windows.Forms.Label();
			this.buttonAdd = new System.Windows.Forms.Button();
			this.buttonStartParse = new System.Windows.Forms.Button();
			this.buttonDeleteSelected = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.radioButtonSummary = new System.Windows.Forms.RadioButton();
			this.radioButtonSummaryToMis = new System.Windows.Forms.RadioButton();
			this.radioButtonMis = new System.Windows.Forms.RadioButton();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// listViewFiles
			// 
			this.listViewFiles.AllowDrop = true;
			this.listViewFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderName});
			this.listViewFiles.FullRowSelect = true;
			this.listViewFiles.GridLines = true;
			this.listViewFiles.Location = new System.Drawing.Point(12, 25);
			this.listViewFiles.Name = "listViewFiles";
			this.listViewFiles.Size = new System.Drawing.Size(460, 395);
			this.listViewFiles.TabIndex = 0;
			this.listViewFiles.UseCompatibleStateImageBehavior = false;
			this.listViewFiles.View = System.Windows.Forms.View.Details;
			this.listViewFiles.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.ListViewFiles_ItemSelectionChanged);
			this.listViewFiles.DragDrop += new System.Windows.Forms.DragEventHandler(this.ListViewFiles_DragDrop);
			this.listViewFiles.DragEnter += new System.Windows.Forms.DragEventHandler(this.ListViewFiles_DragEnter);
			// 
			// columnHeaderName
			// 
			this.columnHeaderName.Text = "Имя файла";
			this.columnHeaderName.Width = 454;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(165, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Список файлов для обработки:";
			// 
			// buttonAdd
			// 
			this.buttonAdd.Location = new System.Drawing.Point(397, 426);
			this.buttonAdd.Name = "buttonAdd";
			this.buttonAdd.Size = new System.Drawing.Size(75, 37);
			this.buttonAdd.TabIndex = 2;
			this.buttonAdd.Text = "Добавить файлы";
			this.buttonAdd.UseVisualStyleBackColor = true;
			this.buttonAdd.Click += new System.EventHandler(this.ButtonAdd_Click);
			// 
			// buttonStartParse
			// 
			this.buttonStartParse.Location = new System.Drawing.Point(397, 512);
			this.buttonStartParse.Name = "buttonStartParse";
			this.buttonStartParse.Size = new System.Drawing.Size(75, 37);
			this.buttonStartParse.TabIndex = 3;
			this.buttonStartParse.Text = "Начать разбор";
			this.buttonStartParse.UseVisualStyleBackColor = true;
			this.buttonStartParse.Click += new System.EventHandler(this.ButtonStartParse_Click);
			// 
			// buttonDeleteSelected
			// 
			this.buttonDeleteSelected.Enabled = false;
			this.buttonDeleteSelected.Location = new System.Drawing.Point(397, 469);
			this.buttonDeleteSelected.Name = "buttonDeleteSelected";
			this.buttonDeleteSelected.Size = new System.Drawing.Size(75, 37);
			this.buttonDeleteSelected.TabIndex = 7;
			this.buttonDeleteSelected.Text = "Удалить выбранное";
			this.buttonDeleteSelected.UseVisualStyleBackColor = true;
			this.buttonDeleteSelected.Click += new System.EventHandler(this.ButtonDeleteSelected_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.radioButtonSummary);
			this.groupBox1.Controls.Add(this.radioButtonSummaryToMis);
			this.groupBox1.Controls.Add(this.radioButtonMis);
			this.groupBox1.Location = new System.Drawing.Point(12, 426);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(379, 123);
			this.groupBox1.TabIndex = 8;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Вариант разбора";
			// 
			// radioButtonSummary
			// 
			this.radioButtonSummary.AutoSize = true;
			this.radioButtonSummary.Location = new System.Drawing.Point(6, 76);
			this.radioButtonSummary.Name = "radioButtonSummary";
			this.radioButtonSummary.Size = new System.Drawing.Size(227, 17);
			this.radioButtonSummary.TabIndex = 2;
			this.radioButtonSummary.TabStop = true;
			this.radioButtonSummary.Text = "Сводная таблица из набора стандартов";
			this.radioButtonSummary.UseVisualStyleBackColor = true;
			// 
			// radioButtonSummaryToMis
			// 
			this.radioButtonSummaryToMis.AutoSize = true;
			this.radioButtonSummaryToMis.Location = new System.Drawing.Point(6, 53);
			this.radioButtonSummaryToMis.Name = "radioButtonSummaryToMis";
			this.radioButtonSummaryToMis.Size = new System.Drawing.Size(320, 17);
			this.radioButtonSummaryToMis.TabIndex = 1;
			this.radioButtonSummaryToMis.TabStop = true;
			this.radioButtonSummaryToMis.Text = "Для загрузки в МИС \"Инфоклиника\" из сводной таблицы";
			this.radioButtonSummaryToMis.UseVisualStyleBackColor = true;
			// 
			// radioButtonMis
			// 
			this.radioButtonMis.AutoSize = true;
			this.radioButtonMis.Location = new System.Drawing.Point(6, 30);
			this.radioButtonMis.Name = "radioButtonMis";
			this.radioButtonMis.Size = new System.Drawing.Size(329, 17);
			this.radioButtonMis.TabIndex = 0;
			this.radioButtonMis.TabStop = true;
			this.radioButtonMis.Text = "Для загрузки в МИС \"Инфоклиника\" из набора стандартов";
			this.radioButtonMis.UseVisualStyleBackColor = true;
			// 
			// FormMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(484, 561);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.buttonDeleteSelected);
			this.Controls.Add(this.buttonStartParse);
			this.Controls.Add(this.buttonAdd);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.listViewFiles);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "FormMain";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Разбор медико-экономических стандартов";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ListView listViewFiles;
		private System.Windows.Forms.ColumnHeader columnHeaderName;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button buttonAdd;
		private System.Windows.Forms.Button buttonStartParse;
		private System.Windows.Forms.Button buttonDeleteSelected;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioButtonSummary;
		private System.Windows.Forms.RadioButton radioButtonSummaryToMis;
		private System.Windows.Forms.RadioButton radioButtonMis;
	}
}

