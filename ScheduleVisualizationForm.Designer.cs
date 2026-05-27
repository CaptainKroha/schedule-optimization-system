namespace ScheduleOptimizationSystem
{
    partial class ScheduleVisualizationForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            canvasPanel = new Panel();
            label1 = new Label();
            label_makespan = new Label();
            SuspendLayout();
            // 
            // canvasPanel
            // 
            canvasPanel.Location = new Point(12, 44);
            canvasPanel.Name = "canvasPanel";
            canvasPanel.Size = new Size(776, 209);
            canvasPanel.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(13, 15);
            label1.Name = "label1";
            label1.Size = new Size(64, 15);
            label1.TabIndex = 1;
            label1.Text = "Makespan:";
            // 
            // label_makespan
            // 
            label_makespan.AutoSize = true;
            label_makespan.Location = new Point(74, 15);
            label_makespan.Name = "label_makespan";
            label_makespan.Size = new Size(25, 15);
            label_makespan.TabIndex = 2;
            label_makespan.Text = "123";
            // 
            // ScheduleVisualizationForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            ClientSize = new Size(800, 450);
            Controls.Add(label_makespan);
            Controls.Add(label1);
            Controls.Add(canvasPanel);
            Name = "ScheduleVisualizationForm";
            Text = "Результаты построения расписания";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel canvasPanel;
        private Label label1;
        private Label label_makespan;
    }
}