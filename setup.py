from cx_Freeze import setup, Executable
import sys

# 빌드 옵션 설정
build_exe_options = {
    "packages": ["os", "glob", "time", "sys"],
    "includes": [
        "PyQt5",
        "PyQt5.QtCore",
        "PyQt5.QtGui",
        "PyQt5.QtWidgets",
        "PyQt5.QtWebEngineWidgets",
        "PyQt5.QtWebEngineCore"
    ],
    "include_files": [
        ('translations/', 'translations/'),  # 번역 파일 폴더 포함
        ('settings.json', 'settings.json')  # settings.json 파일 포함
    ],
    "excludes": ["tkinter", "unittest"],
    "zip_include_packages": ["encodings", "PyQt5"],
}

# Windows에서 GUI 애플리케이션을 빌드할 때 base 설정
base = None
if sys.platform == "win32":
    base = "Win32GUI"

# setup 함수 호출
setup(
    name = "EFT Where am I",
    version = "1.2",
    description = "A Python program to easily get locations in Tarkov",
    options = { "build_exe": build_exe_options },
    executables = [ Executable("main.py", base=base, target_name="EFT_Where_am_I.exe") ]
)
