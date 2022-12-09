using ClosedXML.Excel;
using MedEye.DB;
using System.Linq;

namespace MedEye.ExcelLoader
{
    internal class ExcelGenerator
    {
        static public void GenerateExcelByUserId(int user_id)
        {
            var xlsWorkbook = new XLWorkbook();
            xlsWorkbook.Worksheets.Add("Тир");
            xlsWorkbook.Worksheets.Add("Погоня");
            xlsWorkbook.Worksheets.Add("Совмещение");
            xlsWorkbook.Worksheets.Add("Слияние");

            for(int i = 0; i < xlsWorkbook.Worksheets.Count; ++i)
            {
                var sheet = xlsWorkbook.Worksheets.ElementAt(i);
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
                sheet.Cell("J1").Value = "Вовлечённость";
                sheet.Cell("J1").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                var scores = ScoresWrap.GetScores(user_id, i + 1);
                for (int j = 2; j < scores.Count() + 2; ++j)
                {
                    sheet.Cell("A" + j.ToString()).Value = scores[j - 2].DateCompletion.Substring(0, scores[j - 2].DateCompletion.IndexOf(" "));
                    sheet.Cell("A" + j.ToString()).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    sheet.Cell("B" + j.ToString()).Value = Math.Round(scores[j - 2].MeanDeviationsX, 1);
                    sheet.Cell("B" + j.ToString()).DataType = XLDataType.Text;
                    sheet.Cell("B" + j.ToString()).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    sheet.Cell("C" + j.ToString()).Value = Math.Round(scores[j - 2].MeanDeviationsY, 1);
                    sheet.Cell("C" + j.ToString()).DataType = XLDataType.Text;
                    sheet.Cell("C" + j.ToString()).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    sheet.Cell("D" + j.ToString()).Value = Math.Round(scores[j - 2].MaxDeviationsX, 1);
                    sheet.Cell("D" + j.ToString()).DataType = XLDataType.Text;
                    sheet.Cell("D" + j.ToString()).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    sheet.Cell("E" + j.ToString()).Value = Math.Round(scores[j - 2].MaxDeviationsY, 1);
                    sheet.Cell("E" + j.ToString()).DataType = XLDataType.Text;
                    sheet.Cell("E" + j.ToString()).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    sheet.Cell("F" + j.ToString()).Value = Math.Round(scores[j - 2].MinDeviationsX, 1);
                    sheet.Cell("F" + j.ToString()).DataType = XLDataType.Text;
                    sheet.Cell("F" + j.ToString()).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    sheet.Cell("G" + j.ToString()).Value = Math.Round(scores[j - 2].MinDeviationsY, 1);
                    sheet.Cell("G" + j.ToString()).DataType = XLDataType.Text;
                    sheet.Cell("G" + j.ToString()).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    sheet.Cell("H" + j.ToString()).Value = scores[j - 2].Level;
                    sheet.Cell("H" + j.ToString()).DataType = XLDataType.Text;
                    sheet.Cell("H" + j.ToString()).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    sheet.Cell("I" + j.ToString()).Value = scores[j - 2].Score;
                    sheet.Cell("I" + j.ToString()).DataType = XLDataType.Text;
                    sheet.Cell("I" + j.ToString()).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    sheet.Cell("J" + j.ToString()).Value = scores[j - 2].Involvement;
                    sheet.Cell("J" + j.ToString()).DataType = XLDataType.Text;
                    sheet.Cell("J" + j.ToString()).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                }
            }
            var current_user = Users.GetUserById(user_id);
            var fio = current_user.first_name + " " + current_user.second_name + " " + current_user.last_name;
            xlsWorkbook.SaveAs($"{fio}.xlsx"); //Save the excel file
        }
    }
}
