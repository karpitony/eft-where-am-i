import sys
import os
import glob
import time
import json
import webbrowser
from PyQt5.QtCore import Qt, QUrl, pyqtSlot, QFileSystemWatcher
from PyQt5.QtWidgets import QApplication, QMainWindow, QVBoxLayout, QHBoxLayout, QWidget, QPushButton, QLabel, QComboBox, QCheckBox, QFrame
from PyQt5.QtWebEngineWidgets import QWebEngineView, QWebEngineSettings
from PyQt5.QtGui import QFont


# 설정 파일(settings.json) 관련
settings_file = "settings.json"

def load_settings():
    if os.path.exists(settings_file):
        with open(settings_file, 'r', encoding='utf-8') as file:
            return json.load(file)
    return {
        "auto_screenshot_detection": False,
        "language": "en",
        "screenshot_paths": [
            "Documents\\Escape from Tarkov\\Screenshots\\",
            "문서\\Escape from Tarkov\\Screenshots\\",
            "OneDrive\\Documents\\Escape from Tarkov\\Screenshots\\",
            "OneDrive\\문서\\Escape from Tarkov\\Screenshots\\"
        ]
    }

def save_settings(settings):
    with open(settings_file, 'w', encoding='utf-8') as file:
        json.dump(settings, file, ensure_ascii=False, indent=4)

def load_translations(language):
    translation_file = f"translations/{language}.json"
    if os.path.exists(translation_file):
        with open(translation_file, 'r', encoding='utf-8') as file:
            return json.load(file)
    return dict()

app_settings = load_settings()
translations = load_translations(app_settings["language"])

def tr(text):
    return translations.get(text, text)

def change_map():
    global map
    global site_url
    global where_am_i_click
    select_map = combobox.currentText()
    
    if map != select_map:
        site_url = f"https://tarkov-market.com/maps/{select_map}"
        map = select_map
        browser.setUrl(QUrl(site_url))
        where_am_i_click = False


# 폴더에서 가장 최근 파일을 가져오는 함수
def get_latest_files():
    files = glob.glob(os.path.join(screenshot_path, '*'))
    
    if files:
        latest_file = max(files, key=os.path.getmtime)
        return os.path.basename(latest_file)
    else:
        return None

# 맵의 마커 스타일을 변경하는 함수
def change_marker():
    styleList = [
        ['background', '#ff0000'],
        ['height', '30px'],
        ['width', '30px']
    ]
    
    for i in styleList:
        js_code = f"""
            const marker = document.getElementsByClassName('marker')[0];
            if (marker) {{
                marker.style.setProperty('{i[0]}', '{i[1]}', 'important');
            }} else {{
                console.log('Marker not found');
            }}
        """
        browser.page().runJavaScript(js_code)


# 최신 스크린샷을 찾아 위치를 확인하는 함수
def check_location():
    screenshot = get_latest_files()
    if not screenshot:
        return

    global where_am_i_click
    if not where_am_i_click:
        where_am_i_click = True
        js_code = """
            const button = document.querySelector('#__nuxt > div > div > div.page-content > div > div > div.panel_top.d-flex > div.d-flex.ml-15.fs-0 > button');
            if (button) {
                button.click();
                console.log('Button clicked');
            } else {
                console.log('Button not found');
            }
        """
        browser.page().runJavaScript(js_code)
        time.sleep(0.5)

    js_code = """
        const input = document.querySelector('input[type="text"]');
        if (input) {
            input.value = '{screenshot.replace(".png", "")}';
            input.dispatchEvent(new Event('input'));
            console.log('Input value set');
        } else {
            console.log('Input not found');
        }
    """
    browser.page().runJavaScript(js_code)
    change_marker()


def fullscreen():
    js_code = """
        const button = document.querySelector('#__nuxt > div > div > div.page-content > div > div > div.panel_top.d-flex > button');
        if (button) {
            button.click();
            console.log('Fullscreen button clicked');
        } else {
            console.log('Fullscreen button not found');
        }
    """
    browser.page().runJavaScript(js_code)


def pannelControl():
    js_code = """
        const button = document.evaluate('//*[@id="__nuxt"]/div/div/div[2]/div/div/div[1]/div[1]/button', document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue;
        if (button) {
            button.click();
            console.log('Panel control button clicked');
        } else {
            console.log('Panel control button not found');
        }
    """
    browser.page().runJavaScript(js_code)


