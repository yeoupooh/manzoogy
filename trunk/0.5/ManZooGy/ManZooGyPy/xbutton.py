import clr

clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')

from System.Windows.Forms import *
from System.Drawing import *
from System.ComponentModel import *

from common.observer import *

class XButton(Control, Observer):
	tm = None
	count = 255
	step = 1
	
	def __init__(self):
		c = self
		c.MouseEnter += self.__OnMouseEnter
		c.MouseLeave += self.__OnMouseLeave
		
		# Giving Your Control a Transparent Background
		# ref: http://msdn2.microsoft.com/en-us/library/wk5b13s4(VS.71).aspx
		c.SetStyle(ControlStyles.SupportsTransparentBackColor, True)
		c.BackColor = Color.Transparent

		# ±ôºý°Å¸² ¹æÁö
		c.DoubleBuffered = True
			
		c = self.components = Container()
		t = self.tm = Timer(c)
		t.Interval = 50
		t.Tick += self.__tm_Tick
		t.Enabled = True
		
	def update(self, args):
		self.count = 255
		self.Invalidate()
		self.step = 1
		self.tm.Enabled = True

	def __tm_Tick(self, sender, event):
		c = self.count
		c = c - self.step
		if c < 0:
			c = 0
			self.tm.Enabled = False
		self.count = c
		self.Invalidate()

	def __OnMouseEnter(self, sender, event):
		self.count = 255
		self.Invalidate()
		self.step = 1
		self.tm.Enabled = True
		
	def __OnMouseLeave(self, sender, event):
		self.step = 10
		self.tm.Enabled = True
		
	def OnPaint(self, pe):
		g = pe.Graphics
		b = SolidBrush(Color.FromArgb(self.count, 100, 0, 0));
		bt = SolidBrush(Color.FromArgb(self.count, 255, 255, 255));
		g.FillRectangle(b, self.ClientRectangle)
		g.DrawString(self.Text, self.Font, bt, 0, 0)

		