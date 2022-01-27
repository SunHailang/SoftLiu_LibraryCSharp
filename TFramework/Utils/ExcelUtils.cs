using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;

namespace TFramework.Utils
{
    public class ExcelUtils
    {
        class ExcelFieldItem
        {
            public enum EFieldType
            {
                INT,
                STRING,
            }
            public string fieldName;
            public EFieldType baseType;
            public int arraySize;
            public bool PrimaryKey;
        }

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

        }

        // T 类型

        [Config.TargetExcel("excel_name", sheet = "sheet_name")]
        class DataT
        {
            //Tips提示信息， Config.MarkAsID 主键
            [Config.Tips("编号"), Config.MarkAsID]
            public int id;
        }

        public static void WriteExcel<T>(List<T> dataList, string path)
        {
            if (string.IsNullOrEmpty(path) || dataList == null ||dataList.Count <= 0)
            {
                return;
            }

            Type dataType = typeof(T);
            var excelAttrs = (Config.TargetExcel[])dataType.GetCustomAttributes(typeof(Config.TargetExcel), false);
            if(excelAttrs.Length <= 0)
            {
                return;
            }
            string excelName = excelAttrs[0].excel;
            string sheetName = excelAttrs[0].sheet;

            string filePath = $"{path}/{excelName}.xlsx";
            if(!File.Exists(filePath))
            {
                File.Create(filePath);
            }
            bool containArray = false;
            // 分析字段和属性
            System.Reflection.FieldInfo[] dataFieldList = dataType.GetFields(System.Reflection.BindingFlags.Public);
            var fieldItems = new List<ExcelFieldItem>();
            for (int i = 0; i < dataFieldList.Length; i++)
            {
                ExcelFieldItem item = new ExcelFieldItem();
                var fieldInfo = dataFieldList[i];
                item.fieldName = fieldInfo.Name;
                var capacities = (Config.Capacity[])fieldInfo.GetCustomAttributes(typeof(Config.Capacity), false);
                if (capacities.Length > 0)
                {
                    containArray = true;
                    item.arraySize = capacities[0].capacity;
                    item.baseType = (fieldInfo.FieldType.GetElementType() == typeof(int)) ? ExcelFieldItem.EFieldType.INT : ExcelFieldItem.EFieldType.STRING;
                }
                else
                {
                    item.baseType = (fieldInfo.FieldType == typeof(int)) ? ExcelFieldItem.EFieldType.INT : ExcelFieldItem.EFieldType.STRING;
                }

                var markAsIds = (Config.MarkAsID[])fieldInfo.GetCustomAttributes(typeof(Config.MarkAsID), false);
                if (markAsIds.Length > 0)
                {
                    item.PrimaryKey = true;
                }

                fieldItems.Add(item);
            }
            FileInfo file = new FileInfo(filePath);
            if (file == null)
            {
                return;
            }

            try
            {
                using (var pkg = new ExcelPackage(file))
                {
                    ExcelWorksheet ws = pkg.Workbook.Worksheets[1];
                    
                    // var cells = ws.Cells;
                    int maxColumnNum = ws.Dimension.End.Column;
                    int minRowNum = containArray ? 7 : 5;
                    int maxRowNum = ws.Dimension.End.Row; //工作区结束行号
                                                          //开始写入数据

                    for (int i = 0; i < dataList.Count; i++)
                    {
                        int column = 1;
                        T dataItem = dataList[i];
                        for (int j = 0; j < dataFieldList.Length; j++)
                        {
                            //如果是数组
                            if (fieldItems[j].arraySize > 0)
                            {
                                if (fieldItems[j].baseType == ExcelFieldItem.EFieldType.INT)
                                {
                                    int[] arrObjValue = (int[])dataFieldList[j].GetValue(dataItem);
                                    for (int k = 0; k < fieldItems[j].arraySize; k++)
                                    {
                                        ws.Cells[minRowNum + i, column].Value = arrObjValue[k];
                                        column++;
                                    }
                                }
                                else
                                {
                                    string[] arrObjValue = (string[])dataFieldList[j].GetValue(dataItem);
                                    for (int k = 0; k < fieldItems[j].arraySize; k++)
                                    {
                                        ws.Cells[minRowNum + i, column].Value = arrObjValue[k];
                                        column++;
                                    }
                                }
                            }
                            else
                            {
                                object objValue = dataFieldList[j].GetValue(dataItem);
                                if (objValue != null)
                                {
                                    if (fieldItems[j].baseType == ExcelFieldItem.EFieldType.INT)
                                    {
                                        ws.Cells[minRowNum + i, column].Value = objValue;
                                    }
                                    else
                                    {
                                        ws.Cells[minRowNum + i, column].Value = objValue.ToString();
                                    }
                                }
                                column++;
                            }
                            // 
                        }
                    }

                    if (1 + maxRowNum - minRowNum > dataList.Count)
                    {
                        for (int i = 0; i < 1 + maxRowNum - minRowNum - dataList.Count; i++)
                        {
                            for (int j = 1; j <= maxColumnNum; j++)
                            {
                                ws.Cells[minRowNum + dataList.Count + i, j].Value = null;
                            }

                        }
                    }
                    pkg.Save();
                }
            }
            catch (Exception e)
            {
                
            }
            finally
            {
                // fs.Close();
            }
        }

    }
}
