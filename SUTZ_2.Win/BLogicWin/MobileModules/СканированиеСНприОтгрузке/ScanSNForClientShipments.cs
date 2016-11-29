using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using DevExpress.Xpo;
using SUTZ_2.Module.BO.Documents;
using System.Windows.Forms;
using System.Diagnostics;
using SUTZ_2.Module;
using DevExpress.ExpressApp.Utils;
using SUTZ_2.Module.BO.References;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;

namespace SUTZ_2.MobileSUTZ
{
    class ScanSNForClientShipments : IDisposable
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private Session session_;
        public Session session
        {
            get { return session_; }
            set { session_ = value; }
        }

        private XPObjectSpace objectSpace_;
        public XPObjectSpace objectSpace
        {
            get { return objectSpace_; }
            set { objectSpace_ = value; }
        }

        private BaseDocument currenDocBase_;
        public BaseDocument currentDocBase
        {
            get { return currenDocBase_; }
            set { currenDocBase_ = value; }
        }
        

        // 2. методы для управления формой:

        // 2.1 формирует заголовок формы
        private String getTopCaption()
        {
            if (currentDocBase!=null){
                return currentDocBase.ToString();
                }
            else
            {
                return ""; 
            }
        }

        public ScanSNForClientShipments(Session paramSession)
        {
            //Guard.ArgumentNotNull(paramSession, "paramSession");
            //session = paramSession;
            Guard.ArgumentNotNull(currentSessionSettings.ObjXafApp, "ObjXafApp");
            objectSpace = (XPObjectSpace)currentSessionSettings.ObjXafApp.CreateObjectSpace();
            session = objectSpace.Session;
        }
  
