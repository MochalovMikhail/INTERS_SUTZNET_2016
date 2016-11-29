using System;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Utils;
using System.Linq;
using DevExpress.ExpressApp;
using System.Collections.Generic;

namespace SUTZ_2.Module.BO.References
{
    public partial class StorageCodes : IBaseSUTZReferences
    {
        public StorageCodes(Session session) : base(session)
        {
           
            Guard.ArgumentNotNull(session, "session");
            lokSession = session;
        }
        public override void AfterConstruction() 
        { 
            base.AfterConstruction(); 
        }

        protected override void OnSaving()
        {
            base.OnSaving();
            PalletCode = calculateFullCellCode();
        }
        public String Description
        {
            get { return Code; }
            set { }
        }

        /// <summary>
        /// ћетод определ€ет строку дл€ поиска элемента справочника  оды–азмещени€ из штрих-кода этикетки
        /// кода размещени€. Ўаблон ожидаемой строки: 050181035000
        /// </summary>
        /// <param name="strBarcode"></param>
        /// <returns></returns>
        public static List<String> detectStrStorageCodeFromFullString(string strBarcode)
        {
            List<String> palletFields = new List<String>();
            strBarcode = strBarcode.Trim();
            if (strBarcode.StartsWith("05") && (strBarcode.Length >= 12))
            {
                palletFields.Add(strBarcode.Substring(2, 3));// код стеллажа, символы с 2 по 4, три знака
                palletFields.Add(strBarcode.Substring(5, 1));// код этажа, символы с 5 по 5, один знак
                palletFields.Add(strBarcode.Substring(6, 3));// код €чейки, символы с 6 по 8, три знака
                palletFields.Add(strBarcode.Substring(9, 3));// код под€чейки, символы с 9 по 11 три знака
            }
            return palletFields;
        }

        // 1. метод класса дл€ получени€ типа €чейки по параметрам высота, ширина, глубина.
        public TypesOfCellValue getTypeOfCellBySize(int height, int width, int depth)
        {
            bool returnIsDefaultValue = false; // вернуть тип €чейки по умолчанию
            if (height == 0)
            {
                returnIsDefaultValue = true;
            }
            else if (width == 0)
            {
                returnIsDefaultValue = true;
            }
            else if (depth == 0)
            {
                returnIsDefaultValue = true;
            }

            int lokHeight = 0;
            int lokWidth = 0;
            int lokDepth = 0;
            if (!returnIsDefaultValue)
            {
                lokHeight = height;
                lokWidth = width;
                lokDepth = depth;
            }

            XPQuery<TypesOfCellValue> query1 = new XPQuery<TypesOfCellValue>(lokSession);
            var result = (from ce in query1 where ce.Height == lokHeight && ce.Width == lokWidth && ce.Depth == lokDepth select ce).FirstOrDefault();
            if (result == null)
            {
                //IObjectSpace objSpace = (IObjectSpace)SessionXAF;

                TypesOfCellValue newVolume = new TypesOfCellValue(lokSession);
                newVolume.Height = lokHeight;
                newVolume.Width = lokWidth;
                newVolume.Depth = lokDepth;

                // установка разделител€ по умолчанию дл€ текущего пользовател€
                Users currentUser = (Users)SecuritySystem.CurrentUser;
                if ((currentUser == null) || (currentUser.DefaultDelimeter == null))
                {
                    throw (new Exception("” пользовател€ " + currentUser + " не установлен разделитель по умолчанию!"));
                }

                Delimeters delimeter = lokSession.FindObject<Delimeters>(new BinaryOperator("DelimeterID", currentUser.DefaultDelimeter.DelimeterID, BinaryOperatorType.Equal), true);
                newVolume.Delimeter = delimeter;
                newVolume.Save();
                lokSession.CommitTransaction();

                result = newVolume;
            }
            return result;
        }

        // 2. метод класса дл€ формировани€ полного кода размещени€
        private String calculateFullCellCode()
        {
            return String.Format("{0}.{1}.{2}", Stillage.ToString("D3"), Floor.ToString("D2"), Cell.ToString("D2"));
        }

        // 3. обновление наименовани€ кода размещени€:
        private void setFullCellCode()
        {
            Code = calculateFullCellCode();
        }
    }
}
