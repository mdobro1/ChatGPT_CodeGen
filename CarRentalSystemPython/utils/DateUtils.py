from abc import ABC, abstractmethod
from datetime import datetime
import time


class DateUtils(ABC):
    
    @staticmethod
    def parse_date(strDate: str, defaultDate : datetime.date = datetime.now().date()) -> datetime.date:
        resultDate: datetime = defaultDate
        
        try:
            resultDate = datetime.strptime(strDate, '%Y-%m-%d')
        except ValueError:
            resultDate = defaultDate
        
        return resultDate        
    
    @staticmethod
    def date_to_str(date_value: datetime.date) -> str:
        result = date_value.strftime("%Y-%m-%d") if date_value else None
        return result   