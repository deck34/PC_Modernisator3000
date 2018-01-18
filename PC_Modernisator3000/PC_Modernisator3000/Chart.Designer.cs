namespace PC_Modernisator3000
{
    partial class Chart
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
            this.ChartView = new ZedGraph.ZedGraphControl();
            this.cbMonDev = new System.Windows.Forms.ComboBox();
            this.cbMonSen = new System.Windows.Forms.ComboBox();
            this.cbMonSenType = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // ChartView
            // 
            this.ChartView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ChartView.IsShowPointValues = false;
            this.ChartView.Location = new System.Drawing.Point(8, 24);
            this.ChartView.MinimumSize = new System.Drawing.Size(200, 200);
            this.ChartView.Name = "ChartView";
            this.ChartView.PointValueFormat = "G";
            this.ChartView.Size = new System.Drawing.Size(638, 432);
            this.ChartView.TabIndex = 3;
            // 
            // cbMonDev
            // 
            this.cbMonDev.FormattingEnabled = true;
            this.cbMonDev.Location = new System.Drawing.Point(12, 462);
            this.cbMonDev.Name = "cbMonDev";
            this.cbMonDev.Size = new System.Drawing.Size(195, 21);
            this.cbMonDev.TabIndex = 6;
            // 
            // cbMonSen
            // 
            this.cbMonSen.FormattingEnabled = true;
            this.cbMonSen.Location = new System.Drawing.Point(414, 462);
            this.cbMonSen.Name = "cbMonSen";
            this.cbMonSen.Size = new System.Drawing.Size(195, 21);
            this.cbMonSen.TabIndex = 7;
            // 
            // cbMonSenType
            // 
            this.cbMonSenType.FormattingEnabled = true;
            this.cbMonSenType.Location = new System.Drawing.Point(213, 462);
            this.cbMonSenType.Name = "cbMonSenType";
            this.cbMonSenType.Size = new System.Drawing.Size(195, 21);
            this.cbMonSenType.TabIndex = 8;
            // 
            // Chart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(658, 497);
            this.Controls.Add(this.cbMonSenType);
            this.Controls.Add(this.cbMonSen);
            this.Controls.Add(this.cbMonDev);
            this.Controls.Add(this.ChartView);
            this.Name = "Chart";
            this.Text = "Chart";
            this.ResumeLayout(false);

        }

        #endregion

        private ZedGraph.ZedGraphControl ChartView;
        private System.Windows.Forms.ComboBox cbMonDev;
        private System.Windows.Forms.ComboBox cbMonSen;
        private System.Windows.Forms.ComboBox cbMonSenType;
    }
}