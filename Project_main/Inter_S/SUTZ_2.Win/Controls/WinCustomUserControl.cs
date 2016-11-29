using System;
using DevExpress.Xpo;
using DevExpress.Utils;
using System.Collections;
using SUTZ_2.Module.Editors;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Base;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraEditors.Repository;
using SUTZ_2.Module.BO.References;
using SUTZ_2.Module.BO;

namespace SUTZ_2.Win
{
    /// <summary>
    /// This is a custom WinForms user control that displays persistent data received from XPO.
    /// You do not need to implement the IXpoSessionAwareControl interface if your control gets data from other sources or does not require data at all.
    /// </summary>
    public partial class WinCustomUserControl : DevExpress.XtraEditors.XtraUserControl, IXpoSessionAwareControl
    {
        private Session lokSession;
        //private Type typeOfObject;

        public WinCustomUserControl() {
            InitializeComponent();
        }
        void IXpoSessionAwareControl.UpdateDataSource(Session session) {
            Guard.ArgumentNotNull(session, "session");

            lokSession = session;

            // 1. Общие методы для всех вариантов работы:
            WinCustomForm parentForm = (WinCustomForm)this.ParentForm;
            enVariantsOfSelectedForm workVariant = parentForm.WorkVariant;
            
            Type persistentDataType = null;// тип загружаемого объекта
            //string[] fieldNames;//список загружаемых полей полей для объекта
            Dictionary<string,string> fieldsCollection = new Dictionary<string,string>();
            fieldsCollection.Add("idd", "idd");

            // 2. В зависимости от типа объекта вызов нужного модуля: 
            if (workVariant == enVariantsOfSelectedForm.selectJobTypes)
            {
                persistentDataType = typeof(JobTypes);
                //string[] fieldNames = new string[] { "idd,Description" };// список заполняемых полей из источника данных:
                fieldsCollection.Add("Description", "Выберите вид работы");
            }
            else
            {
                gridControl1.ForceInitialize();
                return;                
            }

            // 3. вариант с фильтром по параметру:
            //IList persistentData = new XPCollection(session, persistentDataType, CriteriaOperator.Parse("Status = 'NotStarted'"))
            IList persistentData = new XPCollection(session, persistentDataType);
            // 4. привязка к источнику данных:
            gridControl1.DataSource = persistentData;

            // 5. получение объекта ColumnView
            ColumnView clView = (ColumnView)gridControl1.MainView;
            clView.OptionsBehavior.AutoPopulateColumns = false;
            clView.Columns.Clear();

            //clView.CellValueChanged += new CellValueChangedEventHandler(clView_CellValueChanged);
            //clView.CellValueChanging += new CellValueChangedEventHandler(clView_CellValueChanging);
            clView.DoubleClick += new EventHandler(clView_DoubleClick);

            // 6. получение объекта gridView
            GridView grView = (GridView)gridControl1.MainView;
            grView.OptionsView.RowAutoHeight = true;
            //grView.Appearance.FocusedRow.BackColor = Color.FromArgb(0xFF, 0xCC, 0x66);
            grView.Appearance.FocusedCell.BackColor = Color.FromArgb(0xFF, 0xCC, 0x66);
            grView.RowCellClick += new RowCellClickEventHandler(grView_RowCellClick);
            grView.RowClick += new RowClickEventHandler(grView_RowClick);
            //grView.Appearance.

            // 7. для переноса текста в строках таблицы нужно использовать этот объект
            RepositoryItemMemoEdit gridMemoEdit = new RepositoryItemMemoEdit();
            gridMemoEdit.WordWrap = true;

            // 8. заполнение представления полями и привязка их к полям источника данных
            foreach (KeyValuePair<string,string> item in fieldsCollection)
            {
                // метод добавляет поле, которое автоматически привязывается к полю в источнике данных с таким же идентификатором:
                GridColumn column = clView.Columns.AddField(item.Key);
                if (item.Key == "idd")
                {
                    column.Visible = false;
                }
                else
                {
                    column.Visible = true;
                }
                column.Caption = item.Value;
                column.OptionsColumn.AllowIncrementalSearch = true;
               
                column.ColumnEdit = gridMemoEdit;
                Font newFont =  new Font(grView.Appearance.Row.Font.FontFamily, 16.0F);
                column.AppearanceCell.Font = newFont;              
            }
            gridControl1.ForceInitialize();
        }

        void grView_RowClick(object sender, RowClickEventArgs e)
        {
            WinCustomForm parentForm = (WinCustomForm)this.ParentForm;
            enVariantsOfSelectedForm workVariant = parentForm.WorkVariant;

            GridView lokGrView = (GridView)sender;

            if (workVariant == enVariantsOfSelectedForm.selectJobTypes)
            {
                int[] massiveSelectRow = lokGrView.GetSelectedRows();
                if (massiveSelectRow.Length>0)
                {
                    int IDD = (int)lokGrView.GetRowCellValue(massiveSelectRow[0], "idd");
                    JobTypes jobType = lokSession.FindObject<JobTypes>(new BinaryOperator("idd", IDD, BinaryOperatorType.Equal));
                    if (jobType!=null)
                    {
                        parentForm.JobType = jobType;
                    }
                 }                
            }
            parentForm.Close();
        }

        void grView_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            string caption = e.Column.Caption;
        }

        void clView_DoubleClick(object sender, EventArgs e)
        {
            WinCustomForm parentForm = (WinCustomForm)this.ParentForm;
            enVariantsOfSelectedForm workVariant = parentForm.WorkVariant;
            if (workVariant == enVariantsOfSelectedForm.selectJobTypes)
            {

                int[] massiveSelectRow = ((GridView)sender).GetSelectedRows();
                //parentForm.JobType =  lokSession.FindObject<JobTypes>(new BinaryOperator("idd", ,BinaryOperatorType.Equal));
            }
        }

        private void gridControl1_DoubleClick(object sender, EventArgs e)
        {
            //WinCustomForm parentForm = (WinCustomForm)this.ParentForm;
            //enVariantsOfSelectedForm workVariant = parentForm.WorkVariant;
            //if (workVariant == enVariantsOfSelectedForm.selectJobTypes)
            //{
                
            //    int[]  ((GridView)sender).GetSelectedRows();
            //    parentForm.JobType =  lokSession.FindObject<JobTypes>(new BinaryOperator("idd", ,BinaryOperatorType.Equal));
            //}
        }

        private void btnPgUp_Click(object sender, EventArgs e)
        {
            gridViewSelectJobType.Focus();
            SendKeys.Send("{PGUP}");
        }

        private void btnPageDown_Click(object sender, EventArgs e)
        {
            gridViewSelectJobType.Focus();
            SendKeys.Send("{PGDN}");
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            gridViewSelectJobType.Focus();
            SendKeys.Send("{UP}");
        }

        private void btnDn_Click(object sender, EventArgs e)
        {
            gridViewSelectJobType.Focus();
            SendKeys.Send("{DOWN}");
        }

        //private void gridControl1_Click(object sender, EventArgs e)
        //{
        //    MessageBoxButtons buttons = MessageBoxButtons.YesNo;
        //    DialogResult result = MessageBox.Show(this.ParentForm, "Действительно выбрать этот элемент?", "Заголовок", buttons);
        //}
    }
}
