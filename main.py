import sys
import os
import glob
import time
import keyboard
from PyQt5.QtCore import QUrl
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
        browser.page().runJavaScript(f"document.getElementsByClassName('marker')[0].style.setProperty('{i[0]}', '{i[1]}', 'important')")

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
        browser.page().runJavaScript("""
            document.querySelector('div[role="button"]').click();
            document.querySelector('input[type="text"]').value = '';
        """)
        time.sleep(0.5)
        browser.page().runJavaScript(f"""
            document.querySelector('input[type="text"]').value = '{screenshot.replace(".png", "")}';
        """)
        changeMarker()
    else:
        browser.page().runJavaScript(f"""
            document.querySelector('input[type="text"]').value = '{screenshot.replace(".png", "")}';
        """)
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
        self.setGeometry(100, 100, 1200, 800)
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

        top_layout = QHBoxLayout()
        main_layout.addLayout(top_layout)

        self.label1 = QLabel('Select The Map.\n맵을 선택해주세요.', self)
        self.label1.setFont(QFont('Helvetica', 16))
        top_layout.addWidget(self.label1)

        global combobox  # combobox를 전역 변수로 설정
        combobox = QComboBox(self)
        combobox.addItems(mapList)
        combobox.setCurrentText("ground-zero")
        combobox.setFont(QFont('Helvetica', 16))
        top_layout.addWidget(combobox)

        self.b1 = QPushButton('Apply', self)
        self.b1.setFont(QFont('Helvetica', 16))
        self.b1.clicked.connect(change_map)
        top_layout.addWidget(self.b1)

        self.label2 = QLabel('Enter the key you use for screenshots.\n스크린샷으로 사용하는 키를 입력해주세요.', self)
        self.label2.setFont(QFont('Helvetica', 16))
        top_layout.addWidget(self.label2)

        global label3  # label3를 전역 변수로 설정
        label3 = QLabel(f'Now: \"{trigger}\"', self)
        label3.setFont(QFont('Helvetica', 16))
        top_layout.addWidget(label3)

        self.b2 = QPushButton('Press to Record', self)
        self.b2.setFont(QFont('Helvetica', 16))
        self.b2.clicked.connect(lambda: keyboard.hook(set_trigger))
        top_layout.addWidget(self.b2)

        self.b_force = QPushButton('Force Run', self)
        self.b_force.setFont(QFont('Helvetica', 16))
        self.b_force.clicked.connect(checkLocation)
        top_layout.addWidget(self.b_force)

        self.b3 = QPushButton('How to use', self)
        self.b3.setFont(QFont('Helvetica', 12, QFont.Bold))
        self.b3.setStyleSheet("color: #0645AD;")
        self.b3.clicked.connect(lambda: open_url("https://github.com/karpitony/eft-where-am-i/blob/main/README.md"))
        top_layout.addWidget(self.b3)

        self.b4 = QPushButton('사용 방법', self)
        self.b4.setFont(QFont('Helvetica', 12, QFont.Bold))
        self.b4.setStyleSheet("color: #0645AD;")
        self.b4.clicked.connect(lambda: open_url("https://github.com/karpitony/eft-where-am-i/blob/main/README_ko_kr.md"))
        top_layout.addWidget(self.b4)

        main_layout.addWidget(browser)

def open_url(url):
    webbrowser.open_new(url)

if __name__ == "__main__":
    app = QApplication(sys.argv)

    window = BrowserWindow()
    window.show()
    sys.exit(app.exec_())
