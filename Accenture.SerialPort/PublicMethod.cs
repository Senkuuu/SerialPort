using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace BXHSerialPort
{
    public class PublicMethod
    {


        public static System.Data.DataTable GetDataFromExcelByCom(bool isFirstRowColumnName, string fileName)
        {


            System.Data.DataTable data = new System.Data.DataTable();
            try
            {
                Aspose.Cells.Workbook workbook = null;

                FileInfo fileInfo = new FileInfo(fileName);
                if (fileInfo.Extension.ToLower().Equals(".xlsx"))
                    workbook = new Aspose.Cells.Workbook(fileName, new Aspose.Cells.LoadOptions(Aspose.Cells.LoadFormat.Xlsx));
                else if (fileInfo.Extension.ToLower().Equals(".xls"))
                    workbook = new Aspose.Cells.Workbook(fileName, new Aspose.Cells.LoadOptions(Aspose.Cells.LoadFormat.Excel97To2003));
                if (workbook != null)
                {
                    Aspose.Cells.Worksheet worksheet = null;
                    worksheet = workbook.Worksheets[0];
                    if (worksheet != null)
                    {
                        data = worksheet.Cells.ExportDataTableAsString(0, 0, worksheet.Cells.MaxRow + 1, worksheet.Cells.MaxColumn + 1,
                            isFirstRowColumnName);
                        return data;
                    }
                }
                else
                {
                    return data;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return data;

        }
        /// <summary>
        /// datatable输出到excel
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="dt">datatable</param>
        /// <param name="UseFirstRowColumnsName">是否使用dt的列名</param>
        public static void OutToExcelFromDataTable(string fileName, System.Data.DataTable dt, bool UseFirstRowColumnsName = true)
        {
            List<Tuple<int, int, object, string>> lt = new List<Tuple<int, int, object, string>>();
            lt = DtToTupe(0, 0, dt, UseFirstRowColumnsName);
            WriteToExcelCell(fileName, lt);
        }
        /// <summary>
        /// dt转换到tuple
        /// </summary>
        /// <param name="Xcell">顶点行</param>
        /// <param name="Ycell">顶点列</param>
        /// <param name="dt">datatable</param>
        /// <param name="UseFirstRowColumnsName">是否使用dt的列名</param>
        /// <returns></returns>
        public static List<Tuple<int, int, object, string>> DtToTupe(int Xcell, int Ycell, System.Data.DataTable dt, bool UseFirstRowColumnsName = true)
        {
            List<Tuple<int, int, object, string>> lt = new List<Tuple<int, int, object, string>>();
            if (UseFirstRowColumnsName)
            {
                for (int y = 0; y < dt.Columns.Count; y++)
                {
                    int xc = Xcell;
                    int yc = y + Ycell;
                    Tuple<int, int, object, string> t =
                        new Tuple<int, int, object, string>(
                            xc, yc, dt.Columns[y].ColumnName, dt.Columns[y].DataType.ToString()
                            );
                    lt.Add(t);
                }
                for (int x = 0; x < dt.Rows.Count; x++)
                {
                    for (int y = 0; y < dt.Columns.Count; y++)
                    {
                        int xc = x + Xcell + 1;
                        int yc = y + Ycell;
                        Tuple<int, int, object, string> t =
                            new Tuple<int, int, object, string>(
                                xc, yc, dt.Rows[x][y], dt.Columns[y].DataType.ToString()
                                );
                        lt.Add(t);
                    }
                }
            }
            else
            {
                for (int x = 0; x < dt.Rows.Count; x++)
                {
                    for (int y = 0; y < dt.Columns.Count; y++)
                    {
                        int xc = x + Xcell;
                        int yc = y + Ycell;
                        Tuple<int, int, object, string> t =
                            new Tuple<int, int, object, string>(
                                xc, yc, dt.Rows[x][y], dt.Columns[y].DataType.ToString()
                                );
                        lt.Add(t);
                    }

                }
            }
            return lt;
        }

        /// <summary>
        /// 插入到excel的cell
        /// </summary>
        /// <param name="fileName">excel文件名</param>
        /// <param name="dst">目标 (行，列，值)</param>
        /// <param name="sheetName">默认sheetName,可以不写</param>
        public static void WriteToExcelCell(string fileName, List<Tuple<int, int, object, string>> customCell, string sheetName = "sheet1")
        {
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            NPOI.SS.UserModel.ISheet sheet = book.CreateSheet(sheetName);
            foreach (var item in customCell)
            {
                int r = item.Item1;
                int c = item.Item2;
                object content = item.Item3;
                NPOI.SS.UserModel.IRow row = null;
                if (sheet.GetRow(r) == null)
                {
                    row = sheet.CreateRow(r);
                }
                else
                {
                    row = sheet.GetRow(r);
                }
                NPOI.SS.UserModel.ICell cell = row.CreateCell(c);
                cell.SetCellValue(content.ToString());
                string objType = item.Item4.ToString();
                #region 类型转换
                try
                {
                    switch (objType)
                    {
                        case "System.String"://字符串类型
                            cell.SetCellValue(content.ToString());
                            break;
                        case "System.DateTime"://日期类型  
                            DateTime dateV;
                            DateTime.TryParse(content.ToString(), out dateV);
                            string strtime = dateV.ToString("yyyy-MM-dd HH:mm:ss");
                            if (strtime.Substring(11, 8) == "00:00:00")
                            {
                                strtime = dateV.ToString("yyyy-MM-dd");
                            }
                            cell.SetCellValue(strtime);
                            break;
                        case "System.Boolean"://布尔型  
                            bool boolV = false;
                            bool.TryParse(content.ToString(), out boolV);
                            cell.SetCellValue(boolV);
                            break;

                        case "System.Int16"://整型  
                            cell.SetCellValue(Convert.ToInt16(content));
                            break;
                        case "System.Int32":
                            cell.SetCellValue(Convert.ToInt32(content));
                            break;
                        case "System.Int64":
                            cell.SetCellValue(Convert.ToInt64(content));
                            break;
                        case "System.Byte":
                            cell.SetCellValue(Convert.ToInt32(content));
                            break;
                        case "System.Decimal"://浮点型  
                            cell.SetCellValue(Convert.ToDouble(content));
                            break;
                        case "System.Double":
                            cell.SetCellValue(Convert.ToDouble(content));
                            break;
                        case "System.DBNull"://空值处理  
                            cell.SetCellValue("");
                            break;
                        default:
                            cell.SetCellValue(content.ToString());
                            break;
                    }
                }
                catch
                {
                    cell.SetCellValue(content.ToString());
                }
                #endregion
            }
            // 写入到客户端  
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            book.Write(ms);
            byte[] b = ms.ToArray();
            File.WriteAllBytes(fileName, b);
            book = null;
            ms.Close();
            ms.Dispose();
        }




        //克隆DGV
        public static DataGridView CloneDataGridView(DataGridView dgv)
        {
            try
            {
                DataGridView ResultDGV = new DataGridView();
                ResultDGV.ColumnHeadersDefaultCellStyle = dgv.ColumnHeadersDefaultCellStyle.Clone();
                DataGridViewCellStyle dtgvdcs = dgv.RowsDefaultCellStyle.Clone();
                dtgvdcs.BackColor = dgv.DefaultCellStyle.BackColor;
                dtgvdcs.ForeColor = dgv.DefaultCellStyle.ForeColor;
                dtgvdcs.Font = dgv.DefaultCellStyle.Font;
                ResultDGV.RowsDefaultCellStyle = dtgvdcs;
                ResultDGV.AlternatingRowsDefaultCellStyle = dgv.AlternatingRowsDefaultCellStyle.Clone();

                for (int i = 0; i < dgv.Columns.Count; i++)
                {
                    DataGridViewColumn DTGVC = dgv.Columns[i].Clone() as DataGridViewColumn;
                    DTGVC.DisplayIndex = dgv.Columns[i].DisplayIndex;
                    if (DTGVC.CellType == null)
                    {
                        DTGVC.CellTemplate = new DataGridViewTextBoxCell();
                        ResultDGV.Columns.Add(DTGVC);
                    }
                    else
                    {
                        ResultDGV.Columns.Add(DTGVC);
                    }
                }
                foreach (DataGridViewRow var in dgv.Rows)
                {
                    DataGridViewRow Dtgvr = var.Clone() as DataGridViewRow;
                    Dtgvr.DefaultCellStyle = var.DefaultCellStyle.Clone();
                    for (int i = 0; i < var.Cells.Count; i++)
                    {
                        Dtgvr.Cells[i].ValueType = typeof(string);
                        Dtgvr.Cells[i].Value = var.Cells[i].Value;
                    }
                    if (var.Index % 2 == 0)
                        Dtgvr.DefaultCellStyle.BackColor = ResultDGV.RowsDefaultCellStyle.BackColor;
                    ResultDGV.Rows.Add(Dtgvr);

                }
                return ResultDGV;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return null;
        }

    }



}
