import os
import base64
import hashlib
from enum import Enum
from cryptography.hazmat.primitives.ciphers import Cipher, algorithms, modes
from cryptography.hazmat.primitives import padding
from cryptography.hazmat.backends import default_backend

class SecurityManager:
    KEY_FILE = "key.bin"
    BLOCK_SIZE = 16
    KEY_SIZE = 32

    def __init__(self, key : bytes = None):
        if key is None:
            self.key = self.get_or_create_key()
        else:
            self.key = key

        self.iv = os.urandom(self.BLOCK_SIZE)
        self.backend = default_backend()
        self.cipher = Cipher(algorithms.AES(self.key), modes.CBC(self.iv), backend=self.backend)
        self.padder = padding.PKCS7(self.BLOCK_SIZE * 8).padder()
        self.unpadder = padding.PKCS7(self.BLOCK_SIZE * 8).unpadder()

    def get_or_create_key(self)->bytes:
        key_path = self.KEY_FILE
        if os.path.exists(key_path):
            with open(key_path, 'rb') as f:
                key = f.read()
        else:
            key = self._generate_key()
            with open(key_path, 'wb') as f:
                f.write(key)
        return key

    def _generate_key(self)->bytes:
        return os.urandom(self.KEY_SIZE)

    def encrypt(self, data:str)->str:
        encryptor = self.cipher.encryptor()
        padded_data = self.padder.update(data.encode()) + self.padder.finalize()    
        encrypted_data = encryptor.update(padded_data) + encryptor.finalize()
        return base64.b64encode(encrypted_data).decode()
    
    def encrypt2(self, data):
        iv = os.urandom(16)
        data_bytes = data.encode('utf-8')
        padder = padding.PKCS7(algorithms.AES.block_size).padder()
        padded_data = padder.update(data_bytes) + padder.finalize()
        cipher = Cipher(algorithms.AES(self.key), modes.CBC(iv), backend=default_backend())
        encryptor = cipher.encryptor()
        encrypted_data = encryptor.update(padded_data) + encryptor.finalize()
        return base64.b64encode(iv + encrypted_data).decode('utf-8')

    def decrypt(self, data:str)->str:   
        decryptor = self.cipher.decryptor()
        data = base64.b64decode(data)
        decrypted_data = decryptor.update(data) + decryptor.finalize()
        unpadded_data = self.unpadder.update(decrypted_data) + self.unpadder.finalize()
        return unpadded_data.decode()

    def authorize(self, user, permission):
        if user is None:
            return False

        if user.Role == UserRole.Admin:
            return True
        elif user.Role == UserRole.PowerUser:
            return permission == "read" or permission == "write"
        elif user.Role == UserRole.User:
            return permission == "read"
        else:
            return False


class User:
    def __init__(self, id, name, role):
        self.Id = id
        self.Name = name
        self.Role = role


class UserRole(Enum):
    ADMIN = 1
    MANAGER = 2
    USER = 3
