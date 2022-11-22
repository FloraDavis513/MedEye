using ClosedXML.Excel;
using MedEye.DB;
using System.Linq;

namespace MedEye.ExcelLoader
{
    internal class ExcelGenerator
    {
        static public void GenerateExcelByUserId(int user_id)
        {
            var scores = ScoresWrap.GetScores(user_id);

            var xlsWorkbook = new XLWorkbook();
            xlsWorkbook.Worksheets.Add("Тир");
            xlsWorkbook.Worksheets.Add("Погоня");
            xlsWorkbook.Worksheets.Add("Совмещение");
            xlsWorkbook.Worksheets.Add("Слияние");

            foreach(var sheet in xlsWorkbook.Worksheets)
            {
                sheet.Cell("A1").Value = "Дата";
                sheet.Cell("A1").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                sheet.Cell("B1").Value = "Среднее отклоение по Х";
                sheet.Cell("B1").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                sheet.Cell("C1").Value = "Среднее отклоение по Y";
                sheet.Cell("C1").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                sheet.Cell("D1").Value = "Максимальное отклоение по Х";
                sheet.Cell("D1").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                sheet.Cell("E1").Value = "Максимальное отклоение по Y";
                sheet.Cell("E1").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                sheet.Cell("F1").Value = "Минимальное отклоение по Х";
                sheet.Cell("F1").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                sheet.Cell("G1").Value = "Минимальное отклоение по Y";
                sheet.Cell("G1").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                sheet.Cell("H1").Value = "Сложность";
                sheet.Cell("H1").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                sheet.Cell("I1").Value = "Очки";
                sheet.Cell("I1").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                for (int i = 2; i < scores.Count() + 2; ++i)
                {
                    sheet.Cell("A" + i.ToString()).Value = scores[i - 2].DateCompletion;
                    sheet.Cell("A" + i.ToString()).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    sheet.Cell("B" + i.ToString()).Value = scores[i - 2].MeanDeviationsX;
                    sheet.Cell("B" + i.ToString()).DataType = XLDataType.Text;
                    sheet.Cell("B" + i.ToString()).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    sheet.Cell("C" + i.ToString()).Value = scores[i - 2].MeanDeviationsY;
                    sheet.Cell("C" + i.ToString()).DataType = XLDataType.Text;
                    sheet.Cell("C" + i.ToString()).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    sheet.Cell("D" + i.ToString()).Value = scores[i - 2].MaxDeviationsX;
                    sheet.Cell("D" + i.ToString()).DataType = XLDataType.Text;
                    sheet.Cell("D" + i.ToString()).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    sheet.Cell("E" + i.ToString()).Value = scores[i - 2].MaxDeviationsY;
                    sheet.Cell("E" + i.ToString()).DataType = XLDataType.Text;
                    sheet.Cell("E" + i.ToString()).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    sheet.Cell("F" + i.ToString()).Value = scores[i - 2].MinDeviationsX;
                    sheet.Cell("F" + i.ToString()).DataType = XLDataType.Text;
                    sheet.Cell("F" + i.ToString()).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    sheet.Cell("G" + i.ToString()).Value = scores[i - 2].MinDeviationsY;
                    sheet.Cell("G" + i.ToString()).DataType = XLDataType.Text;
                    sheet.Cell("G" + i.ToString()).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    sheet.Cell("H" + i.ToString()).Value = scores[i - 2].Level;
                    sheet.Cell("H" + i.ToString()).DataType = XLDataType.Text;
                    sheet.Cell("H" + i.ToString()).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    sheet.Cell("I" + i.ToString()).Value = scores[i - 2].Score + "0";
                    sheet.Cell("I" + i.ToString()).DataType = XLDataType.Text;
                    sheet.Cell("I" + i.ToString()).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                }
            }

            xlsWorkbook.SaveAs("NewExcelFile.xlsx"); //Save the excel file
        }
    }
}
