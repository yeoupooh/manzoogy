import clr

clr.AddReference('System.Drawing')

from System.Drawing import *
from System.Drawing.Drawing2D import *

from drawer import *
from enums import *

class ImageDrawer:
	ViewPart = ViewPartType.Full
	TargetBounds = None
	Image = None
	Rotation = ViewRotationType.Degree0
	UseInterpolation = False
	
	def draw(self, g, rect = None):
		if rect != None:
			self.TargetBounds = rect
		drawingBounds = self.TargetBounds
		sourceBounds = self.TargetBounds
		
		if self.Image != None:
			if self.ViewPart == ViewPartType.Full:
				sourceBounds = Rectangle(0, 0, self.Image.Width, self.Image.Height)
			elif self.ViewPart == ViewPartType.Left:
				sourceBounds = Rectangle(0, 0, self.Image.Width / 2, self.Image.Height)
			elif self.ViewPart == ViewPartType.Right:
				sourceBounds = Rectangle(self.Image.Width / 2, 0, self.Image.Width / 2, self.Image.Height)
			# interpolation
			# ref: http://www.codeproject.com/cs/media/AntiAliasingIssues.asp
			# ref: http://photocontroller.imagecomponent.net/v1/src/controller%20member/controller_interpolation.html
			if self.UseInterpolation == False:
				g.InterpolationMode = InterpolationMode.Default
				g.CompositingQuality = CompositingQuality.HighSpeed
				g.SmoothingMode = SmoothingMode.HighSpeed
				g.PixelOffsetMode = PixelOffsetMode.HighSpeed
			else:
				g.InterpolationMode = InterpolationMode.HighQualityBicubic
				# ref: http://msdn2.microsoft.com/en-us/library/system.drawing.drawing2d.compositingquality.aspx
				g.CompositingQuality = CompositingQuality.HighQuality
				# ref: http://msdn2.microsoft.com/en-us/library/z714w2y9.aspx
				g.SmoothingMode = SmoothingMode.HighQuality
				# ref: http://msdn2.microsoft.com/en-us/library/system.drawing.drawing2d.pixeloffsetmode.aspx
				g.PixelOffsetMode = PixelOffsetMode.HighQuality
			
			m = Matrix()
			if self.Rotation == ViewRotationType.Degree90:
				m.Rotate(90)
				g.Transform = m
				drawingBounds = Rectangle(0, -self.TargetBounds.Width, self.TargetBounds.Height, self.TargetBounds.Width)
			elif self.Rotation == ViewRotationType.Degree180:
				m.Rotate(180)
				g.Transform = m
				drawingBounds = Rectangle(-self.TargetBounds.Width, -self.TargetBounds.Height, self.TargetBounds.Width, drawingBounds.Height)
			elif self.Rotation == ViewRotationType.Degree270:
				m.Rotate(270)
				g.Transform = m
				drawingBounds = Rectangle(-self.TargetBounds.Height, 0, self.TargetBounds.Height, self.TargetBounds.Width)
			
			g.DrawImage(self.Image, drawingBounds, sourceBounds, GraphicsUnit.Pixel)
		else:
			print "Drawer: no image"
			g.DrawRectangle(Pens.Black, drawingBounds)
