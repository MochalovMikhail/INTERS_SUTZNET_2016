using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using SUTZ_2.Module.BO.Exchange.SUTZ1C_SUTZNET;
using DevExpress.XtraEditors;
using System.Windows.Forms;
using DevExpress.ExpressApp.Editors;
using System.Windows.Threading;
using System.Threading;
using NLog;
using System.Linq;
using SUTZ_2.Module.BO.References;
using System.Threading.Tasks;
using DevExpress.ExpressApp.Layout;
using DevExpress.XtraEditors.Repository;
using System.Drawing;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Columns;

namespace SUTZ_2.Module.BO.Exchange.Controllers
{
    public partial class RunInboundDataViewController : ViewController
    {   
        // 1. переменные для работы модуля загрузки
        private BackgroundWorker bwWorker;
        private int messageTotals = 0;// общее количество не загруженных сообщений

        // 2. переменные для работы модуля выгрузки:
        private BackgroundWorker bwWorkerOut;
        //private int messageTotalsOut = 0;// общее количество не выгруженных сообщений

        // 3. общие переменные:
        CancellationTokenSource tokenSource; // маркер отмены действия
        TaskScheduler scheduler;// планировщик задач формы       
     
        private static Logger logger = LogManager.GetCurrentClassLogger();

        // переменные для работы таймера
        private System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer() 
        { 
            Enabled = true,
            Interval = 30000
        };
     
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();

            // 1. создание класса фоновой задачи для загрузки данных
            bwWorker = new BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            //bwWorker.DoWork += new DoWorkEventHandler(RunTestAsync);
            //bwWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_TestWorkerCompleted);
            //bwWorker.ProgressChanged += new ProgressChangedEventHandler(bwWorker_TestProgressChanged);

            bwWorker.DoWork += new DoWorkEventHandler(worker_DoWorkIn);
            bwWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            //bwWorker.ProgressChanged += new ProgressChangedEventHandler(bwWorker_ProgressChanged);

            // 2. создание класса фоновой задачи для выгрузки данных
            bwWorkerOut = new BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            //bwWorker.DoWork += new DoWorkEventHandler(RunTestAsync);
            //bwWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_TestWorkerCompleted);
            //bwWorker.ProgressChanged += new ProgressChangedEventHandler(bwWorker_TestProgressChanged);

            bwWorkerOut.DoWork += new DoWorkEventHandler(worker_DoWorkOut);
            bwWorkerOut.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            //bwWorker.ProgressChanged += new ProgressChangedEventHandler(bwWorker_ProgressChanged);

            // 3. запомним текущий планировщик задач
            scheduler = TaskScheduler.FromCurrentSynchronizationContext();

            // 4. создание таймера           
            timer.Tick += new EventHandler(timer_Elapsed);
            timer.Start();           
        }

        // старая версия, рабочая
        //void timer_Elapsed(object sender, EventArgs e)
        //{
        //    //logger.Trace("Вызов timer_Elapsed");

        //    if (0 == Interlocked.Exchange(ref usingResource, 1))
        //    {
        //        //logger.Trace("Вызов timer_Elapsed, ресурс доступен: usingResource={0}",usingResource);
        //        if (View == null)
        //        {
        //            return;
        //        }

        //        if (((SQL_Exchange_Inbound)View.CurrentObject).IsRunOutBound)
        //        {
        //            // Старая версия:
        //            //((Control)Frame.Template).Invoke(new ThreadStart(delegate()
        //            //{
        //            //    //showMessageInForm("Начало загрузки данных из СУТЗ 1С:7.7");
        //            //    //logger.Trace("Начало загрузки данных из СУТЗ 1С:7.7");
        //            //    RunSQLInboundExchangeSync();

        //            //    //showMessageInForm("Начало выгрузки данных в СУТЗ 1С:7.7");
        //            //    //logger.Trace("Начало выгрузки данных в СУТЗ 1С:7.7");
        //            //    RunSQLOutboundData();
        //            //}
        //            //));

        //            // новая версия, предложенная:
        //            //var t = Task.Factory.StartNew(() => ((Control)Frame.Template).Invoke(new ThreadStart(delegate()
        //            //{
        //            //    //showMessageInForm("Начало загрузки данных из СУТЗ 1С:7.7");
        //            //    //logger.Trace("Начало загрузки данных из СУТЗ 1С:7.7");
        //            //    RunSQLInboundExchangeSync();
        //            //    //showMessageInForm("Начало выгрузки данных в СУТЗ 1С:7.7");
        //            //    //logger.Trace("Начало выгрузки данных в СУТЗ 1С:7.7");
        //            //    RunSQLOutboundData();
        //            //})));
        //            //t.Wait();
        //        };
        //        Interlocked.Exchange(ref usingResource, 0);
        //    }
        //}


