namespace ScheduleOptimizationSystem
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tabControl1 = new TabControl();
            tabPage_WorkDurations = new TabPage();
            DGV_WorkDurations = new DataGridView();
            tabPage_ChangeoverDurations = new TabPage();
            DGV_ChangeoverDurations = new DataGridView();
            tabPage_OrderTypes = new TabPage();
            DGV_OrderTypes = new DataGridView();
            tabPage_Orders = new TabPage();
            DGV_Orders = new DataGridView();
            label1 = new Label();
            numeric_Devices = new NumericUpDown();
            label2 = new Label();
            numeric_JobTypes = new NumericUpDown();
            label3 = new Label();
            label4 = new Label();
            numeric_BufferSize = new NumericUpDown();
            numeric_OrderTypes = new NumericUpDown();
            btn_BuildSchedule = new Button();
            linkShowSchedule = new LinkLabel();
            groupBox_Models = new GroupBox();
            rb_Simple = new RadioButton();
            rb_Hierarchical = new RadioButton();
            btn_import = new Button();
            btn_export = new Button();
            cb_LoggingOn = new CheckBox();
            tabControl1.SuspendLayout();
            tabPage_WorkDurations.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DGV_WorkDurations).BeginInit();
            tabPage_ChangeoverDurations.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DGV_ChangeoverDurations).BeginInit();
            tabPage_OrderTypes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DGV_OrderTypes).BeginInit();
            tabPage_Orders.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DGV_Orders).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numeric_Devices).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numeric_JobTypes).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numeric_BufferSize).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numeric_OrderTypes).BeginInit();
            groupBox_Models.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage_WorkDurations);
            tabControl1.Controls.Add(tabPage_ChangeoverDurations);
            tabControl1.Controls.Add(tabPage_OrderTypes);
            tabControl1.Controls.Add(tabPage_Orders);
            tabControl1.Location = new Point(16, 125);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(539, 313);
            tabControl1.TabIndex = 0;
            // 
            // tabPage_WorkDurations
            // 
            tabPage_WorkDurations.Controls.Add(DGV_WorkDurations);
            tabPage_WorkDurations.Location = new Point(4, 24);
            tabPage_WorkDurations.Name = "tabPage_WorkDurations";
            tabPage_WorkDurations.Padding = new Padding(3);
            tabPage_WorkDurations.Size = new Size(531, 285);
            tabPage_WorkDurations.TabIndex = 0;
            tabPage_WorkDurations.Text = "Длительности выполнения";
            tabPage_WorkDurations.UseVisualStyleBackColor = true;
            // 
            // DGV_WorkDurations
            // 
            DGV_WorkDurations.AllowUserToAddRows = false;
            DGV_WorkDurations.AllowUserToDeleteRows = false;
            DGV_WorkDurations.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DGV_WorkDurations.Cursor = Cursors.IBeam;
            DGV_WorkDurations.Location = new Point(6, 3);
            DGV_WorkDurations.Name = "DGV_WorkDurations";
            DGV_WorkDurations.SelectionMode = DataGridViewSelectionMode.CellSelect;
            DGV_WorkDurations.Size = new Size(519, 133);
            DGV_WorkDurations.TabIndex = 0;
            // 
            // tabPage_ChangeoverDurations
            // 
            tabPage_ChangeoverDurations.Controls.Add(DGV_ChangeoverDurations);
            tabPage_ChangeoverDurations.Location = new Point(4, 24);
            tabPage_ChangeoverDurations.Name = "tabPage_ChangeoverDurations";
            tabPage_ChangeoverDurations.Padding = new Padding(3);
            tabPage_ChangeoverDurations.Size = new Size(531, 285);
            tabPage_ChangeoverDurations.TabIndex = 1;
            tabPage_ChangeoverDurations.Text = "Длительности переналадок";
            tabPage_ChangeoverDurations.UseVisualStyleBackColor = true;
            // 
            // DGV_ChangeoverDurations
            // 
            DGV_ChangeoverDurations.AllowUserToAddRows = false;
            DGV_ChangeoverDurations.AllowUserToDeleteRows = false;
            DGV_ChangeoverDurations.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DGV_ChangeoverDurations.Location = new Point(6, 6);
            DGV_ChangeoverDurations.Name = "DGV_ChangeoverDurations";
            DGV_ChangeoverDurations.Size = new Size(519, 273);
            DGV_ChangeoverDurations.TabIndex = 0;
            // 
            // tabPage_OrderTypes
            // 
            tabPage_OrderTypes.Controls.Add(DGV_OrderTypes);
            tabPage_OrderTypes.Location = new Point(4, 24);
            tabPage_OrderTypes.Name = "tabPage_OrderTypes";
            tabPage_OrderTypes.Padding = new Padding(3);
            tabPage_OrderTypes.Size = new Size(531, 285);
            tabPage_OrderTypes.TabIndex = 2;
            tabPage_OrderTypes.Text = "Типы заказов";
            tabPage_OrderTypes.UseVisualStyleBackColor = true;
            // 
            // DGV_OrderTypes
            // 
            DGV_OrderTypes.AllowUserToAddRows = false;
            DGV_OrderTypes.AllowUserToDeleteRows = false;
            DGV_OrderTypes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DGV_OrderTypes.Location = new Point(6, 6);
            DGV_OrderTypes.Name = "DGV_OrderTypes";
            DGV_OrderTypes.Size = new Size(519, 130);
            DGV_OrderTypes.TabIndex = 0;
            // 
            // tabPage_Orders
            // 
            tabPage_Orders.Controls.Add(DGV_Orders);
            tabPage_Orders.Location = new Point(4, 24);
            tabPage_Orders.Name = "tabPage_Orders";
            tabPage_Orders.Padding = new Padding(3);
            tabPage_Orders.Size = new Size(531, 285);
            tabPage_Orders.TabIndex = 3;
            tabPage_Orders.Text = "Заказы";
            tabPage_Orders.UseVisualStyleBackColor = true;
            // 
            // DGV_Orders
            // 
            DGV_Orders.AllowUserToAddRows = false;
            DGV_Orders.AllowUserToDeleteRows = false;
            DGV_Orders.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DGV_Orders.Location = new Point(6, 6);
            DGV_Orders.Name = "DGV_Orders";
            DGV_Orders.Size = new Size(519, 130);
            DGV_Orders.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(16, 9);
            label1.Name = "label1";
            label1.Size = new Size(133, 15);
            label1.TabIndex = 1;
            label1.Text = "Количество приборов:";
            // 
            // numeric_Devices
            // 
            numeric_Devices.Location = new Point(16, 27);
            numeric_Devices.Name = "numeric_Devices";
            numeric_Devices.Size = new Size(120, 23);
            numeric_Devices.TabIndex = 2;
            numeric_Devices.Value = new decimal(new int[] { 1, 0, 0, 0 });
            numeric_Devices.ValueChanged += numeric_Devices_ValueChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(16, 68);
            label2.Name = "label2";
            label2.Size = new Size(157, 15);
            label2.TabIndex = 3;
            label2.Text = "Количество типов заданий:";
            // 
            // numeric_JobTypes
            // 
            numeric_JobTypes.Location = new Point(16, 86);
            numeric_JobTypes.Name = "numeric_JobTypes";
            numeric_JobTypes.Size = new Size(120, 23);
            numeric_JobTypes.TabIndex = 4;
            numeric_JobTypes.Value = new decimal(new int[] { 1, 0, 0, 0 });
            numeric_JobTypes.ValueChanged += numeric_JobTypes_ValueChanged;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(194, 9);
            label3.Name = "label3";
            label3.Size = new Size(152, 15);
            label3.TabIndex = 5;
            label3.Text = "Размер буфера на выходе:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(194, 68);
            label4.Name = "label4";
            label4.Size = new Size(154, 15);
            label4.TabIndex = 6;
            label4.Text = "Количество типов заказов:";
            // 
            // numeric_BufferSize
            // 
            numeric_BufferSize.Location = new Point(194, 27);
            numeric_BufferSize.Name = "numeric_BufferSize";
            numeric_BufferSize.Size = new Size(120, 23);
            numeric_BufferSize.TabIndex = 7;
            numeric_BufferSize.Value = new decimal(new int[] { 10, 0, 0, 0 });
            // 
            // numeric_OrderTypes
            // 
            numeric_OrderTypes.Location = new Point(194, 86);
            numeric_OrderTypes.Name = "numeric_OrderTypes";
            numeric_OrderTypes.Size = new Size(120, 23);
            numeric_OrderTypes.TabIndex = 8;
            numeric_OrderTypes.Value = new decimal(new int[] { 1, 0, 0, 0 });
            numeric_OrderTypes.ValueChanged += numeric_OrderTypes_ValueChanged;
            // 
            // btn_BuildSchedule
            // 
            btn_BuildSchedule.Location = new Point(629, 366);
            btn_BuildSchedule.Name = "btn_BuildSchedule";
            btn_BuildSchedule.Size = new Size(159, 41);
            btn_BuildSchedule.TabIndex = 10;
            btn_BuildSchedule.Text = "Построить расписание";
            btn_BuildSchedule.UseVisualStyleBackColor = true;
            btn_BuildSchedule.Click += btn_BuildSchedule_Click;
            // 
            // linkShowSchedule
            // 
            linkShowSchedule.AutoSize = true;
            linkShowSchedule.Location = new Point(654, 410);
            linkShowSchedule.Name = "linkShowSchedule";
            linkShowSchedule.Size = new Size(113, 15);
            linkShowSchedule.TabIndex = 12;
            linkShowSchedule.TabStop = true;
            linkShowSchedule.Text = "Показать результат";
            linkShowSchedule.Visible = false;
            linkShowSchedule.LinkClicked += linkShowSchedule_LinkClicked;
            // 
            // groupBox_Models
            // 
            groupBox_Models.Controls.Add(rb_Simple);
            groupBox_Models.Controls.Add(rb_Hierarchical);
            groupBox_Models.Location = new Point(629, 291);
            groupBox_Models.Name = "groupBox_Models";
            groupBox_Models.Size = new Size(159, 69);
            groupBox_Models.TabIndex = 13;
            groupBox_Models.TabStop = false;
            groupBox_Models.Text = "Модель";
            groupBox_Models.Visible = false;
            // 
            // rb_Simple
            // 
            rb_Simple.AutoSize = true;
            rb_Simple.Location = new Point(7, 39);
            rb_Simple.Name = "rb_Simple";
            rb_Simple.Size = new Size(82, 19);
            rb_Simple.TabIndex = 1;
            rb_Simple.Text = "Примитив";
            rb_Simple.UseVisualStyleBackColor = true;
            // 
            // rb_Hierarchical
            // 
            rb_Hierarchical.AutoSize = true;
            rb_Hierarchical.Checked = true;
            rb_Hierarchical.Location = new Point(7, 19);
            rb_Hierarchical.Name = "rb_Hierarchical";
            rb_Hierarchical.Size = new Size(137, 19);
            rb_Hierarchical.TabIndex = 0;
            rb_Hierarchical.TabStop = true;
            rb_Hierarchical.Text = "Иерархическая игра";
            rb_Hierarchical.UseVisualStyleBackColor = true;
            // 
            // btn_import
            // 
            btn_import.Location = new Point(689, 9);
            btn_import.Name = "btn_import";
            btn_import.Size = new Size(99, 23);
            btn_import.TabIndex = 14;
            btn_import.Text = "Импорт";
            btn_import.UseVisualStyleBackColor = true;
            btn_import.Click += btn_import_Click;
            // 
            // btn_export
            // 
            btn_export.Location = new Point(689, 38);
            btn_export.Name = "btn_export";
            btn_export.Size = new Size(99, 23);
            btn_export.TabIndex = 15;
            btn_export.Text = "Экспорт";
            btn_export.UseVisualStyleBackColor = true;
            btn_export.Click += btn_export_Click;
            // 
            // cb_LoggingOn
            // 
            cb_LoggingOn.AutoSize = true;
            cb_LoggingOn.Location = new Point(735, 68);
            cb_LoggingOn.Name = "cb_LoggingOn";
            cb_LoggingOn.Size = new Size(53, 19);
            cb_LoggingOn.TabIndex = 16;
            cb_LoggingOn.Text = "Логи";
            cb_LoggingOn.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(cb_LoggingOn);
            Controls.Add(btn_import);
            Controls.Add(btn_export);
            Controls.Add(groupBox_Models);
            Controls.Add(linkShowSchedule);
            Controls.Add(btn_BuildSchedule);
            Controls.Add(numeric_OrderTypes);
            Controls.Add(numeric_BufferSize);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(numeric_JobTypes);
            Controls.Add(label2);
            Controls.Add(numeric_Devices);
            Controls.Add(label1);
            Controls.Add(tabControl1);
            Name = "Form1";
            Text = "Система оптимизации расписаний";
            tabControl1.ResumeLayout(false);
            tabPage_WorkDurations.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)DGV_WorkDurations).EndInit();
            tabPage_ChangeoverDurations.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)DGV_ChangeoverDurations).EndInit();
            tabPage_OrderTypes.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)DGV_OrderTypes).EndInit();
            tabPage_Orders.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)DGV_Orders).EndInit();
            ((System.ComponentModel.ISupportInitialize)numeric_Devices).EndInit();
            ((System.ComponentModel.ISupportInitialize)numeric_JobTypes).EndInit();
            ((System.ComponentModel.ISupportInitialize)numeric_BufferSize).EndInit();
            ((System.ComponentModel.ISupportInitialize)numeric_OrderTypes).EndInit();
            groupBox_Models.ResumeLayout(false);
            groupBox_Models.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TabControl tabControl1;
        private TabPage tabPage_WorkDurations;
        private TabPage tabPage_ChangeoverDurations;
        private TabPage tabPage_OrderTypes;
        private TabPage tabPage_Orders;
        private Label label1;
        private NumericUpDown numeric_Devices;
        private Label label2;
        private NumericUpDown numeric_JobTypes;
        private Label label3;
        private NumericUpDown numeric_BufferSize;
        private Label label4;
        private NumericUpDown numeric_OrderTypes;
        private DataGridView DGV_WorkDurations;
        private DataGridView DGV_ChangeoverDurations;
        private DataGridView DGV_OrderTypes;
        private DataGridView DGV_Orders;
        private Button btn_BuildSchedule;
        private LinkLabel linkShowSchedule;
        private GroupBox groupBox_Models;
        private RadioButton rb_Simple;
        private RadioButton rb_Hierarchical;
        private Button btn_import;
        private Button btn_export;
        private CheckBox cb_LoggingOn;
    }
}
