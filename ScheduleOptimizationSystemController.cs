using ScheduleOptimizationSystem.HierarchicalGameModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScheduleOptimizationSystem
{
    internal class ScheduleOptimizationSystemController(IView view)
    {

        private readonly IView _view = view;
        private IScheduleOptimizationModel? _model;

        private readonly FileLogger _logger = new();

        /// <summary>
        /// Иницирует построение нового расписания
        /// </summary>
        /// <param name="config">Конфигурация конвейерной системы и решаемой задачи</param>
        /// <param name="modelType">Тип используемой модели</param>
        /// <returns></returns>
        public async Task BuildScheduleAsync(ScheduleConfig config, ModelTypes modelType)
        {
            CreateModel(modelType);
            if (_model is null) return;

            if (_view.LoggingOn)
                _logger.SetLogFile(GetLogFileName(modelType));

            _view.SetBuildButtonEnabled(false);
            _view.SetResultLinkVisible(false);

            try
            {
                bool success = await _model.BuildScheduleAsync(config);
                if (success)
                {
                    _view.ShowNotification("Расписание успешно построено!");
                    _view.SetResultLinkVisible(true);
                }
                else
                {
                    _view.ShowNotification("Построить расписание не удалось.");
                }
            }
            catch (Exception ex)
            {
                _view.ShowNotification($"Ошибка: {ex.Message}");
            }
            finally
            {
                _view.SetBuildButtonEnabled(true);
            }

            _logger.Dispose();
        }

        /// <summary>
        /// Отображение результатов построения расписания
        /// </summary>
        public void ShowSchedule()
        {
            if (_model == null) return;

            var schedule = _model.GetSchedule();
            _view.DisplaySchedule(schedule);

        }

        private void CreateModel(ModelTypes modelType)
        {
            switch (modelType)
            {
                case ModelTypes.HIERARCHICAL:
                    {
                        _model = new HierarchicalGameScheduleOptimizationModel(_logger);
                        break;
                    }
                case ModelTypes.SIMPLE:
                    {
                        _model = new SimpleScheduleOptimizationModel();
                        break;
                    }
            }
        }

        private string GetLogFileName(ModelTypes modelType)
        {
            string fileNamePrefix = string.Empty;
            switch (modelType)
            {
                case ModelTypes.HIERARCHICAL:
                    {
                        fileNamePrefix = "HierarchicalModel_";
                        break;
                    }
                case ModelTypes.SIMPLE:
                    {
                        fileNamePrefix = "Heuristic_";
                        break;
                    }
            }
            string fileNameSuffix = $"{DateTime.Now.Day}_{DateTime.Now.Month}_{DateTime.Now.Year}_{DateTime.Now.Hour}_{DateTime.Now.Minute}_{DateTime.Now.Second}.log";

            return fileNamePrefix + fileNameSuffix;
        }
    }
}
