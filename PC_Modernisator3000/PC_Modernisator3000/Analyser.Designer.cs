namespace PC_Modernisator3000
{
    partial class Analyser
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
            this.components = new System.ComponentModel.Container();
            this.ChartView = new ZedGraph.ZedGraphControl();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.pieChartProblems = new ZedGraph.ZedGraphControl();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // ChartView
            // 
            this.ChartView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ChartView.Location = new System.Drawing.Point(3, 6);
            this.ChartView.MinimumSize = new System.Drawing.Size(200, 200);
            this.ChartView.Name = "ChartView";
            this.ChartView.ScrollGrace = 0D;
            this.ChartView.ScrollMaxX = 0D;
            this.ChartView.ScrollMaxY = 0D;
            this.ChartView.ScrollMaxY2 = 0D;
            this.ChartView.ScrollMinX = 0D;
            this.ChartView.ScrollMinY = 0D;
            this.ChartView.ScrollMinY2 = 0D;
            this.ChartView.Size = new System.Drawing.Size(686, 409);
            this.ChartView.TabIndex = 4;
            this.ChartView.UseExtendedPrintDialog = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(3, 2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(703, 442);
            this.tabControl1.TabIndex = 5;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.ChartView);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(695, 416);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Parts of Computer";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.pieChartProblems);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(695, 416);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "IndicatedProblems";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // pieChartProblems
            // 
            this.pieChartProblems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pieChartProblems.Location = new System.Drawing.Point(4, 4);
            this.pieChartProblems.MinimumSize = new System.Drawing.Size(200, 200);
            this.pieChartProblems.Name = "pieChartProblems";
            this.pieChartProblems.ScrollGrace = 0D;
            this.pieChartProblems.ScrollMaxX = 0D;
            this.pieChartProblems.ScrollMaxY = 0D;
            this.pieChartProblems.ScrollMaxY2 = 0D;
            this.pieChartProblems.ScrollMinX = 0D;
            this.pieChartProblems.ScrollMinY = 0D;
            this.pieChartProblems.ScrollMinY2 = 0D;
            this.pieChartProblems.Size = new System.Drawing.Size(686, 409);
            this.pieChartProblems.TabIndex = 5;
            this.pieChartProblems.UseExtendedPrintDialog = true;
            // 
            // Analyser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(708, 446);
            this.Controls.Add(this.tabControl1);
            this.Name = "Analyser";
            this.Text = "Analyser";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ZedGraph.ZedGraphControl ChartView;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private ZedGraph.ZedGraphControl pieChartProblems;
    }
}