        #region Члены IDisposable
        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
 
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                if (session_ != null)
                {
                    session_.Dispose();
                    session_ = null;
                }
        }
        ~ScanSNForClientShipments()
        {
            Dispose(false);
        }    

        #endregion


        #region ScanBlock
  
        // 1. Стартовый метод, который вызывается при начале работы модуля
        public bool runScanSNForClientShipment()
        {         
            logger.Trace("Вход в функцию runScanSNForClientShipment()");

            // 1. вызов диалога сканирования ШК документа Отбор
            BaseDocument docFinded = scanDocShipment();
            if (docFinded==null)
            {
                logger.Trace("Выход из функции runScanSNForClientShipment(), т.к. ошибка при сканировании документа.");
                return false;
            }
            //Debug.WriteLine("Найден документ " + docFinded);

            // 2. установка найденного документа в настройках пользователя и в текущей форме
            //currentSessionSettings.CurrentDocBase = docFinded;
            currentDocBase = docFinded;
            logger.Trace("в справочнике currentSessionSettings.CurrentDocBase установлен документ = {0}", docFinded.ToString());

            // 3. Вызов основного метода для скаринования товара-SN
            bool scanResult = scanTovarBarcodeSN(docFinded);
            return scanResult;
          }

         // 2. Метод для запроса сканирования ШК документа и поиска его по ИДД
         private BaseDocument scanDocShipment()
         {
             structScanStringParams paramScan = new structScanStringParams();
             paramScan.captionOne = "Отсканируйте Отборочный лист или Расх.накладную:";
             paramScan.captionTwo = "";
             paramScan.scanedBarcode = "";
             paramScan.successScan = false;

             // 2. вызов диалога сканирования ШК
             UserSelect userSelect = new UserSelect();

             BaseDocument docFinded = null;
             //DocInvoiceOrder docInvoicefinded = null;

             while (true)
             {
                 logger.Trace("Перед вызовом сканирования ШК документа");
                 DialogResult dlgResult = userSelect.scanStringDialog(ref paramScan);
                 if (dlgResult != DialogResult.OK)
                 {
                     logger.Trace("Диалог сканирования ШК вернул = {0}", dlgResult);
                     return docFinded;
                 }
                 if (!paramScan.successScan)
                 {
                     logger.Trace("paramScan.successScan = {0}", paramScan.successScan);
                     return docFinded;
                 }
                 string strBarcode = paramScan.scanedBarcode;
                 logger.Trace("отсканирован ШК = {0}", strBarcode);

                 // 3. поиск документа по ШК:
                 //docFinded = new BaseDocument(session).findByIddSutz(strBarcode);
                 docFinded = session.FindObject<BaseDocument>(new BinaryOperator("idGUID", strBarcode, BinaryOperatorType.Equal), true);           
                 if (docFinded == null)
                 {
                     paramScan.captionTwo = "Ошибка поиска Отб.листа или Расх.накладной!";
                     logger.Trace("ошибка поиска документа Отб.лист или Расх.накладной по ШК = {0}", strBarcode);
                     continue;
                 }

                 // 4. определение вида документа
                 if (docFinded.ClassInfo.ClassType == typeof(DocSpecRashoda)) //"DocSpecRashoda"
                 {
                     // 4.1 отсканирован Отборочный лист, определение документа-основания:
                     if (docFinded.DocBase==null)
                     {
                         paramScan.captionTwo = "Ошибка поиска док.основания для Отборочного листа!";
                         logger.Trace("Ошибка поиска док.основания для Отборочного листа = {0}", docFinded.DocNo);
                         continue;
                     }

                     docFinded = docFinded.DocBase;
                     break;
                     
                 }
                 else if (docFinded.ClassInfo.ClassType == typeof(DocInvoiceOrder))//"DocInvoiceOrder"
                 {
                     break;
                 }                 
             }
             return docFinded;
         }

         // 3. основной метод для сканирования товара, сканирования серийного номера товара и привязки их в хранилище
         private bool scanTovarBarcodeSN(BaseDocument docInvoiceBase)
         {
             // 1. приведение базового документа к расходному ордеру
             DocInvoiceOrder docInvoiceOrder = (DocInvoiceOrder)docInvoiceBase;
             XPCollection<BarcodesOfShipments> barcodesOfShipments = new XPCollection<BarcodesOfShipments>(session);

             structScanStringParams paramScan = new structScanStringParams() 
             { 
                 captionTwo = "Сканируйте товар:", 
                 successScan = false, 
                 captionHelp = getTopCaption() 
             };

             UserSelect userSelect = new UserSelect();

             Goods goodFinded = null;
             while (1 == 1)
             {
                 logger.Trace("scanTovarBarcodeSN().Перед вызовом сканирования ШК товара");
                 // 1. вызов диалога сканирования товара
                 DialogResult dlgResult = userSelect.scanStringDialog(ref paramScan);
                 if (dlgResult != DialogResult.OK)
                 {
                     logger.Trace("scanTovarBarcode().Диалог сканирования ШК товара вернул = {0}", dlgResult);
                     return false;
                 }
                 if (!paramScan.successScan)
                 {
                     logger.Trace("paramScan.successScan = {0}", paramScan.successScan);
                     continue;
                 }
                 string strBarcode = paramScan.scanedBarcode;
                 logger.Trace("scanTovarBarcode(). отсканирован ШК товара = {0}", strBarcode);

                 // 1.2 поиск товара по ШК:
                 goodFinded = null;// new Goods(session).findByBarcode(strBarcode);
                 if (goodFinded == null)
                 {
                     paramScan.captionTwo = "Не найден товар по штрихкоду " + strBarcode;
                     logger.Trace("scanTovarBarcode(). ошибка поиска товара в справочнике по ШК = {0}", strBarcode);
                     continue;
                 }

                 // 1.3 поиск товара в табличной части документа:
                 var listOfGoodStrings = from c in docInvoiceOrder.DocListOfGoods where c.Good.idd==goodFinded.idd select c;
                 if (listOfGoodStrings.Count()==0)
                 {
                     paramScan.captionTwo = "В расходной накладной не найден товар:" + goodFinded.ShortDescription;
                     logger.Trace("scanTovarBarcode(). ошибка поиска товара {0} {1} в табличной части РНК= {2}", goodFinded.Code, goodFinded.ShortDescription, docInvoiceOrder.DocNo);
                     continue;
                 }

                 // 2. вызов диалога сканирования СН товара
                 paramScan.captionOne = goodFinded.ShortDescription;
                 paramScan.captionTwo = "Сканируйте серийный номер:";
                 paramScan.captionHelp = getTopCaption();                 

                 dlgResult = userSelect.scanStringDialog(ref paramScan);
                 if (dlgResult != DialogResult.OK)
                 {
                     logger.Trace("scanTovarBarcode().Диалог сканирования ШК серийного номера вернул = {0}", dlgResult);
                     continue;
                 }
                 if (!paramScan.successScan)
                 {
                     logger.Trace("scanTovarBarcode().Диалог сканирования ШК серийного номера вернул paramScan.successScan = {0}", paramScan.successScan);
                     continue;
                 }
                 string strBarcodeSN = paramScan.scanedBarcode;
                 logger.Trace("scanTovarBarcode(). Товар= {0} {1},отсканирован SN товара = {2}", goodFinded.Code, goodFinded.ShortDescription, strBarcodeSN);

                 if (strBarcodeSN.Length==0)
                 {
                     logger.Trace("scanTovarBarcode(). Ошибка: длина SN=0. Товар= {0} {1},отсканирован SN товара = {2}", goodFinded.Code, goodFinded.ShortDescription, strBarcodeSN);
                     continue;
                 }
                 // 2.2 поиск записи в хранилище серийных номеров
                 var listOfBarcodes = from c in barcodesOfShipments where c.Barcode==strBarcodeSN && c.BaseDocument==docInvoiceBase select c;
                 if (listOfBarcodes.Count()>0)
                 {
                     paramScan.captionTwo = "Серийный номер "+strBarcodeSN+" уже сканировался!";
                     logger.Trace("scanTovarBarcode(). Серийный номер {0} уже сканировался для РНК= {1}", strBarcodeSN, docInvoiceOrder.DocNo);
                     continue;
                 }
                 
                 // 2.3 добавление записи в хранилище баркодов клиента
                 BarcodesOfShipments newBarcodeOfShipment = objectSpace.CreateObject<BarcodesOfShipments>();
                 newBarcodeOfShipment.Barcode   = strBarcodeSN;
                 newBarcodeOfShipment.BaseDocument = objectSpace.GetObject<BaseDocument>(docInvoiceBase);
                 newBarcodeOfShipment.Delimeter = objectSpace.GetObject<Delimeters>(currentSessionSettings.UserSetting.ParentID.DefaultDelimeter);
                 newBarcodeOfShipment.DocNumber = docInvoiceBase.DocNo;
                 newBarcodeOfShipment.DTimeScan = DateTime.Now.ToLocalTime();
                 newBarcodeOfShipment.Good      = objectSpace.GetObject<Goods>(goodFinded);
                 newBarcodeOfShipment.idGUID    = Guid.NewGuid();
                 newBarcodeOfShipment.User      = objectSpace.GetObject<Users>(currentSessionSettings.UserSetting.ParentID);

                 newBarcodeOfShipment.Save();
                 try
                 {
                     objectSpace.CommitChanges();
                 }
                 catch (System.Exception ex)
                 {
                     logger.Error(ex, "scanTovarBarcode(). исключение при попытке сохранить в БД элемент таблицы BarcodeOfShipments");
                     paramScan.captionTwo = "ошибка БД. Штрихкод в базе не сохранен!";
                 }                 
                 
                 // 2.4 все успешно добавлено, вывод сообщения в форму и в логи
                 paramScan.captionOne = "SN:"+strBarcodeSN+" успешно записан.";
                 paramScan.captionTwo = "Сканируйте следующий товар:";

                 logger.Trace("scanTovarBarcode: Серийный номер {0} успешно записан для РНК= {1}", strBarcodeSN, docInvoiceOrder.DocNo);
                 break;
             }            
             return true;
         }

        #endregion

    }
}
