using System;
using System.Collections.Generic;
using System.Text;

namespace ScheduleOptimizationSystem
{
    internal interface IView
    {
        /// <summary>
        /// Флаг необходимости осуществлять логирование
        /// </summary>
        bool LoggingOn {  get; }

        /// <summary>
        /// Установка доступности кнопки инициации построения расписания
        /// </summary>
        /// <param name="enabled">Устанавливаемая доступность</param>
        void SetBuildButtonEnabled(bool enabled);

        /// <summary>
        /// Отображение всплывающего окна с сообщением
        /// </summary>
        /// <param name="message">Отображаемое сообщение</param>
        void ShowNotification(string message);

        /// <summary>
        /// Отображение построенного расписания
        /// </summary>
        /// <param name="schedule">Расписание</param>
        void DisplaySchedule(ScheduleDto schedule);

        /// <summary>
        /// Установка видимости гиперссылки отображения построенного расписания
        /// </summary>
        /// <param name="visible">Флаг видимости</param>
        void SetResultLinkVisible(bool visible);
    }
}
