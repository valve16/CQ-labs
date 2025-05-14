import unittest
from unittest.mock import Mock, patch
from car import Car, Direction

class TestCarStyle(unittest.TestCase):
    def setUp(self):
        self.car = Car()

    def test_initial_state(self):
        self.assertFalse(self.car.is_turned_on())
        self.assertEqual(self.car.get_direction(), Direction.STAND)
        self.assertEqual(self.car.get_speed(), 0)
        self.assertEqual(self.car.get_gear(), 0)

    def test_engine_on_logging_with_time_on(self):
        mock_logger = Mock()
        mock_clock = Mock()
        mock_clock.now.return_value = "2025-05-14 12:00"
        car = Car(logger=mock_logger, clock=mock_clock)
        car.turn_on_engine()
        mock_logger.log.assert_called_with("Engine turned on at 2025-05-14 12:00")

    def test_engine_off_logging_with_time_off(self):
        mock_logger = Mock()
        mock_clock = Mock()
        mock_clock.now.return_value = "2025-05-14 12:01"
        car = Car(logger=mock_logger, clock=mock_clock)
        car.turn_on_engine()
        car.turn_off_engine()
        mock_logger.log.assert_called_with("Engine turned off at 2025-05-14 12:01")

    def engine_not_start_if_no_fuel(self):
        mock_fuel = Mock()
        mock_fuel.has_fuel.return_value = False
        car = Car(fuel_system=mock_fuel)
        car.turn_on_engine()
        self.assertFalse(car.is_turned_on())

    def engine_start_if_fuel(self):
        mock_fuel = Mock()
        mock_fuel.has_fuel.return_value = True
        car = Car(fuel_system=mock_fuel)
        car.turn_on_engine() 
        self.assertTrue(car.is_turned_on())

    def test_turn_on_engine_changes_state(self):
        self.car.turn_on_engine()
        self.assertTrue(self.car.is_turned_on())

    def test_turn_off_engine_when_stopped(self):
        self.car.turn_on_engine()
        self.assertTrue(self.car.turn_off_engine())
        self.assertFalse(self.car.is_turned_on())

    def test_try_turn_off_engine_when_moving(self):
        self.car.turn_on_engine()
        self.car.set_gear(1)
        self.car.set_speed(10)
        self.assertFalse(self.car.turn_off_engine())
        self.assertTrue(self.car.is_turned_on())

    def test_try_set_gear_fails_when_engine_off(self):
        self.assertFalse(self.car.set_gear(1))
        self.assertEqual(self.car.get_gear(), 0)
        self.assertTrue(self.car.set_gear(0))
        self.assertEqual(self.car.get_gear(), 0)

    def test_set_reverse_gear_engine_on_and_car_stopped(self):
        self.car.turn_on_engine()
        self.assertTrue(self.car.set_gear(-1))
        self.assertEqual(self.car.get_gear(), -1)

    def test_direction_changes_from_forward_to_stand_on_gear(self):
        self.car.turn_on_engine()
        self.car.set_gear(1)
        self.car.set_speed(10)
        self.assertEqual(self.car.get_direction(), Direction.FORWARD)
        self.car.set_speed(0)
        self.assertEqual(self.car.get_direction(), Direction.STAND)

    def test_direction_changes_from_forward_to_stand_on_neutral(self):
        self.car.turn_on_engine()
        self.car.set_gear(1)
        self.car.set_speed(10)
        self.car.set_gear(0)
        self.car.set_speed(0)
        self.assertEqual(self.car.get_direction(), Direction.STAND)

    def test_set_speed_in_reverse_gear(self):
        self.car.turn_on_engine()
        self.car.set_gear(-1)
        self.assertTrue(self.car.set_speed(10))
        self.assertEqual(self.car.get_direction(), Direction.BACKWARD)

    def test_try_set_reverse_gear_moving_forward(self):
        self.car.turn_on_engine()
        self.car.set_gear(1)
        self.car.set_speed(10)
        self.assertFalse(self.car.set_gear(-1))


    def test_try_up_speed_in_neutral(self):
        self.car.turn_on_engine()
        self.car.set_gear(1)
        self.car.set_speed(10)
        self.car.set_gear(0)
        self.assertFalse(self.car.set_speed(15))
        self.assertTrue(self.car.set_speed(5))
        self.assertEqual(self.car.get_speed(), 5)


    def test_set_valid_speed_from_1_to_5_gear_limit_on_5_gear(self):
        self.car.turn_on_engine()
        self.assertTrue(self.car.set_gear(1))
        self.assertTrue(self.car.set_speed(30))

        self.assertTrue(self.car.set_gear(2))
        self.assertTrue(self.car.set_speed(50))

        self.assertTrue(self.car.set_gear(3))
        self.assertTrue(self.car.set_speed(60))

        self.assertTrue(self.car.set_gear(4))
        self.assertTrue(self.car.set_speed(70))

        self.assertTrue(self.car.set_gear(5))
        self.assertTrue(self.car.set_speed(150))
        self.assertEqual(self.car.get_speed(), 150)

    def test_set_invalid_speed_from_1_to_5_gear(self):
        self.car.turn_on_engine()
        self.assertTrue(self.car.set_gear(1))
        self.assertFalse(self.car.set_speed(-1))
        self.assertFalse(self.car.set_speed(31))
        self.car.set_speed(30)

        self.assertTrue(self.car.set_gear(2))
        self.assertFalse(self.car.set_speed(19))
        self.assertFalse(self.car.set_speed(51))
        self.car.set_speed(40)

        self.assertTrue(self.car.set_gear(3))
        self.assertFalse(self.car.set_speed(29))
        self.assertFalse(self.car.set_speed(61))
        self.car.set_speed(50)

        self.assertTrue(self.car.set_gear(4))
        self.assertFalse(self.car.set_speed(39))
        self.assertFalse(self.car.set_speed(91))
        self.car.set_speed(60)

        self.assertTrue(self.car.set_gear(5))
        self.assertFalse(self.car.set_speed(49))
        self.assertFalse(self.car.set_speed(151))




if __name__ == '__main__':
    unittest.main()