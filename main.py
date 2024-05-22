from tkinter import *
import tkinter.ttk as ttk 
import keyboard
import os
import glob
import time
from selenium import webdriver
from selenium.webdriver.common.by import By
from selenium.webdriver.common.keys import Keys
from selenium.webdriver.chrome.options import Options


def change_map() -> None:
    global map
    global site_url
    select_map = combobox.get()
    if map != select_map:
        site_url = f"https://tarkov-market.com/maps/{select_map}"
        map = select_map
        driver.get(site_url)


def set_trigger(event):
    global trigger
    trigger = event.name
    with open(txt_file_path, 'w') as file:
        file.write(trigger)
        

def get_latest_file(folder_path):
    files = glob.glob(os.path.join(folder_path, '*'))
    if not files:
        return None
    else:
        latest_file = max(files, key=os.path.getmtime)
        return os.path.basename(latest_file)


def changeMarker():
    styleList = [
        ['background','#ff0000'],
        ['height','30px'],
        ['width','30px']
    ]
    for i in styleList:
        driver.execute_script("document.getElementsByClassName('marker')[0].style.setProperty('"+i[0]+"','"+i[1]+"','important')")


# tarkov-market에 위치 확인하는 함수
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
        print("Error: Cant find Screenshot Files.")
        return


    textArea = driver.find_element(By.XPATH,'//*[@id="__nuxt"]/div/div/div[2]/div/div/div[1]/div[2]/button')
    textArea.click()
    textArea2 = driver.find_element(By.XPATH,'//*[@id="__nuxt"]/div/div/div[2]/div/div/div[1]/div[2]/input')
    textArea2.click()
    textArea2.send_keys(Keys.DELETE)
    textArea2.send_keys(screenshot.replace(".png",""))
    changeMarker()


mapList = ['ground-zero', 'factory', 'customs', 'interchange', 'woods', 'shoreline', 'lighthouse', 'reserve', 'streets', 'lab']

    
map = "ground-zero"
site_url = f"https://tarkov-market.com/maps/{map}"
txt_file_path = 'key_data.txt'

# 스크린샷 및 위치 표시할 키 불러오기
if os.path.exists(txt_file_path):   
    with open(txt_file_path, 'r') as file:
        trigger = file.read().strip()
else:
    trigger = None         

chrome_options = Options()
chrome_options.add_experimental_option("detach", True)
chrome_options.add_experimental_option("excludeSwitches", ["enable-logging"])
driver = webdriver.Chrome(options=chrome_options)
driver.implicitly_wait(3)
driver.get(site_url)

keyboard.add_hotkey(trigger, lambda: checkLocation())
    
window = Tk()
window.geometry("600x400")
window.title("EFT Where am I?")
window.resizable(False, False)


label1 = Label(window, text = 'Select The Map.\n맵을 선택해주세요.')
label1.pack()
combobox = ttk.Combobox(window)
combobox.config(values=mapList)   
combobox.config(state="readonly")
combobox.set("ground-zero") 
combobox.pack() 
b1 = Button(window, text = '적용', command = change_map)
b1.pack()

label2 = Label(window, text = 'Enter the key you use for screenshots.\n스크린샷으로 사용하는 키를 입력해주세요.')
label2.pack()
b2 = Button(window, text = 'Press to Record', command = set_trigger)
b2.pack()

label3 = Label(window, text = 'How to use')
label3.pack()

label4 = Label(window, text = '사용 방법')
label4.pack()

window.mainloop()