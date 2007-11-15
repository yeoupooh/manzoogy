#!/usr/bin/python
# -*- coding: euc-kr -*-

CONFIG_VERSION = "0.1"
CONFIG_FIlE = "config.ini"
BG_FILE = "bg.png"

import clr

clr.AddReference('System.Drawing')
clr.AddReference('System.Windows.Forms')

from System import *
from System.IO import *
from System.Drawing import *
from System.Collections.Generic import *
from System.Windows.Forms import *
from System.Text import *

from common.observer import *
from enums import *

class ImageNavigator(Observable):
	# config 
	Folder = None
	ViewDirection = ViewDirectionType.Full
	Drawer = None
	Index = -1
	
	# others
	Filenames = None
	FullScreen = False
	BgImage = None
	
	def __init__(self, drawer):
		self.Drawer = drawer
		
		self.Filenames = List[str]()
	
		"""
		ScreenCapture.CaptureScreen(BG_FILE, ImageFormat.Png)
		BgImage = Image.FromFile(BG_FILE)
		"""
		
	def __set_image(self, index = None):
		if self.Filenames.Count > 0:
			if index != None:
				self.Index = index
			elif self.Index == -1:
				self.Index = 0
				
			if self.Drawer.Image != None:
				self.Drawer.Image.Dispose()
				
			self.Drawer.Image = Image.FromFile(self.Filenames[self.Index])
			print "set image: image loaded. img=" + self.Filenames[self.Index]
			#self.save_config()
			self.__set_changed()
		else:
			self.Index = -1
			self.__set_changed()
		
	def __set_changed(self):
		sb = StringBuilder()
		sb.Append('회전:' + self.Drawer.Rotation.ToString())
		sb.Append(' ')
		sb.Append('방향:' + self.ViewDirection.ToString())
		sb.Append(' ')
		sb.Append('좌우:' + self.Drawer.ViewPart.ToString())
		sb.Append(' ')
		sb.Append('화질:' + self.Drawer.UseInterpolation.ToString())
		sb.Append(' ')
		if self.Index > -1:
			sb.Append('페이지:' + self.Index.ToString())
			sb.Append(' ')
			sb.Append('파일:' + Path.GetFileName(self.Filenames[self.Index]))
		self.notify_observers(sb.ToString())
		
	def __clean_images(self):
		self.Filenames.Clear()
		self.Index = -1
		
	def __add_file(self, filename):
		ext = Path.GetExtension(filename).ToLower()
		if ext == '.png' or ext == '.bmp' or ext == '.jpg' or ext == '.jpeg' or ext == '.gif':
			self.Filenames.Add(filename)
		
	def __set_prev_image(self):
		if self.Filenames.Count > 0:
			if self.Index > 0:
				self.Index = self.Index - 1
			else:
				self.Index = self.Filenames.Count - 1
			self.__set_image()
	
	def __set_next_image(self):
		if self.Filenames.Count > 0:
			if self.Index < self.Filenames.Count - 1:
				self.Index = self.Index + 1
			else:
				self.Index = 0
			self.__set_image()
	
	def __load_folder(self, folder):
		if folder != None:
			if Directory.Exists(folder) == True:
				self.__clean_images()
				for filename in Directory.GetFiles(folder):
					self.__add_file(filename)
					
				if self.Filenames.Count == 0:
					print "no files."
				else:
					print "files=", self.Filenames.Count
			else:
				print "folder not found. folder=", folder
		else:
			print "no folder name."
			
	def load_config(self):
		if File.Exists(CONFIG_FIlE) == True:
			sr = StreamReader(CONFIG_FIlE)
			version = sr.ReadLine()
			index = -1
			if version == "0.1":
				self.Folder = sr.ReadLine()
				self.ViewDirection = Convert.ToInt32(sr.ReadLine())
				index = Convert.ToInt32(sr.ReadLine())
				self.Drawer.ViewPart = Convert.ToInt32(sr.ReadLine())
				self.Drawer.UseInterpolation = Convert.ToBoolean(sr.ReadLine())
				self.Drawer.Rotation = Convert.ToInt32(sr.ReadLine())
			sr.Close()
			
			self.print_config()
			
			self.__load_folder(self.Folder)
			self.__set_image(index)
			print "config file loaded."
		else:
			print "config file not found."

	def save_config(self):
		if self.Filenames.Count > 0:
			sw = StreamWriter(CONFIG_FIlE)
			sw.WriteLine("0.1")
			sw.WriteLine(self.Folder)
			sw.WriteLine(self.ViewDirection.ToString())
			sw.WriteLine(self.Index.ToString())
			sw.WriteLine(self.Drawer.ViewPart.ToString())
			sw.WriteLine(self.Drawer.UseInterpolation.ToString())
			sw.WriteLine(self.Drawer.Rotation.ToString())
			sw.Close()
			print "config file saved."

	def print_config(self):
		print
		print "version=", CONFIG_VERSION
		print "folder=", self.Folder
		print "view direction=", self.ViewDirection
		print "index=", self.Index
		print "viewpart=", self.Drawer.ViewPart
		print "useinterpolation=", self.Drawer.UseInterpolation
		print "rotation=", self.Drawer.Rotation
		print
			
	def go_prev(self):
		print "prev: curr=" + self.Index.ToString() + ", part=" + self.Drawer.ViewPart.ToString()
		if self.Filenames.Count > 0:
			if self.ViewDirection == ViewDirectionType.Full:
				self.__set_prev_image()
			elif self.ViewDirection == ViewDirectionType.LeftToRight:
				if self.Drawer.ViewPart == ViewPartType.Right:
					self.Drawer.ViewPart = ViewPartType.Left
					self.__set_changed()
				elif self.Drawer.ViewPart == ViewPartType.Left:
					self.Drawer.ViewPart = ViewPartType.Right
					self.__set_prev_image()
			elif self.ViewDirection == ViewDirectionType.RightToLeft:
				if self.Drawer.ViewPart == ViewPartType.Right:
					self.Drawer.ViewPart = ViewPartType.Left
					self.__set_prev_image()
				elif self.Drawer.ViewPart == ViewPartType.Left:
					self.Drawer.ViewPart = ViewPartType.Right
					self.__set_changed()
		print "prev: new=" + self.Index.ToString() + ", part=" + self.Drawer.ViewPart.ToString()
	
	def go_next(self):
		print "next: curr=" + self.Index.ToString() + ", part=" + self.Drawer.ViewPart.ToString()
		if self.Filenames.Count > 0:
			if self.ViewDirection == ViewDirectionType.Full:
				self.__set_next_image()
			elif self.ViewDirection == ViewDirectionType.LeftToRight:
				if self.Drawer.ViewPart == ViewPartType.Right:
					self.Drawer.ViewPart = ViewPartType.Left
					self.__set_next_image()
				elif self.Drawer.ViewPart == ViewPartType.Left:
					self.Drawer.ViewPart = ViewPartType.Right
					self.__set_changed()
			elif self.ViewDirection == ViewDirectionType.RightToLeft:
				if self.Drawer.ViewPart == ViewPartType.Right:
					self.Drawer.ViewPart = ViewPartType.Left
					self.__set_changed()
				elif self.Drawer.ViewPart == ViewPartType.Left:
					self.Drawer.ViewPart = ViewPartType.Right
					self.__set_next_image()
		print "next: new=" + self.Index.ToString() + ", part=" + self.Drawer.ViewPart.ToString()
					
	def go_first(self):
		if self.Filenames.Count > 0:
			self.Index = 0
			self.__set_image()
	
	def go_last(self):
		if self.Filenames.Count > 0:
			self.Index = self.Filenames.Count - 1
			self.__set_image()

	def open_folder(self):
		dlg = FolderBrowserDialog()
		if dlg.ShowDialog() == DialogResult.OK:
			folder = self.Folder = dlg.SelectedPath
			print "folder=", folder
			self.save_config()
			self.__load_folder(self.Folder)
			self.__set_image()
		else:
			# 화면이 복구 되지 않기 때문에 업데이트 하도록 한다.
			self.__set_changed()
		
	def open_file(self):
		dlg = OpenFileDialog()
		if dlg.ShowDialog() == DialogResult.OK:
			self.__clean_images()
			self.__add_file(dlg.FileName)
			self.__set_image()
		else:
			# 화면이 복구 되지 않기 때문에 업데이트 하도록 한다.
			self.__set_changed()
	
	def rotate_image(self):
		print "rotate: curr=" + self.Drawer.Rotation.ToString()
		if self.Drawer.Rotation == ViewRotationType.Degree0:
			self.Drawer.Rotation = ViewRotationType.Degree90
		elif self.Drawer.Rotation == ViewRotationType.Degree90:
			self.Drawer.Rotation = ViewRotationType.Degree180
		elif self.Drawer.Rotation == ViewRotationType.Degree180:
			self.Drawer.Rotation = ViewRotationType.Degree270
		elif self.Drawer.Rotation == ViewRotationType.Degree270:
			self.Drawer.Rotation = ViewRotationType.Degree0
		print "rotate: new=" + self.Drawer.Rotation.ToString()
		self.__set_changed()

	def change_view_direction(self):
		print "view part: curr=" + self.ViewDirection.ToString()
		if self.ViewDirection == ViewDirectionType.Full:
			self.ViewDirection = ViewDirectionType.LeftToRight
			self.Drawer.ViewPart = ViewPartType.Left
		elif self.ViewDirection == ViewDirectionType.LeftToRight:
			self.ViewDirection = ViewDirectionType.RightToLeft
			self.Drawer.ViewPart = ViewPartType.Right
		elif self.ViewDirection == ViewDirectionType.RightToLeft:
			self.ViewDirection = ViewDirectionType.Full
			self.Drawer.ViewPart = ViewPartType.Full
		print "view part: new=" + self.ViewDirection.ToString()
		self.__set_changed()
			
	def toggle_use_interpolation(self):
		print "interpolation: curr=" + self.Drawer.UseInterpolation.ToString()
		if self.Drawer.UseInterpolation == False:
			self.Drawer.UseInterpolation = True
		else:
			self.Drawer.UseInterpolation = False
		print "interpolation: new=" + self.Drawer.UseInterpolation.ToString()
		self.__set_changed()

if __name__ == '__main__':
	class ImageObserver(Observer):
		def update(self, args):
			print "image changed: args=" + args

	c = ImageNavigator()
	c.add_observer(ImageObserver())
	
	"""
	# config test
	c.load_config()
	c.print_config()
	c.save_config()
	"""
	
	c.open_folder('')
	"""
	for filename in c.Filenames:
		print filename
	"""
	
	c.go_last()
