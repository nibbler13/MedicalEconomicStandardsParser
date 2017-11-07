using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MedicalEconomicStandardsParser {
	public partial class FormMain : Form {
		public FormMain() {
			InitializeComponent();
		}

		private void ButtonAdd_Click(object sender, EventArgs e) {
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Книга Excel (*.xls*)|*.xls*";
			openFileDialog.CheckFileExists = true;
			openFileDialog.CheckPathExists = true;
			openFileDialog.Multiselect = true;
			openFileDialog.RestoreDirectory = true;

			if (openFileDialog.ShowDialog() == DialogResult.OK)
				AddItemsToListView(openFileDialog.FileNames);

			openFileDialog.Dispose();
		}

		private void ButtonDeleteSelected_Click(object sender, EventArgs e) {
			foreach (ListViewItem item in listViewFiles.SelectedItems)
				listViewFiles.Items.Remove(item);
		}

		private void ButtonStartParse_Click(object sender, EventArgs e) {
			//ExcelWriteTest.TransferXLToTable();

			int filesCount = listViewFiles.Items.Count;
			if (filesCount == 0) {
				MessageBox.Show("Не выбрано ни одного файла", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			} else if (radioButtonSummaryToMis.Checked && filesCount > 1) {
				MessageBox.Show("Для варианта разбора из суммарной таблицы допускается выбор только одного файла",
					"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}
			
			List<string> fileNames = new List<string>();
			foreach (ListViewItem item in listViewFiles.Items)
				fileNames.Add(item.Name);

			ExcelParser.WriteMode writeMode;
			if (radioButtonMis.Checked)
				writeMode = ExcelParser.WriteMode.Mis;
			else if (radioButtonSummary.Checked)
				writeMode = ExcelParser.WriteMode.Summary;
			else if (radioButtonSummaryToMis.Checked)
				writeMode = ExcelParser.WriteMode.SummaryToMis;
			else {
				MessageBox.Show("Не выбран вариант разбора", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}

			FormDetails formParse = new FormDetails(fileNames, writeMode);
			formParse.ShowDialog();
		}

		private void ListViewFiles_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e) {
			buttonDeleteSelected.Enabled = listViewFiles.SelectedItems.Count > 0;
		}

		private void ListViewFiles_DragEnter(object sender, DragEventArgs e) {
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
				e.Effect = DragDropEffects.Link;
		}

		private void ListViewFiles_DragDrop(object sender, DragEventArgs e) {
			string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

			List<string> fileNames = new List<string>();

			foreach (string fileName in files) {
				FileAttributes fileAttribute = File.GetAttributes(fileName);
				if ((fileAttribute & FileAttributes.Directory) == FileAttributes.Directory) {
					try {
						string[] filesInDirectory = Directory.GetFiles(fileName, "*", System.IO.SearchOption.AllDirectories);
						fileNames.AddRange(filesInDirectory);
					} catch (Exception exception) {
						Console.WriteLine(exception.Message + " " + exception.StackTrace);
					}
				} else {
					fileNames.Add(fileName);
				}
			}

			if (fileNames.Count == 0)
				return;
			
			AddItemsToListView(fileNames.ToArray());
		}

		private void AddItemsToListView(string[] fileNames) {
			foreach (string fileFullPath in fileNames) {
				if (!fileFullPath.Contains(".xls"))
					continue;

				string fileName = Path.GetFileName(fileFullPath);

				if (listViewFiles.Items.ContainsKey(fileName))
					continue;

				ListViewItem item = new ListViewItem(fileName);
				item.Name = fileFullPath;
				listViewFiles.Items.Add(item);
			}
		}
	}
}
