using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFramework.Utils
{
    public class ExcelUtils
    {

        public static DataTable ReadExcelToDataTable(string path, string sheet)
        {
            //此连接只能操作Excel2007之前(.xls)文件
            //string connstring = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + path + ";" + "Extended Properties=Excel 8.0;";
            string sheetName = sheet + "$";
            //此连接可以操作.xls与.xlsx文件
            string connstring = "Provider=Microsoft.Ace.OleDb.12.0;" + "data source=" + path + ";Extended Properties='Excel 12.0; HDR=NO; IMEX=1'";
            using (OleDbConnection conn = new OleDbConnection(connstring))
            {
                conn.Open();
                DataTable sheetsName = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "Table" }); //得到所有sheet的名字
                //string firstSheetName = sheetsName.Rows[0][2].ToString(); //得到第一个sheet的名字
                string sql = string.Format("SELECT * FROM [{0}]", sheetName); //查询字符串
                //string sql = string.Format("SELECT * FROM [{0}] WHERE [日期] is not null", firstSheetName); //查询字符串

                OleDbDataAdapter odda = new OleDbDataAdapter(sql, conn);
                DataSet set = new DataSet();
                odda.Fill(set, sheetName);
                DataTable dt = set.Tables[0];
                //dt.WriteXml(@"C:\Users\hlsun\Desktop\excel.xml");

                //WriteDataTableToExcel(dt, string.Empty, string.Empty);
                return dt;
            }
        }

        public static void WriteDataTableToExcel(DataTable dataTable, string path, string sheetName, string password = "")
        {
            if (dataTable != null)
            {
                try
                {
                    //此连接只能操作Excel2007之前(.xls)文件
                    //string connstring = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + path + ";" + "Extended Properties=Excel 8.0;";
                    //此连接可以操作.xls与.xlsx文件
                    string connstring = "Provider=Microsoft.Ace.OleDb.12.0;" + "data source=" + path + ";Extended Properties='Excel 12.0; HDR=NO; IMEX=1'";

                    Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
                    excelApp.Visible = true;
                    //Microsoft.Office.Interop.Excel.Application excelApp;
                    Microsoft.Office.Interop.Excel._Workbook workBook;
                    Microsoft.Office.Interop.Excel._Worksheet workSheet = null;
                    object misValue = System.Reflection.Missing.Value;
                    if (File.Exists(path))
                    {
                        workBook = excelApp.Workbooks.Open(path, misValue, misValue, misValue, misValue, misValue, misValue, misValue, misValue, misValue, misValue, misValue, misValue);
                        //Console.WriteLine("Sheets Count : " + workBook.Sheets.Count);
                        for (int i = 0; i < workBook.Sheets.Count; i++)
                        {
                            Microsoft.Office.Interop.Excel._Worksheet work = (Microsoft.Office.Interop.Excel._Worksheet)workBook.Sheets.get_Item(i + 1);
                            if (work.Name == sheetName)
                            {
                                workSheet = work;
                                break;
                            }
                            //Console.WriteLine(work.Name);
                        }
                        if (workSheet == null)
                        {
                            Console.WriteLine("Not Find Sheet : " + sheetName);
                            return;
                        }
                    }
                    else
                    {
                        workBook = excelApp.Workbooks.Add(misValue);//加载模型
                        workSheet = (Microsoft.Office.Interop.Excel._Worksheet)workBook.Sheets.get_Item(1);//第一个工作薄。
                    }

                    //using (OleDbConnection conn = new OleDbConnection(connstring))
                    //{
                    //    conn.Open();
                    //    DataTable sheetsName = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "Table" }); //得到所有sheet的名字

                    //    //string firstSheetName = sheetsName.Rows[0][2].ToString(); //得到第一个sheet的名字
                    //}
                    //workSheet = (Microsoft.Office.Interop.Excel._Worksheet)workBook.Sheets[sheet];

                    int rowIndex = 0;
                    int colIndex = 0;
                    foreach (DataRow row in dataTable.Rows)
                    {
                        rowIndex++;
                        colIndex = 0;
                        foreach (DataColumn col in dataTable.Columns)
                        {
                            colIndex++;
                            workSheet.Cells[rowIndex, colIndex] = row[col.ColumnName].ToString().Trim();
                        }
                    }


                    workSheet.Protect(password, Type.Missing, Type.Missing, Type.Missing,
                                                Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                                Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                                Type.Missing, true, Type.Missing, Type.Missing);


                    excelApp.DisplayAlerts = false;

                    workBook.SaveAs(path, Type.Missing, "", "", Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, 1, false, Type.Missing, Type.Missing, Type.Missing);
                    //xbook.Save();
                    workBook.Close(false, misValue, misValue);
                    dataTable = null;

                    excelApp.Quit();

                    //PublicMethod.Kill(excelApp);//调用kill当前excel进程  
                    
                }
                catch (Exception msg)
                {
                    Console.WriteLine("Write Sheet Error: " + msg.Message);
                }
            }
        }

    }
}
