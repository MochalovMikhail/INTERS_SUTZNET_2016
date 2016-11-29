using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Drawing;

namespace SUTZ_2.Module.Controls
{
    class CustomProgressBarControl : ProgressBarControl
    {    
        static CustomProgressBarControl() 
        {
            RepositoryItemCustomProgressBarControl.Register();
        }
        public override string EditorTypeName 
        { 
            get { return RepositoryItemCustomProgressBarControl.EditorName; } 
        }

        protected override object ConvertCheckValue(object val) 
        {
            return val;
        }
    }

    public class RepositoryItemCustomProgressBarControl : RepositoryItemProgressBar
    {
        protected internal const string EditorName = "CustomProgressBarControl";

        protected internal static void Register()
        {
            if (!EditorRegistrationInfo.Default.Editors.Contains(EditorName))
            {
                EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(EditorName, typeof(CustomProgressBarControl),
                    typeof(RepositoryItemCustomProgressBarControl), typeof(ProgressBarViewInfo), new ProgressBarPainter(), true, EditImageIndexes.ProgressBarControl, typeof(DevExpress.Accessibility.ProgressBarAccessible)));
            }
        }
        static RepositoryItemCustomProgressBarControl()
        {
            Register();
        }

        protected override int ConvertValue(object val)
        {
            try
            {
                float number = Convert.ToSingle(val);
                return (int)(Minimum + number * Maximum);
            }
            catch
            {

            }

            return Minimum;
        }

        public override string EditorTypeName { get { return EditorName; } }
    }
}
