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
using NLog;

namespace SUTZ_2.Module.BO.References
{
    // For more typical usage scenarios, be sure to check out http://documentation.devexpress.com/#Xaf/clsDevExpressExpressAppViewControllertopic.
    public partial class AllReferences_ViewController : ViewController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public AllReferences_ViewController()
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

        private void AllReferences_ViewController_FrameAssigned(object sender, EventArgs e)
        {
            NewObjectViewController standardController = Frame.GetController<NewObjectViewController>();
            standardController.ObjectCreating += new EventHandler<ObjectCreatingEventArgs>(standardController_ObjectCreating);
            standardController.ObjectCreated += new EventHandler<ObjectCreatedEventArgs>(standardController_ObjectCreated);
        }

        // метод вызывается после интерактивного создания объекта
        private void standardController_ObjectCreated(object sender, ObjectCreatedEventArgs e)
        {
            // 1. заполнение разделителя по умолчанию:
            logger.Trace("Вызов standardController_ObjectCreated-создание нового элемента типа = {0}", View.ObjectTypeInfo.Type.ToString());
            DevExpress.ExpressApp.DC.IMemberInfo viewObjectTypeInfoFindMember = View.ObjectTypeInfo.FindMember("Delimeter");
            Delimeters tekDelimiter = e.ObjectSpace.GetObject(((Users)SecuritySystem.CurrentUser).DefaultDelimeter);

            if (View.ObjectTypeInfo.Implements<IBaseSUTZReferences>())
            {
                logger.Trace("Вызов standardController_ObjectCreated-создание нового элемента типа = {0}", View.ObjectTypeInfo.Type.ToString());
                ((IBaseSUTZReferences)e.CreatedObject).Delimeter = tekDelimiter;
                ((IBaseSUTZReferences)e.CreatedObject).idGUID = Guid.NewGuid();
            }
            //if (viewObjectTypeInfoFindMember!=null)
            //{
                
            //    if (View.ObjectTypeInfo.Type == typeof(Properties))
            //    {
            //        ((Properties)e.CreatedObject).Delimeter = tekDelimiter;
            //    }
            //    else if (View.ObjectTypeInfo.Type == typeof(Barcodes))
            //    {
            //        ((Barcodes)e.CreatedObject).Delimeter = tekDelimiter;
            //    }
            //    else if (View.ObjectTypeInfo.Type == typeof(Goods))
            //    {
            //        ((Goods)e.CreatedObject).Delimeter = tekDelimiter;
            //    }
            //}


         }
        //protected override void OnFrameAssigned()
        //{
        //    TargetObjectType = typeof(Properties);
        //    base.OnFrameAssigned();
        //}
        void standardController_ObjectCreating(object sender, ObjectCreatingEventArgs e)
        {
            //if (View.ObjectTypeInfo.Type == typeof(Barcodes))
            //{
            //    // как получить доступ к мастер-объекту
            //    CollectionSourceBase collectionSource;
            //    if (View is ListView)
            //    {
            //        collectionSource = ((ListView)View).CollectionSource;
            //        PropertyCollectionSource propertyCollection = (PropertyCollectionSource)collectionSource;

            //        if (propertyCollection != null)
            //        {
            //            if (propertyCollection.MasterObject != null)
            //            {
            //                if (propertyCollection.MasterObject is Goods)
            //                {
            //                    ((Barcodes)e.NewObject).ParentId = (Goods)propertyCollection.MasterObject;
            //                }
            //            }
            //        }
            //    }
            //    else
            //    {
            //        e.Cancel = true;
            //    }
            //}
        }

    }
}
