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
			mkbCode = mkbCode.
				ToLower().
				Replace("а", "a").
				Replace("в", "b").
				Replace("с", "c").
				Replace("е", "e").
				Replace("н", "h").
				Replace("к", "k").
				Replace("м", "m").
				Replace("о", "o").
				Replace("р", "p").
				Replace("т", "t").
				Replace("х", "x").
				Replace("у", "y").
				ToUpper().
				Replace(" ", "").
				Replace("*", "").
				Replace("+", "");

			if (MkbCodes.ContainsKey(mkbCode))
				return;

			MkbCodes.Add(mkbCode, mkbCodeName);
		}

		public void AddDataRow(StandardDataRow standardDataRow) {
			DataRows.Add(standardDataRow);
		}
	}
}
