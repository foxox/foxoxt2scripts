import keyboard # https://github.com/boppreh/keyboard
import subprocess

keyboard.press_and_release('left alt+left shift+1')

jet_pedal_process = subprocess.Popen(["python","../JetPedal/jet_pedal.py"], shell=True)

subprocess.run(["C:/XProgramFiles/Sierra/2025preconfigured/GameData/Tribes2.exe","-nologin", "-mod classic"], shell=True)

jet_pedal_process.terminate()