# 폴링 시작하는 함수
def start_auto_detection():
    global watcher
    if watcher is not None:
        watcher.addPath(screenshot_path)
        print(f"Started watching {screenshot_path}")


# 폴링
class FileSystemWatcher(QFileSystemWatcher):
    def __init__(self, parent=None):
        super().__init__(parent)
        self.directoryChanged.connect(self.directory_changed)
        self.fileChanged.connect(self.file_changed)

    def directory_changed(self, path):
        print(f"Directory changed: {path}")
        check_location()

    def file_changed(self, path):
        print(f"File changed: {path}")
        check_location()

def open_url(url):
    webbrowser.open_new(url)



# 기본적인 변수 세팅
mapList = [
    'ground-zero',
    'factory',
    'customs',
    'interchange',
    'woods',
    'shoreline',
    'lighthouse',
    'reserve',
    'streets',
    'lab'
]

map = "ground-zero"
site_url = f"https://tarkov-market.com/maps/{map}"
where_am_i_click = False

home_directory = os.path.expanduser('~')
screenshot_path = None

# 여러 경로 중에서 존재하는 경로 찾기
for relative_path in app_settings.get("screenshot_paths", []):
    full_path = os.path.join(home_directory, relative_path)
    if os.path.isdir(full_path):
        screenshot_path = full_path
        break  # 첫 번째 존재하는 디렉터리를 찾으면 종료

if screenshot_path is None:
    screenshot_path = 'can`t find directory'

