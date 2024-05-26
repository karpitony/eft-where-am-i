from cx_Freeze import setup, Executable
import sys
import os

name = "EFT Where am I"
version = "1.2"
author = 'karpitony'
author_email = 'watemark1017@gmail.com'
url = 'https://github.com/karpitony/eft-where-am-i'
description = "A Python program to easily get directions in Tarkov"

# 필요한 DLL 파일 경로 추가
dll_files = [
    ("C:/Windows/SysWOW64/MSVCP140.dll", "MSVCP140.dll"),  # Visual C++ 재배포 가능 패키지 DLL
    ("C:/Windows/SysWOW64/VCRUNTIME140.dll", "VCRUNTIME140.dll")  # 추가 DLL 파일
]

options = {
    'build_exe': {
        'packages': ["PyQt5", "PyQt5.QtWebEngineWidgets", "os", "glob", "time", "sys"],
        'include_files': dll_files,
        'excludes': [],
    },
    'bdist_msi': {
        'upgrade_code': 'GUID',  # GUID 사용
        'add_to_path': False,
        'initial_target_dir': r'[ProgramFilesFolder]\EFTWhereAmI',
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
    author=author,
    author_email=author_email,
    url=url,
    description=description,
    options=options,
    executables=executables
)
