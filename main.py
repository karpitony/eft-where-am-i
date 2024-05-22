import sys
from PyQt5 import uic
from PyQt5.QtWidgets import QApplication, QMainWindow
import keyboard
import os
import time
from selenium import webdriver
from selenium.webdriver.common.by import By
from selenium.webdriver.common.keys import Keys
from selenium.webdriver.chrome.options import Options

form_class = uic.loadUiType("main.ui")[0]


class WindowClass(QMainWindow, form_class):
    def __init__(self):
        super().__init__()
        self.setupUi(self)
        


    def change_map(self) -> None:
        select_map = self.currentText()
        if map != select_map:
            site_url = f"https://tarkov-market.com/maps/{select_map}"
            map = select_map

    def set_trigger(event):
        global trigger
        trigger = event.name
        with open(txt_file_path, 'w') as file:
            file.write(trigger)
        


# tarkov-market에 위치 확인하는 함수
def checkLocation():
    time.sleep(1)
    files_Path = os.path.expanduser('~')+"\\Documents\\Escape from Tarkov\\Screenshots\\"
    file_name_and_time_lst = []
    for f_name in os.listdir(f"{files_Path}"):
        written_time = os.path.getctime(f"{files_Path}{f_name}")
        file_name_and_time_lst.append((f_name, written_time))
    sorted_file_lst = sorted(file_name_and_time_lst, key=lambda x: x[1], reverse=True)
    textArea = driver.find_element(By.XPATH,'//*[@id="__nuxt"]/div/div/div[2]/div/div/div[1]/div/input')
    textArea.click()
    textArea.send_keys(Keys.DELETE)
    textArea.send_keys(sorted_file_lst[0][0].replace(".png",""))




if __name__ == "__main__":
    
    map = "ground-zero"
    site_url = f"https://tarkov-market.com/maps/{map}"
    txt_file_path = 'key_data.txt'
    
    if os.path.exists(txt_file_path):   # 스크린샷 및 위치 표시할 키
        with open(txt_file_path, 'r') as file:
            trigger = file.read().strip()
    else:
        trigger = None         
    
    chrome_options = Options()
    chrome_options.add_experimental_option("detach", True)
    chrome_options.add_experimental_option("excludeSwitches", ["enable-logging"])
    driver = webdriver.Chrome(options=chrome_options)
    driver.implicitly_wait(3)
    
    keyboard.add_hotkey(trigger, lambda: checkLocation())
    
    app = QApplication(sys.argv)
    myWindow = WindowClass()
    myWindow.show()
    app.exec_()