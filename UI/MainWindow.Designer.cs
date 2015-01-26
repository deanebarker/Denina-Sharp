using System.Drawing;

namespace UI
{
    partial class MainWindow
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
            this.PipelineCommands = new System.Windows.Forms.TextBox();
            this.ExecuteButton = new System.Windows.Forms.Button();
            this.PipelineResults = new System.Windows.Forms.TextBox();
            this.Status = new System.Windows.Forms.Label();
            this.InputTextbox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // PipelineCommands
            // 
            this.PipelineCommands.Font = new System.Drawing.Font("Courier New", 10F);
            this.PipelineCommands.Location = new System.Drawing.Point(386, 11);
            this.PipelineCommands.Multiline = true;
            this.PipelineCommands.Name = "PipelineCommands";
            this.PipelineCommands.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.PipelineCommands.Size = new System.Drawing.Size(386, 540);
            this.PipelineCommands.TabIndex = 0;
            // 
            // ExecuteButton
            // 
            this.ExecuteButton.Location = new System.Drawing.Point(512, 591);
            this.ExecuteButton.Name = "ExecuteButton";
            this.ExecuteButton.Size = new System.Drawing.Size(106, 48);
            this.ExecuteButton.TabIndex = 1;
            this.ExecuteButton.Text = "Execute";
            this.ExecuteButton.UseVisualStyleBackColor = true;
            this.ExecuteButton.Click += new System.EventHandler(this.ExecuteButton_Click);
            // 
            // PipelineResults
            // 
            this.PipelineResults.BackColor = System.Drawing.Color.White;
            this.PipelineResults.Font = new System.Drawing.Font("Courier New", 10F);
            this.PipelineResults.Location = new System.Drawing.Point(791, 11);
            this.PipelineResults.Multiline = true;
            this.PipelineResults.Name = "PipelineResults";
            this.PipelineResults.ReadOnly = true;
            this.PipelineResults.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.PipelineResults.Size = new System.Drawing.Size(350, 667);
            this.PipelineResults.TabIndex = 2;
            // 
            // Status
            // 
            this.Status.Location = new System.Drawing.Point(512, 642);
            this.Status.Name = "Status";
            this.Status.Size = new System.Drawing.Size(106, 17);
            this.Status.TabIndex = 3;
            this.Status.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Status.UseMnemonic = false;
            // 
            // InputTextbox
            // 
            this.InputTextbox.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.InputTextbox.Location = new System.Drawing.Point(12, 12);
            this.InputTextbox.Multiline = true;
            this.InputTextbox.Name = "InputTextbox";
            this.InputTextbox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.InputTextbox.Size = new System.Drawing.Size(350, 666);
            this.InputTextbox.TabIndex = 4;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1153, 692);
            this.Controls.Add(this.InputTextbox);
            this.Controls.Add(this.Status);
            this.Controls.Add(this.PipelineResults);
            this.Controls.Add(this.ExecuteButton);
            this.Controls.Add(this.PipelineCommands);
            this.Name = "MainWindow";
            this.Text = "Text Filter Pipeline Tester";
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox PipelineCommands;
        private System.Windows.Forms.Button ExecuteButton;
        private System.Windows.Forms.TextBox PipelineResults;
        private System.Windows.Forms.Label Status;
        private System.Windows.Forms.TextBox InputTextbox;
    }
}

