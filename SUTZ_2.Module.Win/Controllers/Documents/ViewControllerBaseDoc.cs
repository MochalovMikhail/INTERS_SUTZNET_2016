using System;
using System.Linq;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.Data.Filtering;
using System.Collections.Generic;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Columns;
using SUTZ_2.Module.BO.References;
using DevExpress.XtraEditors.Controls;
using System.Windows.Forms;
using DevExpress.Data;

namespace SUTZ_2.Module.Win.Controllers.Documents
{
    // For more typical usage scenarios, be sure to check out http://documentation.devexpress.com/#Xaf/clsDevExpressExpressAppViewControllertopic.
    public partial class ViewControllerBaseDoc : ViewController
    {
        public ViewControllerBaseDoc()
        {
            InitializeComponent();
            RegisterActions(components);
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }

        private void ViewControllerBaseDoc_ViewControlsCreated(object sender, EventArgs e)
        {
            GridListEditor listEditor = ((DevExpress.ExpressApp.ListView)View).Editor as GridListEditor;
            if (listEditor != null)
            {               
                //XafGridView gridView = listEditor.GridView;
                //gridView. OptionsView.EnableAppearanceOddRow = true;
                //gridView.Appearance.OddRow.BackColor = Color.FromArgb(244, 244, 244);
                ColumnView clView = (ColumnView)listEditor.Grid.MainView;
                clView.ValidateRow += new ValidateRowEventHandler(clView_ValidateRow);
                clView.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(clView_ValidatingEditor);
                clView.InvalidValueException+=new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(clView_InvalidValueException);

                // итог по колонке общей сумме
                GridColumn colTotal = clView.Columns["TotalQuantity"];
                colTotal.SummaryItem.SummaryType = SummaryItemType.Sum;
                colTotal.SummaryItem.DisplayFormat = "{0:n2}";

                // итог по колонке количество коробок
                GridColumn colQuantityOfUnits = clView.Columns["QuantityOfUnits"];
                colQuantityOfUnits.SummaryItem.SummaryType = SummaryItemType.Sum;
                colQuantityOfUnits.SummaryItem.DisplayFormat = "{0:n2}";
 
                // итог по колонке общей сумме
                GridColumn colQuantityOfItems = clView.Columns["QuantityOfItems"];
                colQuantityOfItems.SummaryItem.SummaryType = SummaryItemType.Sum;
                colQuantityOfItems.SummaryItem.DisplayFormat = "{0:n2}"; 
            }
        }

        private void clView_InvalidValueException(object sender, InvalidValueExceptionEventArgs e)
        {
            //Do not perform any default action 
            e.ExceptionMode = DevExpress.XtraEditors.Controls.ExceptionMode.NoAction;
            //Show the message with the error text specified 
            MessageBox.Show(e.ErrorText);
        }

        void clView_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            //GridView view = sender as GridView;
            //if (view.FocusedColumn.FieldName == "QuantityOfUnits")
            //{                
            //    Single discount = Convert.ToSingle(e.Value);
            //    ////Specify validation criteria 
            //    //if (discount > 500)
            //    //{
            //    //    e.Valid = false;
            //    //    e.ErrorText = "Enter a positive value";
            //    //}
            //    //if (discount > 2000)
            //    //{
            //    //    e.Valid = false;
            //    //    e.ErrorText = "Reduce the amount (20% is maximum)";
            //    //}
            //}
        }

        void clView_ValidateRow(object sender, ValidateRowEventArgs e)
        {
            GridView view = sender as GridView;
            
            GridColumn colQuantityOfUnits = view.Columns["QuantityOfUnits"];
            GridColumn colQuantityOfItems = view.Columns["QuantityOfItems"];
            GridColumn colTotalQuantity   = view.Columns["TotalQuantity"];
            GridColumn colUnits           = view.Columns["Unit!"];
            GridColumn colGoods = view.Columns["Good!"];
            GridColumn colKoeff = view.Columns["Coeff"];
            GridColumn colGoodCode = view.Columns["GoodCode"];


            Decimal decQuantUnits = (Decimal)view.GetRowCellValue(e.RowHandle, colQuantityOfUnits);
            Decimal decQuantItems = (Decimal)view.GetRowCellValue(e.RowHandle, colQuantityOfItems);
            Decimal decTotalQuant = (Decimal)view.GetRowCellValue(e.RowHandle, colTotalQuantity);
   
            Units unitValue = (Units)view.GetRowCellValue(e.RowHandle, colUnits);

            // 2. проверка единицы измерения, единица выбрана:
            if (unitValue != null)
            {
                view.SetRowCellValue(e.RowHandle, colKoeff, unitValue.Koeff);
                if (view.FocusedColumn.FieldName == "TotalQuantity")
                {
                    Decimal decTotalItems = decTotalQuant % (decimal)unitValue.Koeff;
                    Decimal decTotalUnits = (decTotalQuant - decTotalItems) / (Decimal)unitValue.Koeff;
                    view.SetRowCellValue(e.RowHandle, colQuantityOfUnits, decTotalUnits);
                    view.SetRowCellValue(e.RowHandle, colQuantityOfItems, decTotalItems);
                }
                else //if (view.FocusedColumn.FieldName.IndexOf("QuantityOfUnits,QuantityOfUnits") > 0)
                {
                    Decimal decTotalQuantity = (Decimal)unitValue.Koeff * decQuantUnits + decQuantItems;
                    Decimal decTotalItems = decTotalQuantity % (Decimal)unitValue.Koeff;
                    Decimal decTotalUnits = (decTotalQuantity - decTotalItems) / (Decimal)unitValue.Koeff;
                    view.SetRowCellValue(e.RowHandle, colTotalQuantity, decTotalQuantity);
                    view.SetRowCellValue(e.RowHandle, colQuantityOfUnits, decTotalUnits);
                    view.SetRowCellValue(e.RowHandle, colQuantityOfItems, decTotalItems);
                }
            }
            else// единица измерения не выбрана
            {
                view.SetRowCellValue(e.RowHandle, colUnits, null);
                view.SetRowCellValue(e.RowHandle, colKoeff, 0);
                view.SetRowCellValue(e.RowHandle, colTotalQuantity, 0);
            }

            // 3. проверка товара
            Goods goodValue = (Goods)view.GetRowCellValue(e.RowHandle, colGoods);
            if (goodValue!=null)
            {
                view.SetRowCellValue(e.RowHandle, colGoodCode, goodValue.Code);
            }
            else
            {
                view.SetRowCellValue(e.RowHandle, colKoeff, 0);
                view.SetRowCellValue(e.RowHandle, colUnits, null);
            }
 

            //Validity criterion
            //if (intQuantUnits>500)
            //{
            //    e.Valid = false;
            //    //Set errors with specific descriptions for the columns
            //    view.SetColumnError(colQuantityOfUnits, "Количество коробок не может быть больше 500!");
            //    view.SetColumnError(colQuantityOfItems, "Количество коробок не может быть больше 501!");
            //}
        }
    }
}
