using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;
using CMCustomUDP;


namespace GenericTelemetryProvider
{
    public partial class UEVRUI : Form
    {


        public UEVRUI()
        {
            InitializeComponent();

            //for (int i = 0; i < (int)FilterModuleCustom.FilterType.Max; ++i)
            //{
            //    gameComboBox.Items.Add(((FilterModuleCustom.FilterType)i).ToString());
            //}
            //            gameComboBox.SelectedIndex = 0;

            string UEVRUserPath = GetCurrentUserUnrealVRModPath();
            string SpaceMonkeyUEVRPath = MainConfig.installPath + "SpaceMonkeyUEVR\\UnrealVRMod";

            List<string> userDirectories = GetDirectoryNames(UEVRUserPath);
            List<string> spaceMonkeyUEVRDirectories = GetDirectoryNames(SpaceMonkeyUEVRPath);

            // Combine the two lists and remove duplicates using LINQ Union.
            List<string> combinedDirectories = userDirectories
                                                .Union(spaceMonkeyUEVRDirectories)
                                                .ToList();

            foreach(string dir in combinedDirectories)
            {
                gameComboBox.Items.Add(dir);
            }

            if(gameComboBox.Items.Count > 0)
                gameComboBox.SelectedIndex = 0;

        }


        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            if (!IsDisposed)
            {
                Dispose();
            }

            Application.ExitThread();
        }

        private void installButton_Click(object sender, EventArgs e)
        {

            string SpaceMonkeyUEVRSourcePath = Path.Combine(MainConfig.installPath, "SpaceMonkeyUEVR");
            string SpaceMonkeyUEVRPath = Path.Combine(MainConfig.installPath, "SpaceMonkeyUEVR");
            string SpaceMonkeyUEVRModPath = Path.Combine(SpaceMonkeyUEVRPath, "UnrealVRMod");
            string UEVRUserPath = GetCurrentUserUnrealVRModPath();
            string selectedItem = (string)gameComboBox.Items[gameComboBox.SelectedIndex];

            string installPath = Path.Combine(Path.Combine(UEVRUserPath,selectedItem),"plugins");

            //check if install path exists
            if (!Directory.Exists(installPath))
            {
                // Create the directory and any subdirectories.
                Directory.CreateDirectory(installPath);
            }

            //copy config
            string sourceConfigPath = Path.Combine(Path.Combine(Path.Combine(SpaceMonkeyUEVRModPath, selectedItem), "plugins"), "sm_game_config.json");
            string defaultConfigPath = Path.Combine(Path.Combine(SpaceMonkeyUEVRPath, "DefaultProfile"), "sm_game_config.json");

            //check if source config file exists
            if (File.Exists(sourceConfigPath))
            {
                //copy file to install path
                File.Copy(sourceConfigPath, Path.Combine(installPath, "sm_game_config.json"), true);

            } 
            else
            {
                //copy file to install path
                if (File.Exists(defaultConfigPath))
                {
                    File.Copy(defaultConfigPath, Path.Combine(installPath, "sm_game_config.json"), true);
                }
            }


            //copy dlls
            string sourcePluginPath = Path.Combine(SpaceMonkeyUEVRSourcePath, "x64");

            if(Directory.Exists(sourcePluginPath))
            {
                // Get all files in the source directory.
                string[] files = Directory.GetFiles(sourcePluginPath);

                // Copy each file to the destination directory.
                foreach (string filePath in files)
                {
                    // Extract the file name from the path.
                    string fileName = Path.GetFileName(filePath);
                    // Combine the destination directory with the file name.
                    string destFilePath = Path.Combine(installPath, fileName);

                    // Copy the file to the destination, overwriting if it already exists.
                    File.Copy(filePath, destFilePath, true);
                }
            }

        }

        private void gameComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


        private string GetCurrentUserUnrealVRModPath()
        {
            // Get the path to the current user's Roaming folder.
            string appDataRoaming = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            // Combine the path with the "UnrealVRMod" folder.
            return Path.Combine(appDataRoaming, "UnrealVRMod");
        }
                

        private List<string> GetDirectoryNames(string path)
        {
            // Check if the path exists; if not, return an empty list.
            if (!Directory.Exists(path))
            {
                return new List<string>();
            }

            // Get all subdirectory paths and then extract just the directory names.
            return Directory.GetDirectories(path)
                            .Select(Path.GetFileName)
                            .ToList();
        }

    }



}
