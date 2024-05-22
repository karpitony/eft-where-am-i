from cx_Freeze import setup, Executable

name = "EFT Where am I"
version = "1.1"
description = "Description of EFT Where am I"

executables = [
    Executable("main.py", base="Win32GUI", target_name="EFT_Where_am_I.exe")
]

options = {
    'build_exe': {
        'packages': ["keyboard", "selenium"],
        'includes': [],
        'include_files': [],
        'excludes': [],
    }
}

setup(
    name=name,
    version=version,
    description=description,
    options=options,
    executables=executables
)
