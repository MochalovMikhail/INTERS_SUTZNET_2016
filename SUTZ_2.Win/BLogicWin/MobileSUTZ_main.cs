using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SUTZ_2.Module.BO.References;
using SUTZ_2.Module.BO;
using DevExpress.Xpo;
using DevExpress.Utils;
using System.Collections;
using System.Windows.Forms;
using SUTZ_2.Win;
using SUTZ_2.Module.Editors;
using System.Diagnostics;
using DevExpress.XtraEditors;
using DevExpress.ExpressApp;
using SUTZ_2.Module.BO.References.Mobile.Modules;
using SUTZ_2.Module;

namespace SUTZ_2.MobileSUTZ
{
    public class ScanFormEventArgs : EventArgs
    {
        //private string barcode_;
        //public string barcode
        //{
        //    get { return barcode_; }
        //    set { barcode_ = value; }
        //}
        
        private JobTypes jobtype_;
        public JobTypes jobType
        {
            get { return jobtype_; }
            set { jobtype_ = value; }
        }

        // штрих-код успешно отсканирован пользователем или нажал отмена
        //private bool successScan_;
        //public bool successScan
        //{
        //    get { return successScan_; }
        //    set { successScan_ = value; }
        //}
        //private string statusScan_;
        //public string statusScan
        //{
        //    get { return statusScan_; }
        //    set { statusScan_ = value; }
        //}
        
        public ScanFormEventArgs()
        {
            //successScan = false;
        }
    }

    // возможные режимы работы диалогового окна:
    public enum enumInputMode : byte
    {
        СканированиеШтрихкода=1,
        ВводТолькоДаты=2,
        ВводТолькоКоличестваФакт=3,
        ВводКоличестваФактИВидимостьПлан=4,
        ВопросДаНет=5,
        ТолькоСообщениеиОК=6
    }

    // для передачи-возврата параметров в диалоги будет использоваться структура:
    public struct structScanStringParams
    {
        public string captionOne;
        public string captionTwo;
        //public string captionError;
        public string scanedBarcode;
        public bool successScan;
        //public string captionTop;
        public string captionHelp;// параметр для сохранения полной информации о текущем состоянии сканирования
        public enumInputMode inputMode;// режим работы диалога сканирования
        public bool enableQuestionButton;
        public bool disableCancelButton;
        
        // параметр для сохранения справочной информации. будет заполняться для каждой формы индивидуально и вызываться по кнопке "?"
        //public string strCurrentHelp;

        // параметры для сохранения информации по количеству:
        public decimal decPlanUnits; // план - количество коробок
        public decimal decPlanItems; // план - количество штук
        public decimal decPlanTotal; // план - общее количество

        public decimal decFactUnits; // факт - количество коробок
        public decimal decFactItems; // факт - количество штук
        public decimal decFactTotal; // факт - общее количество

        // единица измерения для пересчета фактического количества:
        public Units unitFactCalculate; // единица измерения для расчета вводимого фактического количества
        public int iddMobileErrors;// идентификатор ошибки выполнения операции
    }
}

namespace SUTZ_2.MobileSUTZ
{
    [NonPersistent]
    class MobileSUTZ_main : IDisposable
    {
        public IObjectSpace objSpace {get; set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            //if (disposing)
            //    if (session_ != null)
            //    {
            //        session_.Dispose();
            //        session_ = null;
            //    }
        }
        ~MobileSUTZ_main()
        {
            Dispose(false);
        }
        public MobileSUTZ_main(Session lokSession)
        {
            Guard.ArgumentNotNull(lokSession, "lokSession");
            objSpace = currentSessionSettings.ObjXafApp.CreateObjectSpace();
        }
        public void runWorkBySelectedWorkType()
        {
            JobTypes selectedJobType = currentSessionSettings.CurrentJobType;  
            if (selectedJobType==null)
            {
                return;
            }
            if (selectedJobType.TypeOfWork == enTypeOfWorks.ПриемМаркировка)
            {
                prihodPalletLabeling workClass = new prihodPalletLabeling(objSpace); 
                bool returnValue = workClass.runScanBeginPriemMarkingGoods(selectedJobType);
            }
            else if (selectedJobType.TypeOfWork == enTypeOfWorks.СканСНПривязкаКРНК)
            {
                ScanSNForClientShipments workClass = new ScanSNForClientShipments(objSpace.Session());
                bool returnValue = workClass.runScanSNForClientShipment();
            }
            else if (selectedJobType.TypeOfWork== enTypeOfWorks.РазмещениеПрихода)
            {
                MobileSUTZ_RazmesheniePrihoda workClass = new MobileSUTZ_RazmesheniePrihoda(objSpace);
                bool returnValue = workClass.runMain(selectedJobType);
            }
            else if (  (selectedJobType.TypeOfWork == enTypeOfWorks.ПеремещениеВТочкуПередачи)
                    || (selectedJobType.TypeOfWork == enTypeOfWorks.ПеремещениеИзТочкиПередачи)
                    || (selectedJobType.TypeOfWork == enTypeOfWorks.Перемещение))
            {
                MobileSUTZ_Popolnenie workClass = new MobileSUTZ_Popolnenie(objSpace);
                bool returnValue = workClass.startModule(selectedJobType);
            }
            else if (selectedJobType.TypeOfWork == enTypeOfWorks.ОтборТовара)     
            {
                MobileSUTZ_Rashod workClass = new MobileSUTZ_Rashod();
                bool returnValue = workClass.startModule(selectedJobType);
            }
        }
    }
}
