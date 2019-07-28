using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChartCanvas.Utils
{
    public static class ExcelWriter
    {
        /// <summary>
        /// 保存Excel文件至本地
        /// </summary>
        /// <param name="filePath">保存路径</param>
        /// <param name="colName">列名</param>
        /// <param name="data">数据</param>
        public static void Save(string filePath, string[] colName,double[][] data)
        {
            if (string.IsNullOrEmpty(filePath) || colName == null || data == null)
                throw new ArgumentNullException();
            if (colName.Count() != data.Count()) throw new ArgumentException("数据列数与列名不对应");

            XSSFWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("Sheet1");

            //设置列名
            IRow nameRow = sheet.CreateRow(0);
            for (int i = 0; i < colName.Count(); i++)
            {
                nameRow.CreateCell(i).SetCellValue(colName[i]);
            }

            //初始化所有row
            List<IRow> rows = new List<IRow>();
            int dataCount = Min(data);
            for (int i = 0;i < dataCount; i++)
            {
                IRow row = sheet.CreateRow(i + 1);
                rows.Add(row);
            }
            //依次填值
            for(int i = 0; i < data.Count(); i++)
            {
                for (int j = 0; j < dataCount; j++)
                {
                    rows[j].CreateCell(i).SetCellValue(data[i][j]);
                }
            }

            FileStream fs = new FileStream(filePath, FileMode.Create);
            workbook.Write(fs);

            fs.Close();
            workbook.Close();
        }

        private static int Min(double[][] data)
        {
            int Min = data[0].Count();
            foreach(var item in data)
            {
                if (Min > item.Count()) Min = item.Count();
            }
            return Min;
        }
    }
}
