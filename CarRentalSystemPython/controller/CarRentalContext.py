from __future__ import annotations
from typing import Optional
from entities.Transaction import Transaction
from controller.CarRentalSystem import CarRentalSystem

class CarRentalContext:
    def __init__(
        self,
        command: Optional[str] = None,
        customer_id: Optional[str] = None,
        car_id: Optional[str] = None,
        car_rental_system: Optional[CarRentalSystem] = None,
        rental_transaction: Optional[Transaction] = None,
        action_completed: Optional[bool] = None
    ):
        self.command: Optional[str] = command
        self.customer_id: Optional[str] = customer_id
        self.car_id: Optional[str] = car_id
        self.car_rental_system: Optional[CarRentalSystem] = car_rental_system
        self.rental_transaction: Optional[Transaction] = rental_transaction
        self.action_completed: Optional[bool] = action_completed