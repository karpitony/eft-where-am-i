using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;

namespace eft_where_am_i.Classes
{
    public class JavaScriptExecutor
    {
        private readonly WebView2 webView;

        public JavaScriptExecutor(WebView2 webView)
        {
            this.webView = webView ?? throw new ArgumentNullException(nameof(webView));
        }

        /// <summary>
        /// WebView2 초기화 완료 대기
        /// </summary>
        private async Task EnsureWebViewInitializedAsync()
        {
            if (webView.CoreWebView2 == null)
            {
                await webView.EnsureCoreWebView2Async(null);
            }
        }

        /// <summary>
        /// JavaScript 코드를 실행합니다.
        /// </summary>
        /// <param name="script">실행할 JavaScript 코드</param>
        public async Task ExecuteScriptAsync(string script)
        {
            try
            {
                await EnsureWebViewInitializedAsync(); // 초기화 대기

                if (webView.CoreWebView2 != null)
                {
                    await webView.CoreWebView2.ExecuteScriptAsync(script);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"JavaScript 실행 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 특정 버튼을 클릭하는 JavaScript 코드 실행
        /// </summary>
        /// <param name="selector">버튼의 CSS 셀렉터</param>
        public async Task ClickButtonAsync(string selector)
        {
            string script = $@"
                var button = document.querySelector('{selector}');
                if (button) {{
                    button.click();
                    console.log('Button clicked');
                }} else {{
                    console.log('Button not found');
                }}";
            await ExecuteScriptAsync(script);
        }

        /// <summary>
        /// 텍스트 입력 필드에 값을 설정하는 JavaScript 코드 실행
        /// </summary>
        /// <param name="selector">입력 필드의 CSS 셀렉터</param>
        /// <param name="value">설정할 값</param>
        public async Task SetInputValueAsync(string selector, string value)
        {
            string script = $@"
                var input = document.querySelector('{selector}');
                if (input) {{
                    input.value = '{value}';
                    input.dispatchEvent(new Event('input'));
                    console.log('Input value set');
                }} else {{
                    console.log('Input not found');
                }}";
            await ExecuteScriptAsync(script);
        }
    }
}
