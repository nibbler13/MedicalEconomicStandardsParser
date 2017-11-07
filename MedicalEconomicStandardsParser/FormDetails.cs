using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MedicalEconomicStandardsParser {
	public partial class FormDetails : Form {
		private ExcelParser excelParser;
		private ExcelParser.WriteMode writeMode;
		private List<string> fileNames;
		private string newSection = new string('-', 50);

		public FormDetails(List<string> fileNames, ExcelParser.WriteMode writeMode) {
			InitializeComponent();
			excelParser = new ExcelParser(backgroundWorkerParse);
			this.writeMode = writeMode;
			this.fileNames = fileNames;
		}

		private void FormDetails_Load(object sender, EventArgs e) {
			Cursor = Cursors.WaitCursor;
			backgroundWorkerParse.RunWorkerAsync();
		}

		private void backgroundWorkerParse_DoWork(object sender, DoWorkEventArgs e) {
			double currentProgress = 0;

			Dictionary<string, List<string>> mkbCodes = new Dictionary<string, List<string>>();

			if (writeMode == ExcelParser.WriteMode.Mis ||
				writeMode == ExcelParser.WriteMode.SummaryToMis) {
				string mkbCodesFileName = Environment.CurrentDirectory + "\\Sources\\" + Properties.Settings.Default.MkbCodesFileName;
				backgroundWorkerParse.ReportProgress((int)currentProgress, "Считывание файла соответствия кодов МКБ с базой МИС 'Инфоклиника'" +
					Environment.NewLine + "Параметры считывания: " + Environment.NewLine +
					"Имя книги: " + mkbCodesFileName + Environment.NewLine +
					"Имя листа: " + Properties.Settings.Default.MkbCodesSheetName + Environment.NewLine +
					"Столбец DGCODE: " + Properties.Settings.Default.MkbCodesDgcodeColumn + Environment.NewLine +
					"Столбец MKBCODE: " + Properties.Settings.Default.MkbCodesMkbcodeColumn + Environment.NewLine +
					"Данные начинаются со строки: " + Properties.Settings.Default.MkbCodesFirstRowWithData +
					Environment.NewLine + newSection);

				mkbCodes = excelParser.ReadMkbCodes(mkbCodesFileName, currentProgress, 10);

				if (mkbCodes.Count == 0) {
					backgroundWorkerParse.ReportProgress((int)currentProgress, "Не удалось считать таблицу с кодами МКБ, завершение обработки");
					return;
				}
				
				currentProgress = 10;
				backgroundWorkerParse.ReportProgress((int)currentProgress, newSection);
				backgroundWorkerParse.ReportProgress((int)currentProgress, "Считано кодов: " + mkbCodes.Count);
				backgroundWorkerParse.ReportProgress((int)currentProgress, newSection);
			}

			backgroundWorkerParse.ReportProgress((int)currentProgress,
				"Считывание файлов медико-экономических стандартов" + Environment.NewLine +
				"Параметры считывания: " + Environment.NewLine +
				"Имя листа: " + Properties.Settings.Default.MESSheetName + Environment.NewLine +
				"Данные начинаются со строки: " + Properties.Settings.Default.MESFirstRowWithData + Environment.NewLine +
				"Заголовок первого столбца блока МКБ кодов: " + Properties.Settings.Default.MESMkbBlockName + Environment.NewLine +
				"Столбец МКБ10: " + Properties.Settings.Default.MESMkbCodeColumn + Environment.NewLine +
				"Столбец Диагноз: " + Properties.Settings.Default.MESMkbNameColumn + Environment.NewLine +
				"Заголовок первого столбца блока c услугами: " + Properties.Settings.Default.MESDataBlockName + Environment.NewLine +
				"Столбец Код медицинской услуги: " + Properties.Settings.Default.MESDataMedicalServiceCodeColumn + Environment.NewLine +
				"Столбец Наименование медицинской услуги: " + Properties.Settings.Default.МESDataMedicalServiceNameColumn + Environment.NewLine +
				"Столбец Усредненный показатель частоты предоставления: " + Properties.Settings.Default.МESDataAverageFrequencyOfGrantingColumn + Environment.NewLine +
				"Столбец Усредненный показатель кратности применения: " + Properties.Settings.Default.МESDataAverageIndexOfFrequencyOfApplicationColumn + Environment.NewLine +
				"Столбец Наименование услуги: " + Properties.Settings.Default.MESDataServiceNameColumn + Environment.NewLine +
				"Столбец Код: " + Properties.Settings.Default.MESDataCodeColumn + Environment.NewLine +
				"Столбец факт: " + Properties.Settings.Default.MESDataFactColumn + Environment.NewLine +
				"Столбец аванс: " + Properties.Settings.Default.MESDataPrepaidExpenseColumn + Environment.NewLine +
				"Столбец Кол-во: " + Properties.Settings.Default.MESDataCountColumn + Environment.NewLine +
				"Столбец возраст от: " + Properties.Settings.Default.MESDataAgeFromColumn + Environment.NewLine +
				"Столбец возраст до: " + Properties.Settings.Default.MESDataAgeBeforeColumn + Environment.NewLine +
				"Столбец диагностика контроль: " + Properties.Settings.Default.MESDataDiagnosticControlColumn + Environment.NewLine);
			backgroundWorkerParse.ReportProgress((int)currentProgress, newSection);

			double progressStep = (50 - currentProgress) / (double)fileNames.Count;
			List <MedicalEconomicStandard> standards = new List<MedicalEconomicStandard>();
			if (writeMode == ExcelParser.WriteMode.SummaryToMis) {
				standards = excelParser.ReadSummaryFile(fileNames[0], currentProgress, 50);
			} else
				foreach (string fileName in fileNames) {
					MedicalEconomicStandard standard = excelParser.ReadMedicalEconomicStandard(fileName, currentProgress, currentProgress + progressStep);

					if (standard != null)
						standards.Add(standard);

					currentProgress += progressStep;
				}

			if (standards.Count == 0) {
				backgroundWorkerParse.ReportProgress((int)currentProgress, "Не удалось считать ни одного файла, завершение обработки");
				return;
			}

			currentProgress = 50;
			backgroundWorkerParse.ReportProgress((int)currentProgress, newSection);
			backgroundWorkerParse.ReportProgress((int)currentProgress, "Считано стандартов: " + standards.Count);
			backgroundWorkerParse.ReportProgress((int)currentProgress, newSection);
			backgroundWorkerParse.ReportProgress((int)currentProgress, "Запись результатов в книгу Excel");

			progressStep = 100 - currentProgress;
			
			string resultFile = excelParser.ExportDataToExcel(standards, mkbCodes, writeMode, currentProgress, currentProgress + progressStep);

			currentProgress = 100;
			backgroundWorkerParse.ReportProgress((int)currentProgress, newSection);
			backgroundWorkerParse.ReportProgress((int)currentProgress, "Завершено");

			if (!string.IsNullOrEmpty(resultFile)) {
				backgroundWorkerParse.ReportProgress((int)currentProgress, "Результат записан в файл: " + resultFile);
				Process.Start(resultFile);
			} else
				backgroundWorkerParse.ReportProgress((int)currentProgress, "Не удалось записать данные");
		}

		private void backgroundWorkerParse_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
			Cursor = Cursors.Default;
			if (e.Error == null)
				MessageBox.Show("Завершено");
			else
				MessageBox.Show(e.Error.Message + Environment.NewLine + e.Error.StackTrace, "Возникла ошибка", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		private void backgroundWorkerParse_ProgressChanged(object sender, ProgressChangedEventArgs e) {
			progressBarResults.Value = e.ProgressPercentage;

			if (!string.IsNullOrEmpty(e.UserState.ToString()))
				textBoxResults.AppendText(DateTime.Now.ToString("HH:mm:ss") + ": " + e.UserState.ToString() + Environment.NewLine);
		}
	}
}
