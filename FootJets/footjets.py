import pygame
import numpy as np
import time
import math
import mouse

pygame.init()
pygame.joystick.init()

if pygame.joystick.get_count() > 0:
  joystick = pygame.joystick.Joystick(0)
  joystick.init()

  pedal_value = 1.0 # 1.0 is "off" to start
  tps_timer_start = time.time()
  pwm_cycle_start = tps_timer_start
  ticks = 0
  # print('reset')
  latest_tps = 0
  jet_output = 0
  pwm_wavelength = 0.05


  last_jet_output = 0

  running = True
  while running:
    for event in pygame.event.get():
      if event.type == pygame.QUIT:
        running = False
      # elif event.type == pygame.JOYBUTTONDOWN:
      #   print(f"Button {event.button} pressed")
      # elif event.type == pygame.JOYBUTTONUP:
      #   print(f"Button {event.button} released")
      elif event.type == pygame.JOYAXISMOTION:
        axis = event.axis
        # value = event.value
        if axis == 2:
          # print(f"Axis {axis} value: {value}")
          pedal_value = event.value
    # convert joystick 1 to -1 to range 0 to 1
    jet_control_input = -0.5 * (pedal_value - 1.0)
    jet_control_input = np.clip(0, 1, jet_control_input)
    if jet_control_input < 0.07:
      jet_control_input = 0.0
    if jet_control_input > 1.0 - 0.05:
      jet_control_input = 1.0
    # print(pedal_value)
    # print(jet_control_input)

    new_time = time.time()
    tps_time_diff = new_time - tps_timer_start
    if (tps_time_diff > 1.0):
      latest_tps = ticks / tps_time_diff
      # print('latest_tps:', latest_tps)
      tps_timer_start = new_time
      ticks = 0
    
    if latest_tps > 10:
      pwm_delta = new_time - pwm_cycle_start
      pwm_factor = pwm_delta / pwm_wavelength
      pwm_factor_adjusted = np.clip(0.0,1.0,pwm_factor)
      # if pwm_factor_adjusted < 0.07:
      #   pwm_factor_adjusted = 0.0
      # if pwm_factor_adjusted > 1.0 - 0.05:
      #   pwm_factor_adjusted = 1.0
      jet_output = 0 if jet_control_input <= 0.05 or pwm_factor_adjusted > jet_control_input else 1
      # if jet_output == 0 and last_jet_output == 1:
        # print('jet output 0 and last jet output 1','pwm_factor_adjusted:',pwm_factor_adjusted,'jet_control_input:',jet_control_input)
      if pwm_factor >= 1.0:
        pwm_factor -= math.floor(pwm_factor)
        pwm_cycle_start = new_time + pwm_factor * pwm_wavelength # no lost time
      # print('jet_control_input:',jet_control_input,'pwm_delta:',pwm_delta,'jet_output:',jet_output)
    else:
      # print('latest_tps',latest_tps,'<= 10')
      jet_output = 0

    ticks += 1

    # control mouse
    if jet_output == 0 and last_jet_output == 1: # falling edge
      # print('release')
      mouse.release('right')
    elif jet_output == 1 and last_jet_output == 0: # rising edge
      # print('press')
      mouse.press('right')
    last_jet_output = jet_output
    

  pygame.joystick.quit()
  pygame.quit()
else:
  print("No gamepad found.")