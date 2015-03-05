using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using DeninaSharp.Core;

namespace UI
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ExecuteButton_Click(object sender, EventArgs e)
        {
            PipelineResults.BackColor = Color.FromName("White");

            try
            {
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                var pipeline = new Pipeline();
                pipeline.AddCommand(PipelineCommands.Text);
                long parseTime = stopWatch.ElapsedMilliseconds;
                PipelineResults.Text = pipeline.Execute(InputTextbox.Text);
                long executionTime = stopWatch.ElapsedMilliseconds;
                stopWatch.Reset();

                Status.Text = String.Format("{0}ms / {1}ms", parseTime, executionTime);
            }
            catch (Exception exception)
            {
                PipelineResults.Text = String.Join(Environment.NewLine, exception.Message, exception.StackTrace);
                PipelineResults.BackColor = Color.FromName("LightPink");
            }
        }
    }
}