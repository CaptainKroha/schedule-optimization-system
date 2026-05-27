using Newtonsoft.Json;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Xml;

namespace ScheduleOptimizationSystem
{
    public partial class Form1 : Form, IView
    {

        private readonly ScheduleOptimizationSystemController _controller;

        public Form1()
        {
            InitializeComponent();
            InitializeForm();
            _controller = new ScheduleOptimizationSystemController(this);
        }

        private void InitializeForm()
        {
            UpdateWorkDurationsDGV();
            UpdateChangeoverDurationsDGV();
            UpdateOrderTypesDGV();
            UpdateOrdersDGV();
        }

        private void UpdateWorkDurationsDGV()
        {
            int devices = (int)numeric_Devices.Value;
            int jobTypes = (int)numeric_JobTypes.Value;

            UpdateDGV(DGV_WorkDurations, devices, jobTypes);
        }

        private void UpdateChangeoverDurationsDGV()
        {
            int devices = (int)numeric_Devices.Value;
            int jobTypes = (int)numeric_JobTypes.Value;

            UpdateDGV(DGV_ChangeoverDurations, devices * jobTypes, jobTypes);
        }

        private void UpdateOrderTypesDGV()
        {
            int orderTypes = (int)numeric_OrderTypes.Value;
            int jobTypes = (int)numeric_JobTypes.Value;

            UpdateDGV(DGV_OrderTypes, orderTypes, jobTypes);
        }

        private void UpdateOrdersDGV()
        {
            int orderTypes = (int)numeric_OrderTypes.Value;
            UpdateDGV(DGV_Orders, orderTypes, 1);
        }

        private void UpdateDGV(DataGridView dataGridView, int rows, int columns)
        {

            dataGridView.Rows.Clear();
            dataGridView.Columns.Clear();

            dataGridView.RowCount = rows;
            dataGridView.ColumnCount = columns;

            if (rows == 0 || columns == 0)
            {
                return;
            }

            for (int device = 0; device < rows; device++)
            {
                for (int preMType = 0; preMType < columns; preMType++)
                {
                    dataGridView.Rows[device].Cells[preMType].Value = 0;
                }
            }
        }

        private void numeric_Devices_ValueChanged(object sender, EventArgs e)
        {
            UpdateWorkDurationsDGV();
            UpdateChangeoverDurationsDGV();
        }

        private void numeric_JobTypes_ValueChanged(object sender, EventArgs e)
        {
            UpdateWorkDurationsDGV();
            UpdateChangeoverDurationsDGV();
            UpdateOrderTypesDGV();
        }

        private void numeric_OrderTypes_ValueChanged(object sender, EventArgs e)
        {
            UpdateOrderTypesDGV();
            UpdateOrdersDGV();
        }

        void IView.SetBuildButtonEnabled(bool enabled)
        {
            btn_BuildSchedule.Enabled = enabled;
        }

