using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using eft_where_am_i.Classes;

namespace eft_where_am_i
{
    public partial class ServerLocation : UserControl
    {
        private string OFFLINE_MSG = "오프라인 / 매칭안됨";
        private dynamic _langStrings;
        
        // IP 별 위치 정보를 저장하는 캐시 딕셔너리 (앱 구동 중 유지)
        private Dictionary<string, dynamic> _geoCache = new Dictionary<string, dynamic>();

        public ServerLocation()
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint |
                          ControlStyles.OptimizedDoubleBuffer, true);

            typeof(DataGridView).InvokeMember("DoubleBuffered", 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Instance | 
                System.Reflection.BindingFlags.SetProperty, 
                null, dataGridViewHistory, new object[] { true });

            LoadLanguage();
            SettingsHandler.Instance.SettingsChanged += (s) =>
            {
                LoadLanguage();
            };
        }

        private void LoadLanguage()
        {
            try
            {
                string lang = SettingsHandler.Instance.GetSettings().language;
                if (string.IsNullOrEmpty(lang)) lang = "en";
                string jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "translations", $"{lang}.json");
                if (File.Exists(jsonPath))
                {
                    string json = File.ReadAllText(jsonPath);
                    _langStrings = JsonConvert.DeserializeObject(json);
                }
                
                if (_langStrings != null)
                {
                    ApplyTranslations();
                }
            }
            catch (Exception) { }
        }

        private string GetString(string key, string fallback)
        {
            if (_langStrings != null && _langStrings[key] != null)
                return _langStrings[key].ToString();
            return fallback;
        }

        private void ApplyTranslations()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(ApplyTranslations));
                return;
            }
            
            groupBox1.Text = GetString("serverLocation_Title", groupBox1.Text);
            lblHistory.Text = GetString("serverLocation_HistoryTitle", lblHistory.Text);
            btnFindLatest.Text = GetString("serverLocation_BtnFindLatest", btnFindLatest.Text);
            
            colCheck.HeaderText = GetString("serverLocation_ColCheck", colCheck.HeaderText);
            colCheck.Text = GetString("serverLocation_BtnCheck", colCheck.Text);
            colDate.HeaderText = GetString("serverLocation_ColDate", colDate.HeaderText);
            colIp.HeaderText = GetString("serverLocation_ColIp", colIp.HeaderText);
            colCity.HeaderText = GetString("serverLocation_ColCity", colCity.HeaderText);
            colFolder.HeaderText = GetString("serverLocation_ColFolder", colFolder.HeaderText);
            
            string oldOfflineMsg = OFFLINE_MSG;
            OFFLINE_MSG = GetString("serverLocation_OfflineCol", OFFLINE_MSG);
            
            // 기존 표에 들어가 있는 오프라인 메시지도 실시간 변경
            if (oldOfflineMsg != OFFLINE_MSG && dataGridViewHistory.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in dataGridViewHistory.Rows)
                {
                    if (row.Cells["colIp"].Value?.ToString() == oldOfflineMsg)
                    {
                        row.Cells["colIp"].Value = OFFLINE_MSG;
                    }
                }
            }
            
            if (lblCurrentLogFile.Text.Contains("Current Log") || lblCurrentLogFile.Text.Contains("선택된 로그"))
            {
                lblCurrentLogFile.Text = GetString("serverLocation_CurrentLog", "Current Log: ") + "None";
            }
            
            ClearGeoLocationLabels();
        }

        private async void ServerLocation_Load(object sender, EventArgs e)
        {
            if (this.DesignMode) return;
            await LoadAllHistoryAsync();
        }

        private async void btnFindLatest_Click(object sender, EventArgs e)
        {
            btnFindLatest.Enabled = false;
            btnFindLatest.Text = "...";
            await LoadAllHistoryAsync();
            btnFindLatest.Text = GetString("serverLocation_BtnFindLatest", "최신 접속 갱신");
            btnFindLatest.Enabled = true;
        }

        private async Task LoadAllHistoryAsync()
        {
            try
            {
                string logsFolderPath = SettingsHandler.Instance.GetOrFindLogPath();

                if (string.IsNullOrEmpty(logsFolderPath) || !Directory.Exists(logsFolderPath))
                {
                    return;
                }

                dataGridViewHistory.Rows.Clear();

                await Task.Run(() =>
                {
                    var dirs = new DirectoryInfo(logsFolderPath)
                        .GetDirectories()
                        .OrderByDescending(d => d.CreationTime)
                        .ToList();

                    foreach (var dir in dirs)
                    {
                        var latestFile = dir.GetFiles("*application*.log")
                            .OrderByDescending(f => f.LastWriteTime)
                            .FirstOrDefault();

                        string ip = OFFLINE_MSG;
                        string cityVal = "";

                        if (latestFile != null)
                        {
                            ip = GetMatchedIpAddress(latestFile.FullName) ?? OFFLINE_MSG;
                        }

                        // 캐시에 있는 경우 도시명 바로 주입
                        if (ip != OFFLINE_MSG && _geoCache.ContainsKey(ip))
                        {
                            cityVal = _geoCache[ip]["city"]?.ToString();
                        }

                        this.Invoke((MethodInvoker)delegate
                        {
                            dataGridViewHistory.Rows.Add(
                                GetString("serverLocation_BtnCheck", "확인하기"), 
                                dir.CreationTime.ToString("yyyy-MM-dd HH:mm:ss"), 
                                ip, 
                                cityVal, /* 새로운 City 컬럼 (캐시에 없으면 공백) */
                                dir.Name
                            );
                        });
                    }
                });

                if (dataGridViewHistory.Rows.Count > 0)
                {
                    var firstRow = dataGridViewHistory.Rows[0];
                    string latestIp = firstRow.Cells["colIp"].Value?.ToString();
                    string folderName = firstRow.Cells["colFolder"].Value?.ToString();
                    
                    lblCurrentLogFile.Text = GetString("serverLocation_CurrentLog", "선택된 로그: ") + $"{folderName} (*application*.log)";

                    if (string.IsNullOrEmpty(latestIp) || !IPAddress.TryParse(latestIp, out _))
                    {
                        ClearGeoLocationLabels();
                        labelIpAddress.Text = GetString("serverLocation_IpText", "IP 주소 : ") + OFFLINE_MSG;
                        labelCountryName.Text = GetString("serverLocation_OfflineInfo", "해당 세션은 오프라인 게임이거나 아직 서버가 잡히지 않았습니다.");
                    }
                    else
                    {
                        await UpdateGeoLocationAsync(latestIp, firstRow);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"오류 발생: {ex.Message}");
            }
        }

        private string GetMatchedIpAddress(string logFilePath)
        {
            try
            {
                string matchedIpAddress = null;

                using (FileStream fileStream = new FileStream(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            Match match = Regex.Match(line, @"Ip:\s?(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})");

                            if (match.Success)
                            {
                                matchedIpAddress = match.Groups[1].Value;
                            }
                        }
                    }
                }

                return matchedIpAddress;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private async void dataGridViewHistory_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == colCheck.Index)
            {
                var row = dataGridViewHistory.Rows[e.RowIndex];
                string ip = row.Cells["colIp"].Value?.ToString();
                string folderName = row.Cells["colFolder"].Value?.ToString();

                lblCurrentLogFile.Text = GetString("serverLocation_CurrentLog", "선택된 로그: ") + $"{folderName} (*application*.log)";

                if (string.IsNullOrEmpty(ip) || !IPAddress.TryParse(ip, out _))
                {
                    ClearGeoLocationLabels();
                    labelIpAddress.Text = GetString("serverLocation_IpText", "IP 주소 : ") + OFFLINE_MSG;
                    labelCountryName.Text = GetString("serverLocation_OfflineInfo", "해당 세션은 오프라인 게임이거나 아직 매칭된 서버가 없습니다.");
                    row.Cells["colCity"].Value = "-";
                }
                else
                {
                    await UpdateGeoLocationAsync(ip, row);
                }
            }
        }

        private void ClearGeoLocationLabels()
        {
            labelIpAddress.Text = GetString("serverLocation_IpText", "IP 주소 : ");
            labelCountryName.Text = GetString("serverLocation_CountryText", "국가 : ");
            labelRegionName.Text = GetString("serverLocation_RegionText", "지역 : ");
            labelCityName.Text = GetString("serverLocation_CityText", "도시 : ");
        }

        private async Task UpdateGeoLocationAsync(string ipAddress, DataGridViewRow targetRow = null)
        {
            try
            {
                labelIpAddress.Text = GetString("serverLocation_IpText", "IP 주소 : ") + ipAddress;
                
                if (!IPAddress.TryParse(ipAddress, out _))
                {
                    labelCountryName.Text = GetString("serverLocation_OfflineInfo", "해당 세션은 오프라인 게임이거나 아직 서버가 잡히지 않았습니다.");
                    labelRegionName.Text = GetString("serverLocation_RegionText", "지역 : ");
                    labelCityName.Text = GetString("serverLocation_CityText", "도시 : ");
                    return;
                }

                dynamic geoInfo = null;

                // 캐시 확인
                if (_geoCache.ContainsKey(ipAddress))
                {
                    geoInfo = _geoCache[ipAddress];
                }
                else
                {
                    // 비동기 API 요청 (UI 멈춤 방지)
                    string apiUrl = $"http://ip-api.com/json/{ipAddress}";
                    using (WebClient client = new WebClient())
                    {
                        client.Headers.Add("User-Agent", "EFT-Where-Am-I Desktop App");
                        string response = await Task.Run(() => client.DownloadString(apiUrl));
                        geoInfo = JsonConvert.DeserializeObject(response);
                        
                        if (geoInfo != null && geoInfo["status"] == "success")
                        {
                            _geoCache[ipAddress] = geoInfo; // 딕셔너리에 저장
                        }
                    }
                }

                if (geoInfo != null)
                {
                    if (geoInfo["status"] == "success")
                    {
                        labelCountryName.Text = GetString("serverLocation_CountryText", "국가 : ") + geoInfo["country"];
                        labelRegionName.Text = GetString("serverLocation_RegionText", "지역 : ") + geoInfo["regionName"];
                        string cityResult = geoInfo["city"]?.ToString();
                        labelCityName.Text = GetString("serverLocation_CityText", "도시 : ") + cityResult;

                        if (targetRow != null)
                        {
                            targetRow.Cells["colCity"].Value = cityResult;
                        }
                    }
                    else
                    {
                        labelCountryName.Text = GetString("serverLocation_StatusFail", "상태 : 실패 (API 오류)");
                        labelRegionName.Text = GetString("serverLocation_Reason", "사유 : ") + geoInfo["message"];
                        labelCityName.Text = GetString("serverLocation_CityText", "도시 : ");
                        
                        if (targetRow != null)
                        {
                            targetRow.Cells["colCity"].Value = "(실패)";
                        }
                    }
                }
            }
            catch (Exception)
            {
                labelCountryName.Text = GetString("serverLocation_Error", "오류 발생");
                labelRegionName.Text = GetString("serverLocation_Error", "오류 발생");
                labelCityName.Text = GetString("serverLocation_Error", "오류 발생");
                
                if (targetRow != null)
                {
                    targetRow.Cells["colCity"].Value = "(오류)";
                }
            }
        }
    }
}
