using System;
using System.Collections.Generic;
using System.Text;

namespace ScheduleOptimizationSystem.HierarchicalGameModel
{
    internal class FirstLevel
    {
        private readonly ScheduleConfig _config;
        private readonly SecondLevel _secondLevel;

        private readonly ILogger _logger;

        /// <summary>
        /// Данная переменная определяет вектор данных для интерпритации типов данных
        /// </summary>
        private bool[] _i = [];

        /// <summary>
        /// Лучшая матрица составов партий
        /// </summary>
        private List<List<int>> bestMatrixA = [];

        /// <summary>
        /// Матрица составов партий требований на k+1 шаге
        /// </summary>
        private List<List<List<int>>> _a1 = [];

        /// <summary>
        /// Матрица составов партий требований фиксированного типа
        /// [dataTypesCount x ??? x ???] 
        /// </summary>
        private List<List<List<int>>> _a2 = [];

        /// <summary>
        /// Аналог матрицы A - A'
        /// Матрица составов партий требований на k шаге.
        /// matrixA_Prime[i][h], где i - это тип данных. h - это индекс партии, а значение по индексам это количество партий
        /// </summary>
        private List<List<int>> PrimeMatrixA = [];

        private bool isBestSolution = true;

        private ScheduleBuildingResult optimalSolution;

        public FirstLevel(ScheduleConfig config, ILogger logger)
        {
            _config = config;
            _logger = logger;
            _secondLevel = new(config, logger);
            optimalSolution = new ScheduleBuildingResult(false, int.MaxValue, null, null);
        }

        public ScheduleBuildingResult BuildSchedule()
        {

            GenerateStartSolution();

            PullSolution(PrimeMatrixA);

            int optimizationIteration = 1;

            while (CheckType())
            {
                _logger.Print($"_-_-_-_\nИтерация оптимизации №{optimizationIteration++}\n_-_-_-_");
                _logger.Print("Оптимизируемое решение:", PrimeMatrixA);

                if (isBestSolution)
                {
                    _a1 = [];

                    for (var jobType = 0; jobType < _config.JobTypesCount; jobType++)
                    {
                        _a1.Add([]);
                        _a1[jobType].Add([]);
                        _a1[jobType][0] = [.. PrimeMatrixA[jobType]];
                    }
                    isBestSolution = false;
                }

                // Для каждого типа и каждого решения в типе строим новое решение и проверяем его на критерий
                // Строим A2 и параллельно проверяем критерий
                _a2 = new List<List<List<int>>>(_config.JobTypesCount);
                _a2.AddRange(Enumerable.Repeat(new List<List<int>>(), _config.JobTypesCount));


                for (var jobType = 0; jobType < _config.JobTypesCount; jobType++)
                {

                    if (!_i[jobType]) continue;

                    // Формируем новый состав партий для типа jobType
                    _a2[jobType] = NewData(jobType);

                    // Для каждого пакета в новом составе партий выполняем обработку
                    for (var batch = 0; batch < _a2[jobType].Count; batch++)
                    {
                        List<List<int>> tempA = SetTempAFromA2(jobType, batch);
                        PullSolution(tempA);
                    }
                }
                if (!isBestSolution)
                {
                    CombinationType(0, []);
                }

                if (isBestSolution)
                {
                    PrimeMatrixA = [.. bestMatrixA];
                    _logger.Print("На итерации получено новое оптимальное решение:", PrimeMatrixA);
                    continue;
                }

                _logger.Print("На итерации не найдено лучшее решение");

                for (int jobType = 0; jobType < _config.JobTypesCount; jobType++)
                {
                    _a1[jobType] = [.. _a2[jobType]];
                    if (_a1[jobType].Count == 0 || _a1[jobType][0].Count == 0)
                        _i[jobType] = false;
                }
            }

            return optimalSolution;
        }

