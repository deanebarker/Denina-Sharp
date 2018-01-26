using System;
using System.Configuration;
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
        private const string lastIncludeFolderSettingName = "lastIncludeFolder";
        private const string defaultConnectionStringSettingName = "defaultConnectionString";
        public const string DefaultConnectionStringName = "__conn";
        
        public MainWindow()
        {
            InitializeComponent();
            ReadSettings();
            Application.EnableVisualStyles();
        }

        private void ExecuteButton_Click(object sender, EventArgs e)
        {
            PipelineResults.BackColor = Color.FromName("White");

            Status.Text = "Processing...";

            var commands = String.IsNullOrWhiteSpace(PipelineCommands.SelectedText) ? PipelineCommands.Text : PipelineCommands.SelectedText;

            try
            {
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                var pipeline = new Pipeline();

                if (!String.IsNullOrWhiteSpace(DefaultConnectionStringInput.Text))
                {
                    pipeline.SafeSetVariable(DefaultConnectionStringName, DefaultConnectionStringInput.Text.Trim());
                }
                pipeline.AddCommand(commands);
                long parseTime = stopWatch.ElapsedMilliseconds;
                PipelineResults.Text = pipeline.Execute(InputTextbox.Text);
                long executionTime = stopWatch.ElapsedMilliseconds;
                stopWatch.Reset();

                Status.Text = String.Format("{0}ms / {1}ms", parseTime, executionTime);
            }
            catch (Exception exception)
            {
                PipelineResults.Text = exception?.InnerException?.Message ?? exception.Message;
                PipelineResults.BackColor = Color.FromName("LightPink");
                Status.Text = "Error Occured";
            }

            WriteSettings();
        }

        private void PipelineCommands_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.E && Control.ModifierKeys == Keys.Control)
            {
                ExecuteButton_Click(sender, new EventArgs());
            }

            if (e.Control && e.KeyCode == Keys.A)
            {
                PipelineCommands.SelectAll();
            }
        }

        private void WriteSettings()
        {
            var xmlSettings = new XmlWriterSettings()
            {
                NewLineHandling = NewLineHandling.Replace
            };
            using (var xml = XmlWriter.Create(settingsFileLocation, xmlSettings))
            {
                xml.WriteStartDocument();
                xml.WriteStartElement("settings");

                // Last input
                xml.WriteStartElement(lastInputSettingName);
                xml.WriteCData(InputTextbox.Text);
                xml.WriteEndElement();

                // Last commands
                xml.WriteStartElement(lastCommandsSettingName);
                xml.WriteCData(PipelineCommands.Text);
                xml.WriteEndElement();

                // Last include folder
                xml.WriteStartElement(lastIncludeFolderSettingName);
                xml.WriteCData(BaseIncludeFolderInput.Text);
                xml.WriteEndElement();

                // Last include folder
                xml.WriteStartElement(defaultConnectionStringSettingName);
                xml.WriteCData(DefaultConnectionStringInput.Text);
                xml.WriteEndElement();

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
                    InputTextbox.Text = xml.Root.Element(lastInputSettingName).Value.Replace("\n", Environment.NewLine);
                    PipelineCommands.Text = xml.Root.Element(lastCommandsSettingName).Value.Replace("\n", Environment.NewLine);
                    BaseIncludeFolderInput.Text = xml.Root.Element(lastIncludeFolderSettingName).Value;
                    DefaultConnectionStringInput.Text = xml.Root.Element(defaultConnectionStringSettingName).Value;
                }
                catch (Exception e)
                {
                    // Just don't read the settings, I guess
                }

                SetBaseIncludeFolder();
            }
        }

        private void SelectFolderButton_Click(object sender, EventArgs e)
        {
            var folderBrowser = new FolderBrowserDialog();

            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                BaseIncludeFolderInput.Text = folderBrowser.SelectedPath;
            }
        }

        private void BaseIncludeFolderInput_TextChanged(object sender, EventArgs e)
        {
            SetBaseIncludeFolder();
        }

        private void SetBaseIncludeFolder()
        {
            if (!String.IsNullOrWhiteSpace(BaseIncludeFolderInput.Text))
            {
                Pipeline.SetGlobalVariable(DeninaSharp.Core.Filters.File.SANDBOX_VARIABLE_NAME, BaseIncludeFolderInput.Text);
            }
            else
            {
                Pipeline.UnsetGlobalVariable(DeninaSharp.Core.Filters.File.SANDBOX_VARIABLE_NAME);
            }
        }
    }
}