using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using NLog;
using DevExpress.ExpressApp;
using SUTZ_2.Module.BO.Documents;
using DevExpress.ExpressApp.Utils;
using DevExpress.Xpo.DB.Exceptions;
using SUTZ_2.Win;
using DevExpress.Data.Filtering;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using SUTZ_2.Module.BO.References.Mobile;
using SUTZ_2.Module.BO.References;
using SUTZ_2.Module.BO;


namespace SUTZ_2.MobileSUTZ
{
    /// <summary>
    /// Класс для выполнения пополнения товара в мобильной СУТЗ
    /// </summary>
    [NonPersistent]
    public class MobileSUTZ_Popolnenie
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private IObjectSpace objectSpace { get; set; }
        private MobileWorkUnit currentMUnit { get; set; }// текущее задание на работу
        private localClassScanState scanStateFields { get; set; }

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="objectSpace_"> </param>
        public MobileSUTZ_Popolnenie(IObjectSpace objectSpace_)
        {
            Guard.ArgumentNotNull(objectSpace_, "objectSpace_");
            objectSpace = objectSpace_;
        }

        // 2.1 формирует заголовок формы
        private String getTopCaption()
        {
            String strTopCaption = "";
            if (scanStateFields.currentScanState == enumScanState.НачалоРаботы)
            {
                strTopCaption = scanStateFields.sprJobeType.Description.Trim();
            }
            else if (scanStateFields.currentScanState == enumScanState.ОтсканированПаллетОтправитель)
            {
            //    if (scanStateFields.docSpecProhoda != null)
            //    {
            //        strTopCaption += scanStateFields.docSpecProhoda.DocNo;
            //    }
            }
            return strTopCaption;// пока пусто.
        }
        /// <summary>
        /// Метод выполняет составление дерева запроса в зависимости от переданного условия
        /// </summary>
        /// <param name="flFilterUser"> 1- включить в запрос текущего пользователя, 0 - не включать</param>
        /// <returns></returns>
        private IQueryable<MobileWorkUnit> findUnitWithOrWithoutCurrentUser(IQueryable<MobileWorkUnit> listOfUnitsAll, byte flFilterUser)
        {
            // установка фильтра по пользователю
            IQueryable<MobileWorkUnit> returnValue;

            if (flFilterUser==1)
            {
                listOfUnitsAll = from o in listOfUnitsAll where o.AssignedUser.idd == ((Users)SecuritySystem.CurrentUser).idd select o;
            }

            // 2.1 если пользователем установлен фильтр по конкретному документу, то ищем только по нему
            if (scanStateFields.idDocPeremeshenieFilter > 0)
            {
                returnValue = from o in listOfUnitsAll where o.BaseDoc.idd == scanStateFields.idDocPeremeshenieFilter select o;
            }
            // 2.2 если мы ранее работали с предыдущим документом, поищем с учетом него
            else if (scanStateFields.idDocPeremesheniePrev > 0)
            {
                returnValue = from o in listOfUnitsAll where o.BaseDoc.idd == scanStateFields.idDocPeremesheniePrev select o;

                // 2.2.1 если по предыдущему документу ничего не найдено, будем искать без фильтра по документу
                if (returnValue.Count() == 0)
                {
                    returnValue = listOfUnitsAll;
                }
            }
            // 2.3 нет фильтров по документам
            else
            {
                returnValue = listOfUnitsAll;
            }
            return returnValue;
        }

        /// <summary>
        /// поиск задания с учетом возможных фильтров: поиск с максимальным количеством фильтров:
        /// с учетом пользователя и фильтра по документу и виду работы:
        /// </summary>
        private MobileWorkUnit findUnitForFullFilters()
        {
            // 1. Создание базового запроса, к которому будут прицепляться другие запросы по мере необходимости
            XPQuery<MobileWorkUnit> xpQuery = new XPQuery<MobileWorkUnit>(objectSpace.Session());            
            var listOfUnitsAll = from o in xpQuery 
                              where 
                              o.JobType.idd == scanStateFields.sprJobeType.idd
                              //&& o.AssignedUser.idd == ((Users)SecuritySystem.CurrentUser).idd
                              && o.TypeOfMobileState == enTypesOfMobileStates.ДоступенДляРаботы
                              && o.IsMarkDeleted != true select o;
            
            // 2. выполнение запроса с учетом фильтра по номеру документа
            IQueryable<MobileWorkUnit> listOfUnits = findUnitWithOrWithoutCurrentUser(listOfUnitsAll, 1);
            if ((listOfUnits==null)||(listOfUnits.Count()==0))
            {
                listOfUnits = findUnitWithOrWithoutCurrentUser(listOfUnitsAll, 0);
            }

            // 3. получение первого по порядку задания на работу.
            MobileWorkUnit findedUnit = null;
            if (listOfUnits.Count() > 0)
            {
                findedUnit = (from o in listOfUnits orderby o.BaseDoc.DocDateTime select o).FirstOrDefault<MobileWorkUnit>();
                return findedUnit;
            }

            return findedUnit;
        }

        /// <summary>
        /// Метод выполняет поиск и блокировку задания для пользователя, с учетом возможного фильтра по документу, виду работы
        /// или других будущих возможных фильтров.
        /// </summary>
        /// <returns> ссылка на элемент задания</returns>
        private MobileWorkUnit findUnitForWork()
        {
            // 1. Поиск задания с максимальными фильтрами:
            MobileWorkUnit findUnit = findUnitForFullFilters();

            return findUnit;
        }

