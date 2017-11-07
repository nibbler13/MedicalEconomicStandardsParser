namespace MedicalEconomicStandardsParser {
	partial class FormDetails {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.textBoxResults = new System.Windows.Forms.TextBox();
			this.progressBarResults = new System.Windows.Forms.ProgressBar();
			this.backgroundWorkerParse = new System.ComponentModel.BackgroundWorker();
			this.SuspendLayout();
			// 
			// textBoxResults
			// 
			this.textBoxResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxResults.Location = new System.Drawing.Point(12, 12);
			this.textBoxResults.MaxLength = 3276700;
			this.textBoxResults.Multiline = true;
			this.textBoxResults.Name = "textBoxResults";
			this.textBoxResults.ReadOnly = true;
			this.textBoxResults.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textBoxResults.Size = new System.Drawing.Size(460, 508);
			this.textBoxResults.TabIndex = 0;
			// 
			// progressBarResults
			// 
			this.progressBarResults.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.progressBarResults.Location = new System.Drawing.Point(12, 526);
			this.progressBarResults.Name = "progressBarResults";
			this.progressBarResults.Size = new System.Drawing.Size(460, 23);
			this.progressBarResults.TabIndex = 1;
			// 
			// backgroundWorkerParse
			// 
			this.backgroundWorkerParse.WorkerReportsProgress = true;
			this.backgroundWorkerParse.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerParse_DoWork);
			this.backgroundWorkerParse.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorkerParse_ProgressChanged);
			this.backgroundWorkerParse.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerParse_RunWorkerCompleted);
			// 
			// FormDetails
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(484, 561);
			this.Controls.Add(this.progressBarResults);
			this.Controls.Add(this.textBoxResults);
			this.Name = "FormDetails";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Выполнение разбора";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Load += new System.EventHandler(this.FormDetails_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox textBoxResults;
		private System.Windows.Forms.ProgressBar progressBarResults;
		private System.ComponentModel.BackgroundWorker backgroundWorkerParse;
	}
}