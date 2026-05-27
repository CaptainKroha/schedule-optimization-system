using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace ScheduleOptimizationSystem
{
    internal class FileLogger : ILogger
    {

        private StreamWriter _writer;
        private readonly object _lock = new object();

        public FileLogger()
        {
        }

        public void SetLogFile(string filename)
        {
            string directory = "C:\\Users\\ipato\\Documents\\University\\Diplom\\Project\\Logs\\";
            _writer = new StreamWriter(directory + filename, append: true, Encoding.UTF8)
            {
                AutoFlush = true
            };
        }

        void ILogger.Print(string message)
        {
            lock (_lock)
            {
                _writer?.WriteLine(message);
            }
        }

        void ILogger.Print(string message, int[] array)
        {
            string logline = message + " [";
            for (int i = 0; i < array.Length; i++)
            {
                logline += array[i].ToString() + " ";
            }
            logline += "]";
            lock (_lock)
            {
                _writer?.WriteLine(logline);
            }
        }

        void ILogger.Print(string message, List<List<int>> matrix)
        {
            if (matrix == null || matrix.Count == 0)
            {
                lock (_lock)
                {
                    _writer?.WriteLine(message);
                    _writer?.WriteLine("[]");
                }
                return;
            }

            int theLongestRow = 0;
            for (int i = 0; i < matrix.Count; i++)
            {
                if (matrix[i].Count > matrix[theLongestRow].Count) theLongestRow = i;
            }
            int columns = matrix[theLongestRow].Count;
            int[] columnWidths = new int[columns];

            for (int col = 0; col < columns; col++)
            {
                int maxWidth = 0;
                foreach (var row in matrix)
                {
                    if (col < row.Count)
                    {
                        int width = row[col].ToString().Length;
                        if (width > maxWidth) maxWidth = width;
                    }
                }
                columnWidths[col] = maxWidth;
            }

            var sb = new StringBuilder();
            sb.AppendLine(message);

            for (int i = 0; i < matrix.Count; i++)
            {
                var row = matrix[i];
                sb.Append("|");
                for (int j = 0; j < row.Count; j++)
                {
                    if (j > 0) sb.Append(' ');
                    sb.Append(row[j].ToString().PadLeft(columnWidths[j]));
                }
                sb.Append("|");
                if (i < matrix.Count - 1) sb.AppendLine();
            }

            lock (_lock)
            {
                _writer?.WriteLine(sb.ToString());
            }
        }

        void ILogger.Print(string message, int[,] matrix)
        {
            if (matrix == null || matrix.GetLength(0) == 0 || matrix.GetLength(1) == 0)
            {
                lock (_lock)
                {
                    _writer?.WriteLine(message);
                    _writer?.WriteLine("[]");
                }
                return;
            }

            int rows = matrix.GetLength(0);
            int columns = matrix.GetLength(1);
            int[] columnWidths = new int[columns];

            for (int col = 0; col < columns; col++)
            {
                int maxWidth = 0;
                for (int row = 0; row < rows; row++)
                {
                    int width = matrix[row, col].ToString().Length;
                    if (width > maxWidth) maxWidth = width;
                }
                columnWidths[col] = maxWidth;
            }

            var sb = new StringBuilder();
            sb.AppendLine(message);

            for (int i = 0; i < rows; i++)
            {
                sb.Append("|");
                for (int j = 0; j < columns; j++)
                {
                    if (j > 0) sb.Append(' ');
                    sb.Append(matrix[i, j].ToString().PadLeft(columnWidths[j]));
                }
                sb.Append("|");
                if (i < rows - 1) sb.AppendLine();
            }

            lock (_lock)
            {
                _writer?.WriteLine(sb.ToString());
            }
        }

        public void Dispose()
        {
            _writer?.Dispose();
        }

    }
}