        //BackgroundWorker: обработчик для фонового задания загрузки из 7.7
        void worker_DoWorkIn(object sender, DoWorkEventArgs e)
        {
            RunSQLInboundExchange();
        }
        //BackgroundWorker: обработчик для фонового задания выгрузки в 7.7
        void worker_DoWorkOut(object sender, DoWorkEventArgs e)
        {
            RunSQLOutboundData();
        }

        //Timer: обработчик срабатывания таймера формы, единый для всех загрузчиков:
        void timer_Elapsed(object sender, EventArgs e)
        {
            // 1. проверка фонового задания по загрузке объектов:
            if (!bwWorker.IsBusy)
            {
                if ((((SQL_Exchange_Inbound)View.CurrentObject).IsTimerEnabled))
                {
                    bwWorker.RunWorkerAsync();
                }
            }

            // 2. проверка фонового задания по выгрузке объектов:
            if (!bwWorkerOut.IsBusy)
            {
                if ((((SQL_Exchange_Inbound)View.CurrentObject).IsTimerEnabled))
                {
                    bwWorkerOut.RunWorkerAsync();
                }
            }            
        }

        protected override void OnActivated()
        {
            base.OnActivated();

    
            if (((Users)SecuritySystem.CurrentUser).MonitorResolution == enMonitorResolution.Symbol_MS900)
            {
                //DoRunInboundData.Active["Disable for user Сканировщик"] = false;
               // DoRunOutBoundData.Active["Disable for user Сканировщик"] = false;
            }
         }

        public RunInboundDataViewController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        // этот метод передается как делегат в класс SQL_Exchange_Inbound
        //private void showMessageInForm(string data)
        //{
            //SQL_Exchange_Inbound currentObject = (SQL_Exchange_Inbound)View.CurrentObject;
            // вариант вывода сообщений через BackgroundWorker
           // bwWorker.ReportProgress(0, data);

            //// вариант вывода сообщений через задачу:
            //Task task = new Task(() =>
            //{

            //    if (messageTextEditor == null)
            //    {
            //        messagePropertyEditor = ((DetailView)View).FindItem("MessageWindow") as PropertyEditor;
            //        messageTextEditor = messagePropertyEditor.Control as TextEdit;
            //        if (messageTextEditor == null)
            //        {
            //            return;
            //        }
            //    }

            //    String lokData = DateTime.Now.ToLocalTime().ToString("dd.MM.yyyy HH:mm:ss.fff tt") + " " + data;
            //    SQL_Exchange_Inbound currentObject = (SQL_Exchange_Inbound)View.CurrentObject;
            //    currentObject.MessageWindow = lokData + "\r\n" + currentObject.MessageWindow;
            //    if (messageBufferCounter >= messageBufferLenght)
            //    {
            //        //View.Refresh();
            //        messageBufferCounter = 0;
            //    }
            //    else
            //    {
            //        messageBufferCounter += 1;
            //    }

            //    if (currentObject.MessageWindow.Length > 5000)
            //    {
            //        currentObject.MessageWindow = currentObject.MessageWindow.Substring(0, 5000);
            //    }
            //    //currentObject.ListOfMessages.Add(new LogMessages() { Id = ((currentObject.ListOfMessages.Count) + 1), MessageText = data, MessageTime = DateTime.Now, MessageType = "info" });
            //});
            //task.Start(scheduler);

            //if (Thread.CurrentThread != m_UIThread)
            //{
            //    ((Control)Frame.Template).BeginInvoke((ThreadStart)delegate(){
            //        showMessageInForm(data);
            //    });
            //    //if (messageTextEditor==null)
            //    //{
            //    //    messagePropertyEditor = ((DetailView)View).FindItem("MessageWindow") as PropertyEditor;
            //    //    if (messagePropertyEditor.Control != null)
            //    //    {
            //    //        messageTextEditor = messagePropertyEditor.Control as TextEdit;
            //    //    }                
            //    //}
            //    //if (messageTextEditor != null)
            //    //{
            //    //    messageTextEditor.BeginInvoke((ThreadStart)delegate()
            //    //    {
            //    //        showMessageInForm(data);
            //    //    });
            //    //}
            //}
            
  
            // 2. получение объекта окна сообщений:

            //if (messageTextEditor==null)
            //{
            //    messagePropertyEditor = ((DetailView)View).FindItem("MessageWindow") as PropertyEditor;
            //    messageTextEditor = messagePropertyEditor.Control as TextEdit;
            //    if (messageTextEditor==null)
            //    {
            //        return;
            //    }                
            //}                      

