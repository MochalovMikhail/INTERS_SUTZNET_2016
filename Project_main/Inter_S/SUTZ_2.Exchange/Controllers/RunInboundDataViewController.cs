using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using SUTZ_2.Exchange;
using SUTZ_2.Exchange.SUTZ1C_SUTZNET;

namespace SUTZ_2.Module.BO.Exchange.Controllers
{
    public partial class RunInboundDataViewController : ViewController
    {
        private SQL_Exchange_Inbound currentObject;
        private BackgroundWorker bwWorker;
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            currentObject = (SQL_Exchange_Inbound)View.CurrentObject;
            bwWorker = new BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
        }

        public RunInboundDataViewController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        private void showMessageInForm(string data)
        {
            
            //currentObject.MessageWindow = data + "\r\n" + currentObject.MessageWindow;
            //View.Refresh();
        }

        private void DoRunInboundData_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            //BackgroundWorker worker = new BackgroundWorker();
            if (!bwWorker.IsBusy)
            {
                bwWorker.DoWork += new DoWorkEventHandler(worker_DoWork);
                bwWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
                bwWorker.ProgressChanged += new ProgressChangedEventHandler(bwWorker_ProgressChanged);
                bwWorker.RunWorkerAsync();
            }
            else
            {
                bwWorker.CancelAsync();
            }
        }

        void bwWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //showMessageInForm()
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            View.ObjectSpace.Refresh();
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            SQL_Exchange_Inbound inbound = new SQL_Exchange_Inbound(new delegateShowMessage(showMessageInForm), ObjectSpace);

            //((DetailView)View).CurrentObject - тоже самое, что и "currentObject"
            inbound.IsLoadCountries = currentObject.IsLoadCountries;
            inbound.isLoadGoods = currentObject.isLoadGoods;
            inbound.IsLoadManufacturers = currentObject.IsLoadManufacturers;
            inbound.IsLoadOKEI = currentObject.IsLoadManufacturers;
            inbound.IsLoadProperties = currentObject.IsLoadProperties;
            inbound.IsLoadStockRooms = currentObject.IsLoadStockRooms;
            inbound.IsLoadStorageCodes = currentObject.IsLoadStorageCodes;
            inbound.IsLoadUnitOfGoods = currentObject.IsLoadUnitOfGoods;
            inbound.InboundAllData();
        }
    }
}
