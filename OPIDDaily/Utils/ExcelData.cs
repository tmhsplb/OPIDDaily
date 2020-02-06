using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace OPIDDaily.Utils
{
    public class ExcelData
    {
        private string path;

        public ExcelData(string path)
        {
            this.path = path;
        }

        private DataSet GetExcelDataAsDataSet(bool isFirstRowAsColumnNames)
        {
            using (FileStream fileStream = File.Open(path, FileMode.Open, FileAccess.Read))
            {
                IExcelDataReader dataReader;

                if (path.EndsWith(".xls"))
                {
                    dataReader = ExcelReaderFactory.CreateBinaryReader(fileStream);
                }
                else if (path.EndsWith(".xlsx"))
                {
                    dataReader = ExcelReaderFactory.CreateOpenXmlReader(fileStream);
                }
                else
                {
                    // Throw exception for things you cannot correct
                    throw new Exception("The file to be processed is not an Excel file");
                }

                // See https://stackoverflow.com/questions/27634477/using-exceldatareader-to-read-excel-data-starting-from-a-particular-cell
                var conf = new ExcelDataSetConfiguration
                {
                    ConfigureDataTable = _ => new ExcelDataTableConfiguration
                    {
                        UseHeaderRow = isFirstRowAsColumnNames
                    }
                };

                return dataReader.AsDataSet(conf);
            }
        }

        private DataTable GetExcelWorkSheet(string workSheetName, bool isFirstRowAsColumnNames)
        {
            DataSet dataSet = GetExcelDataAsDataSet(isFirstRowAsColumnNames);

            // We are always interested in the first worksheet in the workbook.
            // This eliminates dependence on the particular worksheet name.
            DataTable workSheet = dataSet.Tables[0]; // was [workSheetName];

            if (workSheet == null)
            {
                throw new Exception(string.Format("The worksheet {0} does not exist, has an incorrect name, or does not have any data in the worksheet", workSheetName));
            }

            return workSheet;
        }

        public IEnumerable<DataRow> GetData(string workSheetName, bool isFirstRowAsColumnNames = true)
        {
            DataTable workSheet = GetExcelWorkSheet(workSheetName, isFirstRowAsColumnNames);

            IEnumerable<DataRow> rows = from DataRow row in workSheet.Rows
                                        select row;

            return rows;
        }

        private DataTable GetExcelWorkSheet(bool isFirstRowAsColumnNames)
        {
            DataSet dataSet = GetExcelDataAsDataSet(isFirstRowAsColumnNames);
            // We are always interested in the first worksheet in the workbook.
            // This eliminates dependence on the particular worksheet name.
            DataTable workSheet = dataSet.Tables[0];

            if (workSheet == null)
            {
                throw new Exception(string.Format("This workbook has no worksheets!"));
            }

            return workSheet;
        }

        public IEnumerable<DataRow> GetData(bool isFirstRowAsColumnNames = true)
        {
            DataTable workSheet = GetExcelWorkSheet(isFirstRowAsColumnNames);

            IEnumerable<DataRow> rows = from DataRow row in workSheet.Rows
                                        select row;

            return rows;
        }
    }
}