using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalEconomicStandardsParser {
	public class ExcelParser {
		private BackgroundWorker backgroundWorker;
		public enum WriteMode { Mis, Summary, SummaryToMis }
		private string insertToMis = "insert into medstandarts " +
			"(MEDID, DIAGID, SCHID, VOZRASTFROM, POL, SCOUNT, POKAZ, VOZRASTTO, ONLYIT, REPL$ID, REPL$GRPID, MRAZDEL, NOTJUSTINDAY, AGEUNITTYPE) " +
			"values(" +
			"gen_id(sprav_gen,1)," +
			"'@diagid'," +
			"(select schid from wschema where kodoper = '@schid' and structid = 3)," +
			"'@vozrastfrom'," +
			"'@pol'," +
			"'@scount'," +
			"'@pokaz'," +
			"'@vozrastto'," +
			"'@onlyit', " +
			"gen_id (repl$medstandarts_gen, 1)," +
			"'99'," +
			"'@mrazdel'," +
			"'0'," +
			"'0');";




		public ExcelParser(BackgroundWorker backgroundWorker) {
			this.backgroundWorker = backgroundWorker;
		}



		
		public MedicalEconomicStandard ReadMedicalEconomicStandard(string fileName, double startProgress, double maxProgress) {
			double currentProgress = startProgress;

			MedicalEconomicStandard standard = new MedicalEconomicStandard(fileName);
			DataTable dataTable = ReadExcelFile(fileName, Properties.Settings.Default.MESSheetName, (int)currentProgress);

			if (dataTable.Rows.Count == 0) {
				backgroundWorker.ReportProgress((int)currentProgress, "Таблица не содержит данных");
				return null;
			}

			double progressStep = (maxProgress - currentProgress) / (double)dataTable.Rows.Count;

			try {
				string standardName = dataTable.Rows[0][0].ToString();
				standard.SetStandardName(standardName);
			} catch (Exception e) {
				backgroundWorker.ReportProgress((int)currentProgress, "Не удалось прочитать название стандарта из ячейки А1: " + e.Message);
				return null;
			}

			int currentType = 0; //0 - skip, 1 - mkb, 2 - data
			int columnOffset = 0;
			for (int i = Properties.Settings.Default.MESFirstRowWithData - 1; i < dataTable.Rows.Count; i++) {
				try {
					string firstColumn = dataTable.Rows[i][0].ToString();

					if (firstColumn.ToLower().Equals(Properties.Settings.Default.MESMkbBlockName.ToLower())) {
						backgroundWorker.ReportProgress((int)currentType, "Обнаружен блок МКБ кодов, строка: " + (i + 1));
						currentType = 1;
						continue;
					} else if (firstColumn.ToLower().Equals(Properties.Settings.Default.MESDataBlockName.ToLower())) {
						backgroundWorker.ReportProgress((int)currentType, "Обнаружен блок услуг, строка: " + (i + 1));
						currentType = 2;
						continue;
					} else if (string.IsNullOrEmpty(firstColumn) ||
						string.IsNullOrWhiteSpace(firstColumn)) {
						currentType = 0;
						continue;
					}

					if (currentType == 1) {
						string mkbCode = dataTable.Rows[i][columnOffset + GetExcelColumnNumber(Properties.Settings.Default.MESMkbCodeColumn) - 1].ToString();
						string mkbCodeName = dataTable.Rows[i][columnOffset + GetExcelColumnNumber(Properties.Settings.Default.MESMkbNameColumn) - 1].ToString();
						standard.AddMkbCode(mkbCode, mkbCodeName);
					} else if (currentType == 2)
						standard.AddDataRow(ParseRowToStandardRow(dataTable.Rows[i], columnOffset));
				} catch (Exception e) {
					backgroundWorker.ReportProgress((int)currentProgress, "Не удалось обработать строку №" + (i + 1) + ", " + e.Message);
				}

				currentProgress += progressStep;
				backgroundWorker.ReportProgress((int)currentProgress, "");
			}

			if (standard.MkbCodes.Count == 0) {
				backgroundWorker.ReportProgress((int)currentProgress, "В стандарте отсутствует блок МКБ кодов, пропуск");
				return null;
			}

			if (standard.DataRows.Count == 0) {
				backgroundWorker.ReportProgress((int)currentProgress, "В стандарте отсутствует блок услуг, пропуск");
				return null;
			}

			return standard;
		}

		public List<MedicalEconomicStandard> ReadSummaryFile(string fileFullPath, double startProgress, double maxProgress) {
			double currentProgress = startProgress;
			List<MedicalEconomicStandard> standards = new List<MedicalEconomicStandard>();
			DataTable dataTable = ReadExcelFile(fileFullPath, "", (int)currentProgress);

			if (dataTable.Rows.Count == 0) {
				backgroundWorker.ReportProgress((int)currentProgress, "Таблица не содержит данных");
				return standards;
			}

			if (dataTable.Columns.Count != 16) {
				backgroundWorker.ReportProgress((int)currentProgress, 
					"Количество столбцов в файле не соответствует сводной таблице (должно быть 16)");
				return standards;
			}

			double progressStep = (maxProgress - currentProgress) / (double)dataTable.Rows.Count;
			string currentFilePath = "";
			string splitter = " | ";

			for (int externalRow = 1; externalRow < dataTable.Rows.Count; externalRow++) {
				try {
					currentFilePath = dataTable.Rows[externalRow][0].ToString();

					MedicalEconomicStandard standard = new MedicalEconomicStandard(currentFilePath);

					string standardName = dataTable.Rows[externalRow][1].ToString();
					standard.SetStandardName(standardName);

					string mkbCode = dataTable.Rows[externalRow][2].ToString();
					string diagnose = dataTable.Rows[externalRow][3].ToString();
					
					if (mkbCode.Contains(splitter) && diagnose.Contains(splitter)) {
						string[] mkbCodes = mkbCode.Split(new string[] { splitter }, StringSplitOptions.None);
						string[] diagnoses = diagnose.Split(new string[] { splitter }, StringSplitOptions.None);

						for (int x = 0; x < mkbCodes.Length; x++)
							standard.AddMkbCode(mkbCodes[x], diagnoses[x]);
					} else
						standard.AddMkbCode(mkbCode, diagnose);

					for (int innerRow = externalRow; innerRow < dataTable.Rows.Count; innerRow++) {
						try {
							string filePath = dataTable.Rows[innerRow][0].ToString();

							if (!filePath.Equals(currentFilePath))
								break;

							standard.AddDataRow(ParseRowToStandardRow(dataTable.Rows[innerRow], 4));
						} catch (Exception e) {
							backgroundWorker.ReportProgress((int)currentProgress, 
								"Не удалось разобрать строку №" + (innerRow + 1) + ", " + e.Message);
						}

						externalRow = innerRow;
						currentProgress += progressStep;
						backgroundWorker.ReportProgress((int)currentProgress, "");
					}

					if (standard.MkbCodes.Count == 0) {
						backgroundWorker.ReportProgress((int)currentProgress, "В стандарте отсутствует блок МКБ кодов, пропуск");
						continue;
					}

					if (standard.DataRows.Count == 0) {
						backgroundWorker.ReportProgress((int)currentProgress, "В стандарте отсутствует блок услуг, пропуск");
						continue;
					}

					standards.Add(standard);
				} catch (Exception e) {
					backgroundWorker.ReportProgress((int)currentProgress, "Не удалось обработать строку №" + (externalRow + 1) + ", " + e.Message);
				}
			}

			return standards;
		}

		public Dictionary<string, List<string>> ReadMkbCodes(string mkbCodesFileName, double startProgress, double maxProgress) {
			double currentProgress = startProgress;
			Dictionary <string, List<string>> mkbCodes = new Dictionary<string, List<string>>();
			string sheetName = Properties.Settings.Default.MkbCodesSheetName;
			DataTable dataTable = ReadExcelFile(mkbCodesFileName, sheetName, (int)currentProgress);

			if (dataTable.Rows.Count == 0) {
				backgroundWorker.ReportProgress((int)currentProgress, "Таблица не содержит данных");
				return mkbCodes;
			}

			if (dataTable.Rows.Count - Properties.Settings.Default.MkbCodesFirstRowWithData < 1) {
				backgroundWorker.ReportProgress((int)currentProgress, "Нет данных для обработки");
				return mkbCodes;
			}

			double progressStep = (double) (maxProgress - startProgress) / (double) (dataTable.Rows.Count - Properties.Settings.Default.MkbCodesFirstRowWithData);

			for (int i = Properties.Settings.Default.MkbCodesFirstRowWithData - 1; i < dataTable.Rows.Count; i++) {
				try {
					string mkbCode = dataTable.Rows[i][GetExcelColumnNumber(Properties.Settings.Default.MkbCodesMkbcodeColumn) - 1].ToString();
					string misCode = dataTable.Rows[i][GetExcelColumnNumber(Properties.Settings.Default.MkbCodesDgcodeColumn) - 1].ToString();

					if (mkbCodes.ContainsKey(mkbCode))
						mkbCodes[mkbCode].Add(misCode);
					else
						mkbCodes.Add(mkbCode, new List<string>() { misCode });
				} catch (Exception e) {
					backgroundWorker.ReportProgress((int)currentProgress, "Не удалось обработать строку №" + (i + 1) + ", " + e.Message);
				}

				currentProgress += progressStep;
				backgroundWorker.ReportProgress((int)currentProgress, "");
			}
			
			return mkbCodes;
		}
		
		private DataTable ReadExcelFile(string fileFullPath, string sheetName, int currentProgress) {
			DataTable dataTable = new DataTable();
			backgroundWorker.ReportProgress((int)currentProgress, "Считывание файла: " + fileFullPath);

			if (!File.Exists(fileFullPath)) {
				backgroundWorker.ReportProgress(currentProgress, "Не удается найти файл: " + fileFullPath);
				return dataTable;
			}

			try {
				using (OleDbConnection conn = new OleDbConnection()) {
					conn.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileFullPath + ";" +
						"Extended Properties='Excel 12.0 Xml;HDR=NO;'";

					using (OleDbCommand comm = new OleDbCommand()) {
						if (string.IsNullOrEmpty(sheetName)) {
							conn.Open();
							DataTable dtSchema = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
							sheetName = dtSchema.Rows[0].Field<string>("TABLE_NAME");
							conn.Close();
						} else
							sheetName += "$";

						comm.CommandText = "Select * from [" + sheetName + "]";
						comm.Connection = conn;

						using (OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter()) {
							oleDbDataAdapter.SelectCommand = comm;
							oleDbDataAdapter.Fill(dataTable);
						}
					}
				}
			} catch (Exception e) {
				backgroundWorker.ReportProgress(0, e.Message);
			}

			return dataTable;
		}




		public string ExportDataToExcel(List<MedicalEconomicStandard> standards, Dictionary<string, List<string>> mkbCodes,
			WriteMode writeMode, double progressFrom, double progressTo) {
			double progressCurrent = progressFrom;
			string templateFile = Environment.CurrentDirectory + "\\Sources\\";
			string resultFilePrefix = "Result";

			backgroundWorker.ReportProgress((int)progressCurrent, "Выгрузка данных в Excel, формат: " + writeMode);

			switch (writeMode) {
				case WriteMode.Mis:
				case WriteMode.SummaryToMis:
					templateFile += Properties.Settings.Default.TemplateFileNameMis;
					resultFilePrefix += "Mis_";
					break;
				case WriteMode.Summary:
					templateFile += Properties.Settings.Default.TemplateFileNameSummary;
					resultFilePrefix += "Summary_";
					break;
				default:
					backgroundWorker.ReportProgress((int)progressCurrent, "Неизвестный формат выгрузки данных, пропуск");
					return string.Empty;
			}

			if (!File.Exists(templateFile)) {
				backgroundWorker.ReportProgress((int)progressCurrent, "Не удалось найти файл шаблона: " + templateFile);
				return string.Empty;
			}

			string resultFile = Environment.CurrentDirectory + "\\Results\\" + resultFilePrefix + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";
			try {
				File.Copy(templateFile, resultFile);
			} catch (Exception e) {
				backgroundWorker.ReportProgress((int)progressCurrent, "Не удалось скопировать файл шаблона в новый файл: " + resultFile + ", " + e.Message);
				return string.Empty;
			}

			int totalRows = 0;
			foreach (MedicalEconomicStandard standard in standards)
				totalRows += standard.MkbCodes.Count * standard.DataRows.Count;

			double progressStep = (progressTo - progressFrom) / totalRows;

			IWorkbook workbook;
			using (FileStream stream = new FileStream(resultFile, FileMode.Open, FileAccess.Read)) {
				workbook = new XSSFWorkbook(stream);
				stream.Close();
			}

			switch (writeMode) {
				case WriteMode.Mis:
				case WriteMode.SummaryToMis:
					WriteDataMisMode(ref workbook, standards, mkbCodes, progressCurrent, progressStep);
					break;
				case WriteMode.Summary:
					WriteDataSummaryMode(ref workbook, standards, progressCurrent, progressStep);
					break;
				default:
					backgroundWorker.ReportProgress((int)progressCurrent, "Неверный формат выгрузки");
					break;
			}

			using (FileStream stream = new FileStream(resultFile, FileMode.Open, FileAccess.Write)) {
				workbook.Write(stream);
				stream.Close();
			}

			workbook.Close();

			return resultFile;
		}

		private void WriteDataSummaryMode(ref IWorkbook workbook, List<MedicalEconomicStandard> standards, double progressFrom, double progressStep) {
			double progressCurrent = progressFrom;

			int rowNumber = 1;
			int columnNumber = 0;
			int sheetNumber = 1;

			ISheet sheet = workbook.GetSheet("Data");
			ICreationHelper creationHelper = workbook.GetCreationHelper();

			foreach (MedicalEconomicStandard standard in standards) {
				string mkbCodes = "";
				string diagnoses = "";

				foreach (KeyValuePair<string, string> mkbCode in standard.MkbCodes) {
					mkbCodes += mkbCode.Key + " | ";
					diagnoses += mkbCode.Value + " | ";
				}

				if (mkbCodes.Length > 3)
					mkbCodes = mkbCodes.Substring(0, mkbCodes.Length - 3);

				if (diagnoses.Length > 3)
					diagnoses = diagnoses.Substring(0, diagnoses.Length - 3);

				foreach (StandardDataRow dataRow in standard.DataRows) {
					backgroundWorker.ReportProgress((int)progressCurrent, "");
					progressCurrent += progressStep;
					IRow row = sheet.CreateRow(rowNumber);

					string[] data = new string[] {
						standard.FileName,
						standard.StandardName,
						mkbCodes,
						diagnoses,
						dataRow.MedicalServiceCode,
						dataRow.MedicalServiceName,
						dataRow.AverageFrequencyOfGranting,
						dataRow.AverageIndexOfFrequencyOfApplication,
						dataRow.ServiceName,
						dataRow.Code,
						dataRow.Fact,
						dataRow.PrepaidExpense,
						dataRow.Count,
						dataRow.AgeFrom,
						dataRow.AgeBefore,
						dataRow.DiagnosticControl
					};

					foreach (string value in data) {
						ICell cell = row.CreateCell(columnNumber);
						cell.SetCellValue(creationHelper.CreateRichTextString(value));
						columnNumber++;
					}

					columnNumber = 0;
					rowNumber++;

					if (rowNumber > 1000000) {
						rowNumber = 0;
						sheet = workbook.CreateSheet("Data" + sheetNumber);
						sheetNumber++;
					}
				}
			}
		}

		private void WriteDataMisMode(ref IWorkbook workbook, List<MedicalEconomicStandard> standards, Dictionary<string, List<string>> mkbCodes,
			double progressFrom, double progressStep) {
			double progressCurrent = progressFrom;

			int rowNumber = 1;
			int columnNumber = 0;
			int sheetNumber = 1;

			ISheet sheet = workbook.GetSheet("Data");
			ICreationHelper creationHelper = workbook.GetCreationHelper();

			foreach (MedicalEconomicStandard standard in standards) {
				foreach (StandardDataRow dataRow in standard.DataRows) {
					backgroundWorker.ReportProgress((int)progressCurrent, "");
					progressCurrent += progressStep;

					if (dataRow.Code.ToLower().Equals("нет") ||
						string.IsNullOrEmpty(dataRow.Code) ||
						string.IsNullOrWhiteSpace(dataRow.Code)) {
						//backgroundWorker.ReportProgress((int)progressCurrent, "Отсутствует код услуги в МИС, пропуск" + Environment.NewLine +
						//	"Файл: " + standard.FileName + Environment.NewLine +
						//	"Код медицинской услуги: " + dataRow.MedicalServiceCode);
						continue;
					}

					List<string[]> convertedValues = new List<string[]> {
						new string[] { ConvertPeriodValueToMis(dataRow.Fact), ConvertSectionValueToMis(dataRow.DiagnosticControl, true)},
						new string[] { ConvertPeriodValueToMis(dataRow.PrepaidExpense), ConvertSectionValueToMis(dataRow.DiagnosticControl, false) }
					};

					foreach (string[] convertedValue in convertedValues) {
						if (convertedValue[0].Equals("-1") ||
							convertedValue[1].Equals("-1")) {
							backgroundWorker.ReportProgress((int)progressCurrent,
							"Не удалось корректно распознать все значения в строке (факт или аванс), пропуск" + Environment.NewLine +
							"Файл: " + standard.FileName + Environment.NewLine +
							"Код медицинской услуги: " + dataRow.MedicalServiceCode);
							continue;
						}

						foreach (KeyValuePair<string, string> mkbCode in standard.MkbCodes) {
							foreach (string code in mkbCodes[mkbCode.Key]) {
								IRow row = sheet.CreateRow(rowNumber);

								string insertToMisReplaced = insertToMis.
									Replace("@diagid", code).
									Replace("@schid", dataRow.Code).
									Replace("@vozrastfrom", dataRow.AgeFrom).
									Replace("@pol", "0").
									Replace("@scount", "1").
									Replace("@pokaz", convertedValue[0]).
									Replace("@vozrastto", dataRow.AgeBefore).
									Replace("@onlyit", "0").
									Replace("@mrazdel", convertedValue[1]);

								string[] data = new string[] {
									mkbCode.Key,
									code,
									dataRow.Code,
									dataRow.AgeFrom,
									dataRow.AgeBefore,
									"0",
									"1",
									convertedValue[0],
									"0",
									convertedValue[1],
									insertToMisReplaced
								};

								foreach (string value in data) {
									ICell cell = row.CreateCell(columnNumber);
									cell.SetCellValue(creationHelper.CreateRichTextString(value));
									columnNumber++;
								}
							}

							columnNumber = 0;
							rowNumber++;

							if (rowNumber > 1000000) {
								rowNumber = 0;
								sheet = workbook.CreateSheet("Data" + sheetNumber);
								sheetNumber++;
							}
						}
					}
				}
			}
		}




		private StandardDataRow ParseRowToStandardRow(DataRow row, int columnOffset) {
			string medicalServiceCode = row[columnOffset + GetExcelColumnNumber(Properties.Settings.Default.MESDataMedicalServiceCodeColumn) - 1].ToString();
			string medicalServiceName = row[columnOffset + GetExcelColumnNumber(Properties.Settings.Default.МESDataMedicalServiceNameColumn) - 1].ToString();
			string averageFrequencyOfGranting = row[columnOffset + GetExcelColumnNumber(Properties.Settings.Default.МESDataAverageFrequencyOfGrantingColumn) - 1].ToString();
			string averageIndexOfFrequencyOfApplication = row[columnOffset + GetExcelColumnNumber(Properties.Settings.Default.МESDataAverageIndexOfFrequencyOfApplicationColumn) - 1].ToString();
			string serviceName = row[columnOffset + GetExcelColumnNumber(Properties.Settings.Default.MESDataServiceNameColumn) - 1].ToString();
			string code = row[columnOffset + GetExcelColumnNumber(Properties.Settings.Default.MESDataCodeColumn) - 1].ToString();
			string fact = row[columnOffset + GetExcelColumnNumber(Properties.Settings.Default.MESDataFactColumn) - 1].ToString();
			string prepaidExpense = row[columnOffset + GetExcelColumnNumber(Properties.Settings.Default.MESDataPrepaidExpenseColumn) - 1].ToString();
			string count = row[columnOffset + GetExcelColumnNumber(Properties.Settings.Default.MESDataCountColumn) - 1].ToString();
			string ageFrom = row[columnOffset + GetExcelColumnNumber(Properties.Settings.Default.MESDataAgeFromColumn) - 1].ToString();
			string ageBefore = row[columnOffset + GetExcelColumnNumber(Properties.Settings.Default.MESDataAgeBeforeColumn) - 1].ToString();
			string diagnosticControl = row[columnOffset + GetExcelColumnNumber(Properties.Settings.Default.MESDataDiagnosticControlColumn) - 1].ToString();

			StandardDataRow standardDataRow = new StandardDataRow(
				medicalServiceCode,
				medicalServiceName,
				averageFrequencyOfGranting,
				averageIndexOfFrequencyOfApplication,
				serviceName,
				code,
				fact,
				prepaidExpense,
				count,
				ageFrom,
				ageBefore,
				diagnosticControl);

			return standardDataRow;
		}

		private string ConvertSectionValueToMis(string value, bool isFact) {
			value = value.ToLower();

			if (value.StartsWith("диагност"))
				if (isFact)
					return "4";
				else
					return "990000005";
			else if (value.StartsWith("контр"))
				if (isFact)
					return "5";
				else
					return "990000007";
			else
				return "-1";
		}

		private string ConvertPeriodValueToMis(string value) {
			value = value.ToLower();

			if (value.StartsWith("обязат"))
				return "0";
			else if (value.StartsWith("по пока"))
				return "1";
			else if (value.StartsWith("дополнит"))
				return "2";
			else if (value.StartsWith("внешн"))
				return "3";
			else
				return "-1";
		}

		private int GetExcelColumnNumber(string columnName) {
			if (string.IsNullOrEmpty(columnName)) throw new ArgumentNullException("columnName");

			columnName = columnName.ToUpperInvariant();

			int sum = 0;

			for (int i = 0; i < columnName.Length; i++) {
				sum *= 26;
				sum += (columnName[i] - 'A' + 1);
			}

			return sum;
		}

		private string GetExcelColumnName(int columnNumber) {
			int dividend = columnNumber;
			string columnName = String.Empty;
			int modulo;

			while (dividend > 0) {
				modulo = (dividend - 1) % 26;
				columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
				dividend = (int)((dividend - modulo) / 26);
			}

			return columnName;
		}
	}
}