        /// <summary>
        /// Метод сохраняет задание за текущим пользователем с изменением статуса задания
        /// </summary>
        /// <param name="unitForWork"></param>
        /// <returns></returns>
        private bool saveUnitForCurrentUser(MobileWorkUnit unitForWork)
        {
            // 1. сохранение данных в транзакции
            // https://documentation.devexpress.com/#CoreLibraries/DevExpressXpoDBExceptions

            String strError = "";

            try
            {
                //objectSpace.CommitChanges();
            }
            catch (UserFriendlyException usfException)
            {
                logger.Error(usfException,"saveUnitForCurrentUser: Ошибка UserFriendlyException:");
            }
            catch (LockingException lockException)
            {
                logger.Error(lockException,"saveUnitForCurrentUser: Ошибка LockingException:");
            }
            catch (SqlExecutionErrorException sqlException)
            {
                strError = String.Format("Ошибка sqlException: {0}", sqlException.Message);
                logger.Error(sqlException,"saveUnitForCurrentUser: Ошибка sqlException.");
            }
            catch (UnableToOpenDatabaseException openDBException)
            {
                strError = String.Format("Ошибка openDBException: {0}", openDBException.Message);
                logger.Error(openDBException,"saveUnitForCurrentUser: Ошибка openDBException.");
            }
            catch (Exception ex)
            {
                strError = "Ошибка Exception" + ex.Message;
                logger.Error(ex,"saveUnitForCurrentUser: Ошибка Exception.");
            }

            if (strError.Length > 0)
            {
                // 2. информирование пользователя об ошибке:
                structScanStringParams paramScan = new structScanStringParams()
                {
                    captionOne = "Ошибка при сохранении данных в БД",
                    inputMode = enumInputMode.ТолькоСообщениеиОК,
                    captionTwo = strError,
                    scanedBarcode = "",
                    successScan = false,
                    captionHelp = ""
                };

                UserSelect userSelect = new UserSelect();
                userSelect.userMessage(ref paramScan);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Метод определяет текущее состояние выполнения задания на попополнение:
        /// 1. определение режима сканирования: либо паллет-отправитель либо паллет-получатель и вид работы:
        /// </summary>
        /// <param name="mwUnit"></param>
        /// <returns> 1- ВзятьИз, 2 - ПоложитьВ, 3- ПоложитьВТочкаПередачи, 4 - ВзятьИзТочкаПередачи</returns>
        private byte detectCurrentState(MobileWorkUnit mwUnit)
        {            
            byte rezimRaboty = 0;
            if (mwUnit.JobType.TypeOfWork == enTypeOfWorks.Перемещение)
            {
                if (scanStateFields.currentScanState == enumScanState.НачалоРаботы)
                {
                    rezimRaboty = 1;
                }
                else if (scanStateFields.currentScanState == enumScanState.ОтсканированоВсеКоличество)
                {
                    rezimRaboty = 2;
                }
            }
            else if (mwUnit.JobType.TypeOfWork == enTypeOfWorks.ПеремещениеВТочкуПередачи)
            {
                if (scanStateFields.currentScanState == enumScanState.НачалоРаботы)
                {
                    rezimRaboty = 1;
                }
                else if (scanStateFields.currentScanState == enumScanState.ОтсканированПаллетОтправитель)
                {
                    rezimRaboty = 3;
                }
            }
            else if (mwUnit.JobType.TypeOfWork == enTypeOfWorks.ПеремещениеИзТочкиПередачи)
            {
                if (scanStateFields.currentScanState == enumScanState.НачалоРаботы)
                {
                    rezimRaboty = 4;
                }
                else if (scanStateFields.currentScanState == enumScanState.ОтсканированПаллетОтправитель)
                {
                    rezimRaboty = 2;
                }
            }
            return rezimRaboty;
        }

        /// <summary>
        /// Метод для обработки сканирования ШК в разных формах: подключено сканирование ШК товара
        /// </summary>
        /// <param name="scanedBarcode"></param>
        private int processBarcode(String scanedBarcode)
        {
            logger.Trace("processBarcode: Метод вызван с параметром='" + scanedBarcode + "', scanStateFields.currentScanState=" + scanStateFields.currentScanState);

            String strFuncName = "processBarcode, scanedBarcode="+scanedBarcode;

            // 1. Сканирование товара:
            if (scanStateFields.currentScanState==enumScanState.ОтсканированПаллетОтправитель)
            {
                scanStateFields.strScanedTovarBarcode = scanedBarcode;

                logger.Trace("{0} отсканирована строка товара ='{1}'", strFuncName, scanedBarcode);
                String tovarDescription = scanStateFields.good.ShortDescription.Trim().PadRight(50);
                String captionOne = "<size=40> <backcolor=255, 255, 255> <color=" + Color.RoyalBlue.ToArgb() + ">      " + scanStateFields.storageCodeTakeFrom.PalletCode.PadRight(30) + "    </color></backcolor></size><br>" +
                        "<size=30>" + tovarDescription + "</size><br>" +
                         "<size=36>     Отсканируйте   <br>" +
                         "     штрих-код товара:</size>";

                // ШК товара нужно найти товар и сравнить его с товаром в документе.
                XPQuery<BarcodesOfGoods> xpQueryBG = new XPQuery<BarcodesOfGoods>(objectSpace.Session());
                var tovarsBG = from s in xpQueryBG where s.Barcode == scanedBarcode.Trim() select s.ParentId;

                logger.Trace("{0} Вызов модуля определения частей паллета по строке, получилось {1} части", strFuncName, tovarsBG.Count());

                // не удалось найти ни одного товара по ШК:
                if (tovarsBG.Count() == 0)
                {
                    scanStateFields.strLabelCaptionOne = captionOne + "<br> <size=42> <backcolor=255, 255, 255> <color=" + Color.RoyalBlue.ToArgb() + ">Штрих-кода нет в базе!</color></backcolor></size>";
                    logger.Trace("{0} Ошибка разбора строки кода размещения. Количество частей кода={1}", strFuncName, 0);
                    return 0;
                }

                var tovarInBarcodeOfGoods = (from o in tovarsBG where o.idd == scanStateFields.good.idd select o).FirstOrDefault<Goods>();

                // 2.9 Если товар по отсканированному ШК не найден:
                if (tovarInBarcodeOfGoods == null)
                {
                    //paramScan.captionOne = String.Format("Ошибочный стеллаж!\nОтсканирован:{0}\nНужен:{1}", stillageNumber, stCode.Stillage);
                    scanStateFields.strLabelCaptionOne = captionOne + "<br> <size=36> <backcolor=255, 255, 255> <color=" + Color.RoyalBlue.ToArgb() + ">По штрих-коду " + scanedBarcode.Trim() + " не найден товар БД!</color></backcolor></size>";
                    logger.Trace("{0} По штрих-коду: {1} не найдено ни одного товара в БД!", strFuncName, scanedBarcode);
                    return 0;
                }

                if (tovarInBarcodeOfGoods.idd != scanStateFields.good.idd)
                {
                    scanStateFields.strLabelCaptionOne = captionOne + "<br> <size=36> <backcolor=255, 255, 255> <color=" + Color.RoyalBlue.ToArgb() + ">Ошибочный товар! Отсканирован:" + tovarInBarcodeOfGoods.ShortDescription + ", нужен:" + tovarDescription + "</color></backcolor></size>";
                    logger.Trace("{0} Отсканированный ошибочный товар. Отсканирован(idd):{1} Нужен(idd):{2}", strFuncName, scanStateFields.good.idd, tovarsBG.First().idd);
                    return 0;
                }
            }
            return 1;
        }

        /// <summary>
        /// Метод для обработки сканирования ШК в разных формах: подключено сканирование ШК товара
        /// </summary>
        /// <param name="scanedBarcode"></param>
        private int onEnterQuantity(ref structScanStringParams paramScan)
        {
            //const String strFuncName = "onEnterQuantity:";
            //logger.Trace("{0} Метод вызван с параметром paramScan=" + paramScan.ToString());

            return 1;
        }

        /// <summary>
        /// Метод возвращает форматированную строку для формирования HTML-заголовка диалога пользователя
        /// </summary>
        /// <returns>строка</returns>
        private String getLabelCaption()
        {
            return scanStateFields.strLabelCaptionOne;
        }

        /// <summary>
        /// вызов диалога сканирования ШК кода размещения
        /// </summary>
        /// <returns></returns>
        private DialogResult lokScanKodRazmesheniya(MobileWorkUnit mwUnit)
        {
            const string strFuncName = "lokScanKodRazmesheniya:";

            // 1. Определение текущего режима работы модуля:
            byte rezimRaboty = detectCurrentState(mwUnit);

            logger.Trace("{0} Вход в метод, mwUnit idd={1}, rezimRaboty={2}", strFuncName, scanStateFields.mobileWorkUnitIdd, rezimRaboty);

            // 2. определение строки - кода размещения 
            structScanStringParams paramScan = new structScanStringParams()
            {
                captionOne = "",
                captionTwo = "",
                scanedBarcode = "",
                successScan = false,
                captionHelp = getTopCaption()
            };
  
            // 3. В зависимости от режима работы выполняется запрос для определения реального кода размещения, с которым мы работаем:
            XPQuery<DocSpecPeremeshGoods> xpQuery = new XPQuery<DocSpecPeremeshGoods>(objectSpace.Session());
            StorageCodes stCode = null;
            if (rezimRaboty==1)// ВзятьИз
            {
                stCode = (from s in xpQuery where scanStateFields.docRowIds.Contains(s.idd) select s.TakeFrom).FirstOrDefault<StorageCodes>();
                scanStateFields.storageCodeTakeFrom = stCode;
            }
            else if ((rezimRaboty==2)||(rezimRaboty==4))// ПоложитьВ в обычном режиме или при размещении из точки передачи (4)
            {
                stCode = (from s in xpQuery where scanStateFields.docRowIds.Contains(s.idd) select s.PutTo).FirstOrDefault<StorageCodes>();
                scanStateFields.storageCodePutTo = stCode;
            }
            else if (rezimRaboty==3)// ТочкаПередачи - взяить из точки передачи
            {
                stCode = (from s in xpQuery where scanStateFields.docRowIds.Contains(s.idd) select s.ExchangePoint).FirstOrDefault<StorageCodes>();
                scanStateFields.storageCodeTakeFrom = stCode;
            }

            if (stCode == null)
            {
                paramScan.captionOne = "Ошибка поиска кода размещения!";
                logger.Trace("{0} ошибка определения кода размещения по заданию: MobileWorkUnit.idd={1}, rezimRaboty={2}", strFuncName, scanStateFields.mobileWorkUnitIdd, rezimRaboty);
                return DialogResult.No;
            }
            
            
            // 1.3 установка в заголовке окна кода:
            String captionOne = "";
             if ((rezimRaboty==1)||(rezimRaboty==3))
            {
                captionOne = "<size=36>     Взять из ячейки </size><br>" +
                     "<size=42> <backcolor=255, 255, 255> <color=" + Color.RoyalBlue.ToArgb() + ">    " + stCode.PalletCode + "    </color></backcolor></size><br>" +
                     "<size=36>     Отсканируйте   <br>" +
                     "     штрих-код ячейки:</size>";
            }
            else if ((rezimRaboty==2)||(rezimRaboty==4))
            {
                captionOne = "<size=36>     Положить в ячейку </size><br>" +
                     "<size=42> <backcolor=255, 255, 255> <color=" + Color.RoyalBlue.ToArgb() + ">    " + stCode.PalletCode + "     </color></backcolor></size><br>" +
                     "<size=36>     Отсканируйте   <br>" +
                     "     штрих-код ячейки:</size>";
            }

            paramScan.captionOne =  captionOne;
            paramScan.captionTwo = stCode.PalletCode;
            paramScan.enableQuestionButton = true;

            // 2. вызов диалога сканирования кода размещения:
            UserSelect userSelect = new UserSelect();
            while (1 == 1)
            {
                logger.Trace("{0} Перед вызовом сканирования кода размещения", strFuncName);
                // 2.1 вызов диалога сканирования серийного номера:
                DialogResult dlgResult = userSelect.scanHTMLStringDialog(ref paramScan);
                if (dlgResult != DialogResult.OK)
                {
                    logger.Trace("{0} Диалог сканирования кода размещения вернул = {1}", strFuncName, dlgResult);
                    return dlgResult;
                }

                if (!paramScan.successScan)
                {
                    logger.Trace("{0} paramScan.successScan = {1}", strFuncName, paramScan.successScan);
                    continue;
                }
                if ((rezimRaboty==1)||(rezimRaboty==4))// ВзятьИз или ВзятьИзТочкаПередачи
                {
                    scanStateFields.strScanedKodRazmFrom = paramScan.scanedBarcode;
                }
                else//ПоложитьВ или положитьВ точка передачи
                {
                    scanStateFields.strScanedKodRazmTo = paramScan.scanedBarcode;
                }
                
                logger.Trace("{0} отсканирована строка кода размещения ='{1}'", strFuncName, paramScan.scanedBarcode);
                List<String> listBarcode = StorageCodes.detectStrStorageCodeFromFullString(paramScan.scanedBarcode);

                logger.Trace("{0} Вызов модуля определения частей паллета по строке, получилось {1} части", strFuncName, listBarcode.Count);

                // не удалось разобрать строку с кодом ячейки:
                if (listBarcode.Count == 0)
                {
                    paramScan.captionOne = captionOne + "<br> <size=42> <backcolor=255, 255, 255> <color=" + Color.RoyalBlue.ToArgb() + ">Не верная ячейка!</color></backcolor></size>";
                    logger.Trace("{0} Ошибка разбора строки кода размещения. Количество частей кода={1}", strFuncName, 0);
                    continue;
                }

                String stillageCode = listBarcode.ElementAtOrDefault(0);
                String floorCode = listBarcode.ElementAtOrDefault(1);
                String cellCode = listBarcode.ElementAtOrDefault(2);

                int stillageNumber = 0;
                int floorNumber = 0;
                int cellNumber = 0;

                // 2.6 попытка определить код стеллажа
                if (!Int32.TryParse(stillageCode, out stillageNumber))
                {
                    //paramScan.captionOne = String.Format("Ошибка: не удалось конвертировать '{0}' в номер стеллажа!", stillageCode);
                    paramScan.captionOne = captionOne + "<br> <size=42> <backcolor=255, 255, 255> <color=" + Color.RoyalBlue.ToArgb() + "> Не верный стеллаж! </color></backcolor></size>";
                    logger.Trace("{0} Ошибка разбора строки кода размещения. Не удалось конвертировать строку в номер стеллажа. Значение={1}", strFuncName, stillageCode);
                    continue;
                }

                // 2.7 попытка определить номер этажа
                if (!Int32.TryParse(floorCode, out floorNumber))
                {
                    //paramScan.captionOne = String.Format("Ошибка: не удалось конвертировать '{0}' в номер этажа!", floorCode);
                    paramScan.captionOne = captionOne + "<br> <size=42> <backcolor=255, 255, 255> <color=" + Color.RoyalBlue.ToArgb() + ">Не верный этаж!</color></backcolor></size>";
                    logger.Trace("{0} Ошибка разбора строки кода размещения. Не удалось конвертировать строку в номер этажа. Значение={1}", strFuncName, floorCode);
                    continue;
                }

                // 2.8 попытка определить номер ячейки
                if (!Int32.TryParse(cellCode, out cellNumber))
                {
                    //paramScan.captionOne = String.Format("Ошибка: не удалось конвертировать '{0}' в номер ячейки!", cellCode);
                    paramScan.captionOne = captionOne + "<br> <size=42> <backcolor=255, 255, 255> <color=" + Color.RoyalBlue.ToArgb() + ">Не верная ячейка!</color></backcolor></size>";
                    logger.Trace("{0} Ошибка разбора строки кода размещения. Не удалось конвертировать строку в номер ячейки. Значение={1}", strFuncName, cellCode);
                    continue;
                }

                // 2.9 сравнение номера стеллажа с номером стеллажа в документе:
                if (stCode.Stillage != stillageNumber)
                {
                    //paramScan.captionOne = String.Format("Ошибочный стеллаж!\nОтсканирован:{0}\nНужен:{1}", stillageNumber, stCode.Stillage);
                    paramScan.captionOne = captionOne + "<br> <size=36> <backcolor=255, 255, 255> <color=" + Color.RoyalBlue.ToArgb() + ">Ошибочный стеллаж!</color></backcolor></size>";
                    logger.Trace("{0} Ошибочный стеллаж. Отсканирован:{1} Нужен:{2}", strFuncName, stillageNumber, stCode.Stillage);
                    continue;
                }

                // 2.9 сравнение номера этажа с номером этажа в документе:
                if (stCode.Floor != floorNumber)
                {
                    //paramScan.captionOne = String.Format("Ошибочный этаж!\nОтсканирован:{0}\nНужен:{1}", floorNumber, stCode.Floor);
                    paramScan.captionOne = captionOne + "<br> <size=36> <backcolor=255, 255, 255> <color=" + Color.RoyalBlue.ToArgb() + "> Ошибочный этаж! </color></backcolor></size>";
                    logger.Trace("{0}: Ошибочный этаж! Отсканирован:{1} Нужен:{2}", strFuncName, floorNumber, stCode.Floor);
                    continue;
                }

                // 2.10 сравнение номера ячейки с номером ячейки в документе:
                if (stCode.Cell != cellNumber)
                {
                    //paramScan.captionOne = String.Format("Ошибочная ячейка!\nОтсканирована:{0}\nНужна:{1}", cellNumber, stCode.Cell);
                    paramScan.captionOne = captionOne + "<br> <size=36> <backcolor=255, 255, 255> <color=" + Color.RoyalBlue.ToArgb() + "> Ошибочная ячейка! </color></backcolor></size>";
                    logger.Trace("{0} Ошибочная ячейка! Отсканирована:{1} Нужна:{2}", strFuncName, cellNumber, stCode.Cell);
                    continue;
                }

                // 2.11 сохранение в общей структуре времени сканирования ячейки-отправителя:
               if ((rezimRaboty==1)||(rezimRaboty==4))// ВзятьИз или ВзятьИзТочкаПередачи
                {
                    scanStateFields.dTimeScanStorageCodeFrom = DateTime.Now;
                }
                else if ((rezimRaboty==2)||(rezimRaboty==3))//ПоложитьВ или положитьВ точка передачи
                {
                    scanStateFields.dTimeScanStorageCodeTo = DateTime.Now;
                }                
                break;
            }
            return DialogResult.OK;
        }
   
        /// <summary>
        /// вызов диалога сканирования товара в ячейке-отправителе для подтверждения корректности действия
        /// </summary>
        /// <param name="unitForWork"></param>
        /// <returns></returns>
        private DialogResult lokScanTovar(MobileWorkUnit mwUnit)
        {
            const string strFuncName = "lokScanTovar:";

            // 1. Определение текущего режима работы модуля:
            byte rezimRaboty = detectCurrentState(mwUnit);
            logger.Trace("{0} Вход в метод, mwUnit idd={1}, rezimRaboty={2}", strFuncName, scanStateFields.mobileWorkUnitIdd, rezimRaboty);

            // 2. запрос сканирования товара выводится только в режиме "1".
            //if (rezimRaboty!=2)
            //{
            //    return DialogResult.OK;
            //}

            // 2. определение строки товара
            structScanStringParams paramScan = new structScanStringParams()
            {
                captionOne = "",
                captionTwo = "",
                scanedBarcode = "",
                successScan = false,
                captionHelp = getTopCaption()
            };

            // 3. Выполняется запрос для определения реального товара, с которым мы работаем:
            XPQuery<DocSpecPeremeshGoods> xpQuery = new XPQuery<DocSpecPeremeshGoods>(objectSpace.Session());
            Goods tovar = (from s in xpQuery where scanStateFields.docRowIds.Contains(s.idd) select s.Good).FirstOrDefault<Goods>();

            if (tovar == null)
            {
             //   paramScan.captionOne = "Ошибка определения товара!";
                logger.Trace("{0} ошибка определения товара для сканирования по заданию: MobileWorkUnit.idd={1}, rezimRaboty={2}", strFuncName, scanStateFields.mobileWorkUnitIdd, rezimRaboty);
                return DialogResult.No;
            }

            //3.1 сохранение в общей структуре товара, с которым мы работаем и отсканированного ШК:
            scanStateFields.good = tovar;

            // 1.3 установка в заголовке окна кода:
            String tovarDescription = tovar.ShortDescription.Trim().PadRight(50);
            String captionOne = "<size=40> <backcolor=255, 255, 255> <color=" + Color.RoyalBlue.ToArgb() + ">      " + scanStateFields.storageCodeTakeFrom.PalletCode.PadRight(30) + "    </color></backcolor></size><br>" +
                    "<size=30>" + tovarDescription + "</size><br>" +
                     "<size=36>     Отсканируйте   <br>" +
                     "     штрих-код товара:</size>";
   
            //paramScan.captionOne = captionOne;    

            // 2. вызов диалога сканирования кода размещения:
            UserSelect userSelect = new UserSelect();
            while (1 == 1)
            {
                logger.Trace("{0} Перед вызовом сканирования товара:", strFuncName);
                // 2.1 вызов диалога сканирования серийного номера:
                paramScan.inputMode = enumInputMode.СканированиеШтрихкода;
                paramScan.enableQuestionButton = true;
                scanStateFields.strLabelCaptionOne = captionOne;
                DialogResult dlgResult = userSelect.scanHTMLStringDialog(ref paramScan, processBarcode, getLabelCaption);
                if (dlgResult != DialogResult.OK)
                {
                    logger.Trace("{0} Диалог сканирования кода размещения вернул = {1}", strFuncName, dlgResult);
                    return dlgResult;
                }

                if (!paramScan.successScan)
                {
                    logger.Trace("{0} paramScan.successScan = {1}", strFuncName, paramScan.successScan);
                    continue;
                }

                //scanStateFields.strScanedTovarBarcode = paramScan.scanedBarcode;

                //logger.Trace("{0} отсканирована строка товара ='{1}'", strFuncName, paramScan.scanedBarcode);

                //// ШК товара нужно найти товар и сравнить его с товаром в документе.
                //XPQuery<BarcodesOfGoods> xpQueryBG = new XPQuery<BarcodesOfGoods>(objectSpace.Session());
                //var tovarsBG = from s in xpQueryBG where s.Barcode == paramScan.scanedBarcode select s.ParentId;

                //logger.Trace("{0} Вызов модуля определения частей паллета по строке, получилось {1} части", strFuncName, tovarsBG.Count());

                //// не удалось найти ни одного товара по ШК:
                //if (tovarsBG.Count() == 0)
                //{
                //    scanStateFields.strLabelCaptionOne = captionOne + "<br> <size=42> <backcolor=255, 255, 255> <color=" + Color.RoyalBlue.ToArgb() + ">Штрих-кода нет в базе!</color></backcolor></size>";
                //    logger.Trace("{0} Ошибка разбора строки кода размещения. Количество частей кода={1}", strFuncName, 0);
                //    continue;
                //}

                //var tovarInBarcodeOfGoods = (from o in tovarsBG where o.idd == tovar.idd select o).FirstOrDefault<Goods>();

                //// 2.9 Если товар по отсканированному ШК не найден:
                //if (tovarInBarcodeOfGoods == null)
                //{
                //    //paramScan.captionOne = String.Format("Ошибочный стеллаж!\nОтсканирован:{0}\nНужен:{1}", stillageNumber, stCode.Stillage);
                //    scanStateFields.strLabelCaptionOne = captionOne + "<br> <size=36> <backcolor=255, 255, 255> <color=" + Color.RoyalBlue.ToArgb() + ">По штрих-коду " + paramScan.scanedBarcode + " не найден товар БД!</color></backcolor></size>";
                //    logger.Trace("{0} По штрих-коду: {1} не найдено ни одного товара в БД!", strFuncName, paramScan.scanedBarcode);
                //    continue;
                //}

                //if (tovarInBarcodeOfGoods.idd != tovar.idd)
                //{
                //    scanStateFields.strLabelCaptionOne = captionOne + "<br> <size=36> <backcolor=255, 255, 255> <color=" + Color.RoyalBlue.ToArgb() + ">Ошибочный товар! Отсканирован:" + tovarInBarcodeOfGoods.ShortDescription + ", нужен:" + tovarDescription + "</color></backcolor></size>";
                //    logger.Trace("{0} Отсканированный ошибочный товар. Отсканирован:{1} Нужен:{2}", strFuncName, tovar.idd, tovarsBG.First().idd);
                //    continue;
                //}

                return DialogResult.OK;
            }

        }
 
        /// <summary>
        /// Основной диалог для ввода или сканирования количества товара
        /// </summary>
        /// <returns></returns>
        private DialogResult lokEnterMainMoveDialog()
        {
            const string strFuncName = "lokScanTovar:";
           
            logger.Trace("{0} Вход в метод, mwUnit idd={1}", strFuncName, scanStateFields.mobileWorkUnitIdd);

            // 2. создание структуры для передачи в открываемую форму
            String tovarDescription = scanStateFields.good.ShortDescription.Trim().PadRight(50);
            String captionOne = "<size=40> <backcolor=255, 255, 255> <color=" + Color.RoyalBlue.ToArgb() + ">      " + scanStateFields.storageCodeTakeFrom.PalletCode.PadRight(30) + "    </color></backcolor></size>";
            String captionTwo = "<size=30>" + tovarDescription + "</size>"; 

            // 3. определение единицы измерения, и суммы количества размещаемого товара:
            XPQuery<DocSpecPeremeshGoods> xpQuery = new XPQuery<DocSpecPeremeshGoods>(objectSpace.Session());
            var summByPallet = from s in xpQuery where scanStateFields.docRowIds.Contains(s.idd)
                               group s by s.PutTo.PalletCode into newGRP                               
                select new 
                {
                    key = newGRP.Key,
                    //firstUnit = (from aa in newGRP where aa.Unit!=null select aa.Unit),
                    unitsSumm = newGRP.Sum(a=>a.QuantityOfUnits),
                    itemsSumm = newGRP.Sum(a=>a.QuantityOfItems),
                    totalSumm = newGRP.Sum(a=>a.TotalQuantity)
                };

            if (summByPallet==null)
            {
                return DialogResult.Abort;
            }

            foreach (var item in summByPallet)
	        {
                scanStateFields.decPlanItems = item.itemsSumm;
                scanStateFields.decPlanUnits = item.unitsSumm;
                scanStateFields.decPlanTotal = item.totalSumm;
                //scanStateFields.unit = item.firstUnit;
	        }

            var unitByPallet = (from s in xpQuery
                                where scanStateFields.docRowIds.Contains(s.idd)
                                && s.Unit!=null
                                select s.Unit).FirstOrDefault<Units>();
            scanStateFields.unit = unitByPallet;

            structScanStringParams paramScan = new structScanStringParams()
            {
                captionOne = captionOne,
                captionTwo = captionTwo,
                scanedBarcode = "",
                successScan = false,
                captionHelp = ""
            };

            // 2. вызов диалога ввода количества товара:
            UserSelect userSelect = new UserSelect();
            while (1 == 1)
            {
                logger.Trace("{0} Перед вызовом ввода количества товара:", strFuncName);
                // 2.1 вызов диалога ввода количества товара:

                paramScan.inputMode = enumInputMode.ВводТолькоКоличестваФакт;
                paramScan.enableQuestionButton = true;
                paramScan.decFactItems = 0;
                paramScan.decFactUnits = 0;
                paramScan.decFactTotal = 0;
                paramScan.decPlanItems = scanStateFields.decPlanItems;
                paramScan.decPlanUnits = scanStateFields.decPlanUnits;
                paramScan.decPlanTotal = scanStateFields.decPlanTotal;
                paramScan.unitFactCalculate = scanStateFields.unit;
                //scanStateFields.strLabelCaptionOne = captionOne;

                Func<string, int> processBarcode1 = processBarcode;
                DialogResult dlgResult = userSelect.inputQantityOfUnitsHTML(ref paramScan, processBarcode1, getLabelCaption);
                if (dlgResult != DialogResult.OK)
                {
                    logger.Trace("{0} Диалог сканирования кода размещения вернул = {1}", strFuncName, dlgResult);
                    return dlgResult;
                }

                if (!paramScan.successScan)
                {
                    logger.Trace("{0} paramScan.successScan = {1}", strFuncName, paramScan.successScan);
                    continue;
                }

                // 2.2 установка фактического количества и кода ошибки
                scanStateFields.iddMobileError = paramScan.iddMobileErrors;
                scanStateFields.decFactTotal = paramScan.decFactTotal;
      
                return DialogResult.OK;
            }              
        }
        
        /// <summary>
        /// метод изменяет фактическое количество в документе по факту размещения.
        /// </summary>
        private void lokChangeQuantityToFact()
        {

        }

        /// <summary>
        /// Основной модуль размещения товара в цикле
        /// </summary>
        /// <returns></returns>
        private bool runMainLoop()
        {
            MobileWorkUnit unitForWork = null;
            while (true)
            {
                // 1. первоначальное подключение к операции или продолжение операции
                if ((scanStateFields.currentScanState == enumScanState.НачалоРаботы)
                    || (scanStateFields.currentScanState == enumScanState.СледующееРазмещение))
                {
                  #region 1. Получение задания на перемещение и фиксация его за текущим сотрудником
                  // 1.1 получение строки задания, с учетом документа, по которому мы выполняли ранее размещение.
                    unitForWork = findUnitForWork();

                    // 1.2 не найдено ни одного задания. По идее, здесь нужно как-то проинфомировать о том, что не найдено ни одного задания.
                    if (unitForWork==null)
                    {
                        break;
                    }
                    // установка текущего пользователя, если он не выбран
                    if (unitForWork.AssignedUser==null)
                    {
                        unitForWork.AssignedUser = objectSpace.FindObject<Users>(new BinaryOperator("idd",((Users)SecuritySystem.CurrentUser).idd, BinaryOperatorType.Equal));                            
                    }
                    unitForWork.TypeOfMobileState = enTypesOfMobileStates.ВРаботе;
                    scanStateFields.docRowIds = unitForWork.MobileWorkDocRowsCollection.Select(o => o.BaseDocRowId).AsQueryable<int>();
                    scanStateFields.idDocPeremeshenieCurrent = unitForWork.BaseDoc.idd;

                    // 1.2 попытка записать задание для текущего пользователя:
                    bool bSaveUnit = saveUnitForCurrentUser(unitForWork);
                    // если не удалось записать задание, продолжим цикл
                    if (!bSaveUnit)
                    {
                        Thread.Sleep(1000);// обождем секунду
                        continue;
                    }
                    #endregion

                  #region 2. Запрос сканирования паллета-отправителя
                  // 1.3 если это сканирование паллета-отправителя, запомним время начала работы:
                    scanStateFields.dTimeStartOperation = DateTime.Now;
                    scanStateFields.mobileWorkUnitIdd = unitForWork.idd;

                    // 2. запрос сканирования паллета отправителя: 
					DialogResult dlgResult = lokScanKodRazmesheniya(unitForWork);
					if (dlgResult == DialogResult.Cancel)
					{
                        break;
					}
                    else if (dlgResult != DialogResult.OK)
                    {
                        logger.Trace("runMainLoop: Диалог сканирования ячейки-получателя вернул: {0}", dlgResult);
						scanStateFields.refreshAllField();// сброс всех значений и начать с начала
						continue;
                    }
                    scanStateFields.dTimeScanStorageCodeFrom = DateTime.Now;
					scanStateFields.currentScanState = enumScanState.ОтсканированПаллетОтправитель;
                  #endregion
                }                  
                             
                #region 3. Запрос сканирования товара
                if (scanStateFields.currentScanState==enumScanState.ОтсканированПаллетОтправитель)
                {
                    // 2. запрос сканирования товара:
                    DialogResult dlgResult = lokScanTovar(unitForWork);
                    if (dlgResult != DialogResult.OK)
                    {
                        logger.Trace("runMainLoop: Диалог сканирования товара вернул: {0}", dlgResult);
                        //scanStateFields.refreshAllField();// сброс всех значений и начать с начала
                        scanStateFields.currentScanState = enumScanState.НачалоРаботы;
                        continue;
                    }
                    else
                    {
                        scanStateFields.currentScanState = enumScanState.ОтсканированТовар;
                    }                    
                }
                #endregion

                #region 4. Вывод основного диалога сканирования или ручного ввода количества товара:
                if (scanStateFields.currentScanState == enumScanState.ОтсканированТовар)
                {
                    DialogResult dlgResult = lokEnterMainMoveDialog();
                    if (dlgResult == DialogResult.Abort)
                    {
                        // 4.1 пользователь отсканировал неверный ШК паллета и отказался повторно его размещать
                        scanStateFields.currentScanState = enumScanState.НачалоРаботы;
                        logger.Trace("runMainLoop: Диалог повторного сканирования SN паллетной этикетки вернул: {0}", dlgResult);
                        continue;
                    }
                    else if (dlgResult == DialogResult.OK)
                    {
                        scanStateFields.currentScanState = enumScanState.ОтсканированоВсеКоличество;
                    }
                    else
                    {
                        break;
                    }                    
                }
                #endregion

                #region 5. Отсканировано все количество, запрос сканирования ШК ячейки-получателя.
                if (scanStateFields.currentScanState == enumScanState.ОтсканированоВсеКоличество )
                {
                    // 2. запрос сканирования паллета отправителя: 
                    DialogResult dlgResult = lokScanKodRazmesheniya(unitForWork);
                    if (dlgResult == DialogResult.Cancel)
                    {
                        break;
                    }
                    else if (dlgResult != DialogResult.OK)
                    {
                        logger.Trace("runMainLoop: Диалог сканирования ячейки-получателя вернул: {0}", dlgResult);
                        scanStateFields.refreshAllField();// сброс всех значений и начать с начала
                        continue;
                    }
                    scanStateFields.currentScanState = enumScanState.ОтсканированПаллетПолучатель;
                }
                
                if (scanStateFields.currentScanState == enumScanState.ОтсканированПаллетПолучатель)
                {
                    // 3.5.2 выполнение сохранения результатов маркировки в БД:
                    bool isTrue = lokSaveAllDateToDataBase();
                    // выведем сообщение пользователю
                    if (isTrue)
                    {
                        showUserMessage("Паллет успешно размещен!");
                    }
                    // 3.5.3установим признак, что серийный номер успешно отсканирован;
                    scanStateFields.currentScanState = enumScanState.НачалоРаботы; 
                }
                #endregion
            }
            return true;
        }
        
        private void showUserMessage(String strMessage)
		{
            structScanStringParams paramScan = new structScanStringParams() 
            { 
                captionOne = "", 
                inputMode = enumInputMode.ТолькоСообщениеиОК, 
                captionTwo = strMessage, 
                scanedBarcode = "", 
                successScan = false, 
                captionHelp = getTopCaption() };

			UserSelect userSelect = new UserSelect();
			userSelect.userMessage(ref paramScan);
		}

        /// <summary>
        /// 1. общедоступный стартовый модуль, вызывается пользователями класса
        /// </summary>
        /// <param name="selectedJobType"></param>
        /// <returns></returns>
        public bool startModule(JobTypes selectedJobType)
        {
            logger.Trace("Вход в функцию, selectedJobtype = {0}", selectedJobType);
            scanStateFields = new localClassScanState
            {
                sprJobeType = selectedJobType,
                currentScanState = enumScanState.НачалоРаботы
            };
            // 2. Здесь можно вызвать диалог для сканирования Спец.прихода или требования как фильтра
            // 3. вызов модуля сканирования SN паллета в бесконечном цикле:
            return runMainLoop();
        }

        		// метод выполняет сохранение в базе всех результатов сканирования
		private bool lokSaveAllDateToDataBase()
		{
			const string strFuncName = "lokSaveAllDateToDataBase:";
			logger.Trace("Вход в метод {0}. Выполнение перемещения по заданию: {1}", strFuncName, scanStateFields.mobileWorkUnitIdd);

			if (scanStateFields.docRowIds == null)
			{                
				logger.Error("{0} Пополнение по документу idd={1}. Ошибка: нет строк документа для сохранения изменений.queryDocRows=null", strFuncName, scanStateFields.idDocPeremeshenieCurrent);
				showUserMessage("Ошибка: нет строк спец.перемещения для сохранения изменений");
				return false;
			}

			// 3.1 установка разделителя по умолчанию для текущего пользователя
			Users current_User = (Users)SecuritySystem.CurrentUser;
			Delimeters delimeter = objectSpace.Session().FindObject<Delimeters>(new BinaryOperator("DelimeterID", current_User.DefaultDelimeter.DelimeterID, BinaryOperatorType.Equal), true);

			// 3.2 определение текущего пользователя:
			Users currentUser = objectSpace.Session().FindObject<Users>(new BinaryOperator("idd", current_User.idd, BinaryOperatorType.Equal));

			// 3.4 передача в обмен с СУТЗ 7.7 выполненной единицы работы Размещение товара с идентификаторами выполненных строк Спец.прихода:
			XPQuery<JobTypes> queryJobTypes = new XPQuery<JobTypes>(objectSpace.Session());
            JobTypes elementJobType = scanStateFields.sprJobeType;//(from c in queryJobTypes where c.TypeOfWork == enTypeOfWorks.Перемещение select c).FirstOrDefault<JobTypes>();

			// 3.5 проверка найденных элементов на пустое значение:
			if (currentUser == null)
			{
				logger.Error("{0} Перемещение по документу idd={1}. Ошибка определения текущего пользователя. currentUser=null", strFuncName, scanStateFields.idDocPeremeshenieCurrent);
				showUserMessage("Ошибка определения текущего пользователя!");
				return false;
			}
			if (delimeter == null)
			{
				logger.Error("{0} Перемещение по документу idd={1}. Ошибка определения текущего разделителя учета. delimeter=null", strFuncName, scanStateFields.idDocPeremeshenieCurrent);
				showUserMessage("Ошибка определения текущего разделителя учета!");
				return false;
			}
			if (elementJobType == null)
			{
				logger.Error("{0} Перемещение по документу idd={1}. Ошибка определения текущего вида работы. elementJobType=null", strFuncName, scanStateFields.idDocPeremeshenieCurrent);
				showUserMessage("Ошибка определения текущего вида работы!");
				return false;
			}
			if (scanStateFields.idDocPeremeshenieCurrent == 0)
			{
				logger.Error("{0} Перемещение по документу idd={1}. Ошибка поиска по идд документа спец.перемещения. idDocPeremeshenieCurrent=0", strFuncName, scanStateFields.idDocPeremeshenieCurrent);
				showUserMessage("Ошибка определения текущей спец.перемещения!");
				return false;
			}

			// 3.6.1 запись основной единицы работы:
			if (scanStateFields.mobileWorkUnitIdd == 0)
			{
				logger.Error("{0} Перемещение по документу idd={1}. idd единицы работы = 0. Невозможно выполнить сохранение результатов работы!", strFuncName, scanStateFields.idDocPeremeshenieCurrent);
				showUserMessage("Ошибка определения единицы работы!");
				return false;
			}

			MobileWorkUnit mobileUnit = objectSpace.FindObject<MobileWorkUnit>(new BinaryOperator("idd", scanStateFields.mobileWorkUnitIdd, BinaryOperatorType.Equal));
			if (mobileUnit != null)
			{ 
				mobileUnit.TypeOfMobileState = enTypesOfMobileStates.Выполнен;
				mobileUnit.TypeOfUnitWork = enTypesOfUnitWork.Выполнен;
                mobileUnit.Barcode = scanStateFields.strScanedPalletBarcode;
                mobileUnit.QuantityFact = scanStateFields.decFactTotal;

			};
			mobileUnit.Save();

			// 3.6.2 добавление сотрудника и времени выполнения операции:
            // поиск ошибки мобильной сутз, если она была указана:
            MobileErrors mobileError = objectSpace.GetObjectByKey<MobileErrors>(scanStateFields.iddMobileError);

			MobileWorkUsers newUserWorkRecord = new MobileWorkUsers(objectSpace.Session()) 
			{ 
				Delimeter = delimeter, 
				User = currentUser, 
				ErrorState = mobileError, 
				dTimeStart = scanStateFields.dTimeStartOperation, 
				dTimeEnd = DateTime.Now, 
                dTimeClosedFrom = scanStateFields.dTimeScanStorageCodeFrom,
				dTimeClosedTo = scanStateFields.dTimeScanStorageCodeTo
			};
			newUserWorkRecord.Save();
			mobileUnit.MobileWorkUsersCollection.Add(newUserWorkRecord);

			// 3.6.3 добавление в задание на выгрузку строк документа, по которым было выполнено перемещение:
            NET_SUTZ_Unloads unloadDoc = new NET_SUTZ_Unloads(objectSpace.Session()) 
            { ObjectType = typeof(DocSpecPeremeshenie).FullName, 
                ObjectRow_Id = scanStateFields.idDocPeremeshenieCurrent, 
                Outload = 0, 
                User = currentUser, 
                dTimeInsertRecord = DateTime.Now, 
                dTimeUnloadRecord = null, 
                FullDocTable = false 
            };
            unloadDoc.Save();

            foreach (int item in scanStateFields.docRowIds)
            {
                NET_SUTZ_DocRows unloadRow = new NET_SUTZ_DocRows(objectSpace.Session())
                {
                    ObjectDocTableId = item,
                    BitFlagsOfOuboundFields = enFlagsOfOuboundFields.СтатусПроведения
                                                |enFlagsOfOuboundFields.Количество
                                                |enFlagsOfOuboundFields.КоличествоКор
                                                |enFlagsOfOuboundFields.КоличествоШт
                                                |enFlagsOfOuboundFields.КоличествоФакт
                };
                unloadRow.Save();
                unloadDoc.NET_SUTZ_DocRows.Add(unloadRow);
            }
            unloadDoc.Save();

            // 3.6.4 установка у строки документа спец.перемещения нового статуса проведения:
			XPQuery<DocSpecPeremeshGoods> querySPPrihodaGoods = new XPQuery<DocSpecPeremeshGoods>(objectSpace.Session());
            var listOfDocRows = from c in querySPPrihodaGoods where scanStateFields.docRowIds.Contains(c.idd) select c;

            // 3.6.4.1 Для определения статуса проведения строки документа, нужно проверить признак "точкаПередачи"
            // вообще, нужно сделать признак в классе состояния, для определения, что именно мы размещаем - обычное размещение или
            // через точку передачи
			foreach (DocSpecPeremeshGoods item in listOfDocRows)
			{
			    item.MoveStatus = enDocRowMoveStatus.ЧП_ПоложитьВ;
			    item.Save();			
			}

            // 3.6.4.2 уменьшение количество в табличной части документа, если по факту разместили меньше, чем было нужно:
            int linesCount = listOfDocRows.Count();
            if (linesCount == 2)
            {

                decimal ostLeft = listOfDocRows.First().TotalQuantity;
                decimal ostRight = listOfDocRows.Last().TotalQuantity;

                // 1. сумма всех подячеек по размещаемому паллету:
                decimal totalTask = listOfDocRows.Sum(n => n.TotalQuantity);

                // 2. количество факт.размещения: scanStateFields.decFactTotal
                decimal totalSumAfter = scanStateFields.decFactTotal;
                decimal totalSumMinProp = Decimal.Ceiling(totalSumAfter / linesCount);
                totalSumMinProp = totalSumMinProp - Decimal.Remainder(totalSumMinProp, scanStateFields.unit.Koeff);// округление до целых коробок   

                decimal totalSumMaxProp = totalSumAfter - totalSumMinProp;

                // 3. новый остаток в ячейках:
                decimal ostLeftNew = Math.Min(ostLeft, totalSumMinProp);
                decimal ostRightNew = Math.Min(ostRight, totalSumMaxProp);

                decimal perenosRight = 0;
                decimal perenosLeft = 0;

                if (totalSumMinProp > ostLeftNew)
                {
                    perenosRight = totalSumMinProp - ostLeftNew;
                }
                if (totalSumMaxProp > ostRightNew)
                {
                    perenosLeft = totalSumMaxProp - ostRightNew;
                }

                ostLeftNew += perenosLeft;
                ostRightNew += perenosRight;

                int iii = 0;
                foreach (DocSpecPeremeshGoods item in listOfDocRows)
                {
                    iii++;
                    if (iii == 1)
                    {
                        item.TotalQuantity = ostLeftNew;
                    }
                    else if (iii == 2)
                    {
                        item.TotalQuantity = ostRightNew;
                    }
                    item.QuantityFact = item.TotalQuantity;
                    item.QuantityOfUnits = Decimal.Divide(item.TotalQuantity, item.Unit.Koeff);
                    item.QuantityOfItems = Decimal.Subtract(item.TotalQuantity, (Decimal.Multiply(item.QuantityOfUnits, item.Unit.Koeff)));

                    item.Save();
                }
            }
            else// строк в паллете одна или больше двух
            {
                decimal plan = listOfDocRows.Sum(n => n.TotalQuantity);

                // количество, которое нужно вычесть из табличной части:
                decimal razniza = plan - scanStateFields.decFactTotal;

                foreach (DocSpecPeremeshGoods item in listOfDocRows)
                {
                    //decimal minQuantity = (item.TotalQuantity<razniza) ? 0 : (item.TotalQuantity-razniza);
                    if (razniza<=item.TotalQuantity)
                    {
                        item.QuantityFact = item.TotalQuantity-razniza;
                        item.TotalQuantity = item.TotalQuantity - razniza;
                        razniza = 0;
                    }
                    else
                    {
                        razniza -= item.TotalQuantity;
                        item.QuantityFact = 0;
                        item.TotalQuantity = 0;                        
                    }                    

                    item.QuantityOfUnits = Decimal.Divide(item.TotalQuantity, item.Unit.Koeff);
                    item.QuantityOfItems = Decimal.Subtract(item.TotalQuantity, (Decimal.Multiply(item.QuantityOfUnits, item.Unit.Koeff)));
                    item.QuantityFact = item.TotalQuantity;

                    item.Save();
                 }
            }

			// 3.6.5 запись нового номера паллета кода размещения паллетной этикетки.
            if (scanStateFields.scanedPalletLabel!=null)
            {
                PalletLabels currentPL = objectSpace.Session().FindObject<PalletLabels>(new BinaryOperator("idd", scanStateFields.scanedPalletLabel.idd, BinaryOperatorType.Equal));
                if ((currentPL != null) && (scanStateFields.storageCodeTakeFrom != null))
                {
                    currentPL.PalletCode = scanStateFields.storageCodePutTo.PalletCode;
                    currentPL.Save();
                }
            }			

			// 3.7 нужно выполнить сохранение записанных элементов, чтобы их идд записать в таблицу выгрузки из СУТЗ2:
			if (!lokRunCommitChanges())
			{
				return false;
			}

			// 3.8.1 добавление единицы работы в задание на выгрузку в 7.7
			NET_SUTZ_Unloads unloads = new NET_SUTZ_Unloads(objectSpace.Session());
			unloads.ObjectType = typeof(MobileWorkUnit).FullName;
			unloads.ObjectRow_Id = mobileUnit.idd;
			unloads.Outload = 0;
			unloads.User = currentUser;
			unloads.dTimeInsertRecord = DateTime.Now;
			unloads.dTimeUnloadRecord = null;
			unloads.FullDocTable = false;
			unloads.Save();

            // 3.8.2 проверка выполненных строк документа, если они выполнены все, то у документа ставится признак
            // "РазрешитьДвижение" и разрешение на работу меняется на "Выполнено".
            XPQuery<DocSpecPeremeshGoods> querySPPrihodaGoodsNotComplete = new XPQuery<DocSpecPeremeshGoods>(objectSpace.Session());
            var listOfNotCompletedDocRows = from c in querySPPrihodaGoodsNotComplete 
                                where 
                                c.ParentID.idd==scanStateFields.idDocPeremeshenieCurrent 
                                &&  c.MoveStatus != enDocRowMoveStatus.ЧП_ПоложитьВ
                                select c;

            if ((listOfNotCompletedDocRows==null)||(listOfNotCompletedDocRows.Count()==0))
            {
                DocSpecPeremeshenie currentDocMove = objectSpace.FindObject<DocSpecPeremeshenie>(new BinaryOperator("idd", scanStateFields.idDocPeremeshenieCurrent, BinaryOperatorType.Equal), true);
                // установка в документе признака "РазрешитьДвижение"
                currentDocMove.AllowMovement = true;
                currentDocMove.Save();
                currentDocMove.checkIn();

                // изменение статуса работы документа
                XPQuery<DocJobesProperties> djp = new XPQuery<DocJobesProperties>(objectSpace.Session());
                DocJobesProperties djProp = (from c in djp 
                           where c.BaseDoc.idd==scanStateFields.idDocPeremeshenieCurrent
                           && c.JobType.idd == scanStateFields.sprJobeType.idd
                           select c).FirstOrDefault<DocJobesProperties>();
                if (djProp!=null)
                {
                    djProp.TypeOfMobileState = enTypesOfMobileStates.Выполнен;
                    djProp.Save();
                    // 3.8.3 выгрузка в 7.7 задания на работу: реализовать. добавлено 30.01.2016
                    NET_SUTZ_Unloads unloadsDJP = new NET_SUTZ_Unloads(objectSpace.Session());
                    unloadsDJP.ObjectType = typeof(DocJobesProperties).FullName;
                    unloadsDJP.ObjectRow_Id = djProp.idd;
                    unloadsDJP.Outload = 0;
                    unloadsDJP.User = currentUser;
                    unloadsDJP.dTimeInsertRecord = DateTime.Now;
                    unloadsDJP.dTimeUnloadRecord = null;
                    unloadsDJP.FullDocTable = false;
                    unloadsDJP.Save();
                }               
            }

			if (!lokRunCommitChanges())
			{
				return false;
			}
			return true;
		}

		private bool lokRunCommitChanges()
		{
			// 1. сохранение данных в транзакции
			// https://documentation.devexpress.com/#CoreLibraries/DevExpressXpoDBExceptions

			String strError = "";

			try
			{
				objectSpace.CommitChanges();
			}
			catch (LockingException lockException)
			{
				strError = String.Format("Ошибка LockingException: {0}", lockException.Message);
				logger.Error(lockException,"lokRunCommitChanges: Ошибка LockingException.");
			}
			catch (SqlExecutionErrorException sqlException)
			{
				strError = String.Format("Ошибка sqlException: {0}", sqlException.Message);
				logger.Error(sqlException,"lokRunCommitChanges: Ошибка sqlException.");

			}
			catch (UnableToOpenDatabaseException openDBException)
			{
				strError = String.Format("Ошибка openDBException: {0}", openDBException.Message);
				logger.Error(openDBException,"lokRunCommitChanges: Ошибка openDBException.");
			}
			catch (Exception ex)
			{
				strError = "Ошибка Exception. документ idd=" + scanStateFields.idDocPeremeshenieCurrent;
				logger.Error(ex,"lokRunCommitChanges: Ошибка Exception.");
			}

			if (strError.Length > 0)
			{
				// 2. информирование пользователя об ошибке:
                structScanStringParams paramScan = new structScanStringParams() 
                { 
                    captionOne = "Ошибка при сохранении данных в БД", 
                    inputMode = enumInputMode.ТолькоСообщениеиОК, 
                    captionTwo = strError, 
                    scanedBarcode = "", 
                    successScan = false, 
                    captionHelp = getTopCaption() 
                };

				UserSelect userSelect = new UserSelect();
				userSelect.userMessage(ref paramScan);
				return false;
			}
			return true;
		}

        /// <summary>
        /// локальный класс, для хранения переменных сканирования
        /// </summary>
        [NonPersistent]
        private class localClassScanState
        {
            // 1. текущий вид работы
            internal JobTypes sprJobeType { get; set; }

            // 2. документ спец.перемещения, установленный в качестве фильтра или как предыдущий документ
            internal int idDocPeremeshenieFilter { get; set; }

            // 3. документ спец.перемещения - предыдущий документ
            internal int idDocPeremesheniePrev { get; set; }

            // 3. документ спец.перемещения - Текущий документ
            internal int idDocPeremeshenieCurrent { get; set; }

            // 3. текущий этап работы, перечисление
            internal enumScanState currentScanState { get; set; }

            // 4. текстовое представление текущего этапа работы
            internal String currentWorkInfo { get; set; }

            // 5. поля для работы логистического блока:
            // время, когда сотрудник получил задание - код паллета-отправителя
            internal DateTime dTimeStartOperation { get; set; }
            internal DateTime? dTimeScanStorageCodeFrom { get; set; }
            internal DateTime? dTimeScanStorageCodeTo { get; set; }
            internal DateTime? dTimeScanTovar { get; set; }

            // 6. строка, отсканированный ШК паллета 
            internal string strScanedPalletBarcode { get; set; }

            // 6.1 ссылка на код размещения-отправитель, который выводился в диалоге сканирования ВзятьИз
            internal StorageCodes storageCodeTakeFrom { get; set; }

            // 6.2 ссылка на код размещения-получатель, который выводился в диалоге сканирования ПоложитьВ
            internal StorageCodes storageCodePutTo { get; set; }

            // 7. найденная паллетная этикетка по баркоду
            internal PalletLabels scanedPalletLabel { get; set; }

            // 8. строка, отсканированный ШК кода размещения паллета-отправителя и паллета-получателя
            internal string strScanedKodRazmFrom { get; set; }
            internal string strScanedKodRazmTo { get; set; }

            // 9. Отсканированный ШК товара
            internal string strScanedTovarBarcode { get; set; }

            // 9.1  товар в паллете для размещения
            internal Goods good { get; set; }

            // 10. элемент справочника ЗаданияСклада, по которому выполняется размещение товара:
            internal int mobileWorkUnitIdd { get; set; }

            // 11. список идентификаторов строк документа перемещение, по которым выполняется размещение
            internal IQueryable<int> docRowIds { get; set; }

            // 12. в этом параметре будет передаваться HTML строка для формирования заголовка диалога работы с пользователем
            internal string strLabelCaptionOne { get; set; }

            // 13. поля для сохранения планового и фактического количества и единицы измерения
            internal decimal decFactUnits { get; set; }
            internal decimal decFactItems { get; set; }
            internal decimal decFactTotal { get; set; }

            internal decimal decPlanUnits { get; set; }
            internal decimal decPlanItems { get; set; }
            internal decimal decPlanTotal { get; set; }
            internal Units unit { get; set; }

            // 14. ошибка при выполнении строки документа, выбранная пользователем вручную:
            internal int iddMobileError { get; set; }

            // метод сбрасывает все значения полей при новом сеансе сканирования
            public void refreshAllField()
            {
                this.idDocPeremeshenieFilter = 0;
                this.idDocPeremesheniePrev = 0;
                this.idDocPeremeshenieCurrent = 0;
                this.currentScanState = enumScanState.НачалоРаботы;
                this.currentWorkInfo = "";
                this.dTimeStartOperation = DateTime.Now;
                this.strScanedPalletBarcode = "";
                this.scanedPalletLabel = null;
                this.strScanedKodRazmFrom = "";
                this.strScanedKodRazmTo = "";
                this.mobileWorkUnitIdd = 0;
                this.dTimeScanStorageCodeFrom = null;
                this.dTimeScanStorageCodeTo = null;
                this.docRowIds = null;
                this.good = null;
                this.storageCodeTakeFrom = null;
            }
        }

        /// <summary>
        /// локальное перечисление для сохранения этапов выполнения модуля
        /// </summary>
        private enum enumScanState : byte
        {
            НачалоРаботы = 1,
            ОтсканированПаллетОтправитель = 2,
            ОтсканированТовар = 3,
            ОтсканированоВсеКоличество = 4,
            ОтсканированПаллетПолучатель = 5,
            СледующееРазмещение = 6
        }

    }



}
