import sys
import os
import glob
import time
import webbrowser
from PyQt5.QtCore import Qt, QUrl, pyqtSlot
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


mapList = ['ground-zero', 'factory', 'customs', 'interchange', 'woods', 'shoreline', 'lighthouse', 'reserve', 'streets', 'lab']

map = "ground-zero"
site_url = f"https://tarkov-market.com/maps/{map}"
txt_file_path = 'key_data.txt'
where_am_i_click = False


class BrowserWindow(QMainWindow):
    def __init__(self):
        super().__init__()
        self.setWindowTitle("EFT Where am I? (ver.1.2)")
        self.setGeometry(100, 100, 1200, 1000)
        self.setStyleSheet("background-color: gray; color: white;")

        global browser
        browser = QWebEngineView()
        browser.setUrl(QUrl(site_url))
        browser.loadFinished.connect(self.on_load_finished)
        
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
        map_label = QLabel('Select The Map.\n맵을 선택해주세요.', self)
        map_label.setFont(QFont('Helvetica', 15, QFont.Bold))
        map_label.setAlignment(Qt.AlignCenter)
        left_layout.addWidget(map_label)

        global combobox
        combobox = QComboBox(self)
        combobox.addItems(mapList)
        combobox.setCurrentText("ground-zero")
        combobox.setFont(QFont('Helvetica', 16))
        combobox.setStyleSheet("background-color: white; color: black;")
        left_layout.addWidget(combobox)

        self.b1 = QPushButton('Apply (적용)', self)
        self.b1.setFont(QFont('Helvetica', 16, QFont.Bold))
        self.b1.clicked.connect(change_map)
        left_layout.addWidget(self.b1)
        top_layout_1.addLayout(left_layout)


        center_layout = QVBoxLayout()
        self.b_panel_control = QPushButton('Hide/Show Pannels', self)
        self.b_panel_control.setFont(QFont('Helvetica', 16, QFont.Bold))
        self.b_panel_control.clicked.connect(pannelControl)
        center_layout.addWidget(self.b_panel_control)

        self.b_fullscreen = QPushButton('Full Screen', self)
        self.b_fullscreen.setFont(QFont('Helvetica', 16, QFont.Bold))
        self.b_fullscreen.clicked.connect(fullscreen)
        center_layout.addWidget(self.b_fullscreen)

        self.b_force = QPushButton('Run (실행)', self)
        self.b_force.setFont(QFont('Helvetica', 16, QFont.Bold))
        self.b_force.clicked.connect(checkLocation)
        center_layout.addWidget(self.b_force)
        top_layout_1.addLayout(center_layout)


        right_layout = QVBoxLayout()
        self.b3 = QPushButton('How to use', self)
        self.b3.setFont(QFont('Helvetica', 16, QFont.Bold))
        self.b3.setStyleSheet("color: #0645AD;")
        self.b3.clicked.connect(lambda: open_url("https://github.com/karpitony/eft-where-am-i/blob/main/README.md"))
        right_layout.addWidget(self.b3)

        self.b4 = QPushButton('사용 방법', self)
        self.b4.setFont(QFont('Helvetica', 16, QFont.Bold))
        self.b4.setStyleSheet("color: #0645AD;")
        self.b4.clicked.connect(lambda: open_url("https://github.com/karpitony/eft-where-am-i/blob/main/README_ko_kr.md"))
        right_layout.addWidget(self.b4)

        self.b5 = QPushButton('Bug Report', self)
        self.b5.setFont(QFont('Helvetica', 16, QFont.Bold))
        self.b5.setStyleSheet("color: #0645AD;")
        self.b5.clicked.connect(lambda: open_url("https://github.com/karpitony/eft-where-am-i/issues"))
        right_layout.addWidget(self.b5)

        top_layout_1.addLayout(right_layout)

        main_layout.addWidget(browser)

    @pyqtSlot(bool)
    def on_load_finished(self, ok):
        if ok:
            fullscreen()

def open_url(url):
    webbrowser.open_new(url)

if __name__ == "__main__":
    app = QApplication(sys.argv)
    
    window = BrowserWindow()
    window.show()

    sys.exit(app.exec_())
