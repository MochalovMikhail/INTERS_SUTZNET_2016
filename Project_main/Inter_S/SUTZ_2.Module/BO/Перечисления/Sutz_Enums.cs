﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;

namespace SUTZ_2.Module.BO
{
    // 1. Перечисление, определяющее доступные варианты мониторов пользователей
    // Пример: http://documentation.devexpress.com/#Xaf/CustomDocument2825
    public enum enMonitorResolution : byte
    {
        [ImageName("BO_Position")]
        [XafDisplayName("Обычный монитор")]
        ByDefault = 1,
        [XafDisplayName("Touch panel 800x600")]
        TouchPanel_800x600,
        [XafDisplayName("Symbol MS9000")]
        Symbol_MS900
    }

    // 2. Перечисление, определяющее возможные виды работ в СУТЗ:
    public enum enTypeOfWorks : byte
    {
        [XafDisplayName("Не определен")]
        НеВыбран = 0,
        [XafDisplayName("Разгрузка машины при приеме")]
        ПриемРазгрузка = 1,
        [XafDisplayName("Перемещение при приеме на склад")]
        ПриемПеремещение = 2,
        [XafDisplayName("Сортировка при приеме")]
        ПриемСортировка = 3,
        [XafDisplayName("Маркировка при приеме")]
        ПриемМаркировка = 4,
        [XafDisplayName("Сканирование при приеме на склад")]
        ПриемСканирование = 5,
        [XafDisplayName("Размещение прихода")]
        РазмещениеПрихода = 6,
        [XafDisplayName("Перемещение товара")]
        Перемещение = 7,
        [XafDisplayName("Отбор товара")]
        ОтборТовара = 8,
        [XafDisplayName("Сканирование СН при отгрузке клиенту")]
        СканСНПривязкаКРНК = 16,
        [XafDisplayName("Отбор в точку передачи")]
        ОтборВТочкуПередачи = 17,
        [XafDisplayName("Отбор из точки передачи")]
        ОтборИзТочкиПередачи = 18,
        //[XafDisplayName("Перемещение товара")]
        //Размещение = 20,
        [XafDisplayName("Оптимизация товара")]
        ОптимизацияТовара = 21,
        [XafDisplayName("Перемещение в точку передачи")]
        ПеремещениеВТочкуПередачи = 22,
        [XafDisplayName("Перемещение из точки передачи")]
        ПеремещениеИзТочкиПередачи = 23,
        //[XafDisplayName("Отбор товара")]
        //ОтборТовара = 30,
        [XafDisplayName("Отгрузка товара")]
        ОтгрузкаТовара = 40,
        [XafDisplayName("Отгрузка сборки")]
        ОтгрузкаСборки = 41,
        [XafDisplayName("Инвентаризация")]
        Инвентаризация = 60
    }

    // 2. Перечисление, определяющее состояние строки документа моб.сутз:
    public enum enTypesOfMobileStates : byte
    {
        [XafDisplayName("Не определен")]
        НеОпределен = 0,
        [XafDisplayName("Не доступен для работы")]
        НеДоступенДляРаботы = 1,
        [XafDisplayName("Доступен для работы")]
        ДоступенДляРаботы = 2,
        [XafDisplayName("В процессе отправки")]
        ВПроцессеОтправки = 3,
        [XafDisplayName("В работе")]
        ВРаботе = 4,
        [XafDisplayName("Выполнен")]
        Выполнен = 5,
        [XafDisplayName("В процессе отзыва")]
        ВПроцессеОтзыва = 6,
        [XafDisplayName("Отозван из работы")]
        ОтозванИзРаботы = 7
        //[XafDisplayName("Выполнен с ошибками")]
        //ВыполненСОшибками = 8,
        //[XafDisplayName("Ошибка проведения")]
        //ОшибкаВыполнения = 9
    }

    // 2. Перечисление, определяющее состояние выполнения операций внутри единицы работы:
    public enum enTypesOfUnitWork : byte
    {
        [XafDisplayName("Не определен")]
        НеОпределен = 0,
        [XafDisplayName("Начало работы")]
        НачалоРаботы = 1,
        [XafDisplayName("Отсканирован ВзятьИз")]
        ОтсканированВзятьИз = 2,
        [XafDisplayName("Отсканирован ПоложитьВ")]
        ОтсканированПоложитьВ = 3,
        [XafDisplayName("Выполнен")]
        Выполнен = 9
    }