            //messageTextEditor.BeginInvoke( (ThreadStart)delegate()
            //{
            //    if (boolEnableMessage != null)
            //    {
            //        if ((bool)boolEnableMessage.PropertyValue != true)
            //        {
            //            return;
            //        }
            //    }

                //String lokData = DateTime.Now.ToLocalTime().ToString("dd.MM.yyyy HH:mm:ss.fff tt") + " " + data;
                //SQL_Exchange_Inbound currentObject = (SQL_Exchange_Inbound)View.CurrentObject;
                //currentObject.MessageWindow = lokData + "\r\n" + currentObject.MessageWindow;
                //if (messageBufferCounter >= messageBufferLenght)
                //{
                //    //View.Refresh();
                //    messageBufferCounter = 0;
                //}
                //else
                //{
                //    messageBufferCounter += 1;
                //}

                //if (currentObject.MessageWindow.Length>5000)
                //{
                //    currentObject.MessageWindow=currentObject.MessageWindow.Substring(0,5000);
                //}
                //currentObject.ListOfMessages.Add(new LogMessages() { Id=((currentObject.ListOfMessages.Count)+1), MessageText=data, MessageTime = DateTime.Now, MessageType="info"});
            //System.Windows.Forms.Application.DoEvents();
       // }

        // метод передается как делегат в класс SQL_Exchange_Inbound для обработки количества загружаемых сообщений
        
        private void showCalculatedItems(Type objType, int rowCounts)
        {
            Object[] mParams = {objType, rowCounts};

            var timeX = Task.Factory.StartNew((Object obj_) => ((Control)Frame.Template).BeginInvoke((ThreadStart)delegate()
            {
                SQL_Exchange_Inbound currentObject = (SQL_Exchange_Inbound)View.CurrentObject;
                Object[] lokParams = obj_ as Object[];
                Type objType_ = lokParams[0] as Type;
                int rowCounter = (int)lokParams[1];

                var currentItem = currentObject.ListOfTypeMessages.FirstOrDefault(x => x.objType == objType_);// ElementAt(item.Id);
                if (currentItem != null)
                {
                    currentItem.inbounded += rowCounter;
                    currentItem.totalItemsInbound += rowCounter;
                    if ((currentItem.forInbound - rowCounter) >= 0)
                    {
                        currentItem.forInbound -= rowCounter;
                    }

                    // вычисление процента выполнения:
                    currentItem.percentage =  (currentItem.inbounded * 10000 / (currentItem.inbounded + currentItem.forInbound));

                    currentObject.ListOfTypeMessages.ResetItem(currentObject.ListOfTypeMessages.IndexOf(currentItem));
                }

                // 2.3.2 установка свойств прогресс бара для колонки:
                //ListPropertyEditor listMessagesTypes = ((DetailView)View).FindItem("ListOfTypeMessages") as ListPropertyEditor;
                //if (listMessagesTypes != null)
                //{
                //    DevExpress.ExpressApp.ListView nestedListView = listMessagesTypes.ListView;
                //    GridListEditor listEditor = nestedListView.Editor as GridListEditor;
                //    GridView nestedGridView = listEditor.GridView;
                //    GridColumn barColumn = nestedGridView.Columns.ColumnByFieldName("percentage");
                //    if (barColumn != null)
                //    {
                //        RepositoryItemProgressBar rpiProgrBar = (RepositoryItemProgressBar)((GridListEditor)listMessagesTypes.ListView.Editor).GridView.Columns["inbounded"].ColumnEdit;
                //        System.Diagnostics.Debug.WriteLine(String.Format("showCalculatedItems: objType = {0}, maximum progBar = {1}", objType_, rpiProgrBar.Maximum));

                //    }
                //}

                
            }), mParams);
        }

