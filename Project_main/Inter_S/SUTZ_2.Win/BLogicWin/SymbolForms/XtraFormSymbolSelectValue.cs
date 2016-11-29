using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using NLog;
using SUTZ_2.Module.BO.References;
using SUTZ_2.Module;
using SUTZ_2.Module.BO;
using DevExpress.ExpressApp;
using SUTZ_2.Module.BO.References.Mobile;
using DevExpress.Xpo;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;
using System.Collections;

namespace SUTZ_2.MobileSUTZ
{
    public partial class XtraFormSymbolSelectValue : DevExpress.XtraEditors.XtraForm
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public IObjectSpace objSpace { get; set; }
        public int iddSelectValue { get; set; }
        

        public XtraFormSymbolSelectValue()
        {
            // 1. загрузка стандартных компонент формы
            InitializeComponent();

            // 2. определение фонового цвета:            
            objSpace = currentSessionSettings.ObjXafApp.CreateObjectSpace();
            LogisticsSettings logSettings = new LogisticsSettings(objSpace.Session());
            Color backColor = logSettings.getBackColorForMobileForms();
            if (!backColor.IsEmpty)
            {
                this.Appearance.BackColor = backColor;
            }            

            // 3. создание колонок на форме 

            ColumnView clView = (ColumnView)gridControl1.MainView;
            clView.OptionsBehavior.AutoPopulateColumns = false;
            clView.Columns.Clear();

            // 3.2 Колонка с идентификатором элемента
            GridColumn column = clView.Columns.AddField("idd");
            column.Visible = false;
            column.Caption = "IDD";
            column.OptionsColumn.AllowIncrementalSearch = true;

            // 3.3 Колонка с наименованием элемента
            // 7. для переноса текста в строках таблицы нужно использовать этот объект
            RepositoryItemMemoEdit gridMemoEdit = new RepositoryItemMemoEdit();
            gridMemoEdit.WordWrap = true;
            
            GridColumn column1 = clView.Columns.AddField("Description");
            column1.Visible = true;
            column1.Caption = "Наименование";
            column1.OptionsColumn.AllowIncrementalSearch = true;
            column1.ColumnEdit = gridMemoEdit;
            Font newFont = new Font(((GridView)gridControl1.MainView).Appearance.Row.Font.FontFamily, 16.0F);
            column1.AppearanceCell.Font = newFont;   

            // 6. получение объекта gridView
            GridView grView = (GridView)gridControl1.MainView;
            grView.OptionsView.RowAutoHeight = true;
            grView.OptionsView.ShowAutoFilterRow = true;
            grView.OptionsFind.AllowFindPanel = true;
            grView.OptionsFind.HighlightFindResults = true;

            //grView.Appearance.FocusedRow.BackColor = Color.FromArgb(0xFF, 0xCC, 0x66);
            grView.Appearance.FocusedCell.BackColor = Color.FromArgb(0xFF, 0xCC, 0x66);

            // 7. управление элементами формы:
            // 7.1 отключение MRU панели:
            grView.OptionsFilter.AllowMRUFilterList = false;
            grView.OptionsFilter.AllowColumnMRUFilterList = false;

            // 7.2 отключение панели группировки:
            grView.OptionsView.ShowGroupPanel = false;                       
        }

        // 1. конструктор для выбора из всег списка существующего справочника или документа без ограничений:
        public XtraFormSymbolSelectValue(Type selectType) :this()
        {
            if (selectType == typeof(MobileErrors))
            {
                //XPQuery<MobileErrors> xpTable = new XPQuery<MobileErrors>(objSpace.Session());
                // Фильтр по ошибкам: должны быть включены (признак=1) для моб.сутз и 
                // находящиеся в списке разрешенных для этого вида операции
                //List<MobileErrors> elementList = from s in xpTable where s.
                
                // вариант 1:
                //gridControl1.DataSource = objSpace.GetObjects<MobileErrors>();

                // вариант 2:
                //CollectionSource ds = new CollectionSource(objSpace, typeof(MobileErrors));
                //gridControl1.DataSource = ds;
                //gridControl1.RefreshDataSource();

                // вариант 3:
                XPCollection persistentData =  new XPCollection(objSpace.Session(), objSpace.Session().GetClassInfo(selectType));
                gridControl1.DataSource = persistentData;
            }
            else
            {
                //IList persistentData = new XPCollection(session, persistentDataType);
                // 4. привязка к источнику данных:
                //gridControl1.DataSource = persistentData;
            }
            gridControl1.ForceInitialize();
            GridView grView = (GridView)gridControl1.MainView;
            grView.RefreshData();
        }

        // 2. конструктор для выбора из переданного запроса LINQ

        // 3. конструктор для выбора из переданного словаря:

        private void XtraFormSymbolSelectValue_Load(object sender, EventArgs e)
        {
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.gridView1.Focus();
        }

        private void XtraFormSymbolSelectValue_Shown(object sender, EventArgs e)
        {
            this.Text = "";
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
        }

        private void simpleButtonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void simpleButtonUP_Click(object sender, EventArgs e)
        {
            gridView1.MovePrev();            
        }

        private void simpleButtonDn_Click(object sender, EventArgs e)
        {
            gridView1.MoveNext();
        }

        private void simpleButtonOK_Click(object sender, EventArgs e)
        {
            // 1. определение идентификатора выбранного элемента
            int[] massiveSelectRow = gridView1.GetSelectedRows();
            if (massiveSelectRow.Length == 0)
            {
                return;
            }
            
            this.iddSelectValue = (int)gridView1.GetRowCellValue(massiveSelectRow[0], "idd");

            // 2. установка признака ОК 
            this.DialogResult = DialogResult.OK;

            // 3. закрытие формы
            this.Close();
        }
    }
}