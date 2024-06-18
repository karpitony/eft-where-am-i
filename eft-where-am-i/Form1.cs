using Microsoft.Web.WebView2.Core;
using System;
using System.Windows.Forms;

namespace eft_where_am_i_chasrp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void SelectTheMap_Click(object sender, EventArgs e)
        {
           
        }

        private void Map_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void MapApply_Button_Click(object sender, EventArgs e)
        {

        }
        private void AutoScreenshot_CheckBox_CheckedChanged(object sender, EventArgs e)
        {

        }
        private void HideShowPannel_Button_Click(object sender, EventArgs e)
        {
            string js_code = 
                "var button = document.evaluate('//*[@id=\"__nuxt\"]/div/div/div[2]/div/div/div[1]/div[1]/button', document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue;\n" +
                "if (button) {\n" +
                "    button.click();\n" +
                "    console.log('Panel control button clicked');\n" +
                "} else {\n" +
                "    console.log('Panel control button not found');\n" +
                "}";

            webView2.CoreWebView2.ExecuteScriptAsync(js_code);
        }

        private void FullScreen_Button_Click(object sender, EventArgs e)
        {
            string js_code =
                "var button = document.querySelector('#__nuxt > div > div > div.page-content > div > div > div.panel_top.d-flex > button');\n" +
                "if (button) {\n" +
                "    button.click();\n" +
                "    console.log('Fullscreen button clicked');\n" +
                "} else {\n" +
                "    console.log('Fullscreen button not found');\n" +
                "}";

            webView2.CoreWebView2.ExecuteScriptAsync(js_code);
        }

        private void ForceRun_Button_Click(object sender, EventArgs e)
        {

        }
        private void LanguageSelect_Combobox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void LaguageApply_Button_Click(object sender, EventArgs e)
        {

        }

        private void HowtouseLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        private void BugReport_LinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }
    }
}
