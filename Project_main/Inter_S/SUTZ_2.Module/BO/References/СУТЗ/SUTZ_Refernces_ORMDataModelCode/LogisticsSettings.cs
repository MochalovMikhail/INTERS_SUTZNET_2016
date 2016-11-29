using System;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Utils;
using System.Drawing;
namespace SUTZ_2.Module.BO.References
{

    public partial class LogisticsSettings
    {
        public LogisticsSettings(Session session) : base(session)
        {
            Guard.ArgumentNotNull(session, "session");
            lokSession = session;

        }
        public override void AfterConstruction() { base.AfterConstruction(); }

        public string getValueByKey(string keyValue)
        {
            Guard.ArgumentNotNull(keyValue, "keyValue");

            LogisticsSettings setting = lokSession.FindObject<LogisticsSettings>(new BinaryOperator("Code", keyValue, BinaryOperatorType.Equal));
            if (setting == null)
            {
                return "";
            }
            return setting.LogisticValue;
        }

        // Метод определяет фоновый цвет для форм мобильной СУТЗ:
        public Color getBackColorForMobileForms()
        {
            Color backColor = new Color();
            //LogisticsSettings settings = new LogisticsSettings(lokSession);
            string strRGB = getValueByKey("BackGroundColorOfFormsMobileSUTZ");
            if (strRGB != "")
            {
                try
                {
                    backColor = System.Drawing.ColorTranslator.FromHtml(strRGB);
                }
                catch (System.Exception ex)
                {
                    backColor = Color.RoyalBlue;
                }
            }
            return backColor;
        }
        
    }
}
