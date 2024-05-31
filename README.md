[![GitHub Release](https://img.shields.io/github/v/release/karpitony/eft-where-am-i?include_prereleases&logo=github)](https://github.com/karpitony/eft-where-am-i/releases/latest)
[![GitHub total downloads](https://img.shields.io/github/downloads/karpitony/eft-where-am-i/total.svg?include_prerelease&logo=github)](https://github.com/karpitony/eft-where-am-i/releases)
[![GitHub latest downloads](https://img.shields.io/github/downloads/karpitony/eft-where-am-i/latest/total.svg?include_prerelease/latest&logo=github)](https://github.com/karpitony/eft-where-am-i/releases/latest)
[![GitHub License](https://img.shields.io/github/license/karpitony/eft-where-am-i)](./LICENSE)
[![made-with-python](https://img.shields.io/badge/Made%20with-Python-1f425f.svg)](https://www.python.org/)

# eft-where-am-i

[한국어](README_ko_kr.md)

- This is a Python + GUI program that helps you easily use the Maps feature of `Tarkov-Market`.
- It is based on [Rok's post and source code](https://gall.dcinside.com/m/eft/2143712).
- This program is licensed under the `MIT License`.
- Use at your risk.
<br />

1. [How to Use](#how-to-use)
2. [For devs](#for-devs)
    1. [Requirments](#requirements)
    2. [Insatllation](#installation)

## How to Use

**1. Select a map.**

<img src="assets/screenshot02.png" alt="screenshot02" width="800">

After selecting a map, make sure to click the `Apply` button!
<br />

**2. Take a screenshot during EFT raids.**

- The default screenshot key is `PrtSc`.
- If you check the `Auto Screenshot Detection` checkbox, your location will be automatically updated each time you take a screenshot.
- If you check the `Auto Screenshot Detection` checkbox, you can skip step 3.
  <br />

**3. Click the `Force Run` button.**

<img src="assets/screenshot03.png" alt="screenshot03" width="800">

A red dot indicating your location will appear on the map.

- To update your location, repeat steps 2 and 3.
- You can use the `Hide/Show Panels` or `Full Screen` buttons to view the map more clearly.

## For devs

### Requirements

- Python 3
- Anaconda (for virtual environment. not required. Use at your favor)

### Installation

If you use anaconda

1. Create new virtual environment

```bash
conda create -n eft-wmi python=3.9
```

2. Install dependencies

```bash
conda install --yes --file requirements.txt
```

If you use local python with pip3

1. Just install dependencies

```bash
pip3 install -r requirements.txt
```
