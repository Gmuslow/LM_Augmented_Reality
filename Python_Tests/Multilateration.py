import matplotlib.pyplot as plt
import argparse
import math
import numpy as np

coord_filename = "current_coord.txt"
current_coordinates = [] #the 2D current coordinate of the user.
rssi_values = []

"""Extract coordinate, RSSI from file"""
with open(coord_filename, "r") as infile:
    for line in infile.readlines():
        line = line.replace("{", "").replace("}", "").strip()
        print(line.split(";"))
        coord, RSSI = line.split(";")
        RSSI = float(RSSI)
        print(coord)
        coord = coord.replace("(", "").replace(")", "").strip()
        x = coord.split(",")[0]
        y = coord.split(",")[1]
        current_coordinates.append([float(x), float(y)])
        rssi_values.append(RSSI)
        
print(f"Current Coordinates: {current_coordinates}")
print(f"Current RSSI values: {rssi_values}")

"""Translate RSSI into meters to get radius of circle."""
radii = []
for val in rssi_values:
    N = 4 #environmental factor
    measured_power = 40 #measured power at 1m
    exp = (measured_power - val) / (10 * N)
    radius = math.pow(10, exp)
    radii.append(radius)

"""Create Starting Grid and loop through each point to calculate 
distance from circles"""
maxDist = 10 #max distance in the x and y directions (10, 10) in meters
maxPoints = 21 #how many points will be between (-maxDist, maxDist)
candidate_points = []
possible_x = np.linspace(-maxDist, maxDist, maxPoints)
possible_y = np.linspace(-maxDist, maxDist, maxPoints)

for x in possible_x:
    for y in possible_y:
        candidate_points.append([x, y])
print(candidate_points)

"""Loop through all points"""
for candidate in candidate_points:
    for i in range(len(current_coordinates)):
        vector = candidate - current_coordinates[i]
        mag = math.sqrt(math.pow(vector[0], 2) + math.pow(vector[1], 2)) #distance formula
        distance = abs(mag - radii[i]) #distance = distance from candidate point to the edge of the current circle.


"""The idea is, the larger the circle, the less likely it is to be accurate since
the signals are more likely to be noisy, resulting in inaccurate RSSI values."""
def PriorityFunction(value):
    