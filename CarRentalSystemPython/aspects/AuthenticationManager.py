import os

from enum import Enum
from typing import Dict, List

from aspects import SecurityManager
from aspects import DataManager
from aspects.SecurityManager import UserRole



class AuthenticationManager:
    CREDENTIALS_FILE_PATH = "data/credentials.txt"

    def __init__(self, security_manager: SecurityManager):
        self.credentials = {}
        self.user_roles = {}
        self.security_manager = security_manager

    def login(self, username: str, password: str) -> bool:
        if username in self.credentials and self.credentials[username] == self.__hash_password(password):
            print("Login successful.")
            return True
        else:
            print("Login failed.")
            return False

    def logout(self) -> None:
        print("Logout successful.")

    def load_credentials(self) -> None:
        if not os.path.exists(self.CREDENTIALS_FILE_PATH):
            raise FileNotFoundError("credentials file not found.")

        lines = DataManager.read_lines_from_file(self.CREDENTIALS_FILE_PATH)
        for line in lines:
            parts = line.split(",")
            if len(parts) == 2:
                username = parts[0].strip()
                password_hash = parts[1].strip()
                self.credentials[username] = password_hash

    def load_roles(self) -> None:
        if not os.path.exists(self.CREDENTIALS_FILE_PATH):
            raise FileNotFoundError("credentials file not found.")

        lines = DataManager.read_lines_from_file(self.CREDENTIALS_FILE_PATH)
        for line in lines:
            parts = line.split(",")
            if len(parts) == 2:
                username = parts[0].strip()
                role = UserRole(int(parts[1].strip()))
                self.user_roles[username] = role

    def save_credentials(self) -> None:
        lines = [f"{username},{password_hash}" for username, password_hash in self.credentials.items()]
        DataManager.write_lines_to_file(self.CREDENTIALS_FILE_PATH, lines)

    def save_roles(self) -> None:
        lines = [f"{username},{role.value}" for username, role in self.user_roles.items()]
        DataManager.write_lines_to_file(self.CREDENTIALS_FILE_PATH, lines)

    def register_user(self, username: str, password: str, role: UserRole) -> None:
        self.credentials[username] = self.__hash_password(password)
        self.save_credentials()

        self.user_roles[username] = role
        self.save_roles()

        print("User registered.")

    def unregister_user(self, username: str) -> None:
        if username in self.credentials:
            self.credentials.pop(username)
            self.save_credentials()
            print("User unregistered.")
        elif username in self.user_roles:
            self.user_roles.pop(username)
            self.save_roles()
        else:
            print("User not found.")

    def __hash_password(self, password: str) -> str:
        return self.security_manager.encrypt(password)
