//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
namespace SUTZ_2.Module.BO.References {
	public static class ConnectionHelper {
		public const string ConnectionString = @"XpoProvider=MSSqlServer;data source=hp-pavilion;integrated security=SSPI;initial catalog=SUTZ_2_ver2";
		public static void Connect(DevExpress.Xpo.DB.AutoCreateOption autoCreateOption) {
			XpoDefault.DataLayer = XpoDefault.GetDataLayer(ConnectionString, autoCreateOption);
			XpoDefault.Session = null;
		}
		public static DevExpress.Xpo.DB.IDataStore GetConnectionProvider(DevExpress.Xpo.DB.AutoCreateOption autoCreateOption) {
			return XpoDefault.GetConnectionProvider(ConnectionString, autoCreateOption);
		}
		public static DevExpress.Xpo.DB.IDataStore GetConnectionProvider(DevExpress.Xpo.DB.AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			return XpoDefault.GetConnectionProvider(ConnectionString, autoCreateOption, out objectsToDisposeOnDisconnect);
		}
		public static IDataLayer GetDataLayer(DevExpress.Xpo.DB.AutoCreateOption autoCreateOption) {
			return XpoDefault.GetDataLayer(ConnectionString, autoCreateOption);
		}
	}

}