        private void GenerateStartSolution()
        {
            const int minBatchSize = 2;

            int[] batchCountList = new int[_config.JobTypesCount];
            for (int orderType = 0; orderType < _config.OrderTypesCount; orderType++)
            {
                int currentTypeOrdersCount = _config.Orders[orderType];
                for (int jobType = 0; jobType < _config.JobTypesCount; jobType++)
                {
                    batchCountList[jobType] += currentTypeOrdersCount * _config.OrderTypes[orderType, jobType];
                }

            }

            _i = new bool[_config.JobTypesCount];
            PrimeMatrixA = new List<List<int>>(_config.JobTypesCount);

            for (var jobType = 0; jobType < _config.JobTypesCount; jobType++)
            {
                _i[jobType] = true;

                // Для каждого типа создаём вектор с составом партий и формируем его, как [n_p - 2, 2]
                PrimeMatrixA.Add([]);
                PrimeMatrixA[jobType].Add(batchCountList[jobType] - minBatchSize);
                PrimeMatrixA[jobType].Add(minBatchSize);
            }

            for (var jobType = 0; jobType < _config.JobTypesCount; jobType++)
            {
                // Выполяем проверку на отсутсвие единичных партий
                if (PrimeMatrixA[jobType][0] < 2 || PrimeMatrixA[jobType][0] < PrimeMatrixA[jobType][1])
                {
                    PrimeMatrixA[jobType].Clear();
                    PrimeMatrixA[jobType].Add(batchCountList[jobType]);
                    _i[jobType] = false;
                }
            }

            bestMatrixA = [.. PrimeMatrixA];
        }

        private void PullSolution(List<List<int>> A_Matrix)
        {

            FirstLevelSolution solution = GetFlSolutionFromMatrix(A_Matrix);

            _logger.Print("/////////////");
            _logger.Print("m:", solution.M);
            _logger.Print("A:", solution.A);


            if (_secondLevel.BuildSchedule(solution))
            {
                SecondLevelSolution slSolution = _secondLevel.Solution;
                int f1Current = Makespan(solution, slSolution);
                _logger.Print($"Makespan: {f1Current}");

                if (f1Current < optimalSolution.Makespan)
                {
                    bestMatrixA = [.. A_Matrix];
                    isBestSolution = true;

                    optimalSolution.Success = true;
                    optimalSolution.Makespan = f1Current;
                    optimalSolution.FlSolution = solution;
                    optimalSolution.SlSolution = slSolution;

                }
            }
        }

        private FirstLevelSolution GetFlSolutionFromMatrix(List<List<int>> A_Matrix)
        {
            int[] batchesCount = new int[_config.JobTypesCount];
            int maxBatchesCount = 0;
            for (int jobType = 0; jobType < _config.JobTypesCount; jobType++)
            {
                int jobTypeBatchesCount = A_Matrix[jobType].Count;
                maxBatchesCount = Math.Max(maxBatchesCount, jobTypeBatchesCount);
                batchesCount[jobType] = jobTypeBatchesCount;
            }

            int[,] _A_Matrix = new int[_config.JobTypesCount, maxBatchesCount];
            for (int jobType = 0; jobType < _config.JobTypesCount; jobType++)
            {
                for (int batch = 0; batch < batchesCount[jobType]; batch++)
                {
                    _A_Matrix[jobType, batch] = A_Matrix[jobType][batch];
                }
            }

            return new(batchesCount, _A_Matrix);
        }

        private int Makespan(FirstLevelSolution flSolution, SecondLevelSolution slSolution)
        {
            return slSolution.T_0l.Last().Last().Last() + _config.WorkDurations[_config.DevicesCount - 1, BatchType(flSolution.M.Sum() - 1, slSolution.P)];
        }

        private int BatchType(int batch, int[,] P)
        {
            for (int jobType = 0; jobType < P.GetLength(0); jobType++)
            {
                if (P[jobType, batch] == 1) return jobType;
            }
            return 0;
        }

        private bool CheckType()
        {
            return _i.Contains(true);
        }

        /// <summary>
        /// Рекурсивная комбинация всех типов _a2 с фиксированным решением _a
        /// </summary>
        /// <param name="file"></param>
        /// <param name="tempA"></param>
        /// <param name="type"></param>
        public void CombinationType(int type, List<List<int>> tempM)
        {
            if (type < _config.JobTypesCount)
            {
                for (var variantOfSplitIndex = 0; variantOfSplitIndex < _a2[type].Count; variantOfSplitIndex++)
                {
                    List<List<int>> tempB = [.. tempM];

                    tempB.Add(_a2[type][variantOfSplitIndex]);
                    CombinationType(type + 1, tempB);
                }
            }
            else
            {
                PullSolution(tempM);
            }
        }   

