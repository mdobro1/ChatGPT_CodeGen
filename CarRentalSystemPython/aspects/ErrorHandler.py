import datetime
import traceback


class ErrorHandler:
    

    def __init__(self, rethrow=False):
        self.rethrow = rethrow
        self.error_file_path = "errors.txt"

    def handle_error(self, ex : Exception):
        try:
            with open(self.error_file_path, 'a') as f:
                f.write(f"{datetime.datetime.now()} - {ex}\n")
                f.write(traceback.format_exc())
        except:
            # If an error occurs while writing to the error file, rethrow it
            raise

        if self.rethrow:
            raise ex
