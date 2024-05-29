from cx_Freeze import setup, Executable
import os
import sys


name = "EFT Where am I"
version = "1.2"
description = "A Python program to easily get location in Tarkov"


# 필요한 DLL 파일 경로 추가
dll_files = [
    ("C:/Windows/SysWOW64/MSVCP140.dll", "MSVCP140.dll"),  # Visual C++ 재배포 가능 패키지 DLL
    ("C:/Windows/SysWOW64/VCRUNTIME140.dll", "VCRUNTIME140.dll")  # 추가 DLL 파일
]



# 포함할 파일 및 디렉터리 설정
include_files = dll_files + [
    ('translations/', 'translations/'),  # 번역 파일 폴더 포함
    ('settings.json', 'settings.json')  # settings.json 파일 포함
]

# 빌드 옵션 설정
options = {
    "build_exe": {
        "packages": ["os", "glob", "time", "sys", "json", "webbrowser", "PyQt5" ],
        "includes": ["PyQt5.QtCore", "PyQt5.QtGui", "PyQt5.QtWidgets", "PyQt5.QtWebEngineWidgets", "PyQt5.QtWebEngineCore"],
        "include_files": include_files,
        "excludes": [],
        "include_msvcr": True,  # Windows에서 필요한 경우 MSVC 런타임 라이브러리 포함
        "silent": True,
        "zip_include_packages": ["PyQt5"]
    }
}

# Windows에서 GUI 애플리케이션을 빌드할 때 base 설정
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