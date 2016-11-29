using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo.Metadata;
using SUTZ_2.Module.BO.References;

namespace SUTZ_2.Module.BO
{
    // 1. базовый интерфейс для всех сохраняемых объектов СУТЗ
    interface ISUTZPersistentObject
    {
        //Boolean IsDeleted {get;set;}// пометка удаления
        Delimeters Delimeter { get; set; }// разделитель по умолчанию
        int idd { get; }// ключевое поле 
        Guid idGUID { get; set; }
    }
 
    // 2. интерфейс для всех справочников СУТЗ:
    interface IBaseSUTZReferences: ISUTZPersistentObject
    {
        //Int64 getNewCode(XPClassInfo classInfo);
        String Description { get; set; }// у всех справочников должно быть наименование
    }
}
