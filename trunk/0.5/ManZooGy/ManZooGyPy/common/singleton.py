# ref: http://c2.com/cgi/wiki?PythonSingleton
singletons = {}

def get_singleton(cls):
	if cls in singletons:
		return singletons[cls]
	singletons[cls] = cls()
	return singletons[cls]

def has_singleton(cls):
	if cls in singletons:
		return True
	else:
		return False
