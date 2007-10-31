from singleton import *

class ICommand:
	def execute(self, sender = None, event = None):
		pass

class Commander:
	cmds = {}
	
	def set(self, key, cmd):
		self.cmds[key] = cmd
		print "Command set: cmd=" + key
	
	def get(self, key):
		try:
			cmd = self.cmds[key]
		except KeyError:
			print "command not found: key=" + key
			return None
			
		return self.cmds[key]
		
	def get_by_tag(self, obj):
		return get(obj.Tag)
	
	def execute(self, key, sender = None, event = None):
		cmd = self.get(key)
		if cmd != None:
			cmd.execute(sender, event)
			
	def execute_by_tag(self, obj, sender = None, event = None):
		self.execute(obj.Tag, sender, event)
			
class TestCmd(ICommand):
	msg = ''
	
	def __init__(self, msg):
		self.msg = msg
		
	def execute(self, sender = None, event = None):
		print "test:" + self.msg
		
if __name__ == '__main__':
	get_singleton(Commander).set('test1', TestCmd('yes'))
	get_singleton(Commander).set('test2', TestCmd('no'))
	
	get_singleton(Commander).execute('test1')
	get_singleton(Commander).execute('test2')
	get_singleton(Commander).execute('test1')
	get_singleton(Commander).execute('test2')
	
	get_singleton(Commander).execute('test')