        // 
        private void showCalculatedOutItems(Type objType, int rowCounts)
        {
            Object[] mParams = { objType, rowCounts };

            var timeX = Task.Factory.StartNew((Object obj_) => ((Control)Frame.Template).BeginInvoke((ThreadStart)delegate()
            {
                SQL_Exchange_Inbound currentObject = (SQL_Exchange_Inbound)View.CurrentObject;
                Object[] lokParams = obj_ as Object[];
                Type objType_ = lokParams[0] as Type;
                int rowCounter = (int)lokParams[1];

                var currentItem = currentObject.ListOfTypeOUTMessages.FirstOrDefault(x => x.objType == objType_);// ElementAt(item.Id);
                if (currentItem != null)
                {
                    currentItem.inbounded += rowCounter;
                    currentItem.totalItemsInbound += rowCounter;
                    if ((currentItem.forInbound - rowCounter) >= 0)
                    {
                        currentItem.forInbound -= rowCounter;
                    }
                    // вычисление процента выполнения:
                    currentItem.percentage = (currentItem.inbounded * 10000 / (currentItem.inbounded + currentItem.forInbound));
                    currentObject.ListOfTypeOUTMessages.ResetItem(currentObject.ListOfTypeOUTMessages.IndexOf(currentItem));
                }

                // 2.3.2 установка свойств прогресс бара для колонки:
                //ListPropertyEditor listMessagesTypes = ((DetailView)View).FindItem("ListOfTypeMessages") as ListPropertyEditor;
                //if (listMessagesTypes != null)
                //{
                //    DevExpress.ExpressApp.ListView nestedListView = listMessagesTypes.ListView;
                //    GridListEditor listEditor = nestedListView.Editor as GridListEditor;
                //    GridView nestedGridView = listEditor.GridView;
                //    GridColumn barColumn = nestedGridView.Columns.ColumnByFieldName("percentage");
                //    if (barColumn != null)
                //    {
                //        RepositoryItemProgressBar rpiProgrBar = (RepositoryItemProgressBar)((GridListEditor)listMessagesTypes.ListView.Editor).GridView.Columns["inbounded"].ColumnEdit;
                //        System.Diagnostics.Debug.WriteLine(String.Format("showCalculatedItems: objType = {0}, maximum progBar = {1}", objType_, rpiProgrBar.Maximum));

                //    }
                //}


            }), mParams);
        }
        // синхронный вызов выгрузки данных:
        private void RunSQLOutboundData()
        {
            IObjectSpace nestedObjectspace = Application.CreateObjectSpace();
            SQL_Exchange_NET_77 outbound = new SQL_Exchange_NET_77(nestedObjectspace);
            outbound.delegateForShowMessage += addOutboundMessageToForm;
            outbound.delegateForCalculateItems += showCalculatedOutItems;

            // 2. получение количества выгружаемых объектов с количеством элементов для выгрузки:
            int totalRows = outbound.calculateAllOutBoundRows();
            var timeX = Task.Factory.StartNew((Object obj) => ((Control)Frame.Template).BeginInvoke((ThreadStart)delegate()
            {
                // 2.1 установка настроек для прогресс бара
                ControlDetailItem itemProgressControl = ((DetailView)View).FindItem("ProgressBarControlOut") as ControlDetailItem;
                if (itemProgressControl != null)
                {
                    DevExpress.XtraEditors.ProgressBarControl itemProgressBar = itemProgressControl.Control as DevExpress.XtraEditors.ProgressBarControl;
                    if (itemProgressBar != null)
                    {
                        itemProgressBar.Properties.StartColor = System.Drawing.Color.Tomato;
                        itemProgressBar.Properties.StartColor = System.Drawing.Color.Chartreuse;
                        itemProgressBar.Properties.Step = 1;
                        itemProgressBar.Properties.PercentView = false;
                        itemProgressBar.Properties.ShowTitle = true;
                        itemProgressBar.Properties.Maximum = 100;
                        itemProgressBar.Properties.Minimum = 0;
                        itemProgressBar.ShowProgressInTaskBar = true;
                        itemProgressBar.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(itemProgressBar_CustomDisplayTextOut);
                    }
                }

                // 2.2 установка общего количества невыгруженных сообщений
                messageTotals = totalRows;
                ControlDetailItem itemProgressControl_ = ((DetailView)View).FindItem("ProgressBarControlOut") as ControlDetailItem;
                if (itemProgressControl_ != null)
                {
                    ProgressBarControl itemProgressBar_ = itemProgressControl_.Control as ProgressBarControl;
                    if (itemProgressBar_ != null)
                    {
                        itemProgressBar_.Properties.Maximum = totalRows;
                        itemProgressBar_.EditValue = 0;
                    }
                }

                //// 2.3 установка в табличной части количества выгружаемых сообщений по объектам:
                //SQL_Exchange_Inbound lokInbound = obj as SQL_Exchange_Inbound;
                //if (lokInbound != null)
                //{
                //    // 2.3.1 установка в каждой строке количества загружаемых объектов:
                //    foreach (LogTypeMessages item in lokInbound.ListOfTypeMessages)
                //    {
                //        var currentItem = currentObject.ListOfTypeMessages.FirstOrDefault(x => x.objType == item.objType);// ElementAt(item.Id);
                //        if (currentItem != null)
                //        {
                //            currentItem.forInbound = item.forInbound;
                //            currentItem.percentage = 0;//обнулим процентовку выполнения 
                //            currentItem.inbounded = 0;// обнулим количество ранее загруженных сообщений
                //        }
                //    }

                //    // 2.3.2 установка свойств прогресс бара для колонки:
                //    ListPropertyEditor listMessagesTypes = ((DetailView)View).FindItem("ListOfTypeMessages") as ListPropertyEditor;
                //    if (listMessagesTypes != null)
                //    {
                //        DevExpress.ExpressApp.ListView nestedListView = listMessagesTypes.ListView;
                //        GridListEditor listEditor = nestedListView.Editor as GridListEditor;
                //        GridView nestedGridView = listEditor.GridView;
                //        GridColumn barColumn = nestedGridView.Columns.ColumnByFieldName("percentage");

                //        if (barColumn != null)
                //        {
                //            RepositoryItemProgressBar rpiProgrBar = new RepositoryItemProgressBar();//(RepositoryItemProgressBar)barColumn.ColumnEdit;
                //            rpiProgrBar.Maximum = 10000;
                //            rpiProgrBar.Minimum = 0;
                //            rpiProgrBar.PercentView = false;
                //            rpiProgrBar.ShowTitle = true;
                //            rpiProgrBar.Step = 1;
                //            rpiProgrBar.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(itemProgressBarGrid_CustomDisplayText);
                //            rpiProgrBar.LookAndFeel.UseDefaultLookAndFeel = false;
                //            rpiProgrBar.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
                //            rpiProgrBar.ProgressViewStyle = DevExpress.XtraEditors.Controls.ProgressViewStyle.Solid;
                //            rpiProgrBar.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                //            rpiProgrBar.StartColor = Color.Red;
                //            rpiProgrBar.EndColor = Color.LightPink;
                //            //barColumn.ColumnEdit = rpiProgrBar;

                //            ((GridListEditor)listMessagesTypes.ListView.Editor).GridView.Columns["percentage"].ColumnEdit = rpiProgrBar;
                //        }
                //    }
                //}

            }), outbound);                


            // вызов модуля загрузки
            outbound.runOutboundAllDate();
        }
        // синхронный вызов загрузки данных
        private void RunSQLInboundExchange()
        {
            tokenSource = new CancellationTokenSource();// маркер отмены
            // 3. установка параметров для прогресс бара:
            
            CancellationToken token = tokenSource.Token;
            IObjectSpace nestedObjectspace = Application.CreateObjectSpace();
            SQL_Exchange_Inbound inbound = new SQL_Exchange_Inbound(nestedObjectspace, token);
            inbound.delegateForShowMessage += addInboundMessageToForm;
            inbound.delegateForCalculateItems += showCalculatedItems;

            SQL_Exchange_Inbound currentObject = (SQL_Exchange_Inbound)View.CurrentObject;

            //((DetailView)View).CurrentObject - тоже самое, что и "currentObject"
            inbound.IsLoadUsers = currentObject.IsLoadUsers;
            inbound.IsLoadCountries = currentObject.IsLoadCountries;
            inbound.isLoadGoods = currentObject.isLoadGoods;
            inbound.IsLoadManufacturers = currentObject.IsLoadManufacturers;
            inbound.IsLoadOKEI = currentObject.IsLoadManufacturers;
            inbound.IsLoadProperties = currentObject.IsLoadProperties;
            inbound.IsLoadStockRooms = currentObject.IsLoadStockRooms;
            inbound.IsLoadStorageCodes = currentObject.IsLoadStorageCodes;
            inbound.IsLoadUnitOfGoods = currentObject.IsLoadUnitOfGoods;
            inbound.IsLoadClients = currentObject.IsLoadClients;
            inbound.IsLoadDocTrebovanie = currentObject.IsLoadDocTrebovanie;
            inbound.IsLoadJobTypes = currentObject.IsLoadJobTypes;
            inbound.IsLoadLogistics = currentObject.IsLoadLogistics;
            inbound.IsLoadOKEI = currentObject.IsLoadOKEI;
            inbound.IsLoadCustomerDivisions = currentObject.IsLoadCustomerDivisions;
            inbound.IsLoadCustomerStores = currentObject.IsLoadCustomerStores;
            inbound.IsLoadWarehouses = currentObject.IsLoadWarehouses;
            inbound.IsLoadBarcodesOfGoods = currentObject.IsLoadBarcodesOfGoods;

            // загрузчики документов:
            inbound.IsLoadDocInventory = currentObject.IsLoadDocInventory;
            inbound.IsLoadDocInvoiceOrder = currentObject.IsLoadDocInvoiceOrder;
            inbound.IsLoadDocSpecPeremesh = currentObject.IsLoadDocSpecPeremesh;
            inbound.IsLoadDocSpecPrihoda = currentObject.IsLoadDocSpecPrihoda;
            inbound.IsLoadDocSpecRashoda = currentObject.IsLoadDocSpecRashoda;
            inbound.IsLoadDocTrebovanie = currentObject.IsLoadDocTrebovanie;

            inbound.IsLoadDocProperties = currentObject.IsLoadDocProperties;
            inbound.IsLoadDocJobeProperties = currentObject.IsLoadDocJobeProperties;
            inbound.IsTimerEnabled = currentObject.IsTimerEnabled;

            // 2. определение общего количества загружаемых элементов:
            int totalRows = inbound.calculateAllInboudRows();
            var timeX = Task.Factory.StartNew((Object obj) => ((Control)Frame.Template).BeginInvoke((ThreadStart)delegate()
            {
                // 2.1 установка настроек для прогресс бара
                ControlDetailItem itemProgressControl = ((DetailView)View).FindItem("ProgressBarControlIn") as ControlDetailItem;
                if (itemProgressControl != null)
                {
                    DevExpress.XtraEditors.ProgressBarControl itemProgressBar = itemProgressControl.Control as DevExpress.XtraEditors.ProgressBarControl;
                    if (itemProgressBar != null)
                    {
                        itemProgressBar.Properties.StartColor = System.Drawing.Color.Tomato;
                        itemProgressBar.Properties.StartColor = System.Drawing.Color.Chartreuse;
                        itemProgressBar.Properties.Step = 1;
                        itemProgressBar.Properties.PercentView = false;
                        itemProgressBar.Properties.ShowTitle = true;
                        itemProgressBar.Properties.Maximum = 100;
                        itemProgressBar.Properties.Minimum = 0;
                        itemProgressBar.ShowProgressInTaskBar = true;
                        itemProgressBar.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(itemProgressBar_CustomDisplayText);
                    }
                }      

                // 2.2 установка общего количества незагруженных сообщений
                messageTotals = totalRows;
                SQL_Exchange_Inbound lokInbound = obj as SQL_Exchange_Inbound;
                ControlDetailItem itemProgressControl_ = ((DetailView)View).FindItem("ProgressBarControlIn") as ControlDetailItem;
                if (itemProgressControl_ != null)
                {
                    ProgressBarControl itemProgressBar_ = itemProgressControl_.Control as ProgressBarControl;
                    if (itemProgressBar_ != null)
                    {
                        itemProgressBar_.Properties.Maximum = totalRows;
                        itemProgressBar_.EditValue = 0;
                    }
                }

                // 2.3 установка в табличной части количества загружаемых сообщений по объектам:
                if (lokInbound!=null)
                {
                    // 2.3.1 установка в каждой строке количества загружаемых объектов:
                    foreach (LogTypeMessages item in lokInbound.ListOfTypeMessages)
                    {
                        var currentItem = currentObject.ListOfTypeMessages.FirstOrDefault(x => x.objType == item.objType);// ElementAt(item.Id);
                        if (currentItem != null)
                        {
                            currentItem.forInbound = item.forInbound;
                            currentItem.percentage = 0;//обнулим процентовку выполнения 
                            currentItem.inbounded = 0;// обнулим количество ранее загруженных сообщений
                        }
                    }

                    // 2.3.2 установка свойств прогресс бара для колонки:
                    ListPropertyEditor listMessagesTypes = ((DetailView)View).FindItem("ListOfTypeMessages") as ListPropertyEditor;
                    if (listMessagesTypes != null)
                    {
                        DevExpress.ExpressApp.ListView nestedListView = listMessagesTypes.ListView;
                        GridListEditor listEditor = nestedListView.Editor as GridListEditor;
                        GridView nestedGridView = listEditor.GridView;
                        GridColumn barColumn = nestedGridView.Columns.ColumnByFieldName("percentage");
           
                        if (barColumn != null)
                        {
                            RepositoryItemProgressBar rpiProgrBar = new RepositoryItemProgressBar();//(RepositoryItemProgressBar)barColumn.ColumnEdit;
                            rpiProgrBar.Maximum = 10000;
                            rpiProgrBar.Minimum = 0;
                            rpiProgrBar.PercentView = false;
                            rpiProgrBar.ShowTitle = true;
                            rpiProgrBar.Step = 1;
                            rpiProgrBar.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(itemProgressBarGrid_CustomDisplayText);
                            rpiProgrBar.LookAndFeel.UseDefaultLookAndFeel = false;
                            rpiProgrBar.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
                            rpiProgrBar.ProgressViewStyle = DevExpress.XtraEditors.Controls.ProgressViewStyle.Solid;
                            rpiProgrBar.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                            rpiProgrBar.StartColor = Color.Red;
                            rpiProgrBar.EndColor = Color.LightPink;
                            //barColumn.ColumnEdit = rpiProgrBar;

                            ((GridListEditor)listMessagesTypes.ListView.Editor).GridView.Columns["percentage"].ColumnEdit = rpiProgrBar;
                        }
                    }

                }
                
            }),inbound);                

            inbound.InboundAllData();
        }

