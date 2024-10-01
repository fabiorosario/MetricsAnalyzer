using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using CsvHelper;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetricsExtrator
{
    public class CsvExporter
    {
        public void ExportToCsv(List<LabeledCodeSmellMetrics> labeledCodeSmellMetricsList, string filePath)
        {
            if (labeledCodeSmellMetricsList.Any())
            {

                var project = labeledCodeSmellMetricsList[0].Project;

                using (var writer = new StreamWriter($"{filePath}_{project}.csv"))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteField("Code Smell");
                    csv.WriteField("Project");
                    csv.WriteField("Package");
                    csv.WriteField("Complextype");
                    csv.WriteField("Method");
                    csv.WriteField("IsSmell");
                    csv.WriteField("SeverityLevel");
                    csv.WriteField("Severity");
                    csv.WriteField("ATFD_method");
                    csv.WriteField("FDP_method");
                    csv.WriteField("MAXNESTING_method");
                    csv.WriteField("LOC_method");
                    csv.WriteField("CYCLO_method");
                    csv.WriteField("NOLV_method");
                    csv.WriteField("NOAV_method");
                    csv.WriteField("FANOUT_method");
                    csv.WriteField("CFNAMM_method");
                    csv.WriteField("ATLD_method");
                    csv.WriteField("CINT_method");
                    csv.WriteField("CDISP_method");
                    csv.WriteField("AMW_type");
                    csv.WriteField("ATFD_type");
                    csv.WriteField("CFNAMM_type");
                    csv.WriteField("DIT_type");
                    csv.WriteField("FANOUT_type");
                    csv.WriteField("LCOM5_type");
                    csv.WriteField("LOCNAMM_type");
                    csv.WriteField("LOC_package");
                    csv.WriteField("LOC_type");
                    csv.WriteField("NIM_type");
                    csv.WriteField("NOA_type");
                    csv.WriteField("NOCS_package");
                    csv.WriteField("NOCS_project");
                    csv.WriteField("NOMNAMM_project");
                    csv.WriteField("NOMNAMM_type");
                    csv.WriteField("NOM_project");
                    csv.WriteField("RFC_type");
                    csv.WriteField("TCC_type");
                    csv.WriteField("WMCNAMM_type");
                    csv.WriteField("WMC_type");
                    csv.WriteField("WOC_type");
                    csv.WriteField("NOM_type");
                    csv.WriteField("AMWNAMM_type");
                    csv.WriteField("NOCS_type");
                    csv.WriteField("NOM_package");
                    csv.WriteField("NOMNAMM_package");
                    csv.WriteField("LOCNAMM_package");
                    csv.WriteField("LOC_project");
                    csv.WriteField("LOCNAMM_project");
                    csv.WriteField("NOAM_type");
                    csv.WriteField("NOP_method");
                    csv.WriteField("MaMCL_method");
                    csv.WriteField("MeMCL_method");
                    csv.WriteField("CLNAMM_method");
                    csv.WriteField("num_not_final_not_static_attributes");
                    csv.WriteField("number_private_visibility_attributes");
                    csv.NextRecord();


                    foreach (var labeledCodeSmellMetrics in labeledCodeSmellMetricsList)
                    {
                        csv.WriteField(labeledCodeSmellMetrics.CodeSmell);
                        csv.WriteField(labeledCodeSmellMetrics.Project);
                        csv.WriteField(labeledCodeSmellMetrics.Package);
                        csv.WriteField(labeledCodeSmellMetrics.ClassName);
                        csv.WriteField(labeledCodeSmellMetrics.Method);
                        csv.WriteField(labeledCodeSmellMetrics.IsSmell ? 1 : 0);
                        csv.WriteField(labeledCodeSmellMetrics.SeverityLevel);
                        csv.WriteField(labeledCodeSmellMetrics.Severity);
                        csv.WriteField(labeledCodeSmellMetrics.ATFD_method);
                        csv.WriteField(labeledCodeSmellMetrics.FDP_method);
                        csv.WriteField(labeledCodeSmellMetrics.MAXNESTING_method);
                        csv.WriteField(labeledCodeSmellMetrics.LOC_method);
                        csv.WriteField(labeledCodeSmellMetrics.CYCLO_method);
                        csv.WriteField(labeledCodeSmellMetrics.NOLV_method);
                        csv.WriteField(labeledCodeSmellMetrics.NOAV_method);
                        csv.WriteField(labeledCodeSmellMetrics.FANOUT_method);
                        csv.WriteField(labeledCodeSmellMetrics.CFNAMM_method);
                        csv.WriteField(labeledCodeSmellMetrics.ATLD_method);
                        csv.WriteField(labeledCodeSmellMetrics.CINT_method);
                        csv.WriteField(labeledCodeSmellMetrics.CDISP_method);
                        csv.WriteField(labeledCodeSmellMetrics.AMW_type);
                        csv.WriteField(labeledCodeSmellMetrics.ATFD_type);
                        csv.WriteField(labeledCodeSmellMetrics.CFNAMM_type);
                        csv.WriteField(labeledCodeSmellMetrics.DIT_type);
                        csv.WriteField(labeledCodeSmellMetrics.FANOUT_type);
                        csv.WriteField(labeledCodeSmellMetrics.LCOM5_type);
                        csv.WriteField(labeledCodeSmellMetrics.LOCNAMM_type);
                        csv.WriteField(labeledCodeSmellMetrics.LOC_package);
                        csv.WriteField(labeledCodeSmellMetrics.LOC_type);
                        csv.WriteField(labeledCodeSmellMetrics.NIM_type);
                        csv.WriteField(labeledCodeSmellMetrics.NOA_type);
                        csv.WriteField(labeledCodeSmellMetrics.NOCS_package);
                        csv.WriteField(labeledCodeSmellMetrics.NOCS_project);
                        csv.WriteField(labeledCodeSmellMetrics.NOMNAMM_project);
                        csv.WriteField(labeledCodeSmellMetrics.NOMNAMM_type);
                        csv.WriteField(labeledCodeSmellMetrics.NOM_project);
                        csv.WriteField(labeledCodeSmellMetrics.RFC_type);
                        csv.WriteField(labeledCodeSmellMetrics.TCC_type);
                        csv.WriteField(labeledCodeSmellMetrics.WMCNAMM_type);
                        csv.WriteField(labeledCodeSmellMetrics.WMC_type);
                        csv.WriteField(labeledCodeSmellMetrics.WOC_type);
                        csv.WriteField(labeledCodeSmellMetrics.NOM_type);
                        csv.WriteField(labeledCodeSmellMetrics.AMWNAMM_type);
                        csv.WriteField(labeledCodeSmellMetrics.NOCS_type);
                        csv.WriteField(labeledCodeSmellMetrics.NOM_package);
                        csv.WriteField(labeledCodeSmellMetrics.NOMNAMM_package);
                        csv.WriteField(labeledCodeSmellMetrics.LOCNAMM_package);
                        csv.WriteField(labeledCodeSmellMetrics.LOC_project);
                        csv.WriteField(labeledCodeSmellMetrics.LOCNAMM_project);
                        csv.WriteField(labeledCodeSmellMetrics.NOAM_type);
                        csv.WriteField(labeledCodeSmellMetrics.NOP_method);
                        csv.WriteField(labeledCodeSmellMetrics.MaMCL_method);
                        csv.WriteField(labeledCodeSmellMetrics.MeMCL_method);
                        csv.WriteField(labeledCodeSmellMetrics.CLNAMM_method);
                        csv.WriteField(labeledCodeSmellMetrics.num_not_final_not_static_attributes);
                        csv.WriteField(labeledCodeSmellMetrics.number_private_visibility_attributes);
                        csv.NextRecord();
                    }
                }
            }
        }
    }
}
