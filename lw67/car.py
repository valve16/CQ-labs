from enum import Enum
from datetime import datetime

class Direction(Enum):
    STAND = 0
    FORWARD = 1

    BACKWARD = 2


class FuelSystem:
    def __init__(self, has_fuel=True):
        self._has_fuel = has_fuel

    def has_fuel(self):
        return self._has_fuel

class Car:
    NULL_SPEED = 0
    N_GEAR = 0
    R_GEAR = -1
    FIRST_GEAR = 1
    SECOND_GEAR = 2
    THIRD_GEAR = 3
    FOURTH_GEAR = 4
    FIFTH_GEAR = 5
    MIN_SPEED_SECOND_GEAR = 20
    MIN_SPEED_THIRD_GEAR = 30
    MIN_SPEED_FOURTH_GEAR = 40
    MIN_SPEED_FIFTH_GEAR = 50
    LIMIT_R_GEAR = 20
    LIMIT_FIRST_GEAR = 30
    LIMIT_SECOND_GEAR = 50
    LIMIT_THIRD_GEAR = 60
    LIMIT_FOURTH_GEAR = 90
    LIMIT_FIFTH_GEAR = 150


    def __init__(self, logger=None, gear=0,  clock=None, fuel_system=None, speed=0, engine_on=False, direction=Direction.STAND):
        self._gear = gear
        self._speed = speed
        self._engine_on = engine_on
        self._direction = direction
        self._logger = logger
        self._clock = clock or datetime
        self._fuel_system = fuel_system or FuelSystem()

    def turn_on_engine(self):
        if self._fuel_system.has_fuel():
            if not self._engine_on:
                self._engine_on = True
                if self._logger:
                    self._logger.log("Engine turned on at " + str(self._clock.now()))
        return self._engine_on

    def turn_off_engine(self):
        if not self._engine_on or (self._speed == self.NULL_SPEED and self._gear == self.N_GEAR):
            self._engine_on = False
            if self._logger:
                self._logger.log("Engine turned off at " + str(self._clock.now()))
            return True
        return False

    def is_turned_on(self):
        return self._engine_on

    def get_direction(self):
        return self._direction

    def get_speed(self):
        return self._speed

    def get_gear(self):
        return self._gear

    def set_gear(self, gear):
        if not self._engine_on:
            if gear == self.N_GEAR:
                self._gear = self.N_GEAR
                return True
            else:
                return False
        else:
            if gear == self.R_GEAR and self._speed == self.NULL_SPEED:
                self._gear = self.R_GEAR
                return True
            elif gear == self.N_GEAR:
                self._gear = self.N_GEAR
                return True
            elif gear == self.FIRST_GEAR and self._speed >= self.NULL_SPEED and self._speed <= self.LIMIT_FIRST_GEAR and self._direction != Direction.BACKWARD:
                self._gear = self.FIRST_GEAR
                return True
            elif gear == self.SECOND_GEAR and self._speed >= self.MIN_SPEED_SECOND_GEAR and self._speed <= self.LIMIT_SECOND_GEAR and self._direction == Direction.FORWARD:
                self._gear = self.SECOND_GEAR
                return True
            elif gear == self.THIRD_GEAR and self._speed >= self.MIN_SPEED_THIRD_GEAR and self._speed <= self.LIMIT_THIRD_GEAR and self._direction == Direction.FORWARD:
                self._gear = self.THIRD_GEAR
                return True
            elif gear == self.FOURTH_GEAR and self._speed >= self.MIN_SPEED_FOURTH_GEAR and self._speed <= self.LIMIT_FOURTH_GEAR and self._direction == Direction.FORWARD:
                self._gear = self.FOURTH_GEAR
                return True
            elif gear == self.FIFTH_GEAR and self._speed >= self.MIN_SPEED_FIFTH_GEAR and self._speed <= self.LIMIT_FIFTH_GEAR and self._direction == Direction.FORWARD:
                self._gear = self.FIFTH_GEAR
                return True
            else:
                return False

    def set_speed(self, speed):
        if self._gear == self.N_GEAR:
            if speed < self._speed:
                self._speed = speed
                if speed == self.NULL_SPEED:
                    self._direction = Direction.STAND
                return True
            else:
                return False
        elif speed >= self.NULL_SPEED:
            if self._gear == self.R_GEAR and speed <= self.LIMIT_R_GEAR:
                self._speed = speed
                self._direction = Direction.BACKWARD if speed > self.NULL_SPEED else Direction.STAND
                return True
            elif self._gear == self.FIRST_GEAR and speed <= self.LIMIT_FIRST_GEAR:
                self._speed = speed
                self._direction = Direction.FORWARD if speed > self.NULL_SPEED else Direction.STAND
                return True
            elif self._gear == self.SECOND_GEAR and speed >= self.MIN_SPEED_SECOND_GEAR and speed <= self.LIMIT_SECOND_GEAR:
                self._speed = speed
                return True
            elif self._gear == self.THIRD_GEAR and speed >= self.MIN_SPEED_THIRD_GEAR and speed <= self.LIMIT_THIRD_GEAR:
                self._speed = speed
                return True
            elif self._gear == self.FOURTH_GEAR and speed >= self.MIN_SPEED_FOURTH_GEAR and speed <= self.LIMIT_FOURTH_GEAR:
                self._speed = speed
                return True
            elif self._gear == self.FIFTH_GEAR and speed >= self.MIN_SPEED_FIFTH_GEAR and speed <= self.LIMIT_FIFTH_GEAR:
                self._speed = speed
                return True
            else:
                return False
        else:
            return False