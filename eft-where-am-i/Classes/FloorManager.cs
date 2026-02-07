using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace eft_where_am_i.Classes
{
    public class PolygonPoint
    {
        public double x { get; set; }
        public double y { get; set; }
    }

    public class FloorZone
    {
        public string name { get; set; }
        public string floor_label { get; set; }
        public double z_min { get; set; }
        public double z_max { get; set; }
        public List<PolygonPoint> polygon { get; set; } = new List<PolygonPoint>();
        public List<List<PolygonPoint>> holes { get; set; } = new List<List<PolygonPoint>>();
    }

    public class FloorData
    {
        public string name { get; set; }
        public double z_min { get; set; }
        public double z_max { get; set; }
    }

    public class MapFloorConfig
    {
        public List<FloorData> floors { get; set; } = new List<FloorData>();
        public string default_floor { get; set; } = "Ground";
        public List<FloorZone> zones { get; set; } = new List<FloorZone>();
    }

    public class FloorManager
    {
        private readonly string _filePath;
        private Dictionary<string, MapFloorConfig> _floorDb;

        public FloorManager()
        {
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "floor_db.json");
            Load();
        }

        private void Load()
        {
            try
            {
                if (File.Exists(_filePath))
                {
                    string json = File.ReadAllText(_filePath);
                    _floorDb = JsonConvert.DeserializeObject<Dictionary<string, MapFloorConfig>>(json)
                               ?? new Dictionary<string, MapFloorConfig>();
                }
                else
                {
                    _floorDb = new Dictionary<string, MapFloorConfig>();
                    CreateDefaultDb();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FloorManager] Error loading floor DB: {ex.Message}");
                _floorDb = new Dictionary<string, MapFloorConfig>();
            }
        }

        public void Reload()
        {
            Load();
        }

        private void CreateDefaultDb()
        {
            _floorDb["reserve"] = new MapFloorConfig
            {
                default_floor = "Ground",
                floors = new List<FloorData>
                {
                    new FloorData { name = "Ground", z_min = -5.0, z_max = 100.0 },
                    new FloorData { name = "Underground", z_min = -50.0, z_max = -5.0 }
                },
                zones = new List<FloorZone>()
            };

            _floorDb["factory"] = new MapFloorConfig
            {
                default_floor = "Ground",
                floors = new List<FloorData>
                {
                    new FloorData { name = "Ground", z_min = -5.0, z_max = 100.0 },
                    new FloorData { name = "Underground", z_min = -50.0, z_max = -5.0 }
                },
                zones = new List<FloorZone>()
            };

            _floorDb["lab"] = new MapFloorConfig
            {
                default_floor = "Ground",
                floors = new List<FloorData>
                {
                    new FloorData { name = "Ground", z_min = -5.0, z_max = 100.0 }
                },
                zones = new List<FloorZone>()
            };

            _floorDb["streets"] = new MapFloorConfig
            {
                default_floor = "Ground",
                floors = new List<FloorData>
                {
                    new FloorData { name = "Ground", z_min = -5.0, z_max = 100.0 },
                    new FloorData { name = "Underground", z_min = -50.0, z_max = -5.0 }
                },
                zones = new List<FloorZone>()
            };

            Save();
        }

        public void Save()
        {
            try
            {
                string json = JsonConvert.SerializeObject(_floorDb, Formatting.Indented);
                File.WriteAllText(_filePath, json);
                AppLogger.Debug("FloorManager", $"Save completed. File size: {json.Length} bytes");
            }
            catch (Exception ex)
            {
                AppLogger.Error("FloorManager", $"Error saving floor DB: {ex.Message}");
            }
        }

        /// <summary>
        /// Ray Casting 알고리즘으로 점이 폴리곤 내부에 있는지 판별합니다.
        /// </summary>
        public static bool IsPointInPolygon(double px, double py, List<PolygonPoint> polygon)
        {
            if (polygon == null || polygon.Count < 3)
                return false;

            bool inside = false;
            int n = polygon.Count;
            for (int i = 0, j = n - 1; i < n; j = i++)
            {
                double xi = polygon[i].x, yi = polygon[i].y;
                double xj = polygon[j].x, yj = polygon[j].y;

                if (((yi > py) != (yj > py)) &&
                    (px < (xj - xi) * (py - yi) / (yj - yi) + xi))
                {
                    inside = !inside;
                }
            }

            return inside;
        }

        /// <summary>
        /// 폴리곤 기반 + Z좌표로 층을 판별합니다. (새 시그니처)
        /// zones가 있으면 폴리곤 기반, 없으면 기존 z-range 폴백.
        /// </summary>
        public string GetFloorName(string mapName, double x, double y, double z)
        {
            if (!_floorDb.TryGetValue(mapName, out var config))
                return null;

            string defaultFloor = config.default_floor ?? "Ground";

            // zones가 있으면 폴리곤 기반 판별
            if (config.zones != null && config.zones.Count > 0)
            {
                foreach (var zone in config.zones)
                {
                    // 1. 외곽 폴리곤 안에 있는가?
                    if (!IsPointInPolygon(x, y, zone.polygon))
                        continue;

                    // 2. hole 안에 있는가?
                    bool inHole = false;
                    if (zone.holes != null)
                    {
                        foreach (var hole in zone.holes)
                        {
                            if (IsPointInPolygon(x, y, hole))
                            {
                                inHole = true;
                                break;
                            }
                        }
                    }
                    if (inHole)
                        continue;

                    // 3. Z좌표 범위 체크
                    if (z < zone.z_min || z >= zone.z_max)
                        return defaultFloor;

                    // 4. 모두 통과 → 해당 zone의 층
                    return zone.floor_label;
                }

                // 어떤 zone에도 해당하지 않으면 default_floor
                return defaultFloor;
            }

            // zones가 없으면 기존 z-range 폴백
            return GetFloorNameByZ(config, z);
        }

        /// <summary>
        /// Z좌표로부터 해당 맵의 층 이름을 판별합니다 (기존 호환).
        /// </summary>
        public string GetFloorName(string mapName, double zCoord)
        {
            if (!_floorDb.TryGetValue(mapName, out var config))
                return null;

            return GetFloorNameByZ(config, zCoord);
        }

        private string GetFloorNameByZ(MapFloorConfig config, double zCoord)
        {
            if (config.floors == null || config.floors.Count == 0)
                return null;

            foreach (var floor in config.floors)
            {
                if (zCoord >= floor.z_min && zCoord < floor.z_max)
                {
                    return floor.name;
                }
            }

            return null;
        }

        /// <summary>
        /// 특정 맵의 zones 데이터를 JSON으로 반환 (에디터용)
        /// </summary>
        public string GetZonesJson(string mapName)
        {
            if (!_floorDb.TryGetValue(mapName, out var config))
                return "[]";

            return JsonConvert.SerializeObject(config.zones ?? new List<FloorZone>(), Formatting.None);
        }

        /// <summary>
        /// 특정 맵의 floors 데이터를 JSON으로 반환 (에디터용)
        /// </summary>
        public string GetFloorsJson(string mapName)
        {
            if (!_floorDb.TryGetValue(mapName, out var config))
                return "[]";

            return JsonConvert.SerializeObject(config.floors ?? new List<FloorData>(), Formatting.None);
        }

        /// <summary>
        /// 특정 맵의 zones 데이터를 JSON에서 업데이트 (에디터용)
        /// </summary>
        public void UpdateZonesFromJson(string mapName, string zonesJson)
        {
            try
            {
                AppLogger.Info("FloorManager", $"UpdateZonesFromJson called for map: {mapName}");
                AppLogger.Debug("FloorManager", $"Zones JSON (first 500 chars): {zonesJson.Substring(0, Math.Min(500, zonesJson.Length))}");

                var zones = JsonConvert.DeserializeObject<List<FloorZone>>(zonesJson)
                            ?? new List<FloorZone>();

                AppLogger.Info("FloorManager", $"Deserialized {zones.Count} zones");

                if (!_floorDb.ContainsKey(mapName))
                {
                    AppLogger.Info("FloorManager", $"Creating new map config for: {mapName}");
                    _floorDb[mapName] = new MapFloorConfig();
                }

                _floorDb[mapName].zones = zones;
                Save();
                AppLogger.Info("FloorManager", $"Zones saved successfully to {_filePath}");
            }
            catch (Exception ex)
            {
                AppLogger.Error("FloorManager", $"Error updating zones from JSON: {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 전체 DB를 JSON 문자열로 반환 (에디터용)
        /// </summary>
        public string GetDbJson()
        {
            return JsonConvert.SerializeObject(_floorDb, Formatting.Indented);
        }

        /// <summary>
        /// JSON 문자열로부터 DB를 업데이트합니다 (에디터용)
        /// </summary>
        public void UpdateFromJson(string json)
        {
            try
            {
                _floorDb = JsonConvert.DeserializeObject<Dictionary<string, MapFloorConfig>>(json)
                           ?? new Dictionary<string, MapFloorConfig>();
                Save();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FloorManager] Error updating from JSON: {ex.Message}");
            }
        }
    }
}