        void IView.ShowNotification(string message)
        {
            MessageBox.Show(this, message, "Уведомление", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        void IView.DisplaySchedule(ScheduleDto schedule)
        {
            if (schedule == null)
            {
                MessageBox.Show("Расписание ещё не построено.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var scheduleVisualizationForm = new ScheduleVisualizationForm(schedule);
            scheduleVisualizationForm.ShowDialog();

        }

        void IView.SetResultLinkVisible(bool visible)
        {
            linkShowSchedule.Visible = visible;
        }

        bool IView.LoggingOn => cb_LoggingOn.Checked;

        private ScheduleConfig GetConfig()
        {
            return new ScheduleConfig(
                (int)numeric_Devices.Value,
                (int)numeric_JobTypes.Value,
                (int)numeric_OrderTypes.Value,
                (int)numeric_BufferSize.Value,
                DGVToMatrix(DGV_WorkDurations),
                ChangeoverDurationsMatrixArray(),
                DGVToMatrix(DGV_OrderTypes),
                OrdersArray()
                );
        }

        private int[][,] ChangeoverDurationsMatrixArray()
        {
            int devices = (int)numeric_Devices.Value;
            int jobTypes = (int)numeric_JobTypes.Value;
            int[][,] result = new int[devices][,];
            for (int device = 0; device < devices; device++)
            {
                result[device] = new int[jobTypes, jobTypes];
                for (int i = 0; i < jobTypes; i++)
                {
                    for (int j = 0; j < jobTypes; j++)
                    {
                        result[device][i, j] = Convert.ToInt32(
                            DGV_ChangeoverDurations.Rows[device * jobTypes + i].Cells[j].Value
                        );
                    }
                }
            }
            return result;
        }

        private int[,] DGVToMatrix(DataGridView dataGridView)
        {
            int rows = dataGridView.RowCount;
            int columns = dataGridView.ColumnCount;

            int[,] result = new int[rows, columns];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    result[i, j] = Convert.ToInt32(dataGridView.Rows[i].Cells[j].Value);
                }
            }
            return result;
        }

        private int[] OrdersArray()
        {
            int rows = DGV_Orders.RowCount;
            int[] result = new int[rows];
            for (int i = 0; i < rows; i++)
            {
                result[i] = Convert.ToInt32(DGV_Orders.Rows[i].Cells[0].Value);
            }
            return result;
        }

        private void ImportConfig(ScheduleConfig config)
        {
            numeric_Devices.Value = config.DevicesCount;
            numeric_JobTypes.Value = config.JobTypesCount;
            numeric_OrderTypes.Value = config.OrderTypesCount;
            numeric_BufferSize.Value = config.BufferSize;

            UpdateWorkDurationsDGV();
            UpdateChangeoverDurationsDGV();
            UpdateOrderTypesDGV();
            UpdateOrdersDGV();

            SetWorkDurations(config.WorkDurations);
            SetChangeoverDurations(config.ChangeoverDurations);
            SetOrderTypes(config.OrderTypes);
            SetOrders(config.Orders);
        }

        private void SetWorkDurations(int[,] data)
        {
            for (int device = 0; device < data.GetLength(0); device++)
            {
                for (int jobType = 0; jobType < data.GetLength(1); jobType++)

                    DGV_WorkDurations.Rows[device].Cells[jobType].Value = data[device, jobType];
            }
        }

        private void SetChangeoverDurations(int[][,] data)
        {
            int dataTypesCount = data[0].GetLength(0);

            // Для каждого прибора
            for (int device = 0; device < data.Length; device++)

                // Для каждого типа данных
                for (int fromDataType = 0; fromDataType < dataTypesCount; fromDataType++)

                    // Для каждого типа данных
                    for (int toDataType = 0; toDataType < dataTypesCount; toDataType++)

                        // Устанавливаем значение матрицы времени переналадки
                        DGV_ChangeoverDurations.Rows[device * dataTypesCount + fromDataType].Cells[toDataType].Value = data[device][fromDataType, toDataType];
        }

        private void SetOrderTypes(int[,] data)
        {
            for (int orderType = 0; orderType < data.GetLength(0); orderType++)
            {
                for (int jobType = 0; jobType < data.GetLength(1); jobType++)

                    DGV_OrderTypes.Rows[orderType].Cells[jobType].Value = data[orderType, jobType];
            }
        }

        private void SetOrders(int[] data)
        {
            for (int orderType = 0; orderType < data.GetLength(0); orderType++)
            {
                DGV_Orders.Rows[orderType].Cells[0].Value = data[orderType];
            }
        }

        private async void btn_BuildSchedule_Click(object sender, EventArgs e)
        {
            await _controller.BuildScheduleAsync(GetConfig(), GetModelType());
        }

        private ModelTypes GetModelType()
        {
            if (rb_Hierarchical.Checked) return ModelTypes.HIERARCHICAL;
            else return ModelTypes.SIMPLE;
        }

        private void linkShowSchedule_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            _controller.ShowSchedule();
        }

        private void btn_import_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new()
            {
                Filter = "json files (*.json)|*.json",
                FilterIndex = 2,
                RestoreDirectory = true
            };

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                string jsonText = File.ReadAllText(fileDialog.FileName);

                try
                {
                    ScheduleConfig? config = JsonConvert.DeserializeObject<ScheduleConfig>(jsonText);

                    if (config is null) return;

                    ImportConfig(config);
                }
                catch (JsonException)
                {
                    MessageBox.Show("Указанный имеет некорректный формат.", "Ошибка");
                    return;
                }

            }
            else
            {
                MessageBox.Show("Указан некорректный файл.", "Предупреждение");
                return;
            }
        }

        private void btn_export_Click(object sender, EventArgs e)
        {
            ScheduleConfig config = GetConfig();
            string jsonText = JsonConvert.SerializeObject(config, Newtonsoft.Json.Formatting.Indented);
            Stream myStream;
            SaveFileDialog fileDialog = new()
            {
                Filter = "json files (*.json)|*.json",
                FilterIndex = 2,
                RestoreDirectory = true
            };

            if (fileDialog.ShowDialog() == DialogResult.OK)
                if ((myStream = fileDialog.OpenFile()) != null)
                {
                    byte[] buffer = Encoding.Default.GetBytes($"{jsonText}");
                    myStream.Write(buffer, 0, buffer.Length);
                    myStream.Close();
                }
        }

    }
}
