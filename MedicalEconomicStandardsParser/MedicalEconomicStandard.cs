using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalEconomicStandardsParser {
	public class MedicalEconomicStandard {
		public string FileName { get; private set; }
		public string StandardName { get; private set; }
		public Dictionary<string, string> MkbCodes { get; private set; }
		public List<StandardDataRow> DataRows { get; private set; }

		public MedicalEconomicStandard(string fileName) {
			FileName = fileName;
			MkbCodes = new Dictionary<string, string>();
			DataRows = new List<StandardDataRow>();
		}

		public void SetStandardName(string standardName) {
			StandardName = standardName;
		}

		public void AddMkbCode(string mkbCode, string mkbCodeName) {
			if (MkbCodes.ContainsKey(mkbCode))
				return;

			MkbCodes.Add(mkbCode, mkbCodeName);
		}

		public void AddDataRow(StandardDataRow standardDataRow) {
			DataRows.Add(standardDataRow);
		}
	}
}