        // метод передается как делегат класс загрузки
        void addInboundMessageToForm(String strMessage, String traceLevel)
        {
            //System.Diagnostics.Debug.WriteLine(String.Format("addMessageToForm: Current thread id = {0}", Thread.CurrentThread.ManagedThreadId));
            if (Frame==null)
            {
                return;
            }
            var timeX = Task.Factory.StartNew(() => ((Control)Frame.Template).BeginInvoke((ThreadStart)delegate()
            {
                SQL_Exchange_Inbound currentObject = (SQL_Exchange_Inbound)View.CurrentObject;
                //System.Diagnostics.Debug.WriteLine(String.Format("addMessageToForm: Current thread id = {0}", Thread.CurrentThread.ManagedThreadId));

                // 1. добавление сообщения в таблицу всех сообщений
                if (currentObject.IsShowMessageInLogWindow)
                {
                    LogMessages newMessage = new LogMessages()
                    {
                        Id = ((currentObject.ListOfMessages.Count) + 1),
                        MessageText = strMessage,
                        MessageTime = DateTime.Now,
                        MessageType = traceLevel
                    };
                    currentObject.ListOfMessages.Add(newMessage);   
                }
 
                // 2. обновление общего прогресс бара на форме:
                ControlDetailItem itemProgressControl = ((DetailView)View).FindItem("ProgressBarControlIn") as ControlDetailItem;
                if (itemProgressControl != null)
                {
                    DevExpress.XtraEditors.ProgressBarControl itemProgressBar = itemProgressControl.Control as DevExpress.XtraEditors.ProgressBarControl;
                    if (itemProgressBar != null)
                    {
                        itemProgressBar.PerformStep();
                        if (itemProgressBar.Properties.Maximum<101)
                        {
                            itemProgressBar.Update();
                        }
                        else if (((int)itemProgressBar.EditValue % 10) == 0)
                        {
                            itemProgressBar.Update();
                        }
                    }
                }             

                // 3. 
            }));

            //timeX.Wait();            
        }

