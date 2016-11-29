using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using NLog;
using SUTZ_2.MobileSUTZ;

// пространство имен для описания делегатов для работы модуля пополнения:
//namespace SUTZ_2.Module.BO.Mobile.Modules.Delegates
//{
//    public delegate int delegateProcessBarcode(String message);
//    public delegate String delegateGetLabelCaption();
//    //public delegate int delegateGetLabelCaption();
//    //public delegate void delegateCalculateItems(Type objType, int rowCounts);
//}

namespace SUTZ_2.MobileSUTZ
{
    class UserSelect  
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// В этом методе можно будет получить idd выбранного элемента из списка
        /// </summary>
        public int selectValue {get; set;}

        public DialogResult scanStringDialog(ref structScanStringParams paramCaptionsDialog)
        {
            paramCaptionsDialog.inputMode = enumInputMode.СканированиеШтрихкода;
            XtraFormSymbolScanBarcode ScanForm = new XtraFormSymbolScanBarcode(paramCaptionsDialog);            
            ScanForm.ShowDialog();
            if (ScanForm.DialogResult != DialogResult.OK)
            {
                return ScanForm.DialogResult;
            }
            paramCaptionsDialog = ScanForm.structParams;
            //Debug.WriteLine("Отсканирован текст: " + paramCaptionsDialog.scanedBarcode);
            logger.Trace("scanStringDialog(): Пользователь отсканировал текст {0}", paramCaptionsDialog.scanedBarcode);
            return ScanForm.DialogResult;
        }

        /// <summary>
        /// метод для вызова нового диалога с лейблом с HTML-разметкой
        /// </summary>
        /// <param name="paramCaptionsDialog"></param>
        /// <returns>DialogResult</returns>
        public DialogResult scanHTMLStringDialog(ref structScanStringParams paramCaptionsDialog, Func<String, int> dlgProcessBarcode, Func<String> dlgGetLabelCaption)
        {
            XtraFormSymbolScanBarcodeHTMLLabel ScanForm = new XtraFormSymbolScanBarcodeHTMLLabel(paramCaptionsDialog);
            ScanForm.dlgGetLabelCaption += dlgGetLabelCaption;
            ScanForm.dlgProcessBarcode += dlgProcessBarcode;

            ScanForm.ShowDialog();
            if (ScanForm.DialogResult != DialogResult.OK)
            {
                return ScanForm.DialogResult;
            }
            paramCaptionsDialog = ScanForm.structParams;
            //Debug.WriteLine("Отсканирован текст: " + paramCaptionsDialog.scanedBarcode);
            logger.Trace("scanHTMLStringDialog: Пользователь отсканировал текст {0}", paramCaptionsDialog.scanedBarcode);
            return ScanForm.DialogResult;
        }

        /// <summary>
        /// метод для вызова нового диалога с лейблом с HTML-разметкой
        /// </summary>
        /// <param name="paramCaptionsDialog"></param>
        /// <returns>DialogResult</returns>
        public DialogResult scanHTMLStringDialog(ref structScanStringParams paramCaptionsDialog)
        {
            paramCaptionsDialog.inputMode = enumInputMode.СканированиеШтрихкода;
            XtraFormSymbolScanBarcodeHTMLLabel ScanForm = new XtraFormSymbolScanBarcodeHTMLLabel(paramCaptionsDialog);
            ScanForm.ShowDialog();
            if (ScanForm.DialogResult != DialogResult.OK)
            {
                return ScanForm.DialogResult;
            }
            paramCaptionsDialog = ScanForm.structParams;
            //Debug.WriteLine("Отсканирован текст: " + paramCaptionsDialog.scanedBarcode);
            logger.Trace("scanHTMLStringDialog: Пользователь отсканировал текст {0}", paramCaptionsDialog.scanedBarcode);
            return ScanForm.DialogResult;
        }


        // диалог ввода даты:
        public DialogResult scanDateDialog(ref structScanStringParams paramCaptionsDialog)
        {
            paramCaptionsDialog.inputMode = enumInputMode.ВводТолькоДаты;
            XtraFormSymbolScanBarcode ScanForm = new XtraFormSymbolScanBarcode(paramCaptionsDialog);            
            ScanForm.ShowDialog();
            if (ScanForm.DialogResult != DialogResult.OK)
            {
                return ScanForm.DialogResult;
            }
            paramCaptionsDialog = ScanForm.structParams;

            logger.Trace("scanDateDialog(): Пользователь ввел дату {0}", paramCaptionsDialog.scanedBarcode);
            return ScanForm.DialogResult;
        }

        // диалог ввода количества товара:
        public DialogResult inputQantityOfUnits(ref structScanStringParams paramCaptionsDialog)
        {
            XtraFormSymbolInputQuantity ScanForm = new XtraFormSymbolInputQuantity(paramCaptionsDialog);

            ScanForm.ShowDialog();
            if (ScanForm.DialogResult != DialogResult.OK)
            {
                return ScanForm.DialogResult;
            }
            paramCaptionsDialog = ScanForm.structParams;
            logger.Trace("inputQantityOfUnits(): Пользователь количество коробок={0}, штук={1}, итого={2}", paramCaptionsDialog.decFactUnits, paramCaptionsDialog.decFactItems, paramCaptionsDialog.decPlanTotal);
            return ScanForm.DialogResult;
        }
        
        // HTML диалог ввода количества товара:
        public DialogResult inputQantityOfUnitsHTML(ref structScanStringParams paramCaptionsDialog, Func<string,int> dlgProcessBarcode, Func<string> dlgGetLabelCaption)
        {
            XtraFormSymbolInputQuantityHTML ScanForm = new XtraFormSymbolInputQuantityHTML(paramCaptionsDialog);
            ScanForm.dlgGetLabelCaption += dlgGetLabelCaption;
            ScanForm.dlgProcessBarcode += dlgProcessBarcode;

            ScanForm.ShowDialog();
            if (ScanForm.DialogResult != DialogResult.OK)
            {
                return ScanForm.DialogResult;
            }
            paramCaptionsDialog = ScanForm.structParams;
            logger.Trace("inputQantityOfUnitsHTML: Пользователь количество коробок={0}, штук={1}, итого={2}", paramCaptionsDialog.decFactUnits, paramCaptionsDialog.decFactItems, paramCaptionsDialog.decPlanTotal);
            return ScanForm.DialogResult;
        }

        public DialogResult userMessage(ref structScanStringParams paramCaptionsDialog)
        {
            XtraFormSymbolUserMessage newForm = new XtraFormSymbolUserMessage(paramCaptionsDialog);
            newForm.ShowDialog();

            return newForm.DialogResult;
        }

        // диалог выбора значения из класса 
        public DialogResult inputValueFromType(Type type)
        {
            XtraFormSymbolSelectValue newForm = new XtraFormSymbolSelectValue(type);
            newForm.ShowDialog();
            if (newForm!=null)
            {
                this.selectValue = newForm.iddSelectValue;
            }
            return newForm.DialogResult;
        }
    }
}
