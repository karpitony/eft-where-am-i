from tkinter import *
import webbrowser
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
    label3.config(text=f'Now: \"{trigger}\"')
    keyboard.add_hotkey(trigger, lambda: checkLocation())
        
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
    trigger = 'print screen'

chrome_options = Options()
chrome_options.add_experimental_option("detach", True)
chrome_options.add_experimental_option("excludeSwitches", ["enable-logging"])
driver = webdriver.Chrome(options=chrome_options)
driver.implicitly_wait(3)
driver.get(site_url)

keyboard.add_hotkey(trigger, lambda: checkLocation())

# 색상 변수
bg_color = 'gray'
box_color = '#333333'
text_color = 'white'

# UI 생성
window = Tk()
window.geometry("500x600")
window.title("EFT Where am I?   v1.0")
window.resizable(False, False)
window.configure(bg=bg_color)

main_frame = Frame(window, padx=10, pady=10, bg=bg_color)
main_frame.pack(expand=True)

def create_rounded_rectangle(canvas, x1, y1, x2, y2, radius=25, **kwargs):
    points = [x1+radius, y1,
              x1+radius, y1,
              x2-radius, y1,
              x2-radius, y1,
              x2, y1,
              x2, y1+radius,
              x2, y1+radius,
              x2, y2-radius,
              x2, y2-radius,
              x2, y2,
              x2-radius, y2,
              x2-radius, y2,
              x1+radius, y2,
              x1+radius, y2,
              x1, y2,
              x1, y2-radius,
              x1, y2-radius,
              x1, y1+radius,
              x1, y1+radius,
              x1, y1]
    return canvas.create_polygon(points, **kwargs, smooth=True)

def open_url(url):
    webbrowser.open_new(url)

canvas1 = Canvas(main_frame, width=480, height=200, bg=bg_color, highlightthickness=0)
canvas1.grid(row=0, column=0, pady=(10, 20))
create_rounded_rectangle(canvas1, 10, 10, 470, 190, radius=20, fill=box_color)

frame1 = Frame(canvas1, bg=box_color)
frame1.place(relx=0.5, rely=0.5, anchor=CENTER)

label1 = Label(frame1, text='Select The Map.\n맵을 선택해주세요.', font=('Helvetica', 16), bg=box_color, fg=text_color)
label1.grid(row=0, column=0, columnspan=3, pady=(10, 10))

combobox = ttk.Combobox(frame1, values=mapList, state="readonly", font=('Helvetica', 16))   
combobox.set("ground-zero") 
combobox.grid(row=1, column=0, columnspan=2, pady=(0, 10))

b1 = Button(frame1, text='적용', command=change_map, font=('Helvetica', 16))
b1.grid(row=1, column=2, padx=(10, 0))

canvas2 = Canvas(main_frame, width=480, height=200, bg=bg_color, highlightthickness=0)
canvas2.grid(row=1, column=0, pady=(0, 20))
create_rounded_rectangle(canvas2, 10, 10, 470, 190, radius=20, fill=box_color)

frame2 = Frame(canvas2, bg=box_color)
frame2.place(relx=0.5, rely=0.5, anchor=CENTER)

label2 = Label(frame2, text='Enter the key you use for screenshots.\n스크린샷으로 사용하는 키를 입력해주세요.', font=('Helvetica', 16), bg=box_color, fg=text_color)
label2.grid(row=0, column=0, columnspan=3, pady=(10, 10))

label3 = Label(frame2, text=f'Now: \"{trigger}\"', font=('Helvetica', 16), bg=box_color, fg=text_color)
label3.grid(row=1, column=0, columnspan=3, pady=(0, 10))

b2 = Button(frame2, text='Press to Record', command=lambda: keyboard.hook(set_trigger), font=('Helvetica', 16))
b2.grid(row=2, column=0, pady=(10, 10))

# 강제 실행 버튼 추가
b_force = Button(frame2, text='Force Run', command=checkLocation, font=('Helvetica', 16))
b_force.grid(row=2, column=1, pady=(10, 10), padx=(10, 0))

b3 = Button(main_frame, text='How to use', font=('Helvetica', 12, 'bold'), bg=bg_color, fg="#0645AD", command=lambda: open_url("https://github.com/karpitony/eft-where-am-i/blob/main/README.md"))
b3.grid(row=2, column=0, pady=(10, 10))

b4 = Button(main_frame, text='사용 방법', font=('Helvetica', 12, 'bold'), bg=bg_color, fg="#0645AD", command=lambda: open_url("https://github.com/karpitony/eft-where-am-i/blob/main/README_ko_kr.md"))
b4.grid(row=3, column=0)

window.mainloop()
