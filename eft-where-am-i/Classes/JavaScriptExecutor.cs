using System;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Web.WebView2.WinForms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

        public async Task<bool> CheckInputAble()
        {
            string script = $@"
            (function() {{
                const buttons = document.querySelectorAll('button');
                for (const btn of buttons) {{
                    if (btn.textContent.trim() === 'Where am i?') {{
                        const parent = btn.parentElement;
                        if (!parent) return false;

                        const input = parent.querySelector('input');
                        if (!input) return false;

                        const isVisible = input.offsetParent !== null;
                        const isEnabled = !input.disabled && !input.readOnly;

                        return isVisible && isEnabled;
                    }}
                }}
                return false;
            }})()
            ";
            try
            {
                string result = await webView.ExecuteScriptAsync(script);
                return result.Trim().ToLower() == "true";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"JS 실행 실패: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 텍스트 입력 필드에 값을 설정하는 JavaScript 코드 실행
        /// Vue/React 호환 방식으로 nativeInputValueSetter + InputEvent를 사용합니다.
        /// </summary>
        /// <param name="selector">입력 필드의 CSS 셀렉터</param>
        /// <param name="value">설정할 값</param>
        public async Task SetInputValueAsync(string selector, string value)
        {
            string escapedValue = value.Replace("\\", "\\\\").Replace("'", "\\'");
            string script = $@"
                (function() {{
                    var input = document.querySelector('{selector}');
                    if (!input) {{ console.log('Input not found'); return; }}

                    // Use native setter to bypass Vue/React getter/setter
                    var nativeSetter = Object.getOwnPropertyDescriptor(window.HTMLInputElement.prototype, 'value').set;
                    nativeSetter.call(input, '{escapedValue}');

                    // Dispatch multiple events for framework compatibility
                    input.dispatchEvent(new InputEvent('input', {{ bubbles: true, cancelable: true, inputType: 'insertText', data: '{escapedValue}' }}));
                    input.dispatchEvent(new Event('change', {{ bubbles: true }}));

                    // Also try focus/blur to trigger validation
                    input.focus();
                    input.blur();

                    console.log('Input value set to: ' + input.value);
                }})();";
            await ExecuteScriptAsync(script);
        }

        /// <summary>
        /// 퀘스트 컨테이너가 로드될 때까지 폴링하며 대기합니다.
        /// Nuxt/SPA 페이지의 동적 렌더링 완료를 감지합니다.
        /// </summary>
        /// <param name="timeoutMs">최대 대기 시간 (밀리초)</param>
        /// <returns>컨테이너 로드 성공 여부</returns>
        public async Task<bool> WaitForQuestContainerAsync(int timeoutMs = 15000)
        {
            string script = @"
            (function() {
                const container = document.querySelector('div.items.scroll');
                return container && container.children.length > 0;
            })()";

            var startTime = DateTime.Now;
            while ((DateTime.Now - startTime).TotalMilliseconds < timeoutMs)
            {
                try
                {
                    await EnsureWebViewInitializedAsync();
                    if (webView.CoreWebView2 == null)
                    {
                        await Task.Delay(500);
                        continue;
                    }

                    string result = await webView.CoreWebView2.ExecuteScriptAsync(script);
                    if (result.Trim().ToLower() == "true")
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[JS] WaitForQuestContainer polling error: {ex.Message}");
                }

                await Task.Delay(500);
            }

            Console.WriteLine($"[JS] WaitForQuestContainer timed out after {timeoutMs}ms");
            return false;
        }

        /// <summary>
        /// 현재 선택된 퀘스트 이름 목록을 추출합니다.
        /// 선택된 퀘스트는 'selected' CSS 클래스로 구분됩니다.
        /// </summary>
        public async Task<List<string>> GetSelectedQuestsAsync()
        {
            string script = @"
            (function() {
                const container = document.querySelector('div.items.scroll');
                if (!container) return JSON.stringify([]);
                const items = container.querySelectorAll('div.no-wrap.d-flex');
                const selected = [];
                for (const item of items) {
                    if (item.classList.contains('selected')) {
                        const span = item.querySelector('span:not(.alt)');
                        if (span) {
                            selected.push(span.innerText.trim());
                        }
                    }
                }
                return JSON.stringify(selected);
            })()";

            try
            {
                string result = await webView.ExecuteScriptAsync(script);
                // Result comes back as a JSON string wrapped in quotes
                if (string.IsNullOrEmpty(result) || result == "null")
                    return new List<string>();

                // Unescape the JSON string (WebView2 returns it double-encoded)
                string json = JsonConvert.DeserializeObject<string>(result);
                return JsonConvert.DeserializeObject<List<string>>(json) ?? new List<string>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[JS] GetSelectedQuests error: {ex.Message}");
                return new List<string>();
            }
        }

        /// <summary>
        /// 퀘스트 이름으로 해당 퀘스트를 우클릭하여 선택합니다.
        /// </summary>
        public async Task SelectQuestByNameAsync(string questName)
        {
            string escapedName = questName.Replace("'", "\\'").Replace("\"", "\\\"");
            string script = $@"
            (function() {{
                console.log('[Quest Load] Searching for:', '{escapedName}');

                const container = document.querySelector('div.items.scroll');
                if (!container) {{
                    console.log('[Quest Load] Container not found');
                    return;
                }}

                const items = container.querySelectorAll('div.no-wrap.d-flex');
                console.log('[Quest Load] Items count:', items.length);

                for (const item of items) {{
                    const span = item.querySelector('span:not(.alt)');
                    if (span && span.innerText.trim() === '{escapedName}') {{
                        console.log('[Quest Load] Found, dispatching contextmenu');
                        item.dispatchEvent(new MouseEvent('contextmenu', {{ bubbles: true, cancelable: true }}));
                        return;
                    }}
                }}

                console.log('[Quest Load] Quest not found in list');
            }})()";

            await ExecuteScriptAsync(script);
        }

        /// <summary>
        /// 층 버튼을 클릭하여 맵 층을 전환합니다.
        /// </summary>
        public async Task ClickFloorAsync(string floorName)
        {
            string escapedName = floorName.Replace("'", "\\'");
            string script = $@"
            (function() {{
                const inputs = document.querySelectorAll('.no-wrap input[name=""layers""]');
                for (const input of inputs) {{
                    if (input.parentNode.innerText.includes('{escapedName}')) {{
                        input.click();
                        break;
                    }}
                }}
            }})()";

            await ExecuteScriptAsync(script);
        }

        /// <summary>
        /// 층 버튼을 이름으로 찾아 클릭합니다. 매칭되면 true, 없으면 false를 반환합니다.
        /// </summary>
        public async Task<bool> ClickFloorByNameAsync(string floorName)
        {
            try
            {
                await EnsureWebViewInitializedAsync();
                if (webView.CoreWebView2 == null) return false;

                string escapedName = floorName.Replace("'", "\\'");
                string script = $@"
                (function() {{
                    const inputs = document.querySelectorAll('.no-wrap input[name=""layers""]');
                    for (const input of inputs) {{
                        if (input.parentNode.innerText.includes('{escapedName}')) {{
                            input.click();
                            return 'true';
                        }}
                    }}
                    return 'false';
                }})()";

                string result = await webView.CoreWebView2.ExecuteScriptAsync(script);
                return result?.Trim('"') == "true";
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 여러 층 이름 후보를 한 번의 JS 호출로 시도합니다. 순서대로 매칭하여 첫 번째 매칭을 클릭.
        /// </summary>
        public async Task<bool> ClickFloorByFirstMatchAsync(string[] floorNames)
        {
            try
            {
                await EnsureWebViewInitializedAsync();
                if (webView.CoreWebView2 == null) return false;

                string jsArray = "[" + string.Join(",", floorNames.Select(n => $"'{n.Replace("'", "\\'")}'")) + "]";

                string script = $@"
                (function() {{
                    const names = {jsArray};
                    const inputs = document.querySelectorAll('.no-wrap input[name=""layers""]');
                    for (const name of names) {{
                        for (const input of inputs) {{
                            if (input.parentNode.innerText.includes(name)) {{
                                input.click();
                                return 'true';
                            }}
                        }}
                    }}
                    return 'false';
                }})()";

                string result = await webView.CoreWebView2.ExecuteScriptAsync(script);
                return result?.Trim('"') == "true";
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 층 버튼을 인덱스로 클릭하여 맵 층을 전환합니다.
        /// index 0 = 마지막 버튼 (Underground), index 1~4 = (index-1)번째 버튼.
        /// </summary>
        public async Task ClickFloorByIndexAsync(int index)
        {
            string script = $@"
            (function() {{
                const inputs = document.querySelectorAll('.no-wrap input[name=""layers""]');
                if (inputs.length === 0) return;
                var target;
                if ({index} === 0) {{
                    target = inputs[inputs.length - 1];
                }} else {{
                    var i = {index} - 1;
                    if (i < inputs.length) target = inputs[i];
                }}
                if (target) target.click();
            }})()";

            await ExecuteScriptAsync(script);
        }

        /// <summary>
        /// 마커의 CSS left/top 위치를 읽습니다.
        /// </summary>
        private async Task<(bool found, double left, double top)> ReadMarkerPositionAsync()
        {
            try
            {
                string raw = await webView.CoreWebView2.ExecuteScriptAsync(Constants.READ_MARKER_POSITION_SCRIPT);
                if (string.IsNullOrEmpty(raw) || raw == "null") return (false, 0, 0);

                string json = JsonConvert.DeserializeObject<string>(raw);
                var obj = JObject.Parse(json);
                bool found = obj["found"]?.Value<bool>() ?? false;
                if (!found) return (false, 0, 0);

                double left = obj["left"]?.Value<double>() ?? 0;
                double top = obj["top"]?.Value<double>() ?? 0;
                int count = obj["count"]?.Value<int>() ?? 0;
                AppLogger.Debug("Calibration", $"Marker pos: left={left}, top={top}, count={count}");
                return (true, left, top);
            }
            catch (Exception ex)
            {
                AppLogger.Error("Calibration", $"ReadMarkerPosition error: {ex.Message}");
                return (false, 0, 0);
            }
        }

        /// <summary>
        /// C# 오케스트레이션 방식 캘리브레이션.
        /// 기존에 잘 되는 SetInputValueAsync를 사용하여 프로브 좌표를 입력하고,
        /// 마커의 CSS left/top으로 pixel 좌표를 읽습니다.
        /// </summary>
        public async Task<bool> CalibrateMapAsync()
        {
            try
            {
                AppLogger.Info("Calibration", "Starting C#-orchestrated calibration...");

                await EnsureWebViewInitializedAsync();
                if (webView.CoreWebView2 == null)
                {
                    AppLogger.Error("Calibration", "CoreWebView2 is null");
                    return false;
                }

                // Save original input value
                string origRaw = await webView.CoreWebView2.ExecuteScriptAsync(
                    "(function(){ var i = document.querySelector('input[type=\"text\"]'); return i ? i.value : ''; })()");
                string originalValue = origRaw?.Trim('"') ?? "";
                AppLogger.Debug("Calibration", $"Original input value: [{originalValue}]");

                // Probe A: set (0,0,0) with proper filename format
                // Format: YYYY-MM-DD[HH-MM]_x, y, z_quatX, quatY, quatZ, quatW_speed
                string probeA = "2000-01-01[00-00]_0.00, 0.00, 0.00_0.00, 0.00, 0.00, 1.00_0.00";
                AppLogger.Debug("Calibration", $"Setting probe A: {probeA}");
                await SetInputValueAsync("input[type=\"text\"]", probeA);
                await Task.Delay(2000);

                var posA = await ReadMarkerPositionAsync();
                if (!posA.found)
                {
                    AppLogger.Error("Calibration", "Probe A: no marker found");
                    await SetInputValueAsync("input[type=\"text\"]", originalValue);
                    return false;
                }
                AppLogger.Info("Calibration", $"Probe A: left={posA.left}, top={posA.top}");

                // Probe B: set (1000,0,1000) with proper filename format
                // Note: y is height in Tarkov, so we change x and z for map X/Y axes
                string probeB = "2000-01-01[00-00]_1000.00, 0.00, 1000.00_0.00, 0.00, 0.00, 1.00_0.00";
                AppLogger.Debug("Calibration", $"Setting probe B: {probeB}");
                await SetInputValueAsync("input[type=\"text\"]", probeB);
                await Task.Delay(2000);

                var posB = await ReadMarkerPositionAsync();
                if (!posB.found)
                {
                    AppLogger.Error("Calibration", "Probe B: no marker found");
                    await SetInputValueAsync("input[type=\"text\"]", originalValue);
                    return false;
                }
                AppLogger.Info("Calibration", $"Probe B: left={posB.left}, top={posB.top}");

                // Check positions differ
                if (Math.Abs(posA.left - posB.left) < 1 && Math.Abs(posA.top - posB.top) < 1)
                {
                    AppLogger.Error("Calibration", "Probes at same position - marker did not move");
                    await SetInputValueAsync("input[type=\"text\"]", originalValue);
                    return false;
                }

                // Calculate affine: pixel = game * scale + offset
                double scaleX = (posB.left - posA.left) / 1000.0;
                double scaleY = (posB.top - posA.top) / 1000.0;
                double offsetX = posA.left;
                double offsetY = posA.top;

                AppLogger.Info("Calibration",
                    $"Success: scaleX={scaleX:F4}, scaleY={scaleY:F4}, offsetX={offsetX:F1}, offsetY={offsetY:F1}");

                // Inject JS conversion functions
                string injectScript = string.Format(
                    System.Globalization.CultureInfo.InvariantCulture,
                    Constants.INJECT_CALIBRATION_FUNCTIONS,
                    scaleX, scaleY, offsetX, offsetY);
                await webView.CoreWebView2.ExecuteScriptAsync(injectScript);

                // Restore original input
                await SetInputValueAsync("input[type=\"text\"]", originalValue);

                return true;
            }
            catch (Exception ex)
            {
                AppLogger.Error("Calibration", $"Exception: {ex.Message}\n{ex.StackTrace}");
                return false;
            }
        }

        /// <summary>
        /// 맵 위에 폴리곤 오버레이 + 에디터 UI를 주입합니다.
        /// </summary>
        public async Task EnableFloorEditModeAsync(string existingZonesJson, string floorsJson)
        {
            // Inject overlay
            await ExecuteScriptAsync(Constants.POLYGON_OVERLAY_SCRIPT);

            // Set existing zones data
            string escapedZonesJson = existingZonesJson.Replace("\\", "\\\\").Replace("'", "\\'");
            await ExecuteScriptAsync($"window.__floorEditorZones = JSON.parse('{escapedZonesJson}');");

            // Set floors data for select dropdown
            string escapedFloorsJson = floorsJson.Replace("\\", "\\\\").Replace("'", "\\'");
            await ExecuteScriptAsync($"window.__floorEditorFloors = JSON.parse('{escapedFloorsJson}');");

            // Render existing zones
            await ExecuteScriptAsync("if(window.__renderFloorZones) window.__renderFloorZones(window.__floorEditorZones);");

            // Inject editor UI
            await ExecuteScriptAsync(Constants.FLOOR_EDITOR_UI_SCRIPT);
        }

        /// <summary>
        /// 폴리곤 오버레이 + 에디터 UI를 제거합니다.
        /// </summary>
        public async Task DisableFloorEditModeAsync()
        {
            await ExecuteScriptAsync(@"
            (function() {
                var overlay = document.getElementById('floor-polygon-overlay');
                if (overlay) overlay.remove();
                var panel = document.getElementById('floor-editor-panel');
                if (panel) panel.remove();
                window.__floorEditClickEnabled = false;
                window.__floorEditorZones = [];
                window.__floorCurrentVertices = [];
            })();");
        }

        /// <summary>
        /// CDP (Chrome DevTools Protocol)를 사용하여 trusted 마우스 드래그를 실행합니다.
        /// isTrusted: true 이벤트를 생성하여 tarkov-market의 맵 핸들러가 인식합니다.
        /// </summary>
        /// <param name="animate">true면 각 단계 사이에 딜레이를 주어 부드러운 애니메이션 효과</param>
        private async Task<bool> CdpMouseDragAsync(double startX, double startY, double endX, double endY, int steps = 10, bool animate = true)
        {
            try
            {
                var cdp = webView.CoreWebView2;

                // mousePressed at start position
                await cdp.CallDevToolsProtocolMethodAsync("Input.dispatchMouseEvent",
                    string.Format(CultureInfo.InvariantCulture,
                        "{{\"type\":\"mousePressed\",\"x\":{0},\"y\":{1},\"button\":\"left\",\"clickCount\":1}}",
                        startX, startY));

                // mouseMoved in steps (with optional delay for animation)
                int delayMs = animate ? 20 : 0;
                for (int s = 1; s <= steps; s++)
                {
                    double t = (double)s / steps;
                    double cx = startX + (endX - startX) * t;
                    double cy = startY + (endY - startY) * t;
                    await cdp.CallDevToolsProtocolMethodAsync("Input.dispatchMouseEvent",
                        string.Format(CultureInfo.InvariantCulture,
                            "{{\"type\":\"mouseMoved\",\"x\":{0},\"y\":{1},\"button\":\"left\",\"buttons\":1}}",
                            cx, cy));

                    if (delayMs > 0 && s < steps)
                        await Task.Delay(delayMs);
                }

                // mouseReleased at end position
                await cdp.CallDevToolsProtocolMethodAsync("Input.dispatchMouseEvent",
                    string.Format(CultureInfo.InvariantCulture,
                        "{{\"type\":\"mouseReleased\",\"x\":{0},\"y\":{1},\"button\":\"left\",\"clickCount\":1}}",
                        endX, endY));

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AutoPan] CDP drag failed: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 마커가 데드존 밖에 있으면 자동으로 맵을 팬합니다.
        /// 1차: __deadZoneCalc()로 좌표 계산 → CDP trusted mouse drag
        /// 2차: CDP 실패 시 __deadZoneAutoPanCSS() CSS fallback (애니메이션 지원)
        /// </summary>
        /// <param name="deadZonePercent">내부 경계 비율 (93 = 가장자리 3.5% 여백)</param>
        /// <param name="animate">CSS fallback 시 애니메이션 적용 여부</param>
        /// <returns>팬 실행 여부</returns>
        public async Task<bool> AutoPanToMarkerAsync(int deadZonePercent = 93, bool animate = true)
        {
            try
            {
                await EnsureWebViewInitializedAsync();
                if (webView.CoreWebView2 == null) return false;

                // Step 1: __deadZoneCalc()로 팬 필요 여부 및 좌표 계산
                string calcScript = $"(function(){{ return window.__deadZoneCalc ? window.__deadZoneCalc({deadZonePercent}) : JSON.stringify({{ needsPan: false, reason: 'not-injected' }}); }})()";
                string calcRaw = await webView.CoreWebView2.ExecuteScriptAsync(calcScript);

                if (string.IsNullOrEmpty(calcRaw) || calcRaw == "null")
                    return false;

                string calcJson = JsonConvert.DeserializeObject<string>(calcRaw);
                var calcObj = JObject.Parse(calcJson);
                bool needsPan = calcObj["needsPan"]?.Value<bool>() ?? false;
                string reason = calcObj["reason"]?.ToString() ?? "";

                if (!needsPan)
                {
                    // inside-boundary 또는 no-marker/no-map
                    return false;
                }

                double dx = calcObj["dx"]?.Value<double>() ?? 0;
                double dy = calcObj["dy"]?.Value<double>() ?? 0;
                double startX = calcObj["startX"]?.Value<double>() ?? 0;
                double startY = calcObj["startY"]?.Value<double>() ?? 0;
                double endX = calcObj["endX"]?.Value<double>() ?? 0;
                double endY = calcObj["endY"]?.Value<double>() ?? 0;

                // Step 2: CDP trusted mouse drag 시도 (애니메이션 옵션 포함)
                bool cdpSuccess = await CdpMouseDragAsync(startX, startY, endX, endY, steps: 10, animate: animate);
                if (cdpSuccess)
                {
                    Console.WriteLine($"[AutoPan] Panned via CDP: dx={dx}, dy={dy}");
                    return true;
                }

                // Step 3: CDP 실패 시 CSS transform fallback (애니메이션 옵션 포함)
                string animateJs = animate ? "true" : "false";
                string fallbackScript = $"(function(){{ return window.__deadZoneAutoPanCSS ? window.__deadZoneAutoPanCSS({deadZonePercent}, {animateJs}) : JSON.stringify({{ panned: false, reason: 'not-injected' }}); }})()";
                string fallbackRaw = await webView.CoreWebView2.ExecuteScriptAsync(fallbackScript);

                if (!string.IsNullOrEmpty(fallbackRaw) && fallbackRaw != "null")
                {
                    string fallbackJson = JsonConvert.DeserializeObject<string>(fallbackRaw);
                    var fallbackObj = JObject.Parse(fallbackJson);
                    bool fallbackPanned = fallbackObj["panned"]?.Value<bool>() ?? false;

                    if (fallbackPanned)
                    {
                        Console.WriteLine($"[AutoPan] Panned via CSS fallback: dx={fallbackObj["dx"]}, dy={fallbackObj["dy"]}, animated={animate}");
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AutoPan] Error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 패널이 현재 숨겨져 있는지 확인합니다.
        /// 버튼 텍스트가 "Show pannels"를 포함하면 패널이 숨겨진 상태입니다.
        /// </summary>
        public async Task<bool> IsPanelHiddenAsync()
        {
            try
            {
                await EnsureWebViewInitializedAsync();
                if (webView.CoreWebView2 == null) return false;

                string script = @"
                (function() {
                    var btn = document.querySelector('#__nuxt > div > div > div.page-content > div > div > div.panel_top > div > div.mr-15 > button');
                    if (!btn) return 'false';
                    return btn.textContent.includes('Show pannels') ? 'true' : 'false';
                })()";

                string result = await webView.CoreWebView2.ExecuteScriptAsync(script);
                return result?.Trim('"') == "true";
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 퀘스트 컨테이너에 우클릭 리스너를 주입하여 퀘스트 선택/해제를 감지합니다.
        /// 퀘스트가 토글되면 WebMessage로 C#에 전송합니다.
        /// </summary>
        public async Task InjectQuestClickListenerAsync()
        {
            string script = @"
            (function() {
                const container = document.querySelector('div.items.scroll');
                if (!container || container.dataset.questListenerAttached) return;
                container.dataset.questListenerAttached = 'true';

                container.addEventListener('contextmenu', function(e) {
                    const item = e.target.closest('div.no-wrap.d-flex');
                    if (!item) return;

                    const span = item.querySelector('span:not(.alt)');
                    if (!span) return;

                    const questName = span.innerText.trim();

                    // 약간의 딜레이 후 상태 확인 (클릭 처리 완료 대기)
                    setTimeout(() => {
                        const isSelected = item.classList.contains('selected');

                        console.log('[Quest Save]', questName, 'isSelected:', isSelected);
                        console.log('[Quest Save] classList:', item.classList.toString());

                        window.chrome.webview.postMessage(JSON.stringify({
                            action: 'quest-toggled',
                            questName: questName,
                            isSelected: isSelected
                        }));
                    }, 100);
                }, true);

                console.log('[Quest] Click listener attached');
            })()";

            await ExecuteScriptAsync(script);
        }
    }
}
