import sys
import os
import glob
import time
import keyboard
import webbrowser
from PyQt5.QtCore import Qt, QUrl
from PyQt5.QtWidgets import QApplication, QMainWindow, QVBoxLayout, QHBoxLayout, QWidget, QPushButton, QLabel, QComboBox
from PyQt5.QtWebEngineWidgets import QWebEngineView, QWebEngineSettings
from PyQt5.QtGui import QFont

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

def set_trigger(event):
    global trigger
    trigger = event.name
    with open(txt_file_path, 'w') as file:
        file.write(trigger)
    label3.setText(f'Now: \"{trigger}\"')
    keyboard.add_hotkey(trigger, checkLocation)

def get_latest_file(folder_path):
    files = glob.glob(os.path.join(folder_path, '*'))
    if not files:
        return None
    else:
        latest_file = max(files, key=os.path.getmtime)
        return os.path.basename(latest_file)

def changeMarker():
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

def checkLocation():
    time.sleep(1)
    paths = [
        os.path.expanduser('~') + "\\Documents\\Escape from Tarkov\\Screenshots\\",
        os.path.expanduser('~') + "\\문서\\Escape from Tarkov\\Screenshots\\",
        os.path.expanduser('~') + "\\OneDrive\\Documents\\Escape from Tarkov\\Screenshots\\",
        os.path.expanduser('~') + "\\OneDrive\\문서\\Escape from Tarkov\\Screenshots\\"
    ]

    screenshot = None
    for path in paths:
        screenshot = get_latest_file(path)
        if screenshot:
            break

    if screenshot is None:
        return

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
        changeMarker()
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
        changeMarker()

mapList = ['ground-zero', 'factory', 'customs', 'interchange', 'woods', 'shoreline', 'lighthouse', 'reserve', 'streets', 'lab']

map = "ground-zero"
site_url = f"https://tarkov-market.com/maps/{map}"
txt_file_path = 'key_data.txt'
where_am_i_click = False

if os.path.exists(txt_file_path):
    with open(txt_file_path, 'r') as file:
        trigger = file.read().strip()
else:
    trigger = 'print screen'

keyboard.add_hotkey(trigger, checkLocation)

class BrowserWindow(QMainWindow):
    def __init__(self):
        super().__init__()
        self.setWindowTitle("EFT Where am I?   v1.2")
        self.setGeometry(100, 100, 1200, 1000)
        self.setStyleSheet("background-color: gray; color: white;")

        global browser  # browser를 전역 변수로 설정
        browser = QWebEngineView()
        browser.setUrl(QUrl(site_url))
        
        # WebEngineSettings 수정
        settings = browser.settings()
        settings.setAttribute(QWebEngineSettings.JavascriptEnabled, True)
        settings.setAttribute(QWebEngineSettings.LocalStorageEnabled, True)
        settings.setAttribute(QWebEngineSettings.LocalContentCanAccessRemoteUrls, True)
        settings.setAttribute(QWebEngineSettings.AllowRunningInsecureContent, True)

        central_widget = QWidget(self)
        self.setCentralWidget(central_widget)

        main_layout = QVBoxLayout(central_widget)

        # 첫 번째 줄
        top_layout_1 = QHBoxLayout()
        main_layout.addLayout(top_layout_1)

        # 맵 선택
        map_layout = QVBoxLayout()
        map_label = QLabel('Select The Map.\n맵을 선택해주세요.', self)
        map_label.setFont(QFont('Helvetica', 16, QFont.Bold))
        map_label.setAlignment(Qt.AlignCenter)
        map_layout.addWidget(map_label)

        global combobox  # combobox를 전역 변수로 설정
        combobox = QComboBox(self)
        combobox.addItems(mapList)
        combobox.setCurrentText("ground-zero")
        combobox.setFont(QFont('Helvetica', 16))
        combobox.setStyleSheet("background-color: white; color: black;")
        map_layout.addWidget(combobox)

        self.b1 = QPushButton('Apply', self)
        self.b1.setFont(QFont('Helvetica', 16))
        self.b1.clicked.connect(change_map)
        map_layout.addWidget(self.b1)
        top_layout_1.addLayout(map_layout)

        # 스크린샷 키 설정
        key_layout = QVBoxLayout()
        key_label = QLabel('Enter the key you use for screenshots.\n스크린샷으로 사용하는 키를 입력해주세요.', self)
        key_label.setFont(QFont('Helvetica', 16, QFont.Bold))
        key_label.setAlignment(Qt.AlignCenter)
        key_layout.addWidget(key_label)

        global label3  # label3를 전역 변수로 설정
        label3 = QLabel(f'Now: \"{trigger}\"', self)
        label3.setFont(QFont('Helvetica', 16))
        label3.setAlignment(Qt.AlignCenter)
        key_layout.addWidget(label3)

        self.b2 = QPushButton('Press to Record', self)
        self.b2.setFont(QFont('Helvetica', 16))
        self.b2.clicked.connect(lambda: keyboard.hook(set_trigger))
        key_layout.addWidget(self.b2)
        top_layout_1.addLayout(key_layout)

        # 사용 설명서
        usage_layout = QVBoxLayout()
        self.b3 = QPushButton('How to use', self)
        self.b3.setFont(QFont('Helvetica', 12, QFont.Bold))
        self.b3.setStyleSheet("color: #0645AD;")
        self.b3.clicked.connect(lambda: open_url("https://github.com/karpitony/eft-where-am-i/blob/main/README.md"))
        usage_layout.addWidget(self.b3)

        self.b4 = QPushButton('사용 방법', self)
        self.b4.setFont(QFont('Helvetica', 12, QFont.Bold))
        self.b4.setStyleSheet("color: #0645AD;")
        self.b4.clicked.connect(lambda: open_url("https://github.com/karpitony/eft-where-am-i/blob/main/README_ko_kr.md"))
        usage_layout.addWidget(self.b4)

        self.b_force = QPushButton('Force Run', self)
        self.b_force.setFont(QFont('Helvetica', 16))
        self.b_force.clicked.connect(checkLocation)
        usage_layout.addWidget(self.b_force)
        top_layout_1.addLayout(usage_layout)

        main_layout.addWidget(browser)

def open_url(url):
    webbrowser.open_new(url)

if __name__ == "__main__":
    app = QApplication(sys.argv)

    window = BrowserWindow()
    window.show()
    sys.exit(app.exec_())
