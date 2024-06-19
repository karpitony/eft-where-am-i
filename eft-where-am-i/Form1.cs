using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Web.WebView2.Core;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace eft_where_am_i_chasrp
{
    public partial class Form1 : Form
    {
        private string[] mapList = { "ground-zero", "factory", "customs", "interchange", "woods", "shoreline", "lighthouse", "reserve", "streets", "lab" };
        private string siteUrl;
        private bool whereAmIClick = false;
        private string settingsFile = "settings.json";
        private Settings appSettings;
        private string screenshotPath;

        public Form1()
        {
            InitializeComponent();
            LoadSettings();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeMapComboBox();
            siteUrl = "https://tarkov-market.com/maps/ground-zero";
            webView2.Source = new Uri(siteUrl);
        }
        private void InitializeMapComboBox()
        {
            cmbMapSelect.Items.AddRange(mapList);
            cmbMapSelect.SelectedItem = "ground-zero"; // 기본 입력값 설정
        }
        private void LoadSettings()
        {
            if (File.Exists(settingsFile))
            {
                string json = File.ReadAllText(settingsFile);
                appSettings = JsonConvert.DeserializeObject<Settings>(json);
            }
            else
            {
                appSettings = new Settings
                {
                    auto_screenshot_detection = false,
                    language = "en",
                    screenshot_paths_list = new List<string>
                    {
                        "Documents\\Escape from Tarkov\\Screenshots\\",
                        "문서\\Escape from Tarkov\\Screenshots\\",
                        "OneDrive\\Documents\\Escape from Tarkov\\Screenshots\\",
                        "OneDrive\\문서\\Escape from Tarkov\\Screenshots\\"
                    },
                    screenshot_path = ""
                };
            }

            if (string.IsNullOrEmpty(appSettings.screenshot_path))
            {
                FindAndSetScreenshotPath();
            }
            else
            {
                screenshotPath = appSettings.screenshot_path;
            }
        }

        private void FindAndSetScreenshotPath()
        {
            string homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            foreach (string relativePath in appSettings.screenshot_paths_list)
            {
                string fullPath = Path.Combine(homeDirectory, relativePath);
                if (Directory.Exists(fullPath))
                {
                    screenshotPath = fullPath;
                    appSettings.screenshot_path = fullPath;
                    SaveSettings();
                    break;
                }
            }

            if (string.IsNullOrEmpty(screenshotPath))
            {
                MessageBox.Show("Screenshot directory not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveSettings()
        {
            string json = JsonConvert.SerializeObject(appSettings, Formatting.Indented);
            File.WriteAllText(settingsFile, json);
        }
        private string GetLatestFiles()
        {
            if (!Directory.Exists(screenshotPath))
            {
                return null; // 경로가 존재하지 않으면 null 반환
            }

            var directoryInfo = new DirectoryInfo(screenshotPath);
            var files = directoryInfo.GetFiles().OrderByDescending(f => f.LastWriteTime).ToList();

            if (files.Count == 0)
            {
                return null; // 파일이 없으면 null 반환
            }

            var latestFile = files.First();
            return latestFile.Name; // 가장 최근에 수정된 파일 이름 반환
        }

        

        // Settings class to match the JSON structure
        public class Settings
        {
            public bool auto_screenshot_detection { get; set; }
            public string language { get; set; }
            public List<string> screenshot_paths_list { get; set; }
            public string screenshot_path { get; set; }
        }



        private async Task CheckLocationAsync()
        {
            string screenshot = GetLatestFiles();
            if (screenshot == null)
                return;

            if (!whereAmIClick)
            {
                whereAmIClick = true;
                string jsCodeClick =
                    "var button = document.querySelector('#__nuxt > div > div > div.page-content > div > div > div.panel_top.d-flex > div.d-flex.ml-15.fs-0 > button');\n" +
                    "if (button) {\n" +
                    "    button.click();\n" +
                    "    console.log('Button clicked');\n" +
                    "} else {\n" +
                    "    console.log('Button not found');\n" +
                    "}";
                await webView2.CoreWebView2.ExecuteScriptAsync(jsCodeClick);
                await Task.Delay(500);  // Wait for the button click action to complete
            }

            string jsInputCode =
                "var input = document.querySelector('input[type=\"text\"]');\n" +
                "if (input) {\n" +
                "    input.value = '" + screenshot.Replace(".png", "") + "';\n" +
                "    input.dispatchEvent(new Event('input'));\n" +
                "    console.log('Input value set');\n" +
                "} else {\n" +
                "    console.log('Input not found');\n" +
                "}";
            await webView2.CoreWebView2.ExecuteScriptAsync(jsInputCode);
            ChangeMarker();
        }

        private async void ChangeMarker()
        {
            string[] styleList = {
                "background: #ff0000",
                "height: 20px",
                "width: 20px"
            };

            foreach (var style in styleList)
            {
                string jsCode =
                    "var marker = document.getElementsByClassName('marker')[0];\n" +
                    "if (marker) {\n" +
                    "    marker.style.setProperty('" + style.Split(':')[0].Trim() + "', '" + style.Split(':')[1].Trim() + "', 'important');\n" +
                    "} else {\n" +
                    "    console.log('Marker not found');\n" +
                    "}";
                await webView2.CoreWebView2.ExecuteScriptAsync(jsCode);
            }
        }

        private void btnMapApply_Click(object sender, EventArgs e)
        {
            string selectedMap = cmbMapSelect.SelectedItem.ToString();
            if (selectedMap != null)
            {
                siteUrl = $"https://tarkov-market.com/maps/{selectedMap}";
                webView2.Source = new Uri(siteUrl);
                whereAmIClick = false; // Reset the click state when map changes
            }
        }

        private void chkAutoScreenshot_CheckedChanged(object sender, EventArgs e)
        {
            // Implement auto screenshot detection logic here
        }

        private async void btnHideShowPannel_Click(object sender, EventArgs e)
        {
            string jsCode =
                "var button = document.evaluate('//*[@id=\"__nuxt\"]/div/div/div[2]/div/div/div[1]/div[1]/button', document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue;\n" +
                "if (button) {\n" +
                "    button.click();\n" +
                "    console.log('Panel control button clicked');\n" +
                "} else {\n" +
                "    console.log('Panel control button not found');\n" +
                "}";
            await webView2.CoreWebView2.ExecuteScriptAsync(jsCode);
        }

        private async void btnFullScreen_Click(object sender, EventArgs e)
        {
            string jsCode =
                "var button = document.querySelector('#__nuxt > div > div > div.page-content > div > div > div.panel_top.d-flex > button');\n" +
                "if (button) {\n" +
                "    button.click();\n" +
                "    console.log('Fullscreen button clicked');\n" +
                "} else {\n" +
                "    console.log('Fullscreen button not found');\n" +
                "}";
            await webView2.CoreWebView2.ExecuteScriptAsync(jsCode);
        }

        private async void btnForceRun_Click(object sender, EventArgs e)
        {
            await CheckLocationAsync();
        }

        private void cmbLanguageSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Implement language selection logic here
        }

        private void btnLaguageApply_Click(object sender, EventArgs e)
        {
            // Implement language apply logic here
        }

        private void lblHowtouse_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/karpitony/eft-where-am-i/blob/main/README.md");
        }

        private void lblBugReport_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/karpitony/eft-where-am-i/issues");
        }


    }
}
