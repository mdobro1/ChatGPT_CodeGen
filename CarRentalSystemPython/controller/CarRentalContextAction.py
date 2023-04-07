class CarRentalContextAction:
    def __init__(self, func):
        self.func = func

    def __call__(self, *args, **kwargs):
        return self.func(*args, **kwargs)
    
    '''
    def __call__(self, action: CarRentalContextAction) -> CarRentalContextAction:
        result = action(self)
        if isinstance(result, CarRentalContextAction):
            return result
        else:
            return None
    '''
