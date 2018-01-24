using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using DeninaSharp.Core;
using Microsoft.Win32;
using System.Xml;
using System.Xml.Linq;
using System.IO;

namespace UI
{
    public partial class MainWindow : Form
    {
        private const string settingsFileLocation = "settings.xml";
        private const string lastInputSettingName = "lastInput";
        private const string lastCommandsSettingName = "lastCommands";

        public MainWindow()
        {
            InitializeComponent();
            ReadSettings();
        }

        private void ExecuteButton_Click(object sender, EventArgs e)
        {
            PipelineResults.BackColor = Color.FromName("White");

            var commands = String.IsNullOrWhiteSpace(PipelineCommands.SelectedText) ? PipelineCommands.Text : PipelineCommands.SelectedText;

            try
            {
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                var pipeline = new Pipeline();
                pipeline.AddCommand(commands);
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

            WriteSettings();
        }

        private void PipelineCommands_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.E && Control.ModifierKeys == Keys.Control)
            {
                ExecuteButton_Click(sender, new EventArgs());
            }
        }

        private void WriteSettings()
        {
            using (var xml = XmlWriter.Create(settingsFileLocation))
            {
                xml.WriteStartDocument();
                xml.WriteStartElement("settings");
                xml.WriteStartElement(lastInputSettingName);
                xml.WriteCData(InputTextbox.Text);
                xml.WriteEndElement();
                xml.WriteStartElement(lastCommandsSettingName);
                xml.WriteCData(PipelineCommands.Text);
                xml.WriteEndElement();
                xml.WriteEndDocument();
                xml.Flush();
                xml.Close();
            }
        }

        private void ReadSettings()
        {
            if (File.Exists(settingsFileLocation))
            {
                try
                {
                    var xml = XDocument.Load(settingsFileLocation);
                    InputTextbox.Text = xml.Root.Element(lastInputSettingName).Value;
                    PipelineCommands.Text = xml.Root.Element(lastCommandsSettingName).Value;
                }
                catch (Exception e)
                {
                    // Just don't read the settings, I guess
                }
            }
        }

        
    }
}