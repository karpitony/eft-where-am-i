using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Newtonsoft.Json;
using eft_where_am_i.Classes;

namespace eft_where_am_i
{
    public partial class ServerLocation : UserControl
    {
        public ServerLocation()
        {
            InitializeComponent();
        }

        private void btnFindServer_Click(object sender, EventArgs e)
        {
            try
            {
                string logsFolderPath = SettingsHandler.Instance.GetOrFindLogPath();

                if (string.IsNullOrEmpty(logsFolderPath) || !Directory.Exists(logsFolderPath))
                {
                    MessageBox.Show("로그 폴더 경로를 찾을 수 없습니다. 설정을 확인해주세요.", "경로 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 가장 최근 로그 폴더 찾기
                var latestDir = new DirectoryInfo(logsFolderPath)
                    .GetDirectories()
                    .OrderByDescending(d => d.CreationTime)
                    .FirstOrDefault();

                if (latestDir == null)
                {
                    MessageBox.Show("로그 폴더(세션 폴더)를 찾을 수 없습니다.", "폴더 없음", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // 해당 폴더에서 가장 최근 application 로그 파일 찾기
                var latestFile = latestDir.GetFiles("*application*.log")
                    .OrderByDescending(f => f.LastWriteTime)
                    .FirstOrDefault();

                if (latestFile != null)
                {
                    // 파일에서 가장 최근 IP 주소 추출
                    string lastIpAddress = GetFirstIpAddress(latestFile.FullName);
                    if (!string.IsNullOrEmpty(lastIpAddress))
                    {
                        labelIpAddress.Text = $"IP 주소 : {lastIpAddress}";
                        SetGeoLocationLabels(lastIpAddress);
                    }
                    else
                    {
                        MessageBox.Show("로그 파일에서 IP 주소를 찾을 수 없습니다.", "IP 주소 없음", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("해당 폴더에 '*application*.log' 파일을 찾을 수 없습니다.", "파일 없음", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"오류 발생: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetFirstIpAddress(string logFilePath)
        {
            try
            {
                string firstIpAddress = null;

                // FileStream을 사용하여 파일을 읽기 전용으로 엽니다.
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
                                firstIpAddress = match.Groups[1].Value;
                                // 루프를 계속 돌아서 파일 내 가장 마지막(최근) IP를 얻습니다.
                            }
                        }
                    }
                }

                return firstIpAddress;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"오류 발생: {ex.Message}");
                return null;
            }
        }

        // IP 주소를 받아서 지리적 위치 정보를 가져와 라벨에 표시하는 메서드
        private void SetGeoLocationLabels(string ipAddress)
        {
            try
            {
                string apiUrl = $"http://ip-api.com/json/{ipAddress}";
                using (WebClient client = new WebClient())
                {
                    // Adding a User-Agent is sometimes required by ip-api
                    client.Headers.Add("User-Agent", "EFT-Where-Am-I Desktop App");
                    string response = client.DownloadString(apiUrl);
                    dynamic geoInfo = JsonConvert.DeserializeObject(response);

                    if (geoInfo["status"] == "success")
                    {
                        // 국가, 도시 정보를 라벨에 표시
                        labelCountryName.Text = $"국가 : {geoInfo["country"]}";
                        labelRegionName.Text = $"지역 : {geoInfo["regionName"]}";
                        labelCityName.Text = $"도시 : {geoInfo["city"]}";
                    }
                    else
                    {
                        labelCountryName.Text = "상태 : 실패 API 오류";
                        labelRegionName.Text = $"지역 : {geoInfo["message"]}";
                        labelCityName.Text = $"도시 : ";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"오류 발생: {ex.Message}");

                // 오류 발생 시 라벨에 오류 메시지 표시
                labelCountryName.Text = "국가 : 오류";
                labelRegionName.Text = "지역 : 오류";
                labelCityName.Text = "도시 : 오류";
            }
        }
    }
}
