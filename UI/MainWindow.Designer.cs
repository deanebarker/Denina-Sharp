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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.ExecuteButton = new System.Windows.Forms.Button();
            this.Status = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.PipelineResults = new System.Windows.Forms.TextBox();
            this.PipelineCommands = new System.Windows.Forms.TextBox();
            this.InputTextbox = new System.Windows.Forms.TextBox();
            this.BaseIncludeFolderInput = new System.Windows.Forms.TextBox();
            this.SelectFolderButton = new System.Windows.Forms.Button();
            this.DefaultConnectionStringInput = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ExecuteButton
            // 
            this.ExecuteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ExecuteButton.Location = new System.Drawing.Point(1022, 559);
            this.ExecuteButton.Name = "ExecuteButton";
            this.ExecuteButton.Size = new System.Drawing.Size(106, 48);
            this.ExecuteButton.TabIndex = 1;
            this.ExecuteButton.Text = "Execute";
            this.ExecuteButton.UseVisualStyleBackColor = true;
            this.ExecuteButton.Click += new System.EventHandler(this.ExecuteButton_Click);
            // 
            // Status
            // 
            this.Status.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Status.Location = new System.Drawing.Point(894, 575);
            this.Status.Name = "Status";
            this.Status.Size = new System.Drawing.Size(106, 17);
            this.Status.TabIndex = 3;
            this.Status.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Status.UseMnemonic = false;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.PipelineResults, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.PipelineCommands, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.InputTextbox, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(22, 22);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1106, 531);
            this.tableLayoutPanel1.TabIndex = 5;
            // 
            // PipelineResults
            // 
            this.PipelineResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PipelineResults.BackColor = System.Drawing.Color.White;
            this.PipelineResults.Font = new System.Drawing.Font("Courier New", 10F);
            this.PipelineResults.Location = new System.Drawing.Point(739, 3);
            this.PipelineResults.Multiline = true;
            this.PipelineResults.Name = "PipelineResults";
            this.PipelineResults.ReadOnly = true;
            this.PipelineResults.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.PipelineResults.Size = new System.Drawing.Size(364, 525);
            this.PipelineResults.TabIndex = 7;
            // 
            // PipelineCommands
            // 
            this.PipelineCommands.AcceptsReturn = true;
            this.PipelineCommands.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PipelineCommands.Font = new System.Drawing.Font("Courier New", 10F);
            this.PipelineCommands.Location = new System.Drawing.Point(371, 3);
            this.PipelineCommands.Multiline = true;
            this.PipelineCommands.Name = "PipelineCommands";
            this.PipelineCommands.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.PipelineCommands.Size = new System.Drawing.Size(362, 525);
            this.PipelineCommands.TabIndex = 6;
            this.PipelineCommands.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PipelineCommands_KeyDown);
            // 
            // InputTextbox
            // 
            this.InputTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.InputTextbox.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.InputTextbox.Location = new System.Drawing.Point(3, 3);
            this.InputTextbox.Multiline = true;
            this.InputTextbox.Name = "InputTextbox";
            this.InputTextbox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.InputTextbox.Size = new System.Drawing.Size(362, 525);
            this.InputTextbox.TabIndex = 5;
            // 
            // BaseIncludeFolderInput
            // 
            this.BaseIncludeFolderInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BaseIncludeFolderInput.Location = new System.Drawing.Point(186, 561);
            this.BaseIncludeFolderInput.Name = "BaseIncludeFolderInput";
            this.BaseIncludeFolderInput.Size = new System.Drawing.Size(335, 20);
            this.BaseIncludeFolderInput.TabIndex = 6;
            this.BaseIncludeFolderInput.TextChanged += new System.EventHandler(this.BaseIncludeFolderInput_TextChanged);
            // 
            // SelectFolderButton
            // 
            this.SelectFolderButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.SelectFolderButton.Location = new System.Drawing.Point(25, 559);
            this.SelectFolderButton.Name = "SelectFolderButton";
            this.SelectFolderButton.Size = new System.Drawing.Size(147, 23);
            this.SelectFolderButton.TabIndex = 7;
            this.SelectFolderButton.Text = "Select Include Folder...";
            this.SelectFolderButton.UseVisualStyleBackColor = true;
            this.SelectFolderButton.Click += new System.EventHandler(this.SelectFolderButton_Click);
            // 
            // DefaultConnectionStringInput
            // 
            this.DefaultConnectionStringInput.Location = new System.Drawing.Point(186, 586);
            this.DefaultConnectionStringInput.Name = "DefaultConnectionStringInput";
            this.DefaultConnectionStringInput.Size = new System.Drawing.Size(335, 20);
            this.DefaultConnectionStringInput.TabIndex = 8;
            this.DefaultConnectionStringInput.TextChanged += new System.EventHandler(this.DefaultConnectionStringInput_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(34, 589);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(146, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "\"$__conn\" Connection String";
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1153, 624);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.DefaultConnectionStringInput);
            this.Controls.Add(this.SelectFolderButton);
            this.Controls.Add(this.BaseIncludeFolderInput);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.Status);
            this.Controls.Add(this.ExecuteButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainWindow";
            this.Text = "Denina Pipeline Tester";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button ExecuteButton;
        private System.Windows.Forms.Label Status;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox PipelineResults;
        private System.Windows.Forms.TextBox PipelineCommands;
        private System.Windows.Forms.TextBox InputTextbox;
        private System.Windows.Forms.TextBox BaseIncludeFolderInput;
        private System.Windows.Forms.Button SelectFolderButton;
        private System.Windows.Forms.TextBox DefaultConnectionStringInput;
        private System.Windows.Forms.Label label1;
    }
}

