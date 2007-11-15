#!/usr/bin/python
# -*- coding: euc-kr -*-

PROGRAM_NAME = "만죽이"
PROGRAM_VERSION = "0.5.1.3"
PROGRAM_AUTHOR = "yeoupooh at gmail dot com"
PROGRAM_INFO = PROGRAM_NAME + " " + PROGRAM_VERSION + " (" + PROGRAM_AUTHOR + ")"
PROGRAM_UPDATE_URL = 'http://yeoupooh.us.to:8080/ManZooGy'

import clr

clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')

clr.AddReference('PenStrokeRecognizer')

from System.Windows.Forms import *
from System.Drawing import *
from System.ComponentModel import *

from image.drawer import *
from image.navigator import *
from common.observer import *

from autoupdater import *
from PenStrokeRecognizer import *
from xbutton import *

class ImageObserver(Observer):
	control = None
	
	def __init__(self, control):
		self.control = control
	
	def update(self, args):
		#print "IO: update: args=" + args
		self.control.Refresh()

class FormMain(Form, Observable):
	idr = None
	inv = None
	rc = None
	
	help = None 
	oldDateTime = 0
	
	count = 255
	rect = Rectangle(0, 100, 500, 20)

	def __init__(self):
		print "initializing..."
	
		self.help = Image.FromFile("res\\help.png")
	
		c = form = self
		c.Text = PROGRAM_NAME + " " + PROGRAM_VERSION
		c.Icon = Icon("res\\pooh.ico")
		c.Width = 680
		c.Height = 400
		c.Left = 50
		c.Top = 50
		#c.StartPosition = FormStartPosition.CenterScreen
		c.WindowState = FormWindowState.Maximized
		c.FormBorderStyle = FormBorderStyle.None
		c.DoubleBuffered = True
		
		c.Paint += self.__form_Paint
		c.KeyDown += self.__form_KeyDown
		c.KeyUp += self.__form_KeyUp
		c.KeyPress += self.__form_KeyPress
		c.FormClosing += self.__form_Closing
		"""
		c.MouseDown += self.__form_MouseDown
		c.MouseMove += self.__form_MouseMove
		c.MouseUp += self.__form_MouseUp
		"""
		
		idr = self.idr = ImageDrawer()
		inv = self.inv = ImageNavigator(idr)
		inv.add_observer(ImageObserver(self))
		inv.load_config()
		
		rc = self.rc = RecognizerController(self)
		rc.Recognized += self.__rc_Recognized;
		
		self.oldDateTime = DateTime.Now
		
		c = self.components = Container()
		t = self.tm = Timer(c)
		t.Interval = 50
		t.Enabled = True
		t.Tick += self.__tm_Tick
		
		c = XButton()
		c.Location = Point(10, 10)
		c.Width = self.ClientRectangle.Width - 20
		c.Height = 40
		c.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
		c.Text = self.Text
		c.Font = Font("굴림", 20)
		c.KeyDown += self.__form_KeyDown
		c.KeyUp += self.__form_KeyUp
		self.Controls.Add(c)
		inv.add_observer(XButtonImageObserver(c))
		#self.add_observer(c)
		
	def __tm_Tick(self, sender, event):
		c = self.count
		c = c - 5
		if c < 0:
			c = 0
			t = self.tm
			t.Enabled = False
		self.count = c
		self.Invalidate(self.rect)
		
	def __form_KeyDown(self, sender, event):
		kc = event.KeyCode
		print "keydown: keycode=" + kc.ToString()
		print "keydown: keydata=", event.KeyData
		print "keydown: keyvalue=", event.KeyValue
		print "keydown: modifiers=", event.Modifiers

	def __form_KeyUp(self, sender, event):
		kc = event.KeyCode
		print "keyup: keycode=" + kc.ToString()

		print "ms=", DateTime.Now
		ts = DateTime.Now.Subtract(self.oldDateTime)
		if ts.TotalMilliseconds < 1000:
			return
		self.oldDateTime = DateTime.Now
		
		if kc == Keys.PageUp:
			self.inv.go_prev()
		elif kc == Keys.PageDown:
			self.inv.go_next()
			
	def __form_KeyPress(self, sender, event):
		kc = event.KeyChar
		print "keypress: keychar=", kc

	"""
	def __form_MouseDown(self, sender, event):
		print event.Location
		print event.Button
		print event.Delta
		
	def __form_MouseMove(self, sender, event):
		print event.Location
		print event.Button
		print event.Delta
		
	def __form_MouseUp(self, sender, event):
		print event.Location
		print event.Button
		print event.Delta
	"""
		
	def __form_Paint(self, sender, event):
		g = event.Graphics
		
		idr = self.idr
		if self.inv.Index == -1:
			g.DrawImage(self.help, 0, 0)
		else:
			idr.draw(g, self.ClientRectangle)
		
	def __form_Closing(self, sender, event):
		self.inv.save_config()
		
	def __rc_Recognized(self, sender, event):
		t = event.Text
		print "rc_text=[" + t + "]"
		
		if t == 'A':
			print PROGRAM_INFO
			self.inv.print_config()
		elif t == 'space':
			self.inv.go_next()
		elif t == 'back':
			self.inv.go_prev()
		elif t == 'O':
			self.inv.open_folder()
		elif t == 'R':
			self.inv.rotate_image()
		elif t == 'V':
			self.inv.change_view_direction()
		elif t == 'I':
			self.inv.toggle_use_interpolation()
		elif t == 'X':
			self.Close()
		elif t == 'F':
			print self.WindowState
			if self.WindowState == FormWindowState.Maximized:
				self.WindowState = FormWindowState.Normal
			else:
				self.WindowState = FormWindowState.Maximized
			self.Refresh()
	
if __name__ == '__main__':
	print PROGRAM_INFO
	c = AutoUpdater(PROGRAM_NAME, PROGRAM_VERSION, PROGRAM_UPDATE_URL)
	if c.has_new_version() == False:
		form = FormMain()
		Application.Run(form)
