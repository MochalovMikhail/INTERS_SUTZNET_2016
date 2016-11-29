using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;

namespace SUTZ_2.Module.Editors
{
    public partial class ViewControllerSelectForm : ViewController
    {
        public ViewControllerSelectForm()
        {
            InitializeComponent();
            RegisterActions(components);
        }
    }
}