        // метод передается как делегат класс загрузки
        void addOutboundMessageToForm(String strMessage, String traceLevel)
        {
            //System.Diagnostics.Debug.WriteLine(String.Format("addMessageToForm: Current thread id = {0}", Thread.CurrentThread.ManagedThreadId));
            if (Frame == null)
            {
                return;
            }
            var timeX = Task.Factory.StartNew(() => ((Control)Frame.Template).BeginInvoke((ThreadStart)delegate()
            {
                SQL_Exchange_Inbound currentObject = (SQL_Exchange_Inbound)View.CurrentObject;
                //System.Diagnostics.Debug.WriteLine(String.Format("addMessageToForm: Current thread id = {0}", Thread.CurrentThread.ManagedThreadId));

                // 1. добавление сообщения в таблицу всех сообщений
                if (currentObject.IsShowMessageInLogWindow)
                {
                    LogMessages newMessage = new LogMessages()
                    {
                        Id = ((currentObject.ListOfOUTMessages.Count) + 1),
                        MessageText = strMessage,
                        MessageTime = DateTime.Now,
                        MessageType = traceLevel
                    };
                    currentObject.ListOfOUTMessages.Add(newMessage);
                }

                // 2. обновление общего прогресс бара на форме:
                ControlDetailItem itemProgressControl = ((DetailView)View).FindItem("ProgressBarControlOut") as ControlDetailItem;
                if (itemProgressControl != null)
                {
                    DevExpress.XtraEditors.ProgressBarControl itemProgressBar = itemProgressControl.Control as DevExpress.XtraEditors.ProgressBarControl;
                    if (itemProgressBar != null)
                    {
                        itemProgressBar.PerformStep();
                        if (itemProgressBar.Properties.Maximum < 101)
                        {
                            itemProgressBar.Update();
                        }
                        else if (((int)itemProgressBar.EditValue % 10) == 0)
                        {
                            itemProgressBar.Update();
                        }
                    }
                }

                // 3. 
            }));

            //timeX.Wait();            
        }
        //void bwWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        //{
        //    String strMessage = (String)e.UserState;
            
