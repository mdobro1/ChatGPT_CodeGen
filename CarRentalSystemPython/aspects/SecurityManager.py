import os
import base64
import hashlib
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

        iv = os.urandom(self.BLOCK_SIZE)
        self.backend = default_backend()
        self.cipher = Cipher(algorithms.AES(key), modes.CBC(iv), backend=self.backend)
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


class UserRole:
    Admin = "Admin"
    PowerUser = "PowerUser"
    User = "User"
    Guest = "Guest"
