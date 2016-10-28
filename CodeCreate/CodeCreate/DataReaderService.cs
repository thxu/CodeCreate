using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model生成器
{
    /// <summary>
    /// 数据读取服务
    /// </summary>
    [DataContract]
    public class DataReaderService
    {
        /// <summary>
        /// 数据连接字符串
        /// </summary>
        [DataMember]
        private static string StrConn { get; set; }

        /// <summary>
        /// 读取excel文件数据
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>datatable</returns>
        public static DataTable ReadExcelData(string filePath)
        {
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            string fileType = System.IO.Path.GetExtension(filePath);
            if (string.IsNullOrWhiteSpace(fileType))
            {
                return null;
            }

            StrConn = string.Format(fileType == ".xls"
                ? "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=\"Excel 8.0;HDR=YES;IMEX=1\""
                : "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0;HDR=YES;IMEX=1\"", filePath);
            string strSql = "SELECT * FROM {0}";
            OleDbConnection conn = null;
            OleDbDataAdapter da = null;
            DataTable dtSheetName = null;
            using (conn = new OleDbConnection(StrConn))
            {
                try
                {
                    conn.Open();

                    string sheetName = string.Empty;
                    dtSheetName = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables,
                        new object[] { null, null, null, "TABLE" });

                    da = new OleDbDataAdapter();
                    for (int i = 0; i < dtSheetName.Rows.Count; i++)
                    {
                        sheetName = (string)dtSheetName.Rows[i]["TABLE_NAME"];
                        if (sheetName.Contains("$") && !sheetName.Replace("'", "").EndsWith("$"))
                        {
                            continue;
                        }

                        da.SelectCommand = new OleDbCommand(string.Format(strSql, sheetName), conn);
                        DataSet dsItem = new DataSet();
                        da.Fill(dsItem, "sheet1");

                        ds.Tables.Add(dsItem.Tables[0].Copy());
                    }
                    dt = ds.Tables[0];
                }
                catch (Exception ex)
                {
                    throw;
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                        da.Dispose();
                        conn.Dispose();
                    }
                }
            }
            return dt;
        }
    }
}