        //    addMessageToForm(strMessage);
        //}

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            View.ObjectSpace.Refresh();
            if (e.Error != null)
            {
                addInboundMessageToForm(e.Error.ToString(), "error");
            }
        }

 
        // обработчик нажатия кнопки на форме:
        private void DoRunInboundData_Execute(object sender, SimpleActionExecuteEventArgs e)
        {

            if (!bwWorker.IsBusy)
            {
                
                bwWorker.RunWorkerAsync();
            }
            else
            {
                DialogResult myDlgResult = XtraMessageBox.Show("Остановить загрузку данных?", "Обработка загрузки данных", MessageBoxButtons.YesNo);
                if (myDlgResult == DialogResult.Yes)
                {
                    bwWorker.CancelAsync();
                    tokenSource.Cancel();
                }
            }
        }

        void itemProgressBar_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            ProgressBarControl bar = sender as ProgressBarControl;
            if (bar != null)
            {
                //bar.Properties.StartColor = ((int)e.Value < 95000 ? Color.Red : Color.Green);
                //bar.Properties.EndColor = ((int)e.Value < 95000 ? Color.LightPink : Color.LightGreen);
                e.DisplayText = String.Format("Загружено {0} элементов из {1}", (int)e.Value, messageTotals);
            }
        }

        void itemProgressBar_CustomDisplayTextOut(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            ProgressBarControl bar = sender as ProgressBarControl;
            if (bar != null)
            {
                //bar.Properties.StartColor = ((int)e.Value < 95000 ? Color.Red : Color.Green);
                //bar.Properties.EndColor = ((int)e.Value < 95000 ? Color.LightPink : Color.LightGreen);
                e.DisplayText = String.Format("Выгружено {0} элементов из {1}", (int)e.Value, messageTotals);
            }
        }


        void itemProgressBarGrid_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            RepositoryItemProgressBar bar = sender as RepositoryItemProgressBar;
            if (bar != null)
            {
                bar.StartColor = ((((int)e.Value) / 100) < 10 ? Color.Red : Color.Green);
                bar.EndColor = ((((int)e.Value) / 100) < 10 ? Color.LightPink : Color.LightGreen);
                //bar.Properties.StartColor = ((int)e.Value < 95000 ? Color.Red : Color.Green);
                //bar.Properties.EndColor = ((int)e.Value < 95000 ? Color.LightPink : Color.LightGreen);
                e.DisplayText = String.Format("Загружено {0} %", (((int)e.Value)/100));
            }
        }

        private void RunInboundDataViewController_ViewControlsCreated(object sender, EventArgs e)
        {
            // установка ссылок на элементы формы:
            //boolEnableMessage = ((DetailView)View).FindItem("IsShowMessageInLogWindow") as PropertyEditor;
            //messagePropertyEditor = ((DetailView)View).FindItem("MessageWindow") as PropertyEditor;
            //messageTextEditor = messagePropertyEditor.Control as TextEdit;

        }

        private void RunInboundDataViewController_Activated(object sender, EventArgs e)
        {
            if (((Users)SecuritySystem.CurrentUser).MonitorResolution == enMonitorResolution.Symbol_MS900)
            {
                //((SimpleAction)sender).Active = false;
               // int a = 1;
            }
        }

        private void RunInboundDataViewController_Deactivated(object sender, EventArgs e)
        {
            if (timer!=null)
            {
                timer.Stop();
            }
            //usingResource = 0;
        }


       #region ТестовыеМетодыДляТестированияФоновогоПотока
	       private void RunTestAsync(object sender, DoWorkEventArgs e)
	        {
                for (int i = 1; i <= 100000; i++)
                {
                    if (bwWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(500);
                        bwWorker.ReportProgress(i); // Don't forget that i represents a percentage from 0 to 100.
                    }                    
                }
	        }

           void worker_TestWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
           {
               //View.ObjectSpace.Refresh();
               if (e.Error != null)
               {
                   addInboundMessageToForm(e.Error.ToString(), "error");
               }
               else
               {
                   addInboundMessageToForm("Задача выполнена!", "info");
               }
               bwWorker.Dispose();
           }
	
	        private void simpleAction1_Execute(object sender, SimpleActionExecuteEventArgs e)
	        {
	            if (!bwWorker.IsBusy)
	            {

	                bwWorker.RunWorkerAsync();
	            }
	            else
	            {
	                DialogResult myDlgResult = XtraMessageBox.Show("Остановить загрузку данных?", "Обработка загрузки данных", MessageBoxButtons.YesNo);
	                if (myDlgResult == DialogResult.Yes)
	                {
	                    bwWorker.CancelAsync();
	                }
	            }
	        }
        #endregion
    }
}
