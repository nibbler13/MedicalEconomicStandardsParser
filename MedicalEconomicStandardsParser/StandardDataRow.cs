using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalEconomicStandardsParser {
	public class StandardDataRow {
		public string MedicalServiceCode { get; private set; }
		public string MedicalServiceName { get; private set; }
		public string AverageFrequencyOfGranting { get; private set; }
		public string AverageIndexOfFrequencyOfApplication { get; private set; }
		public string ServiceName { get; private set; }
		public string Code { get; private set; }
		public string Fact { get; private set; }
		public string PrepaidExpense { get; private set; }
		public string Count { get; private set; }
		public string AgeFrom { get; private set; }
		public string AgeBefore { get; private set; }
		public string DiagnosticControl { get; private set; }

		public StandardDataRow(
			string medicalServiceCode,
			string medicalServiceName,
			string averageFrequencyOfGranting,
			string averageIndexOfFrequencyOfApplication,
			string serviceName,
			string code,
			string fact,
			string prepaidExpense,
			string count,
			string ageFrom,
			string ageBefore,
			string diagnosticControl) {
			MedicalServiceCode = medicalServiceCode;
			MedicalServiceName = medicalServiceName;
			AverageFrequencyOfGranting = averageFrequencyOfGranting;
			AverageIndexOfFrequencyOfApplication = averageIndexOfFrequencyOfApplication;
			ServiceName = serviceName;
			Code = code;
			Fact = fact;
			PrepaidExpense = prepaidExpense;
			Count = count;
			AgeFrom = ageFrom;
			AgeBefore = ageBefore;
			DiagnosticControl = diagnosticControl;
		}
	}
}
