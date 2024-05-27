import sys
import os
import glob
import time
import webbrowser
from PyQt5.QtCore import Qt, QUrl, pyqtSlot, QThread, pyqtSignal, QTranslator, QLocale, QCoreApplication, QFileSystemWatcher
from PyQt5.QtWidgets import QApplication, QMainWindow, QVBoxLayout, QHBoxLayout, QWidget, QPushButton, QLabel, QComboBox, QCheckBox, QFrame
from PyQt5.QtWebEngineWidgets import QWebEngineView, QWebEngineSettings
from PyQt5.QtGui import QFont

# 맵 URL을 선택된 맵에 따라 변경하는 함수
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
    
    if not files:
        return None
    else:
        latest_file = max(files, key=os.path.getmtime)
        return os.path.basename(latest_file)

# 맵의 마커 스타일을 변경하는 함수
def change_marker():
    styleList = [
        ['background', '#ff0000'],
        ['height', '30px'],
        ['width', '30px']
    ]
    
    for i in styleList:
        js_code = f"""
            var marker = document.getElementsByClassName('marker')[0];
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

    global where_am_i_click
    if where_am_i_click == False:
        where_am_i_click = True
        js_code = """
            var button = document.querySelector('#__nuxt > div > div > div.page-content > div > div > div.panel_top.d-flex > div.d-flex.ml-15.fs-0 > button');
            if (button) {
                button.click();
                console.log('Button clicked');
            } else {
                console.log('Button not found');
            }
        """
        browser.page().runJavaScript(js_code)
        time.sleep(0.5)
        js_code = f"""
            var input = document.querySelector('input[type="text"]');
            if (input) {{
                input.value = '{screenshot.replace(".png", "")}';
                input.dispatchEvent(new Event('input'));
                console.log('Input value set');
            }} else {{
                console.log('Input not found');
            }}
        """
        browser.page().runJavaScript(js_code)
        change_marker()
    else:
        js_code = f"""
            var input = document.querySelector('input[type="text"]');
            if (input) {{
                input.value = '{screenshot.replace(".png", "")}';
                input.dispatchEvent(new Event('input'));
                console.log('Input value set');
            }} else {{
                console.log('Input not found');
            }}
        """
        browser.page().runJavaScript(js_code)
        change_marker()

def fullscreen():
    js_code = """
        var button = document.querySelector('#__nuxt > div > div > div.page-content > div > div > div.panel_top.d-flex > button');
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
        var button = document.evaluate('//*[@id="__nuxt"]/div/div/div[2]/div/div/div[1]/div[1]/button', document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue;
        if (button) {
            button.click();
            console.log('Panel control button clicked');
        } else {
            console.log('Panel control button not found');
        }
    """
    browser.page().runJavaScript(js_code)

# 자동 감지 기능을 시작하는 함수
def start_auto_detection():
    global watcher
    if watcher is not None:
        watcher.addPath(screenshot_path)

# 자동 감지 파일 시스템 감시자 설정
class FileSystemWatcher(QFileSystemWatcher):
    def __init__(self, parent=None):
        super().__init__(parent)
        self.directoryChanged.connect(self.directory_changed)
        self.fileChanged.connect(self.file_changed)

    def directory_changed(self, path):
        check_location()

    def file_changed(self, path):
        check_location()

# 언어를 설정하는 함수
def set_language(language):
    global translator
    if not translator.load(f"translations/app_{language}.qm"):
        print(f"Failed to load translation file: translations/app_{language}.qm")
        return
    app.installTranslator(translator)
    window.retranslateUi()

def open_url(url):
    webbrowser.open_new(url)

# 기본적인 변수 세팅
mapList = ['ground-zero', 'factory', 'customs', 'interchange', 'woods', 'shoreline', 'lighthouse', 'reserve', 'streets', 'lab']

map = "ground-zero"
site_url = f"https://tarkov-market.com/maps/{map}"
where_am_i_click = False

home_directory = os.path.expanduser('~')
paths = []
with open("file_path.txt", 'r', encoding='utf-8') as file:
    for line in file:
        paths.append(line.strip())

screenshot_path = 'can`t find directory'
for item in paths:
    full_path = os.path.join(home_directory, item)
    if os.path.isdir(full_path):
        screenshot_path = full_path
        break  # 첫 번째 존재하는 디렉터리를 찾으면 종료

class BrowserWindow(QMainWindow):
    def __init__(self):
        super().__init__()
        self.setWindowTitle("EFT Where am I? (ver.1.2)")
        self.setGeometry(100, 100, 1200, 1000)
        self.setStyleSheet("background-color: gray; color: white;")

        global browser
        browser = QWebEngineView()
        browser.setUrl(QUrl(site_url))
        
        settings = browser.settings()
        settings.setAttribute(QWebEngineSettings.JavascriptEnabled, True)
        settings.setAttribute(QWebEngineSettings.LocalStorageEnabled, True)
        settings.setAttribute(QWebEngineSettings.LocalContentCanAccessRemoteUrls, True)
        settings.setAttribute(QWebEngineSettings.AllowRunningInsecureContent, True)

        central_widget = QWidget(self)
        self.setCentralWidget(central_widget)

        main_layout = QVBoxLayout(central_widget)

        top_layout_1 = QHBoxLayout()
        main_layout.addLayout(top_layout_1)

        left_layout = QVBoxLayout()
        self.map_label = QLabel(self.tr('Select The Map.'), self)
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

        self.b1 = QPushButton(self.tr('Apply'), self)
        self.b1.setFont(QFont('Helvetica', 16, QFont.Bold))
        self.b1.clicked.connect(change_map)
        combobox_layout.addWidget(self.b1)

        left_layout.addLayout(combobox_layout)

        self.auto_detect_checkbox = QCheckBox(self.tr('Auto Screenshot Detection'), self)
        self.auto_detect_checkbox.setFont(QFont('Helvetica', 16, QFont.Bold))
        self.auto_detect_checkbox.setStyleSheet("margin: 0 auto;")
        self.auto_detect_checkbox.setLayoutDirection(Qt.LeftToRight)
        self.auto_detect_checkbox.toggled.connect(self.toggle_auto_detection)
        left_layout.addWidget(self.auto_detect_checkbox)

        top_layout_1.addLayout(left_layout)

        left_separator = QFrame()
        left_separator.setFrameShape(QFrame.VLine)
        left_separator.setFrameShadow(QFrame.Sunken)
        top_layout_1.addWidget(left_separator)

        center_layout = QVBoxLayout()
        self.b_panel_control = QPushButton(self.tr('Hide/Show Pannels'), self)
        self.b_panel_control.setFont(QFont('Helvetica', 16, QFont.Bold))
        self.b_panel_control.clicked.connect(pannelControl)
        center_layout.addWidget(self.b_panel_control)

        self.b_fullscreen = QPushButton(self.tr('Full Screen'), self)
        self.b_fullscreen.setFont(QFont('Helvetica', 16, QFont.Bold))
        self.b_fullscreen.clicked.connect(fullscreen)
        center_layout.addWidget(self.b_fullscreen)

        self.b_force = QPushButton(self.tr('Force Run'), self)
        self.b_force.setFont(QFont('Helvetica', 16, QFont.Bold))
        self.b_force.clicked.connect(lambda: check_location())
        center_layout.addWidget(self.b_force)
        top_layout_1.addLayout(center_layout)

        center_separator = QFrame()
        center_separator.setFrameShape(QFrame.VLine)
        center_separator.setFrameShadow(QFrame.Sunken)
        top_layout_1.addWidget(center_separator)

        right_layout = QVBoxLayout()

        # 언어 선택 레이아웃
        language_layout = QHBoxLayout()
        self.language_combobox = QComboBox(self)
        self.language_combobox.addItem("English", "en")
        self.language_combobox.addItem("한국어", "ko")
        self.language_combobox.setFont(QFont('Helvetica', 16))
        language_layout.addWidget(self.language_combobox)

        self.language_apply_button = QPushButton(self.tr('Apply Language'), self)
        self.language_apply_button.setFont(QFont('Helvetica', 16, QFont.Bold))
        self.language_apply_button.clicked.connect(self.apply_language)
        language_layout.addWidget(self.language_apply_button)

        right_layout.addLayout(language_layout)

        self.b3 = QPushButton(self.tr('How to use'), self)
        self.b3.setFont(QFont('Helvetica', 16, QFont.Bold))
        self.b3.setStyleSheet("color: #0645AD;")
        self.b3.clicked.connect(lambda: open_url("https://github.com/karpitony/eft-where-am-i/blob/main/README.md"))
        right_layout.addWidget(self.b3)

        self.b5 = QPushButton(self.tr('Bug Report'), self)
        self.b5.setFont(QFont('Helvetica', 16, QFont.Bold))
        self.b5.setStyleSheet("color: #0645AD;")
        self.b5.clicked.connect(lambda: open_url("https://github.com/karpitony/eft-where-am-i/issues"))
        right_layout.addWidget(self.b5)

        top_layout_1.addLayout(right_layout)

        main_layout.addWidget(browser)

    def apply_language(self):
        language = self.language_combobox.currentData()
        set_language(language)
        self.retranslateUi()

    @pyqtSlot(bool)
    def toggle_auto_detection(self, checked):
        if checked:
            start_auto_detection()

    def retranslateUi(self):
        self.setWindowTitle(self.tr("EFT Where am I? (ver.1.2)"))
        self.map_label.setText(self.tr('Select The Map.'))
        self.b1.setText(self.tr('Apply'))
        self.auto_detect_checkbox.setText(self.tr('Auto Screenshot Detection'))
        self.b_panel_control.setText(self.tr('Hide/Show Pannels'))
        self.b_fullscreen.setText(self.tr('Full Screen'))
        self.b_force.setText(self.tr('Force Run'))
        self.b3.setText(self.tr('How to use'))
        self.b5.setText(self.tr('Bug Report'))
        self.language_apply_button.setText(self.tr('Apply Language'))

if __name__ == "__main__":
    app = QApplication(sys.argv)

    translator = QTranslator()
    locale = QLocale.system().name()
    if not translator.load(f"translations/app_{locale}.qm"):
        print(f"Failed to load translation file: translations/app_{locale}.qm")
    app.installTranslator(translator)

    watcher = FileSystemWatcher()

    window = BrowserWindow()
    window.show()

    sys.exit(app.exec_())
