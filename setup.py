from cx_Freeze import setup, Executable
import os
import sys

name = "EFT Where am I"
version = "1.2"
description = "Description of EFT Where am I"

# 필요한 DLL 파일 경로 추가
dll_files = [
    ("C:/Windows/SysWOW64/MSVCP140.dll", "MSVCP140.dll"),  # Visual C++ 재배포 가능 패키지 DLL
    ("C:/Windows/SysWOW64/VCRUNTIME140.dll", "VCRUNTIME140.dll")  # 추가 DLL 파일
]

# 패키지와 파일 포함 옵션 설정
options = {
    'build_exe': {
        'packages': ["tkinter", "webbrowser", "keyboard", "os", "glob", "time", "selenium"],
        'includes': ["tkinter.ttk"],
        'include_files': dll_files,
        'excludes': [],
    }
}

base = None
if sys.platform == "win32":
    base = "Win32GUI"

executables = [
    Executable("main.py", base=base, target_name="EFT_Where_am_I.exe")
]

setup(
    name=name,
    version=version,
    description=description,
    options=options,
    executables=executables
)
