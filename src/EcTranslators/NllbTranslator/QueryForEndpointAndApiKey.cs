using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace SilEncConverters40.EcTranslators.NllbTranslator
{
    public partial class QueryForEndpointAndApiKey : Form
    {
        private const string ButtonLabelOverwriteProject = "&Overwrite Existing Project";

        public const string DefaultPort = "8000";
        public const string DefaultModelName = "facebook/nllb-200-distilled-600M";
        public const int DefaultDeviceIndex = -1;
        public const string AddGpuToDockerRunCommandFormat = "--gpus \"device={0}\" ";
        public const string AddDockerCpu = "--index-url https://download.pytorch.org/whl/cpu   # if you switch to using a gpu: comment out: --index-url https://download.pytorch.org/whl/cpu";
        public const string AddDockerGpu = "# --index-url https://download.pytorch.org/whl/cpu   # if you switch to using a cpu: uncomment out: --index-url https://download.pytorch.org/whl/cpu";

        private const int RowStyleIndexUseGpu = 3;

        // these are the files that we write out to build the Docker container from
        private const string FileNameIndexHtml = "index.html";
        private const string FileNameDockerfile = "Dockerfile";
        private const string FileNamePyImportModel = "import_model.py";
        private const string FileNameReadme = "README.md";
        private const string FileNamePs1BuildDocker = "buildDocker.ps1";
        private const string FileNamePyServer = "server.py";
        private const string FileNamePySettings = "settings.py";

        private readonly List<string> FileNames = new List<string>
        {
            FileNameIndexHtml, FileNameDockerfile, FileNamePyImportModel, FileNameReadme, FileNamePs1BuildDocker, FileNamePyServer, FileNamePySettings
        };

        // these are the regular expressions to harvest the existing values of these from the settings.py file
        private readonly Regex _regexApiKey = new Regex("API_KEY = 'SIL-NLLB-Auth-Key (.*?)'");
        private readonly Regex _regexPort = new Regex(@"PORT = (\d+)");
        private readonly Regex _regexModelName = new Regex(@"MODEL_NAME = '(.*?)'");
        private readonly Regex _regexFindPortInEndpoint = new Regex(@":(\d+)");
        private readonly Regex _regexDevice = new Regex(@"DEVICE = (-?\d+)");

        private string _pathToDockerProjectFolder;

        public QueryForEndpointAndApiKey(string pathToDockerProjectFolder, string apiKey, string endpoint)
        {
            InitializeComponent();

            _pathToDockerProjectFolder = pathToDockerProjectFolder;

            var modelName = DefaultModelName;
            if (!Directory.Exists(_pathToDockerProjectFolder))
                Directory.CreateDirectory(_pathToDockerProjectFolder);

            var hasGpu = HasGpu();
            var nDevice = hasGpu ? 0 : DefaultDeviceIndex;
            var action = "create";
            var filesInFolder = Directory.GetFiles(_pathToDockerProjectFolder)?.ToList();
            var filesExist = filesInFolder.Any(fi => FileNames.Any(fn => fi.Contains(fn)));
            if (filesExist)
            {
                buttonOK.Text = ButtonLabelOverwriteProject;
                action = "overwrite";

                var pathToSettingsPy = filesInFolder.FirstOrDefault(fn => fn.Contains(FileNamePySettings));
                if (pathToSettingsPy != null)
                {
                    var settingsFileContents = File.ReadAllText(pathToSettingsPy);

                    // grab the settings currently in the settings file and display them. e.g.:
                    //  API_KEY = 'SIL-NLLB-Auth-Key your-api-key-here'
                    //  PORT = 8000
                    //  MODEL_NAME = 'facebook/nllb-200-distilled-600M'
                    NllbTranslatorEncConverter.SearchForSetting(_regexApiKey, settingsFileContents, ref apiKey);
                    var port = DefaultPort;
                    NllbTranslatorEncConverter.SearchForSetting(_regexFindPortInEndpoint, endpoint, ref port);

                    // port now contains the value that came in from the caller. Keep it so we can replace it after the next step
                    var endpointPort = port;
                    NllbTranslatorEncConverter.SearchForSetting(_regexPort, settingsFileContents, ref port);
                    if (endpointPort != port)
                        endpoint = endpoint.Replace(endpointPort, port);

                    // first check if the model is local
                    string localModelPath = null;
                    NllbTranslatorEncConverter.SearchForSetting(NllbTranslatorEncConverter.RegexLocalModelPath, settingsFileContents, ref localModelPath);
                    if (NllbTranslatorEncConverter.LocalModelFoundExists(localModelPath))
                    {
                        modelName = localModelPath;
                    }
                    else
                    {
                        NllbTranslatorEncConverter.SearchForSetting(_regexModelName, settingsFileContents, ref modelName);
                    }

                    var device = $"{DefaultDeviceIndex}";   // assume cpu
                    NllbTranslatorEncConverter.SearchForSetting(_regexDevice, settingsFileContents, ref device);
                    nDevice = Int32.Parse(device);
                }
            }

            if ((nDevice == DefaultDeviceIndex) && !hasGpu)
            {
                // no GPU found, so hide the row in the table layout panel
                var rowStyle = tableLayoutPanel.RowStyles[RowStyleIndexUseGpu];
                checkBoxUseGpu.Visible = false;
                rowStyle.Height = 0;
            }
            else
            {
                checkBoxUseGpu.Checked = (nDevice != DefaultDeviceIndex);
            }

            toolTip.SetToolTip(buttonOK, $"Click this button to {action} the Docker Project Files in the '{_pathToDockerProjectFolder}' folder with the above settings.");

            // Set the combobox value - if the value exists in Items select it, otherwise put it in the editable text
            if (comboBoxNllbModel.Items.Contains(modelName))
                comboBoxNllbModel.SelectedItem = modelName;
            else
                comboBoxNllbModel.Text = modelName;

            TranslatorApiKey = apiKey;
            Endpoint = endpoint;
        }

        public static bool HasGpu()
        {
            using (var searcher = new ManagementObjectSearcher("select * from Win32_VideoController"))
            {
                foreach (var obj in searcher.Get())
                {
                    var name = obj["Name"]?.ToString();
                    if (name != null && (name.Contains("NVIDIA") || name.Contains("AMD")))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public string TranslatorApiKey
        {
            get
            {
                return textBoxNllbApiKey.Text;
            }
            set
            {
                textBoxNllbApiKey.Text = value;
            }
        }

        public string Endpoint
        {
            get
            {
                return textBoxNllbEndpoint.Text;
            }
            set
            {
                textBoxNllbEndpoint.Text = value;
            }
        }

        private string _modelName;
        public string ModelName
        {
            get => _modelName;
            set => _modelName = value;
        }

        public string ModelNameSuffix
        {
            get
            {
                var index = ModelName.LastIndexOf("-");
                if (index == -1)
                    return null;

                return ModelName.Substring(index).ToLower();
            }
        }

        public string FromLanguageName;

        public string ToLanguageName;

        private string _port = DefaultPort;
        public string Port
        {
            get => _port;
            set => _port = value;
        }

        private int _device = DefaultDeviceIndex;
        public int Device
        {
            get => _device;
            set => _device = value;
        }

        private void buttonOK_Click(object sender, System.EventArgs e)
        {
            if (buttonOK.Text == ButtonLabelOverwriteProject)
            {
                if (DialogResult.No == MessageBox.Show($"Are you sure you want to overwrite the files in the {_pathToDockerProjectFolder} folder?",
                                                       EncConverters.cstrCaption, MessageBoxButtons.YesNo))
                {
                    return;
                }
            }

            // create the files using the entered information. All the model configuration settings will
            //    be stored in the settings.py file and used from there for the other scripts/files.
            var port = DefaultPort;
            NllbTranslatorEncConverter.SearchForSetting(_regexFindPortInEndpoint, textBoxNllbEndpoint.Text, ref port);
            Port = port;

            // use the editable text of the combobox so either a HF model or a local path is supported
            ModelName = comboBoxNllbModel.Text;

            var isLocalModel = radioButtonLocalModel.Checked && NllbTranslatorEncConverter.LocalModelFoundExists(ModelName);

            Device = checkBoxUseGpu.Checked ? 0 : DefaultDeviceIndex;

            //  first the index.html template
            var htmlFilePath = Path.Combine(_pathToDockerProjectFolder, "templates");
            if (!Directory.Exists(htmlFilePath))
                Directory.CreateDirectory(htmlFilePath);
            File.WriteAllText(Path.Combine(htmlFilePath, FileNameIndexHtml), isLocalModel ? Properties.Resources.indexLocalModel : Properties.Resources.index);

            // the Dockerfile
            var cpuAddition = AddDockerCpu;
            if (Device >= 0)
            {
                cpuAddition = AddDockerGpu;
            }

            var dockerFileContents = String.Format(isLocalModel ?  Properties.Resources.DockerfileLocalModel : Properties.Resources.Dockerfile, cpuAddition);
            File.WriteAllText(Path.Combine(_pathToDockerProjectFolder, FileNameDockerfile), dockerFileContents);

            // the import_model.py file (not needed for the local model)
            if (!isLocalModel)
                File.WriteAllText(Path.Combine(_pathToDockerProjectFolder, FileNamePyImportModel),Properties.Resources.import_model);

            // the README.md file
            var gpuAddition = String.Empty;
            if (Device >= 0)
            {
                gpuAddition = String.Format(AddGpuToDockerRunCommandFormat, Device);
            }

            // the settings.py file
            var translatorApiKey = TranslatorApiKey?.Trim();
            if (!String.IsNullOrEmpty(translatorApiKey))
                translatorApiKey = NllbTranslatorEncConverter.NllbAuthenticationPrefix + translatorApiKey;

            // the build powershell script file
            //  NB: this turned out to be a mostly non-starter, since script we would create here can't be run normally
            //    so the instructions now say, look in the README.md file to see what the commands to run manually are.
            //    but for those users who know how to enable this... create it anyway
            var pathToPsScript = Path.Combine(_pathToDockerProjectFolder, FileNamePs1BuildDocker);
            var pathToReadme = Path.Combine(_pathToDockerProjectFolder, FileNameReadme);

            var srcLgCode = "src";
            var trgLgCode = "trg";
            string buildPsScriptContents, readmeFileContents, settingsFileContents;
            if (isLocalModel)
            {
                // for local models, we need to get the source and target lang codes from the config.yaml file, which looks like this:
                //    src: hi-xnrhin_2026_02_16
                //    trg: xnr-xnr_2026_02_16
                // or in regex: "src:\s(.*?)-.*?trg:\s(.*?)-"
                var configFile = Directory.GetFiles(ModelName, "config.yml", SearchOption.AllDirectories).FirstOrDefault();
                if (!String.IsNullOrEmpty(configFile) && File.Exists(configFile))
                {
                    var configFileContents = File.ReadAllText(configFile);
                    var match = Regex.Match(configFileContents, @"src:\s(.*?)-.*?trg:\s(.*?)-", RegexOptions.Singleline);
                    if (match.Success)
                    {
                        FromLanguageName = srcLgCode = match.Groups[1].ToString();
                        ToLanguageName = trgLgCode = match.Groups[2].ToString();
                    }
                }

                buildPsScriptContents = String.Format(Properties.Resources.builddockerLocalModel,
                                                      srcLgCode, trgLgCode, Port, ModelName, gpuAddition);

                string srcLgName, trgLgName;
                NllbTranslatorEncConverter.FindLanguageNames(srcLgCode, trgLgCode, out srcLgName, out trgLgName);

                readmeFileContents = String.Format(Properties.Resources.READMELocalModel, srcLgCode, trgLgCode, Port, ModelName, gpuAddition,
                                                srcLgName, trgLgName);

                settingsFileContents = string.Format(Properties.Resources.settingsLocalModel, $"'{translatorApiKey}'", Port, ModelName, Device, srcLgCode, trgLgCode, srcLgName, trgLgName);
            }
            else
            {
                buildPsScriptContents = String.Format(Properties.Resources.buildDocker, ModelNameSuffix, Port, gpuAddition);
                readmeFileContents = String.Format(Properties.Resources.README, ModelName, Port, gpuAddition);
                settingsFileContents = string.Format(Properties.Resources.settings, $"'{translatorApiKey}'", Port, $"'{ModelName}'", Device);
            }

            File.WriteAllText(pathToPsScript, buildPsScriptContents);
            File.WriteAllText(pathToReadme, readmeFileContents);

            // the server.py file
            File.WriteAllText(Path.Combine(_pathToDockerProjectFolder, FileNamePyServer),
                                isLocalModel ? Properties.Resources.serverLocalModel : Properties.Resources.server);

            File.WriteAllText(Path.Combine(_pathToDockerProjectFolder, FileNamePySettings), settingsFileContents);

            MessageBox.Show($"If you have Docker installed and a recent version of Powershell (see instructions on the About tab), open a Powershell Window and run the two 'docker' commands listed in the '{pathToReadme}' file to build the NLLB Docker project (or run them in the '{pathToPsScript}' script if you know how to enable running scripts from the internet). When finished, return to this message and click 'OK' to connect to the launched endpoint and continue.",
                            EncConverters.cstrCaption);

            LaunchProgram(textBoxNllbEndpoint.Text, null);

            DialogResult = DialogResult.OK;
            Close();
        }

        static protected void LaunchProgram(string strProgram, string strArguments)
        {
            try
            {
                Process myProcess = new Process();

                myProcess.StartInfo.FileName = strProgram;
                myProcess.StartInfo.Arguments = strArguments;
                myProcess.Start();
                // we don't want to wait...
                // myProcess.WaitForExit();    // wait until finished, so we can reinitialize when done add/removing
            }
            catch { }    // we tried...
        }

        // When model location radio selection changes:
        // - If local folder selected: show a FolderBrowserDialog. If user picks a folder, put the path into the combobox text.
        // - If the user cancels the folder picker, revert selection back to Hugging Face.
        private void radioButtonModelLocation_CheckedChanged(object sender, EventArgs e)
        {
            var radioButton = sender as RadioButton;
            if (!radioButton.Checked && (radioButton == radioButtonLocalModel))
            {
                // this means a Local Model path was set in the combo box, but the user clicked the Hugging face radio button
                comboBoxNllbModel.SelectedIndex = 1;
            }

            if (NllbTranslatorEncConverter.LocalModelFoundExists(comboBoxNllbModel.Text))
            {
                if (radioButton.Checked && (radioButton == radioButtonHuggingFace))
                    return;
                if (!radioButtonLocalModel.Checked)
                    radioButtonLocalModel.Checked = true;
                return;
            }
            if ((radioButton.Checked == radioButtonLocalModel.Checked) && radioButtonLocalModel.Checked)
            {
                using (var dlg = new FolderBrowserDialog())
                {
                    dlg.Description = "Select local fine-tuned NLLB model folder";
                    dlg.ShowNewFolderButton = false;
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        var path = dlg.SelectedPath;
                        // Add to combobox items for convenience and set as text/selection
                        if (!comboBoxNllbModel.Items.Contains(path))
                            comboBoxNllbModel.Items.Insert(0, path);
                        comboBoxNllbModel.Text = path;
                    }
                    else
                    {
                        // user cancelled - revert to hugging face option
                        radioButtonHuggingFace.Checked = true;
                    }
                }
            }
            else
            {
                // Hugging Face selected: keep existing items (predefined HF model names).
                // If the combobox currently contains a local path as text, don't change it automatically.
                comboBoxNllbModel.SelectedIndex = 1;
            }
        }

        private void comboBoxNllbModel_TextChanged(object sender, EventArgs e)
        {
            if (NllbTranslatorEncConverter.LocalModelFoundExists(comboBoxNllbModel.Text))
            {
                radioButtonLocalModel.Checked = true;
            }
            else
            {
                radioButtonHuggingFace.Checked = true;
            }
        }
    }
}
