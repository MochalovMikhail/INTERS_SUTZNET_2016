using System;
using System.Configuration;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Win;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using SUTZ_2.Module.BO.References;
using SUTZ_2.Module;
using SUTZ_2.Module.BO;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Xpo;
using NLog;

namespace SUTZ_2.Win
{
	static class Program
	{
        private static Logger logger = LogManager.GetCurrentClassLogger();
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] arguments)
		{
#if EASYTEST
			DevExpress.ExpressApp.Win.EasyTest.EasyTestRemotingRegistration.Register();
#endif            
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			EditModelPermission.AlwaysGranted = System.Diagnostics.Debugger.IsAttached;
			SUTZ_2WindowsFormsApplication winApplication = new SUTZ_2WindowsFormsApplication();

			// 1. Подлкючение собственного шаблона главного окна:
			winApplication.CreateCustomTemplate += winApplication_CreateCustomTemplate;
            winApplication.LoggedOn += new EventHandler<LogonEventArgs>(winApplication_LoggedOn);
            winApplication.LastLogonParametersRead += new EventHandler<LastLogonParametersReadEventArgs>(winApplication_LastLogonParametersRead);
#if EASYTEST
			if(ConfigurationManager.ConnectionStrings["EasyTestConnectionString"] != null) {
				winApplication.ConnectionString = ConfigurationManager.ConnectionStrings["EasyTestConnectionString"].ConnectionString;
			}
#endif
            // разбор параметров командной строки: /usrАдминистратор /psw
            string userName = "";
            string passw = "";
            foreach (string arg in arguments)
            {
                if (arg.StartsWith("/usr"))
                {
                    userName = arg.Substring(4).Trim();
                }
                else if (arg.StartsWith("/psw"))
                {
                    passw = arg.Substring(4).Trim();
                }
            }
            if (userName.Length > 0)
            {
               try
               {
	               CustomAuthentication loginUser = new CustomAuthentication(userName, passw);
                   winApplication.Security = new SecurityStrategyComplex(typeof(Users), typeof (UsersRole), loginUser);
               }
               catch (System.Exception ex)
               {
                   logger.Error("Ошибка в Main(): {0}", ex.Message);
                   return;
               }
            }

			if (ConfigurationManager.ConnectionStrings["ConnectionString"] != null)
			{
				winApplication.ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
			}
			try
			{
				winApplication.Setup();
				winApplication.Start();				
			}
			catch (Exception e)
			{
				winApplication.HandleException(e);
			}
		}

        static void winApplication_LastLogonParametersRead(object sender, LastLogonParametersReadEventArgs e)
        {
            //throw new NotImplementedException();
        }

        static void winApplication_LoggedOn(object sender, LogonEventArgs e)
        {
            //Int32 newint = 1;
            //SUTZ_2WindowsFormsApplication xafApp = (SUTZ_2WindowsFormsApplication)sender;
            XafApplication xafApp = (XafApplication)sender;
            if (currentSessionSettings.ObjXafApp==null)
            {
                currentSessionSettings.ObjXafApp = xafApp;
                currentSessionSettings.inizializeOnBeginWork();
            }
            //xafApp.CustomizeTemplate += new EventHandler<CustomizeTemplateEventArgs>(xafApp_CustomizeTemplate);
            //xafApp.CreateCustomTemplate -= winApplication_CreateCustomTemplate;
        }

        static void winApplication_CreateCustomTemplate(object sender, CreateCustomTemplateEventArgs e)
        {
            if (SecuritySystem.CurrentUser != null)
            {
                if (e.Context == TemplateContext.ApplicationWindow)
                {
                    XafApplication xafApp = (XafApplication)e.Application;
                    if (((Users)SecuritySystem.CurrentUser).MonitorResolution == enMonitorResolution.Symbol_MS900)
                    {
                        e.Template = new SymbolMainFormTemplate2((XafApplication)e.Application);
                    }
                    xafApp.CreateCustomTemplate -= winApplication_CreateCustomTemplate;
                }
            }
        }
	}

}
