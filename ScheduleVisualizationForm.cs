using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;

namespace ScheduleOptimizationSystem
{
    public partial class ScheduleVisualizationForm : Form
    {

        private ScheduleDto _schedule;
        private float timeScale = 30f;      // пикселей на единицу времени
        private int rowHeight = 30;          // высота строки устройства
        private int leftMargin = 80;         // отступ слева для названий устройств
        private int topMargin = 40;          // отступ сверху для шкалы времени

        public ScheduleVisualizationForm(ScheduleDto schedule)
        {
            InitializeComponent();
            _schedule = schedule;

            label_makespan.Text = schedule.MakeSpan.ToString();

            UpdatePanelSize();
            canvasPanel.Paint += CanvasPanel_Paint;
        }

        private void UpdatePanelSize()
        {
            if (_schedule == null) return;

            int totalWidth = leftMargin + (int)(_schedule.MaxTime * timeScale);
            int totalHeight = topMargin + _schedule.Count * rowHeight;

            canvasPanel.AutoScrollMinSize = new Size(totalWidth, totalHeight);
        }

        private void CanvasPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Определяем область отрисовки с учётом прокрутки
            int scrollX = canvasPanel.AutoScrollPosition.X;
            int scrollY = canvasPanel.AutoScrollPosition.Y;
            g.TranslateTransform(scrollX, scrollY);

            int maxTime = _schedule.MaxTime;

            int y_end = topMargin + _schedule.Count * rowHeight;

            int step = 1; // шаг меток
            for (int t = 0; t <= maxTime; t += step)
            {
                int x = leftMargin + (int)(t * timeScale);
                // Вертикальная линия сетки
                g.DrawLine(Pens.LightGray, x, topMargin, x, y_end);
                // Подпись времени
                string timeText = t.ToString();
                SizeF textSize = g.MeasureString(timeText, SystemFonts.DefaultFont);
                g.DrawString(timeText, SystemFonts.DefaultFont, Brushes.Black, x - textSize.Width / 2, 5);
            }

            // Рисуем горизонтальные линии разделители устройств и заголовки
            for (int i = 0; i < _schedule.Count; i++)
            {
                int y = topMargin + i * rowHeight;
                // Горизонтальная линия
                g.DrawLine(Pens.LightGray, leftMargin, y, leftMargin + (int)(maxTime * timeScale), y);
                // Подпись устройства
                string deviceText = $"Прибор {i + 1}";
                g.DrawString(deviceText, SystemFonts.DefaultFont, Brushes.Black, 5, y + rowHeight / 2 - 8);

                // Рисуем прямоугольники задач для этого устройства
                var deviceTasks = _schedule[i];
                foreach (var task in deviceTasks)
                {
                    int x = leftMargin + (int)(task.Start * timeScale);
                    int width = (int)(task.Duration * timeScale);
                    
                    Brush brush = GetTaskBrush(task.Title);
                    g.FillRectangle(brush, x, y + 2, width, rowHeight - 4);
                    g.DrawRectangle(Pens.Black, x, y + 2, width, rowHeight - 4);
                    
                    string taskLabel = $"{task.Title}";
                    SizeF labelSize = g.MeasureString(taskLabel, SystemFonts.DefaultFont);
                    if (width > labelSize.Width + 4)
                        g.DrawString(taskLabel, SystemFonts.DefaultFont, Brushes.Black, x + 2, y + rowHeight / 2 - 8);
                }
            }

            g.DrawLine(Pens.LightGray, leftMargin, y_end, leftMargin + (int)(maxTime * timeScale), y_end);
        }

        private Brush GetTaskBrush(string title)
        {
            int hash = Math.Abs(title.GetHashCode());
            Color color = Color.FromArgb(100 + hash % 156, 100 + (hash / 256) % 156, 100 + (hash / 65536) % 156);
            return new SolidBrush(color);
        }

    }
}
