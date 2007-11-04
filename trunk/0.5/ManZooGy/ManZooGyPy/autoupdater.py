import clr

clr.AddReference('System.Windows.Forms')

clr.AddReference('RBPGUI.AutoUpdater')

from System import *
from System.Net import *
from System.IO import *
from System.Diagnostics import *
from System.Windows.Forms import *

from RBPGUI.AutoUpdater import *

MSG_NEW_VERSION_HERE = '{0} {1} is available. Do you want to upgrade?'
LABEL_QUESTION = 'Question'

MSG_CONNECTING = 'Connecting...{0}'
MSG_DOWNLOADED = '{0}...{1}%'
MSG_COMPLETED = 'Completed.'
MSG_CANCELED = 'Canceled.'

class AutoUpdater:
	updateUrl = ''
	downPath = ''
	appName = ''
	appVersion = ''
	
	def __init__(self, appName, appVersion, updateUrl, downPath = 'C:\\Temp'):
		self.updateUrl = updateUrl
		self.downPath = downPath
		self.appName = appName
		self.appVersion = appVersion
		
	def has_new_version(self):
		wc = WebClient()
		try:
			downStr = wc.DownloadString(self.updateUrl + "/autoupdate.txt")
			destVerStr = downStr.Split('|')[0]
			print "dest version=" + destVerStr
			destVer = Version(destVerStr)

			srcVerStr = self.appVersion
			print "src version=" + srcVerStr
			srcVer = Version(srcVerStr)

            # dest >  src: 1
            # dest == src: 0
            # dest <  src: -1
			if destVer.CompareTo(srcVer) > 0:
				if MessageBox.Show(str.Format(MSG_NEW_VERSION_HERE, self.appName, destVerStr), LABEL_QUESTION, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes:
					setupFile = downStr.Split('|')[1]

					Directory.CreateDirectory(self.downPath)
					dlg = FormDownload(self.updateUrl + "/" + setupFile, self.downPath + "\\" + setupFile);
					if dlg.ShowDialog() == DialogResult.OK:
						process = Process()
						process.StartInfo.FileName = self.downPath + "\\" + setupFile
						process.Start()
						return True
		except WebException, e:
			print e
			
		return False

class FormDownload(FormDownloadBase):
	wc = None
	restSec = 30
	url = ''
	file = ''
	
	def __init__(self, url, file):
		self.url = url
		self.file = file
	
		c = self.btCancel
		c.Click += self.__btCancel_Click
		
		c = self.wc = WebClient()
		c.DownloadFileCompleted += self.__wc_DownloadFileAsyncCompleted
		c.DownloadProgressChanged += self.__wc_DownloadProgressChanged
		c.DownloadFileAsync(Uri(self.url), self.file)

		self.lblMessage.Text = str.Format(MSG_CONNECTING, self.restSec.ToString())
		print "downloading..." + self.url
		
		c = self.tmSkip
		c.Tick += self.__tmSkip_Tick
		c.Interval = 1000
		c.Enabled = True
			
		print "downloader initialized."

	def __wc_DownloadFileAsyncCompleted(self, sender, event):
		self.tmSkip.Enabled = False
		self.lblMessage.Text = MSG_COMPLETED
		print "downloaded..." + self.url

		if event.Error == None:
			self.DialogResult = DialogResult.OK
		else:
			self.DialogResult = DialogResult.Cancel
		self.wc.Dispose()
		self.Close()

	def __wc_DownloadProgressChanged(self, sender, event):
		self.tmSkip.Enabled = False
		if event.ProgressPercentage >= 0 and event.ProgressPercentage <= 100:
			msg = str.Format(MSG_DOWNLOADED, Path.GetFileName(self.file), event.ProgressPercentage.ToString())
			self.lblMessage.Text = msg
			print msg

			self.pb.Value = event.ProgressPercentage

	def __tmSkip_Tick(self, sender, event):
		self.restSec = self.restSec - 1
		self.lblMessage.Text = str.Format(MSG_CONNECTING, self.restSec)
		if self.restSec == 0:
			__cancel()
	
	def __btCancel_Click(self, sender, event):
		self.__cancel()
	
	def __cancel(self):
		tm = self.tmSkip
		tm.Enabled = False;
		self.lblMessage.Text = MSG_CANCELED
		print "downloading canceled."
		self.wc.CancelAsync()
		self.DialogResult = DialogResult.Cancel
		self.Close()
		
if __name__ == '__main__':
	c = AutoUpdater('MyAPP', '0.1.2.61', 'http://localhost/')
	if c.has_new_version() == True:
		print "updated"
	else:
		print "no new version"
