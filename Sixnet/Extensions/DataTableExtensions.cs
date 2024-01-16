using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Sixnet.App;
using Sixnet.Code;

namespace System.Data
{
    public static class DataTableExtensions
    {
        #region Write to csv file

        /// <summary>
        /// Write datatable to a csv file
        /// </summary>
        /// <param name="dataTable">Data table</param>
        /// <param name="savePath">Save path,Will save file to the application root directory when the parameter value is null or empty</param>
        /// <param name="fileName">Without extension file name,Will use a random file name when the parameter value is null or empty</param>
        /// <param name="ignoreTitle">Whether ignore column name,Default is false</param>
        /// <returns>Return the file full path</returns>
        public static string WriteToCSVFile(this DataTable dataTable, string savePath = "", string fileName = "", bool ignoreTitle = false, bool emptyForBooleanFalseValue = false)
        {
            if (dataTable == null)
            {
                throw new ArgumentNullException(nameof(dataTable));
            }
            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = GuidHelper.GetGuidUniqueCode().ToLower();
            }
            if (string.IsNullOrWhiteSpace(savePath))
            {
                savePath = ApplicationManager.GetRootPath();
            }
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            string filePath = Path.Combine(savePath, $"{fileName}.csv");
            using (var sw = new StreamWriter(filePath, false, new UTF8Encoding(false)))
            {
                var lineDatas = new List<string>(dataTable.Columns.Count);
                if (!ignoreTitle)
                {
                    foreach (DataColumn col in dataTable.Columns)
                    {
                        lineDatas.Add(col.ColumnName);
                    }
                    string titles = string.Join(CultureInfo.CurrentCulture.TextInfo.ListSeparator, lineDatas);
                    sw.WriteLine(titles);
                }
                int columnCount = dataTable.Columns.Count;
                foreach (DataRow row in dataTable.Rows)
                {
                    lineDatas.Clear();
                    for (int i = 0; i < columnCount; i++)
                    {
                        string rowValue = Convert.IsDBNull(row[i]) ? string.Empty : row[i].ToString();
                        lineDatas.Add(rowValue);
                    }
                    sw.WriteLine(string.Join(CultureInfo.CurrentCulture.TextInfo.ListSeparator, lineDatas));
                }
                sw.Close();
                sw.Dispose();
            }
            return filePath;
        }

        #endregion
    }
}