        /// <summary>
        /// Построчное формирование матрицы промежуточного решени
        /// </summary>
        /// <param name="dataType">тип рассматриваемого решения</param>
        /// <param name="batchIndex">индекс подставляемого решения</param>
        /// <returns>матрица А с подставленным новым решением в соответствующий тип</returns>
        private List<List<int>> SetTempAFromA2(int dataType, int batchIndex)
        {
            List<List<int>> result = [.. PrimeMatrixA];
            if (batchIndex < _a2[dataType].Count)
                result[dataType] = [.. _a2[dataType][batchIndex]];
            return result;
        }

        /// <summary>
        /// Функция получения неповторяющихся решений в матрице А2 на шаге 9
        /// </summary>
        /// <param name="inMatrix">входная матрица сформированных решений</param>
        /// <returns>Новые решения без повторений</returns>
        private List<List<int>> SortedMatrix(List<List<int>> inMatrix)
        {
            List<List<int>> temp = [.. inMatrix];

            //Удаление повторяющихся строк
            var countLoops = 0;
            while (true)
            {
                for (var i = 1; i < temp.Count; i++)
                {
                    var lastIndexForDelete = temp.FindLastIndex(delegate (List<int> inList)
                    {
                        if (inList.Count != temp[i].Count)
                        {
                            return false;
                        }
                        var countFind = inList.Where((t, k) => t == temp[i][k]).Count();
                        return countFind == inList.Count;
                    });
                    if (lastIndexForDelete == i) continue;
                    temp.RemoveAt(lastIndexForDelete);
                    inMatrix.RemoveAt(lastIndexForDelete);
                }

                if (++countLoops > 100)
                    break;
            }
            return inMatrix;
        }

        /// <summary>
        /// Удаление повторений новых решений совпадающих с A1
        /// </summary>
        /// <param name="inMatrix">матрица новых решений</param>
        /// <param name="dataType">рассматриваемый тип</param>
        /// <returns>Полученные новые решения</returns>
        private List<List<int>> CheckMatrix(List<List<int>> inMatrix, int dataType)
        {

            foreach (var row2 in _a1[dataType])
            {
                foreach (var rowMatrix in inMatrix.ToList())
                {
                    if (rowMatrix.Zip(row2, (a, b) => new { a, b }).All(pair => pair.a == pair.b))
                    {
                        inMatrix.Remove(rowMatrix);
                    }
                }
            }
            return inMatrix;
        }


        /// <summary>
        /// Формирование новых решений по составим партий текущего типа данных
        /// </summary>
        /// <param name="dataType">рассматриваемый тип</param>
        /// <returns>новые решения для этого типа</returns>
        private List<List<int>> NewData(int dataType)
        {
            var result = new List<List<int>>();
            foreach (var row in _a1[dataType])
            {
                for (var j = 1; j < row.Count; j++)
                {
                    result.Add([.. row]);
                    if (row[0] <= row[j] + 1) continue;
                    result.Last()[0]--;
                    result.Last()[j]++;
                }
                if (result.Last()[0] != row[0]) continue;
                {
                    var summ = row[0];
                    result.Last().Add(2);
                    for (var j = 1; j < row.Count; j++)
                    {
                        summ += row[j];
                        result.Last()[j] = 2;
                    }
                    result.Last()[0] = summ - 2 * (result.Last().Count - 1);
                }
            }
            var count = 0;
            while (true)
            {
                for (var i = 1; i < result.Count; i++)
                {
                    for (var j = 1; j < result[i].Count; j++)
                    {
                        if (result[i][j] <= result[i][j - 1]) continue;
                        result.Remove(result[i]);
                        break;
                    }
                }
                count++;
                if (count > 3)
                    break;
            }

            result = SortedMatrix(result);
            result = CheckMatrix(result, dataType);
            return result;
        }

    }
}
