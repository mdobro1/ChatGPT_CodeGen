import datetime

class MessageHandler:

    def __init__(self, error_handler):
        self.log_file_path = "log.txt"
        self.error_handler = error_handler

    def log(self, message):
        try:
            with open(self.log_file_path, 'a') as f:
                f.write(f"{datetime.datetime.now()} - {message}\n")
        except Exception as ex:
            self.error_handler.handle_error(ex)

    def show_message(self, message):
        print(message)

    def log_plus_message(self, message):
        self.log(message)
        self.show_message(message)
