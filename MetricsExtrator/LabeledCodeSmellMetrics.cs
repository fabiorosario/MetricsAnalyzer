using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetricsExtrator
{
    public class LabeledCodeSmellMetrics
    {
        private int _severity;
        public string CodeSmell { get; set; }
        public string Project { get; set; }
        public string Package { get; set; }
        public string ClassName { get; set; }
        public string Method { get; set; }
        public bool IsSmell { get; set; }
        public int SeverityLevel { get; set; }
        public int Severity 
        {
            get { return _severity; }
            set 
            {
                if (!string.IsNullOrEmpty(CodeSmell) && value > 0)
                {
                    _severity = NewSeverity();
                }
                _severity = value;
            }
        }
        public int ATFD_method { get; set; }
        public int FDP_method { get; set; }
        public int MAXNESTING_method { get; set; }
        public int LOC_method { get; set; }
        public int CYCLO_method { get; set; }
        public int NOLV_method { get; set; }
        public int NOAV_method { get; set; }
        public int FANOUT_method { get; set; }
        public int CFNAMM_method { get; set; }
        public int ATLD_method { get; set; }
        public int CINT_method { get; set; }
        public double CDISP_method { get; set; }
        public double AMW_type { get; set; }
        public int ATFD_type { get; set; }
        public int CFNAMM_type { get; set; }
        public int DIT_type { get; set; }
        public int FANOUT_type { get; set; }
        public double LCOM5_type { get; set; }
        public int LOCNAMM_type { get; set; }
        public int LOC_package { get; set; }
        public int LOC_type { get; set; }
        public int NIM_type { get; set; }
        public int NOA_type { get; set; }
        public int NOCS_package { get; set; }
        public int NOCS_project { get; set; }
        public int NOMNAMM_project { get; set; }
        public int NOMNAMM_type { get; set; }
        public int NOM_project { get; set; }
        public int RFC_type { get; set; }
        public double TCC_type { get; set; }
        public int WMCNAMM_type { get; set; }
        public int WMC_type { get; set; }
        public double WOC_type { get; set; }
        public int NOM_type { get; set; }
        public double AMWNAMM_type { get; set; }
        public int NOCS_type { get; set; }
        public int NOM_package { get; set; }
        public int NOMNAMM_package { get; set; }
        public int LOCNAMM_package { get; set; }
        public int LOC_project { get; set; }
        public int LOCNAMM_project { get; set; }
        public int NOAM_type { get; set; }
        public int NOP_method { get; set; }
        public int MaMCL_method { get; set; }
        public double MeMCL_method { get; set; }
        public int CLNAMM_method { get; set; }
        public int num_not_final_not_static_attributes {  get; set; }
        public int number_private_visibility_attributes { get; set; }

        private int NewSeverity()
        {
            int severity = Severity;

            if (CodeSmell.Equals("Long Method"))
            {
                severity = severity + 3;
            }
            else if (CodeSmell.Equals("Data Class"))
            {
                severity = severity + 6;
            }
            else if (CodeSmell.Equals("Large Class"))
            {
                severity = severity + 9;
            }
            else if (CodeSmell.Equals("Refused Bequest"))
            {
                severity = severity + 12;
            }

            return severity;
        }
    }
}