    // 3. Перечисление, определяющее 
    public enum enTypeOfStockRooms : byte
    {
        [XafDisplayName("Не определен")]
        НеВыбран = 0,
        [XafDisplayName("Обычный запас")]
        ОбычныйЗапас = 1,
        [XafDisplayName("Обычный отбор")]
        ОбычныйОтбор = 2,
        [XafDisplayName("Множественный отбор")]
        МножественныйОтбор = 3,
        [XafDisplayName("Множественный запас")]
        МножественныйЗапас = 4,
        [XafDisplayName("Проходные стеллажи")]
        ПроходныеСтеллажи = 5,
        [XafDisplayName("Сырьевой склад")]
        СырьевойСклад = 6,
        [XafDisplayName("Отгрузка")]
        КладоваяОтгрузки = 7,
        [XafDisplayName("Гравитация запас")]
        ГравитацияЗапас = 8,
        [XafDisplayName("Гравитация отбор")]
        ГравитацияОтбор = 9,
        [XafDisplayName("Буферная кладовая")]
        БуфернаяКладовая = 10,
        [XafDisplayName("Мезонин")]
        Мезонин = 11
    }

    // 4. Перечисление, определяющее возможные состояния кода размещения:
    public enum enCellStates : byte
    {
        [XafDisplayName("Используется")]
        Используется = 1,
        [XafDisplayName("Не используется")]
        НеИспользуется
    }

    // 5. Перечисление статусов документов:
    public enum DocStates : byte
    {
        [XafDisplayName("Помечен на удаление")]
        ПометкаУдаления = 1,
        [XafDisplayName("Записан")]
        Записан = 2,
        [XafDisplayName("Проведен")]
        Проведен = 3
    }

    // 6. Перечисление - состояние расходного ордера:
    public enum DocStatesASUInvoice : byte
    {
        [XafDisplayName("Не выбран")]
        НеВыбран = 0,
        [XafDisplayName("Зарезервировано")]
        Зарезервировано = 1,
        [XafDisplayName("Заявка на склад")]
        ЗаявкаНаСклад = 2,
        [XafDisplayName("Ответ менеджеру")]
        ОтветМенеджеру = 3,
        [XafDisplayName("Заявка на отбор")]
        ЗаявкаНаОтбор = 4,
        [XafDisplayName("Несоответствие")]
        Несоответствие = 5,
        [XafDisplayName("Товар отобран")]
        ТоварОтобран = 6,
        [XafDisplayName("Товар отгружен")]
        ТоварОтгружен = 7,
        [XafDisplayName("Приостановить")]
        Приостановить = 8
    }

    // 7.
    public enum enVariantsOfSelectedForm : byte
    {
        [XafDisplayName("Выбор вида работы")]
        selectJobTypes = 1
    }

    // 8. соответствует перечислению СУТЗ ВидыДоставок
    public enum enDeliveryTypes : byte
    {
        [XafDisplayName("Не выбран")]
        НеВыбран = 0,
        [XafDisplayName("Холодный вагон")]
        ХолодныйВагон = 1,
        [XafDisplayName("Теплый вагон")]
        ТеплыйВагон = 2,
        [XafDisplayName("Авиа")]
        Авиа = 3,
        [XafDisplayName("Рефрижератор")]
        Рефрижератор = 4,
        [XafDisplayName("РЖД")]
        РЖД = 5,
    }

    // 9. Перечисление - варианты состояний паллетной этикетки
    public enum enPalletLabelStatus: byte
    {
        [XafDisplayName("Не выбран")]
        НеВыбран = 0,
        [XafDisplayName("Создан")]
        ЭтикеткаСоздана = 10,
        [XafDisplayName("На остатке")]
        ЭтикеткаНаОстатке = 20,
        [XafDisplayName("Отгружена")]
        ЭтикеткаОтгружена = 30,
    }

    // 10. Перечисление - состояние расходного ордера:
    public enum enDocStatesInventory : byte
    {
        [XafDisplayName("Не выбран")]
        НеВыбран = 0,
        [XafDisplayName("Плановая инвентаризация")]
        ПлановаяИнвентаризация = 1,
        [XafDisplayName("Текущая инвентаризия")]
        ТекущаяИнвентаризация= 2
    }

