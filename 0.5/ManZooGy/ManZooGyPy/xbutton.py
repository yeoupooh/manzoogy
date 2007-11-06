import clr

clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')

from System.Windows.Forms import *
from System.Drawing import *
from System.ComponentModel import *

from common.observer import *

START_ALPHA = 150
FAST_STEP = 10
SLOW_STEP = 5

class XButtonImageObserver(Observer):
	control = None
	
	def __init__(self, control):
		self.control = control
	
	def update(self, args):
		c = self.control
		c.Text = args
		c.wakeup()
		#self.control.disappear()

class XButton(Control):
	tm = None
	count = START_ALPHA
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
		
	def wakeup(self):
		self.count = START_ALPHA
		self.Invalidate()
		self.step = SLOW_STEP
		self.tm.Enabled = True

	def disappear(self):
		self.step = FAST_STEP
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
		self.wakeup()
		
	def __OnMouseLeave(self, sender, event):
		self.disappear()
		
	def OnPaint(self, pe):
		g = pe.Graphics
		b = SolidBrush(Color.FromArgb(self.count, 100, 0, 0));
		bt = SolidBrush(Color.FromArgb(self.count, 255, 255, 255));
		g.FillRectangle(b, self.ClientRectangle)
		# string format
		# ref: http://blog.paranoidferret.com/index.php/2007/08/23/csharp-snippet-tutorial-how-to-draw-text-on-an-image/
		sf = StringFormat()
		sf.Alignment = StringAlignment.Center
		sf.LineAlignment = StringAlignment.Center
		# Rectangle to RectangleF (typecasting)
		# ref: http://msdn2.microsoft.com/ko-kr/library/system.drawing.rectanglef.op_implicit(VS.80).aspx
		r = self.ClientRectangle
		rf = RectangleF(r.X, r.Y, r.Width, r.Height)
		g.DrawString(self.Text, self.Font, bt, rf, sf)
		