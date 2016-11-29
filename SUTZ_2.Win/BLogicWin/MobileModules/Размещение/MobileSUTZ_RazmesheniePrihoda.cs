using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using DevExpress.Xpo;
using DevExpress.ExpressApp.Utils;
using SUTZ_2.Module.BO.References;
using SUTZ_2.Module.BO.Documents;
using SUTZ_2.Module.BO;
using System.Windows.Forms;
using SUTZ_2.Module.BO.References.Mobile;
using DevExpress.ExpressApp;
using DevExpress.Data.Filtering;
using DevExpress.Xpo.DB.Exceptions;
using SUTZ_2.Module;

namespace SUTZ_2.MobileSUTZ
{
	/// <summary>
	/// Класс мобильной СУТЗ для размещения прихода после маркировки
	/// </summary>
	class MobileSUTZ_RazmesheniePrihoda: IDisposable
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();
		//public Session session {get;set;}
		private localClassScanState scanStateFields { get; set; }
		public IObjectSpace objSpace { get; set; }

		#region Члены IDisposable
		void IDisposable.Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
				if (objSpace != null)
				{
					objSpace.Dispose();
				}
		}

		public MobileSUTZ_RazmesheniePrihoda(IObjectSpace paramObjSpace)
		{
			Guard.ArgumentNotNull(paramObjSpace, "paramObjSpace");
			objSpace = paramObjSpace;

           //IObjectSpace iObjSpace = currentSessionSettings.ObjXafApp.CreateObjectSpace(); 
		}

		~MobileSUTZ_RazmesheniePrihoda()
		{
			Dispose(false);
		}

		// 2.1 формирует заголовок формы
		private String getTopCaption()
		{
            String strTopCaption = "";
            if (scanStateFields.currentScanState == enumScanState.НачалоРаботы)
            {
                strTopCaption = scanStateFields.sprJobeType.Description.Trim();
            }
            else if (scanStateFields.currentScanState==enumScanState.ОтсканированСерийныйНомер)
            {
                if (scanStateFields.docSpecProhoda!=null)
                {
                    strTopCaption += scanStateFields.docSpecProhoda.DocNo;
                }
            }
    		return strTopCaption;// пока пусто.
		}

		/// <summary>
		/// Метод расчета свободного количества паллет, готовых для размещения на склад.
		/// </summary>
		/// <returns></returns>
		public int calculateFreePalletsForPlacement()
		{
			XPQuery<MobileWorkUnit> queryJobTypes = new XPQuery<MobileWorkUnit>(objSpace.Session());
			var listOfMobileWorkUnits = from c in queryJobTypes 
										where 
										c.JobType.idd == scanStateFields.sprJobeType.idd &&
										c.TypeOfMobileState == enTypesOfMobileStates.ДоступенДляРаботы
										select c;
			if (listOfMobileWorkUnits==null)
			{
				return 0;
			}
			return listOfMobileWorkUnits.Count();
		}

		/// <summary>
		/// 1. стартовый модуль
		/// </summary>
		/// <param name="selectedJobType"></param>
		/// <returns></returns>
		public bool runMain(JobTypes selectedJobType)
		{
			logger.Trace("Вход в функцию, selectedJobtype = {0}", selectedJobType);
			scanStateFields = new localClassScanState() 
			{ 
				sprJobeType = selectedJobType, 
				currentScanState = enumScanState.НачалоРаботы 
			};
			// 2. Здесь можно вызвать диалог для сканирования Спец.прихода или требования как фильтра
			// 3. вызов модуля сканирования SN паллета в бесконечном цикле:
			return runMainLoop();
		}
		 
		/// <summary>
		/// вызов диалога ввода серийного номера паллетной этикетки
		/// </summary>
		/// <returns></returns>
		private DialogResult lokInputPalletLabelSerialNumber()
		{
            const string strFuncName = "lokInputPalletLabelSerialNumber:";

			// 1. вызов диалога сканирования паллетной этикетки:
			structScanStringParams paramScan = new structScanStringParams() 
			{ 
				captionOne = "",
                captionTwo = "Сканируйте паллетную этикетку:",
				scanedBarcode = "", 
				successScan = false, 
				captionHelp = getTopCaption()
			};           

			UserSelect userSelect = new UserSelect();
			while (1 == 1)
			{
				logger.Trace("{0} Перед вызовом сканирования СН паллетной этикетки", strFuncName);

				// 1.3 выполнение запроса по классу MobileWorkUnit - расчет свободного количества паллет для размещения:
				int countOfMUW = calculateFreePalletsForPlacement();
				if (countOfMUW == 0)
				{
					paramScan.captionOne = "Нет доступных паллет для размещения!";
					logger.Trace("{0} Нет доступных для размещения паллет!",strFuncName);
					break;
				}

				// 3. вызов диалога сканирования серийного номера, в случае, если это начало работы.
				// Если это размещение неправильно повторно отсканированного SN, то диалог сканирования SN пропускаем.
				if (scanStateFields.currentScanState==enumScanState.НачалоРаботы)
				{
					DialogResult dlgResult = userSelect.scanStringDialog(ref paramScan);
					if (dlgResult != DialogResult.OK)
					{
						logger.Trace("{0} Диалог сканирования СН паллетной этикетки вернул = {1}", strFuncName, dlgResult);
						return dlgResult;
					}
					if (!paramScan.successScan)
					{
						logger.Trace("{0} paramScan.successScan = {0}", strFuncName, paramScan.successScan);
						continue;
					}
					scanStateFields.strScanedPalletBarcode = paramScan.scanedBarcode.Trim();
				}

                logger.Trace("{0} отсканирован СН паллетной этикетки = {1}", strFuncName, scanStateFields.strScanedPalletBarcode);

				string strBarcode = PalletLabels.detectPalletBarcodeFromFullBarcode(scanStateFields.strScanedPalletBarcode);
				logger.Trace("{0} Определенный ШК паллета = {1}",strFuncName, strBarcode);

				// 4. поиск элемента паллетной этикетки по отсканированному ШК:
				XPQuery<PalletLabels> qPalletLabels = new XPQuery<PalletLabels>(objSpace.Session());
				PalletLabels palletLabel = (from c in qPalletLabels where c.Barcode == strBarcode select c).FirstOrDefault();
				if (palletLabel == null)
				{
					paramScan.captionOne = String.Format("Не найден в базе серийный номер: \n{0}", strBarcode);
					logger.Trace("{0} ошибка поиска объекта PalletLabels по ШК: {1}", strFuncName, strBarcode);
					continue;
				}
				scanStateFields.scanedPalletLabel = palletLabel;

				// пока закомментировано, но потом можно будет включить при необходимости.
				//// 4.1 проверка статуса паллетной этикетки - она не должна быть ранее наклеена:
				//if ((palletLabel.PalletLabelStatus == enPalletLabelStatus.ЭтикеткаНаОстатке)
				//    || (palletLabel.PalletLabelStatus == enPalletLabelStatus.ЭтикеткаОтгружена))
				//{
				//    paramScan.captionError = "Паллетная этикетка уже была наклеена!";
				//    logger.Trace("lokInputPalletLabelSerialNumber(). Паллетная этикетка уже была наклеена. ШК: {0}, документ:", strBarcode, scanStateFields.docTrebovanie.DocNo);
				//    continue;
				//}

				// 5. поиск отсканированной этикетки в доступных для размещения строках спец.прихода.
				// Это доступные задания на работу, с типом работы Размещение и конкретной этикеткой.
				XPQuery<MobileWorkDocRows> queryMobileWorkRows = new XPQuery<MobileWorkDocRows>(objSpace.Session());
				XPQuery<DocSpecPrihodaGoods> queryDocSpPrRows = new XPQuery<DocSpecPrihodaGoods>(objSpace.Session());
				var listOfMobileDocRows = from mob in queryMobileWorkRows
									join docRows in queryDocSpPrRows on mob.BaseDocRowId equals docRows.idd
											where
											mob.ParentId.JobType.idd == scanStateFields.sprJobeType.idd &&
											mob.ParentId.TypeOfMobileState == enTypesOfMobileStates.ДоступенДляРаботы &&
											docRows.PalletLabel.idd == palletLabel.idd
											select new {
												docRowidd = docRows.idd,
												mobileUnitidd = mob.ParentId.idd,
												docSpecPrihidd = docRows.ParentID.idd
												};

				// 5.1 нет доступных для размещения строк с этой паллетной этикеткой!                
				if (listOfMobileDocRows.Count() == 0)
				{
					paramScan.captionOne = String.Format("Паллет: {0} не доступен для размещения!", strBarcode);
					logger.Trace("{0} Ошибка поиска доступного для размещения паллета {1} в табличной части подч.Спец.Прихода", strFuncName, scanStateFields.strScanedPalletBarcode);
					continue;
				}
				// 6. сохранение в общей структуре запроса с номерами строк, для обработки в методе сохранения изменений в БД:
				scanStateFields.queryDocRows = from s in listOfMobileDocRows select s.docRowidd;
				scanStateFields.docSpecProhoda = objSpace.Session().FindObject<DocSpecPrihoda>(new BinaryOperator("idd", listOfMobileDocRows.First().docSpecPrihidd, BinaryOperatorType.Equal), true);
				scanStateFields.mobileWorkUnitIdd = listOfMobileDocRows.First().mobileUnitidd;
				break;
			}
			return DialogResult.OK;
		}

		/// <summary>
		/// повторный вызов диалога ввода серийного номера паллетной этикетки
		/// </summary>
		/// <returns></returns>
		private DialogResult lokReEnterInputPalletLabelSerialNumber()
		{
			const string strFuncName = "lokReEnterInputPalletLabelSerialNumber:";

			// 1. вызов диалога сканирования паллетной этикетки:
			structScanStringParams paramScan = new structScanStringParams() 
			{ 
				captionTwo = "Повторно сканируйте паллетную этикетку:", 
				captionOne = "",
				scanedBarcode = "", 
				successScan = false, 
				captionHelp = getTopCaption()
			};        

			UserSelect userSelect = new UserSelect();
			while (1 == 1)
			{
				logger.Trace("{0} Перед вызовом сканирования СН паллетной этикетки", strFuncName);
				 // 3. вызов диалога сканирования серийного номера:
				DialogResult dlgResult = userSelect.scanStringDialog(ref paramScan);
				if (dlgResult != DialogResult.OK)
				{
					logger.Trace("{0} Диалог сканирования СН паллетной этикетки вернул = {1}", strFuncName, dlgResult);
					return dlgResult;
				}
				if (!paramScan.successScan)
				{
					logger.Trace("{0} paramScan.successScan = {0}", strFuncName, paramScan.successScan);
					continue;
				}
				
				logger.Trace("{0} Повторно отсканирован СН паллетной этикетки = {1}", strFuncName, paramScan.scanedBarcode);

				string strBarcode = PalletLabels.detectPalletBarcodeFromFullBarcode(paramScan.scanedBarcode);
				logger.Trace("{0} Определенный ШК паллета = {1}",strFuncName, strBarcode);

				// сравнение серийных номеров:
				if (scanStateFields.strScanedPalletBarcode!=strBarcode)
				{
					logger.Trace("{0} Ошибка: не совпадает повторно отсканированный SN паллета: первое сканирование='{1}', повторное сканирование='{2}'", strFuncName, scanStateFields.strScanedPalletBarcode, strBarcode);

                    paramScan.captionOne = "";
                    paramScan.captionTwo = String.Format("Не верный SN паллета:\n{0}\nВыполнить новое размещение?", strBarcode);
					paramScan.inputMode = enumInputMode.ВопросДаНет;
					dlgResult = userSelect.userMessage(ref paramScan);

					if (dlgResult==DialogResult.OK)
					{
						// здесь мы должны перейти к логике размещения нового паллета, диалог сканирования СН паллета пропускается
						scanStateFields.strScanedPalletBarcode = strBarcode;
						return DialogResult.Abort;
					}                
					// пользователь выбрал "Отмена", мы должны повторно запросить сканирование СН паллета!
					else if (dlgResult == DialogResult.Cancel)
					{
                        paramScan.captionTwo = "Повторно сканируйте паллетную этикетку:";
                        paramScan.captionOne = "";
						continue;
					}
				}
				break;
			}
			return DialogResult.OK;
		}

		/// <summary>
		/// вызов диалога сканирования ШК кода размещения
		/// </summary>
		/// <returns></returns>
		private DialogResult lokScanKodRazmesheniya()
		{
			const string strFuncName = "lokScanKodRazmesheniya:";

			// 1. определение строки - кода размещения ПоложитьВ:
			if (scanStateFields.queryDocRows == null)
			{
				return DialogResult.Cancel;
			}
			structScanStringParams paramScan = new structScanStringParams()
			{
				captionOne = "Сканируйте код размещения",
				captionTwo = "",
				scanedBarcode = "",
				successScan = false,
				captionHelp = getTopCaption() 
			};

			StorageCodes putTo = null;
			XPQuery<DocSpecPrihodaGoods> xpQuery = new XPQuery<DocSpecPrihodaGoods>(objSpace.Session());
			foreach (var item in scanStateFields.queryDocRows)
			{
				putTo = (from s in xpQuery where s.idd==item select s.PutTo).FirstOrDefault<StorageCodes>(); 
				if (putTo==null)
				{
					continue;
				}
				break;
			}

			if (putTo==null)
			{
				paramScan.captionOne = "Ошибка поиска ячейки-получателя!";
				logger.Trace("{0} ошибка определения ячейки-получателя по ШК: {1}", strFuncName, scanStateFields.strScanedPalletBarcode);
				return DialogResult.No;
			}
			// 1.3 установка в заголовке окна кода ячейки-получателя:
			paramScan.captionTwo = String.Format("Ячейка-получатель:\n {0}", putTo.PalletCode);

			// 2. вызов диалога сканирования кода размещения:
			UserSelect userSelect = new UserSelect();
			while (1 == 1)
			{
				logger.Trace("{0} Перед вызовом сканирования кода размещения", strFuncName);
				// 2.1 вызов диалога сканирования серийного номера:
				DialogResult dlgResult = userSelect.scanStringDialog(ref paramScan);
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
				scanStateFields.strScanedKodRazm = paramScan.scanedBarcode;
				logger.Trace("{0} отсканирована строка кода размещения ='{1}'", strFuncName, scanStateFields.strScanedKodRazm);
				List<String> listBarcode = StorageCodes.detectStrStorageCodeFromFullString(scanStateFields.strScanedKodRazm);

				logger.Trace("{0} Вызов модуля определения частей паллета по строке, получилось {1} части", strFuncName, listBarcode.Count);

				// не удалось разобрать строку с кодом ячейки:
				if (listBarcode.Count==0)
				{
					paramScan.captionOne = String.Format("Ошибочный код размещения! \n{0}", paramScan.scanedBarcode);
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
					 paramScan.captionOne = String.Format("Ошибка: не удалось конвертировать '{0}' в номер стеллажа!", stillageCode);
					logger.Trace("{0} Ошибка разбора строки кода размещения. Не удалось конвертировать строку в номер стеллажа. Значение={1}", strFuncName, stillageCode);
					continue;
				}

				// 2.7 попытка определить номер этажа
				if (!Int32.TryParse(floorCode, out floorNumber))
				{
					paramScan.captionOne = String.Format("Ошибка: не удалось конвертировать '{0}' в номер этажа!", floorCode);
					logger.Trace("{0} Ошибка разбора строки кода размещения. Не удалось конвертировать строку в номер этажа. Значение={1}", strFuncName, floorCode);
					continue;
				}

				 // 2.8 попытка определить номер ячейки
				if (!Int32.TryParse(cellCode, out cellNumber))
				{
					paramScan.captionOne = String.Format("Ошибка: не удалось конвертировать '{0}' в номер ячейки!", cellCode);
					logger.Trace("{0} Ошибка разбора строки кода размещения. Не удалось конвертировать строку в номер ячейки. Значение={1}", strFuncName, cellCode);
					continue;
				}

				// 2.9 сравнение номера стеллажа с номером стеллажа в документе:
				if (putTo.Stillage!=stillageNumber)
				{
					paramScan.captionOne = String.Format("Ошибочный стеллаж!\nОтсканирован:{0}\nНужен:{1}", stillageNumber, putTo.Stillage);
					logger.Trace("{0} Ошибочный стеллаж. Отсканирован:{1} Нужен:{2}",strFuncName, stillageNumber, putTo.Stillage);
					continue;
				}

                // 2.9 сравнение номера этажа с номером этажа в документе:
                if (putTo.Floor != floorNumber)
                {
                    paramScan.captionOne = String.Format("Ошибочный этаж!\nОтсканирован:{0}\nНужен:{1}", floorNumber, putTo.Floor);
                    logger.Trace("{0}: Ошибочный этаж! Отсканирован:{1} Нужен:{2}", strFuncName, floorNumber, putTo.Floor);
                    continue;
                }

				// 2.10 сравнение номера ячейки с номером ячейки в документе:
				if (putTo.Cell != cellNumber)
				{
					paramScan.captionOne = String.Format("Ошибочная ячейка!\nОтсканирована:{0}\nНужна:{1}", cellNumber, putTo.Cell);
					logger.Trace("{0} Ошибочная ячейка! Отсканирована:{1} Нужна:{2}", strFuncName, cellNumber, putTo.Cell);
					continue;
				}
				
				// 2.11 сохранение в общей структуре кода размещения из документа, по которому выполнялось сравнение:
				scanStateFields.scanedStorageCode = putTo;
				scanStateFields.dTimeScanStorageCode = DateTime.Now;
				break;
			}
			return DialogResult.OK;
		}

		/// <summary>
		/// Вызов диалога сканирования SN паллета в бесконечном цикле.
		/// </summary>
		/// <returns></returns>
		private bool runMainLoop()
		{
			while (true)
			{
				// 1.1 вызов диалог сканирования СН паллетной этикетки:
				#region СканированиеСНПаллетнойЭтикетки
				if ((scanStateFields.currentScanState == enumScanState.НачалоРаботы)
					||(scanStateFields.currentScanState == enumScanState.НужноРазместитьПовторныйSN))
				{                   
					// 1.1.1 если это начальное сканирование этикетки, запомним время начала работы:
					if (scanStateFields.currentScanState == enumScanState.НачалоРаботы)
					{
						scanStateFields.dTimeStartOperation = DateTime.Now;
					}

					DialogResult dlgResult = lokInputPalletLabelSerialNumber();
					if (dlgResult != DialogResult.OK)
					{
						logger.Trace("runMainLoop: Диалог сканирования SN паллетной этикетки вернул: {0}", dlgResult);
						return false;
					}
					scanStateFields.currentScanState = enumScanState.ОтсканированСерийныйНомер;
				}
				#endregion

				#region Сканирование ячейки-получателя
				if (scanStateFields.currentScanState == enumScanState.ОтсканированСерийныйНомер)
				{
					DialogResult dlgResult = lokScanKodRazmesheniya();
					if (dlgResult != DialogResult.OK)
					{
						logger.Trace("runMainLoop: Диалог сканирования ячейки-получателя вернул: {0}", dlgResult);
						scanStateFields.refreshAllField();// сброс всех значений и начать с начала
						continue;
					}
					scanStateFields.currentScanState = enumScanState.ОтсканированаЯчейкаПолучатль;
				}
				#endregion

				// 1.3 повторный вызов диалог сканирования СН паллетной этикетки:
				#region ПовторноеСканированиеСНПаллетнойЭтикетки
				if (scanStateFields.currentScanState == enumScanState.ОтсканированаЯчейкаПолучатль)
				{
					DialogResult dlgResult = lokReEnterInputPalletLabelSerialNumber();
					if (dlgResult == DialogResult.Abort)
					{
                        // пользователь отсканировал неверный ШК паллета и отказался повторно его размещать
                        scanStateFields.currentScanState = enumScanState.НачалоРаботы;
						logger.Trace("runMainLoop: Диалог повторного сканирования SN паллетной этикетки вернул: {0}", dlgResult);
						continue;
					}
                    else if (dlgResult == DialogResult.OK)
                    {

                    }
					scanStateFields.currentScanState = enumScanState.ПовторноОтсканированСерийныйНомер;

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

		// метод выполняет сохранение в базе всех результатов сканирования
		private bool lokSaveAllDateToDataBase()
		{
			const string strFuncName = "lokSaveAllDateToDataBase:";
			logger.Trace("Вход в метод {0}. Размещение прихода по документу: {1}", strFuncName, scanStateFields.docSpecProhoda);

			//IQueryable<int> rowsListOfDoc = scanStateFields.queryDocRows;
			//String strUserMessage = "";
			if (scanStateFields.queryDocRows == null)
			{                
				logger.Error("{0} Размещение прихода по документу {1}. Ошибка: нет строк документа для сохранения изменений.queryDocRows=null", strFuncName, scanStateFields.docSpecProhoda);
				showUserMessage("Ошибка: нет строк спец.прихода для сохранения изменений");
				return false;
			}

			// 3.1 установка разделителя по умолчанию для текущего пользователя
			Users current_User = (Users)SecuritySystem.CurrentUser;
			Delimeters delimeter = objSpace.Session().FindObject<Delimeters>(new BinaryOperator("DelimeterID", current_User.DefaultDelimeter.DelimeterID, BinaryOperatorType.Equal), true);

			// 3.2 определение текущего пользователя:
			Users currentUser = objSpace.Session().FindObject<Users>(new BinaryOperator("idd", current_User.idd, BinaryOperatorType.Equal));

			// 3.4 передача в обмен с СУТЗ 7.7 выполненной единицы работы Размещение товара с идентификаторами выполненных строк Спец.прихода:
			XPQuery<JobTypes> queryJobTypes = new XPQuery<JobTypes>(objSpace.Session());
			JobTypes elementJobType = (from c in queryJobTypes where c.TypeOfWork == enTypeOfWorks.РазмещениеПрихода select c).FirstOrDefault<JobTypes>();

			// 3.5 проверка найденных элементов на пустое значение:
			if (currentUser == null)
			{
				logger.Error("{0} Размещение прихода по документу {1}. Ошибка определения текущего пользователя. currentUser=null", strFuncName, scanStateFields.docSpecProhoda);
				showUserMessage("Ошибка определения текущего пользователя!");
				return false;
			}
			if (delimeter == null)
			{
				logger.Error("{0} Размещение прихода по документу: {1}. Ошибка определения текущего разделителя учета. delimeter=null", strFuncName, scanStateFields.docSpecProhoda);
				showUserMessage("Ошибка определения текущего разделителя учета!");
				return false;
			}
			if (elementJobType == null)
			{
				logger.Error("{0} Размещение прихода по документу: {1}. Ошибка определения текущего вида работы. elementJobType=null", strFuncName, scanStateFields.docSpecProhoda);
				showUserMessage("Ошибка определения текущего вида работы!");
				return false;
			}
			if (scanStateFields.docSpecProhoda == null)
			{
				logger.Error("{0} Размещение прихода по документу: {1}. Ошибка поиска по идд документа спец.прихода. currentDocSpecPrih=null", strFuncName, scanStateFields.docSpecProhoda);
				showUserMessage("Ошибка определения текущей спец.прихода!");
				return false;
			}

			// 3.6.1 запись основной единицы работы:
			if (scanStateFields.mobileWorkUnitIdd == 0)
			{
				logger.Error("{0} Размещение прихода по документу: {1}. idd единицы работы = 0. Не возможно выполнить сохранение результатов работы!", strFuncName, scanStateFields.docSpecProhoda);
				showUserMessage("Ошибка определения единицы работы!");
				return false;
			}

			MobileWorkUnit mobileUnit = objSpace.Session().FindObject<MobileWorkUnit>(new BinaryOperator("idd", scanStateFields.mobileWorkUnitIdd, BinaryOperatorType.Equal));
			if (mobileUnit != null)
			{ 
				mobileUnit.TypeOfMobileState = enTypesOfMobileStates.Выполнен;
				mobileUnit.TypeOfUnitWork = enTypesOfUnitWork.Выполнен;
                mobileUnit.Barcode = scanStateFields.strScanedPalletBarcode;
			};
			mobileUnit.Save();

			// 3.6.2 добавление сотрудника и времени выполнения операции:
			MobileWorkUsers newUserWorkRecord = new MobileWorkUsers(objSpace.Session()) 
			{ 
				Delimeter = delimeter, 
				User = currentUser, 
				ErrorState = null, 
				dTimeStart = scanStateFields.dTimeStartOperation, 
				dTimeEnd = DateTime.Now, 
				dTimeClosedTo = scanStateFields.dTimeScanStorageCode,
			};
			newUserWorkRecord.Save();
			mobileUnit.MobileWorkUsersCollection.Add(newUserWorkRecord);

			// 3.6.3 добавление в задание на выгрузку строк документа, по которым было выполнено размещение прихода:
            NET_SUTZ_Unloads unloadDoc = new NET_SUTZ_Unloads(objSpace.Session()) 
            { ObjectType = typeof(DocSpecPrihoda).FullName, 
                ObjectRow_Id = scanStateFields.docSpecProhoda.idd, 
                Outload = 0, 
                User = currentUser, 
                dTimeInsertRecord = DateTime.Now, 
                dTimeUnloadRecord = null, 
                FullDocTable = false 
            };
            unloadDoc.Save();

            foreach (int item in scanStateFields.queryDocRows)
            {
                NET_SUTZ_DocRows unloadRow = new NET_SUTZ_DocRows(objSpace.Session())
                {
                    ObjectDocTableId = item,
                    BitFlagsOfOuboundFields = enFlagsOfOuboundFields.СтатусПроведения|enFlagsOfOuboundFields.Количество
                };
                unloadRow.Save();
                unloadDoc.NET_SUTZ_DocRows.Add(unloadRow);
            }

            // 3.6.4 установка у строки документа спец.прихода нового статуса проведения:
			XPQuery<DocSpecPrihodaGoods> querySPPrihodaGoods = new XPQuery<DocSpecPrihodaGoods>(objSpace.Session());
            var listOfDocRows = from c in querySPPrihodaGoods where scanStateFields.queryDocRows.Contains(c.idd) select c;

			foreach (DocSpecPrihodaGoods item in listOfDocRows)
			{
			    item.MoveStatus = enDocRowMoveStatus.ЧП_ПоложитьВ;
			    item.Save();			
			}

			// 3.6.5 запись нового номера паллета кода размещения паллетной этикетки.
			PalletLabels currentPL = objSpace.Session().FindObject<PalletLabels>(new BinaryOperator("idd", scanStateFields.scanedPalletLabel.idd, BinaryOperatorType.Equal));
			if ((currentPL != null) && (scanStateFields.scanedStorageCode != null))
			{
				currentPL.PalletCode = scanStateFields.scanedStorageCode.PalletCode;
				currentPL.Save();
			}

			// 3.7 нужно выполнить сохранение записанных элементов, чтобы их идд записать в таблицу выгрузки из СУТЗ2:
			if (!lokRunCommitChanges())
			{
				return false;
			}

			// 3.8.1 добавление единицы работы в задание на выгрузку в 7.7
			NET_SUTZ_Unloads unloads = new NET_SUTZ_Unloads(objSpace.Session());
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
            XPQuery<DocSpecPrihodaGoods> querySPPrihodaGoodsNotComplete = new XPQuery<DocSpecPrihodaGoods>(objSpace.Session());
            var listOfNotCompletedDocRows = from c in querySPPrihodaGoodsNotComplete 
                                where 
                                c.ParentID.idd==scanStateFields.docSpecProhoda.idd 
                                &&  c.MoveStatus != enDocRowMoveStatus.ЧП_ПоложитьВ
                                select c;

            if ((listOfNotCompletedDocRows==null)||(listOfNotCompletedDocRows.Count()==0))
            {
                //objSpace.Session().FindObject<DocSpecPrihoda>(new BinaryOperator("idd", listOfMobileDocRows.First().docSpecPrihidd, BinaryOperatorType.Equal), true);
                // установка в документе признака "РазрешитьДвижение"
                scanStateFields.docSpecProhoda.AllowMovement = true;
                scanStateFields.docSpecProhoda.Save();

                // изменение статуса работы документа
                XPQuery<DocJobesProperties> djp = new XPQuery<DocJobesProperties>(objSpace.Session());
                var djProp = (from c in djp 
                           where 
                           c.BaseDoc.idd==scanStateFields.docSpecProhoda.idd
                           && c.JobType.idd == scanStateFields.sprJobeType.idd
                           select c).FirstOrDefault<DocJobesProperties>();
                if (djProp is DocJobesProperties)
                {
                    djProp.TypeOfMobileState = enTypesOfMobileStates.Выполнен;
                    djProp.Save();
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
				objSpace.Session().CommitTransaction();
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
				strError = "Ошибка Exception. " + scanStateFields.docTrebovanie.DocNo;
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
        internal class localClassScanState
        {
            // 1. текущий вид работы
            internal JobTypes sprJobeType { get; set; }
            // 2. документ требование
            internal BaseDocument docTrebovanie { get; set; }
            // 3. текущий документ спец.прихода, по которому выполняется размещение
            internal DocSpecPrihoda docSpecProhoda { get; set; }

            //4. найденные строки документа по SN:
            // сохраняет строки спец.прихода между запросом сканирования ШК этикетки и методом сохраненения изменения в БД
            internal IQueryable<int> queryDocRows { get; set; }

            // 5. текущий этап работы, перечисление
            internal enumScanState currentScanState { get; set; }

            // 6. текстовое представление текущего этапа работы
            internal String currentWorkInfo { get; set; }

            // 7. поля для работы логистического блока:
            internal DateTime dTimeStartOperation { get; set; }
            internal DateTime? dTimeScanStorageCode { get; set; }

            // 8. строка, отсканированный ШК паллета 
            internal string strScanedPalletBarcode { get; set; }

            // 9. строка, найденная паллетная этикетка по баркоду
            internal PalletLabels scanedPalletLabel { get; set; }

            // 10. строка, отсканированный ШК кода размещения 
            internal string strScanedKodRazm { get; set; }

            // 11. строка, найденная паллетная этикетка по баркоду
            internal StorageCodes scanedStorageCode { get; set; }

            // 12. элемент справочника ЗаданияСклада, по которому выполняется размещение товара:
            internal int mobileWorkUnitIdd { get; set; }

            // метод сбрасывает все значения полей при новом сеансе сканирования
            public void refreshAllField()
            {
                this.docSpecProhoda = null;
                this.queryDocRows = null;
                this.currentScanState = enumScanState.НачалоРаботы;
                this.currentWorkInfo = "";
                this.dTimeStartOperation = DateTime.Now;
                this.strScanedPalletBarcode = "";
                this.scanedPalletLabel = null;
                this.strScanedKodRazm = "";
                this.scanedStorageCode = null;
                this.mobileWorkUnitIdd = 0;
                this.dTimeScanStorageCode = null;
            }
        }

        /// <summary>
        /// локальное перечисление для сохранения этапов выполнения модуля
        /// </summary>
        internal enum enumScanState : byte
        {
            НачалоРаботы = 1,
            ОтсканированСерийныйНомер = 2,
            ОтсканированаЯчейкаПолучатль = 3,
            ПовторноОтсканированСерийныйНомер = 4,
            НужноРазместитьПовторныйSN = 5
        }

	}

}