class BrowserWindow(QMainWindow):
    def __init__(self):
        super().__init__()
        self.setWindowTitle(tr("EFT Where am I? (ver.1.2)"))
        self.setGeometry(100, 100, 1200, 1000)
        self.setStyleSheet("background-color: gray; color: white;")

        global browser
        browser = QWebEngineView()
        browser.setUrl(QUrl(site_url))
        
        browser_settings = browser.settings()
        browser_settings.setAttribute(QWebEngineSettings.JavascriptEnabled, True)
        browser_settings.setAttribute(QWebEngineSettings.LocalStorageEnabled, True)
        browser_settings.setAttribute(QWebEngineSettings.LocalContentCanAccessRemoteUrls, True)
        browser_settings.setAttribute(QWebEngineSettings.AllowRunningInsecureContent, True)

        central_widget = QWidget(self)
        self.setCentralWidget(central_widget)

        main_layout = QVBoxLayout(central_widget)

        top_layout_1 = QHBoxLayout()
        main_layout.addLayout(top_layout_1)

        left_layout = QVBoxLayout()
        self.map_label = QLabel(tr('Select The Map.'), self)
        self.map_label.setFont(QFont('Helvetica', 18, QFont.Bold))
        self.map_label.setAlignment(Qt.AlignCenter)
        left_layout.addWidget(self.map_label)

        combobox_layout = QHBoxLayout()
        global combobox
        combobox = QComboBox(self)
        combobox.addItems(mapList)
        combobox.setCurrentText("ground-zero")
        combobox.setFont(QFont('Helvetica', 16))
        combobox.setStyleSheet("background-color: white; color: black;")
        combobox_layout.addWidget(combobox)

        self.b1 = QPushButton(tr('Apply'), self)
        self.b1.setFont(QFont('Helvetica', 16, QFont.Bold))
        self.b1.clicked.connect(change_map)
        combobox_layout.addWidget(self.b1)

        left_layout.addLayout(combobox_layout)

        self.auto_detect_checkbox = QCheckBox(tr('Auto Screenshot Detection'), self)
        self.auto_detect_checkbox.setFont(QFont('Helvetica', 16, QFont.Bold))
        self.auto_detect_checkbox.setStyleSheet("margin: 0 auto;")
        self.auto_detect_checkbox.setLayoutDirection(Qt.LeftToRight)
        self.auto_detect_checkbox.setChecked(app_settings["auto_screenshot_detection"])
        self.auto_detect_checkbox.toggled.connect(self.toggle_auto_detection)
        left_layout.addWidget(self.auto_detect_checkbox)

        top_layout_1.addLayout(left_layout)

        left_separator = QFrame()
        left_separator.setFrameShape(QFrame.VLine)
        left_separator.setFrameShadow(QFrame.Sunken)
        top_layout_1.addWidget(left_separator)

        center_layout = QVBoxLayout()
        self.b_panel_control = QPushButton(tr('Hide/Show Pannels'), self)
        self.b_panel_control.setFont(QFont('Helvetica', 16, QFont.Bold))
        self.b_panel_control.clicked.connect(pannelControl)
        center_layout.addWidget(self.b_panel_control)

        self.b_fullscreen = QPushButton(tr('Full Screen'), self)
        self.b_fullscreen.setFont(QFont('Helvetica', 16, QFont.Bold))
        self.b_fullscreen.clicked.connect(fullscreen)
        center_layout.addWidget(self.b_fullscreen)

        self.b_force = QPushButton(tr('Force Run'), self)
        self.b_force.setFont(QFont('Helvetica', 16, QFont.Bold))
        self.b_force.clicked.connect(lambda: check_location())
        center_layout.addWidget(self.b_force)
        top_layout_1.addLayout(center_layout)

        center_separator = QFrame()
        center_separator.setFrameShape(QFrame.VLine)
        center_separator.setFrameShadow(QFrame.Sunken)
        top_layout_1.addWidget(center_separator)

        right_layout = QVBoxLayout()

        language_layout = QHBoxLayout()
        self.language_combobox = QComboBox(self)
        self.language_combobox.addItem("English", "en")
        self.language_combobox.addItem("한국어", "ko")
        self.language_combobox.setFont(QFont('Helvetica', 16))
        self.language_combobox.setCurrentIndex(0 if app_settings["language"] == "en" else 1)
        language_layout.addWidget(self.language_combobox)

        self.language_apply_button = QPushButton(tr('Apply Language'), self)
        self.language_apply_button.setFont(QFont('Helvetica', 16, QFont.Bold))
        self.language_apply_button.clicked.connect(self.apply_language)
        language_layout.addWidget(self.language_apply_button)

        right_layout.addLayout(language_layout)

        self.b3 = QPushButton(tr('How to use'), self)
        self.b3.setFont(QFont('Helvetica', 16, QFont.Bold))
        self.b3.setStyleSheet("color: #0645AD;")
        self.b3.clicked.connect(lambda: open_url("https://github.com/karpitony/eft-where-am-i/blob/main/README.md"))
        right_layout.addWidget(self.b3)

        self.b5 = QPushButton(tr('Bug Report'), self)
        self.b5.setFont(QFont('Helvetica', 16, QFont.Bold))
        self.b5.setStyleSheet("color: #0645AD;")
        self.b5.clicked.connect(lambda: open_url("https://github.com/karpitony/eft-where-am-i/issues"))
        right_layout.addWidget(self.b5)

        top_layout_1.addLayout(right_layout)

        main_layout.addWidget(browser)

        if app_settings["auto_screenshot_detection"]:
            start_auto_detection()

    def apply_language(self):
        language = self.language_combobox.currentData()
        app_settings["language"] = language
        save_settings(app_settings)
        global translations
        translations = load_translations(language)
        self.retranslateUi()

    @pyqtSlot(bool)
    def toggle_auto_detection(self, checked):
        app_settings["auto_screenshot_detection"] = checked
        save_settings(app_settings)
        if checked:
            start_auto_detection()

    def retranslateUi(self):
        self.setWindowTitle(tr("EFT Where am I? (ver.1.2)"))
        self.map_label.setText(tr('Select The Map.'))
        self.b1.setText(tr('Apply'))
        self.auto_detect_checkbox.setText(tr('Auto Screenshot Detection'))
        self.b_panel_control.setText(tr('Hide/Show Pannels'))
        self.b_fullscreen.setText(tr('Full Screen'))
        self.b_force.setText(tr('Force Run'))
        self.b3.setText(tr('How to use'))
        self.b5.setText(tr('Bug Report'))
        self.language_apply_button.setText(tr('Apply Language'))

if __name__ == "__main__":
    app = QApplication(sys.argv)

    locale = app_settings["language"]
    translations = load_translations(locale)

    watcher = FileSystemWatcher()

    window = BrowserWindow()
    window.show()

    sys.exit(app.exec_())