    // 11. Перечисление - виды пиктограмм документа:
    public enum enDocPropertyStatus : byte
    {
        [XafDisplayName("Не выбран")]
        НеВыбран = 0,
        [ImageName("RedFlag")]
        [XafDisplayName("Выполнен")]
        Выполнен = 1,// красный флажок
        [XafDisplayName("Не выполнен")]// синий минус
        НеВыполнен = 2,
        [XafDisplayName("Ошибка")]// желтый круг с красной полосой
        Ошибка = 3,
        [XafDisplayName("Частично выполнен")]// синий флажок
        ЧастичноВыполнен = 4
        //[XafDisplayName("Ошибка")]// желтый круг с красной полосой
        //НеВыполнен = 5,
        //[XafDisplayName("Ошибка")]// желтый круг с красной полосой
        //НеВыполнен = 3,
        //[XafDisplayName("Ошибка")]// желтый круг с красной полосой
        //НеВыполнен = 3,
        //[XafDisplayName("Ошибка")]// желтый круг с красной полосой
        //НеВыполнен = 3,
        //[XafDisplayName("Ошибка")]// желтый круг с красной полосой
        //НеВыполнен = 3,
    }

    // 12. Перечисление - виды документов.
    public enum enDocumentType : byte
    {
        [XafDisplayName("Акт несоответствия")]
        АктНесоответствия = 1,
        [XafDisplayName("Акт приема")]
        АктПриема = 5,
        [XafDisplayName("Буфер изменений")]
        БуферИзменений = 10,
        [XafDisplayName("Возврат пересортицы")]
        ВозвратПересортицы = 15,
        [XafDisplayName("Возврат от покупателя")]
        ВозвратОтПокупателя = 20,
        [XafDisplayName("Возврат поставщику")]
        ВозвратПоставщику = 25,
        [XafDisplayName("Заявка на пересчет")]
        ЗаявкаНаПересчет = 30,
        [XafDisplayName("Заявка покупателя")]
        ЗаявкаПокупателя = 35,
        [XafDisplayName("Заказ поставщику")]
        ЗаказПоставщику = 40,
        [XafDisplayName("Инвентаризация")]
        ПересчетСклада = 45,
        [XafDisplayName("Маршрутный лист")]
        МаршрутныйЛист = 50, 
        [XafDisplayName("Операции смены")]
        ОперацииСмены = 55,
        [XafDisplayName("Оприходование пересортицы")]
        ОприходованиеПересортицы = 60,
        [XafDisplayName("Отборочный лист")]
        СпецификацияРасхода = 65,
        [XafDisplayName("Перемещение товаров")]
        ПеремещениеТоваров = 70,
        [XafDisplayName("Приходная накладная")]
        ПриходнаяНакладная = 75,
        [XafDisplayName("Расходная накладная")]
        РасходнаяНакладная = 80,
        [XafDisplayName("Списание пересортицы")]
        СписаниеПересортицы = 85,
        [XafDisplayName("Спецификация прихода")]
        СпецификацияПрихода = 90,
        [XafDisplayName("Спецификация перемещения")]
        СпецификацияПеремещения = 95,
        [XafDisplayName("Транспортный лист")]
        ТранспортныйЛист = 100,
        [XafDisplayName("Требование на разгрузку")]
        ТребованиеНаРазгрузку = 105,
        [XafDisplayName("Управленческий остаток")]
        УправленческийОстаток = 110        
    }

    // 13. Перечисление - статусы частичного проведения документа
    public enum enDocRowMoveStatus : byte
    {
        [XafDisplayName("Не выбран")]
        НеВыбран = 0,
        [XafDisplayName("чп отправитель")]
        ЧП_ВзятьИз = 1,// частичное проведение по отправителю
        [XafDisplayName("тчк передачи ПоложитьВ ")]
        ЧП_ТЧ_ПоложитьВ = 2,// частичное проведение по точке передачи-получателю
        [XafDisplayName("тчк передачи ВзятьИз")]
        ЧП_ТЧ_ВзятьИз = 3,// частичное проведение по точке передачи-отправителю
        [XafDisplayName("чп получатель")]
        ЧП_ПоложитьВ = 4
    }

    // 14. Набор битовых флагов, для определения полей, которые должна выгрузить обработка выгрузки документа
    // из NET в СУТЗ:
    [Flags]
    public enum enFlagsOfOuboundFields : int
    {
        НеВыбран = 0x0000,
        Товар = 0x0001,
        ЕдиницаИзмерения = 0x0002,
        СрокГодности = 0x0004,
        ВзятьИз = 0x0010,
        ПоложитьВ = 0x0020,
        Характеристика = 0x0040,
        КоличествоКор = 0x0080,
        КоличествоШт = 0x0100,
        Количество = 0x0200,
        ПаллетнаяЭтикетка = 0x0400,
        СтатусПроведения = 0x1000,
        ТочкаПередачи = 0x2000,
        КоличествоФакт = 0x4000
    }
}
