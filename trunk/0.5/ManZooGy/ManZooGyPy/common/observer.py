from System import *
from System.Collections.Generic import *

class Observer(Object):
	def update(self, args = None):
		pass
	
class Observable:
	observers = List[Observer]()
	
	def add_observer(self, observer):
		self.observers.Add(observer)
		
	def remove_observer(self, observer):
		self.observers.Remove(observer)
		
	def notify_observers(self, args = None):
		for observer in self.observers:
			observer.update(args)

if __name__ == '__main__':
	class TestObserver(Observer):
		def __init__(self, name):
			self.name = name
			
		def update(self):
			print self.name + " updated"
			
	class TestObservable(Observable):
		def __init__(self):
			self.add_observer(TestObserver('1'))
			self.add_observer(TestObserver('2'))
			self.add_observer(TestObserver('3'))
			
		def changed(self):
			self.notify_observers()

	c = TestObservable()
	c.changed()
