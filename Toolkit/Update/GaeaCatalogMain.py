# coding=UTF-8
import os
import json
import hashlib
from tkinter import *
from tkinter import ttk
import tkinter.messagebox as messagebox
import tkinter.filedialog as filedialog
import shutil

class GaeaCatalogMain():
    row_processLast = 0
    row_lastCatalog = row_processLast + 1
    row_lastVersion = row_lastCatalog + 1
    row_lastCatalogUrl = row_lastVersion + 1
    row_lastcdn1 = row_lastCatalogUrl + 1
    row_lastcdn2 = row_lastcdn1 + 1
    row_platform = row_lastcdn2 + 1
    row_version = row_platform + 1
    row_versionUrl = row_version + 1
    row_catalogUrl = row_versionUrl + 1
    row_cdn1 = row_catalogUrl + 1
    row_cdn2 = row_cdn1 + 1
    row_serverlist = row_cdn2 + 1
    row_announcement = row_serverlist + 1
    row_localRootPath = row_announcement + 1
    row_svnWorkPath = row_localRootPath + 1
    row_generate = row_svnWorkPath + 1
    row_svncommit = row_svnWorkPath + 1
    row_showlog = 0
    row_showlogButton = row_svnWorkPath + 1

    def __init__(self):
        self.window = Tk()
        self.window.title('Gaea Catalog Generator')
        self.window.geometry('1500x600')

        self.lastCatalogData = None
        self.lastVersionData = None
        self.differences = None
        self.refreshLastWidgets()

    def refreshLastWidgets(self):
        self.var_processLast = None
        self.btn_processLast = Button(self.window, text='Process Last Catalog', command=lambda : self.onClickLast(self.var_processLast))
        self.btn_processLast.grid(row=self.row_processLast, column=0, columnspan=2, sticky=W+E+N+S)

        Label(self.window, text='last Catalog').grid(row=self.row_lastCatalog, sticky=E, pady=5)
        self.var_lastCatalog = StringVar()
        self.txt_lastCatalog = Entry(self.window, width=60, state='readonly', borderwidth=0, textvariable=self.var_lastCatalog)
        self.txt_lastCatalog.grid(row=self.row_lastCatalog, column=1)

        Label(self.window, text='last Version').grid(row=self.row_lastVersion, sticky=E, pady=5)
        self.var_lastVersion = StringVar()
        self.txt_lastVersion = Entry(self.window, width=60, state='readonly', borderwidth=0, textvariable=self.var_lastVersion)
        self.txt_lastVersion.grid(row=self.row_lastVersion, column=1)
        
        Label(self.window, text='last Catalog Url').grid(row=self.row_lastCatalogUrl, sticky=E, pady=5)
        self.var_lastCatalogUrl = StringVar()
        self.txt_lastCatalogUrl = Entry(self.window, width=60, state='readonly', borderwidth=0, textvariable=self.var_lastCatalogUrl)
        self.txt_lastCatalogUrl.grid(row=self.row_lastCatalogUrl, column=1)

        Label(self.window, text='last cdn1').grid(row=self.row_lastcdn1, sticky=E, pady=5)
        self.var_lastcdn1 = StringVar()
        self.txt_lastcdn1 = Entry(self.window, width=60, state='readonly', borderwidth=0, textvariable=self.var_lastcdn1)
        self.txt_lastcdn1.grid(row=self.row_lastcdn1, column=1)

        Label(self.window, text='last cdn2').grid(row=self.row_lastcdn2, sticky=E, pady=5)
        self.var_lastcdn2 = StringVar()
        self.txt_lastcdn2 = Entry(self.window, width=60, state='readonly', borderwidth=0, textvariable=self.var_lastcdn2)
        self.txt_lastcdn2.grid(row=self.row_lastcdn2, column=1)

        Label(self.window, text='platform').grid(row=self.row_platform, sticky=E, pady=5)
        self.var_platform = StringVar()
        self.txt_platform = Entry(self.window, width=60, textvariable=self.var_platform)
        self.txt_platform.grid(row=self.row_platform, column=1, sticky=W)

        Label(self.window, text='version').grid(row=self.row_version, sticky=E, pady=5)
        self.var_version = StringVar()
        self.txt_version = Entry(self.window, width=60, textvariable=self.var_version)
        self.txt_version.grid(row=self.row_version, column=1, sticky=W)

        Label(self.window, text='versionUrl').grid(row=self.row_versionUrl, sticky=E, pady=5)
        self.var_versionUrl = StringVar()
        self.txt_versionUrl = Entry(self.window, width=60, textvariable=self.var_versionUrl)
        self.txt_versionUrl.grid(row=self.row_versionUrl, column=1, sticky=W)
        
        Label(self.window, text='catalogUrl').grid(row=self.row_catalogUrl, sticky=E, pady=5)
        self.var_catalogUrl = StringVar()
        self.txt_catalogUrl = Entry(self.window, width=60, textvariable=self.var_catalogUrl)
        self.txt_catalogUrl.grid(row=self.row_catalogUrl, column=1, sticky=W)

        Label(self.window, text='cdn1').grid(row=self.row_cdn1, sticky=E, pady=5)
        self.var_cdn1 = StringVar()
        self.txt_cdn1 = Entry(self.window, width=60, textvariable=self.var_cdn1)
        self.txt_cdn1.grid(row=self.row_cdn1, column=1, sticky=W)

        Label(self.window, text='cdn2').grid(row=self.row_cdn2, sticky=E, pady=5)
        self.var_cdn2 = StringVar()
        self.txt_cdn2 = Entry(self.window, width=60, textvariable=self.var_cdn2)
        self.txt_cdn2.grid(row=self.row_cdn2, column=1, sticky=W)

        Label(self.window, text='serverlist').grid(row=self.row_serverlist, sticky=E, pady=5)
        self.var_serverlist = StringVar()
        self.txt_serverlist = Entry(self.window, width=60, textvariable=self.var_serverlist)
        self.txt_serverlist.grid(row=self.row_serverlist, column=1, sticky=W)

        Label(self.window, text='announcement').grid(row=self.row_announcement, sticky=E, pady=5)
        self.var_announcement = StringVar()
        self.txt_announcement = Entry(self.window, width=60, textvariable=self.var_announcement)
        self.txt_announcement.grid(row=self.row_announcement, column=1, sticky=W)

        Label(self.window, text='local Root Path').grid(row=self.row_localRootPath, sticky=E, pady=5)
        self.var_localRootPath = StringVar()
        self.txt_localRootPath = Entry(self.window, width=60, state='readonly', borderwidth=0, textvariable=self.var_localRootPath)
        self.txt_localRootPath.grid(row=self.row_localRootPath, column=1)
        self.btn_localRootPath = Button(self.window, text='Open', command=lambda : self.onClickLocal(self.var_localRootPath))
        self.btn_localRootPath.grid(row=self.row_localRootPath, column=2)

        Label(self.window, text='svn Work Path').grid(row=self.row_svnWorkPath, sticky=E, pady=5)
        self.var_svnWorkPath = StringVar()
        self.txt_svnWorkPath = Entry(self.window, width=60, state='readonly', borderwidth=0, textvariable=self.var_svnWorkPath)
        self.txt_svnWorkPath.grid(row=self.row_svnWorkPath, column=1)
        self.btn_svnWorkPath = Button(self.window, text='Open', command=lambda : self.onClickSvn(self.var_svnWorkPath))
        self.btn_svnWorkPath.grid(row=self.row_svnWorkPath, column=2)

        self.btn_generate = Button(self.window, text='Generate Catalog & Version', command=self.onClickGenerate)
        self.btn_generate.grid(row=self.row_generate, column=0)
        self.btn_svnCommit = Button(self.window, text='SVN commit', command=self.onClickCommit)
        self.btn_svnCommit.grid(row=self.row_generate, column=1)

        self.frame_showlog = Frame(self.window)
        self.frame_showlog.grid(row=self.row_showlog, column=3, rowspan=13, sticky=W+E+S+N)
        self.txt_showlog = Text(self.frame_showlog, background='gray')
        self.scl_showlog = Scrollbar(self.frame_showlog)
        self.txt_showlog.config(yscrollcommand=self.scl_showlog.set)
        self.scl_showlog.config(command=self.txt_showlog.yview)
        self.txt_showlog.pack(side=LEFT, fill=Y)
        self.scl_showlog.pack(side=LEFT, fill=Y)
        
        self.btn_showlogCopy = Button(self.window, text='Copy log', command=self.onClickCopy)
        self.btn_showlogCopy.grid(row=self.row_showlogButton, column=3, sticky=W)
        self.btn_showlogClear = Button(self.window, text='Clear log', command=self.onClickClear)
        self.btn_showlogClear.grid(row=self.row_showlogButton, column=3, sticky=E)

        self.var_copyProgress = DoubleVar()
        self.bar_copyProgress = ttk.Progressbar(self.window, mode='determinate', orient=HORIZONTAL, variable=self.var_copyProgress)
        # self.bar_copyProgress.grid(row=15, columnspan=4, sticky=W+E)
        # self.btn_start
        '''
        self.nameInput = tkinter.Entry(self)
        self.nameInput.pack()
        self.alertButton = tkinter.Button(self, text='Hello', command=self.onClickLast)
        self.alertButton.pack()
        '''

    def onClickLast(self, initialpath=None):
        answer = filedialog.askopenfilename(initialdir=initialpath or os.getcwd(), title="Select a catalog file", filetypes=[('catalog file', '.catalog')])
        if answer:
            self.var_lastCatalog.set(answer)
            try:
                with open(answer, 'r') as lastCatalog:
                    self.lastCatalogData = json.load(lastCatalog)
                    self.var_lastVersion.set(self.lastCatalogData['version'])
                    self.var_platform.set(self.lastCatalogData['platform'])
                   
                with open(os.path.join(os.path.dirname(answer), 'version'), 'r') as lastVersion:
                    self.lastVersionData = json.load(lastVersion)
                    self.var_lastCatalogUrl.set(self.lastVersionData['catalogurl'])
                    self.var_lastcdn1.set(self.lastVersionData['cdn1'])
                    self.var_lastcdn2.set(self.lastVersionData['cdn2'])
                    self.var_serverlist.set(self.lastVersionData['serverlist'])
                    self.var_announcement.set(self.lastVersionData['announcement'])
                    self.var_versionUrl.set(self.lastVersionData['versionurl'])
            except Exception as e:
                self.debugLog(e)
            finally:
                if self.differences:
                    self.differences.clear()

    def onClickLocal(self, initialpath=None):
        answer = filedialog.askdirectory(initialdir=initialpath.get() or os.getcwd(), title='Select local resource directory')
        if answer:
            self.var_localRootPath.set(answer)

    def onClickSvn(self, initialpath=None):
        answer = filedialog.askdirectory(initialdir=initialpath.get() or os.getcwd())
        if answer:
            self.var_svnWorkPath.set(answer)

    def onClickGenerate(self):
        if not self.checkForGenerate():
            return
        # todo
        passdataCatalog = {
            'version': self.var_version.get(),
            'platform': self.var_platform.get(),
            'assets': {},
        }
        passdataVersion = {
            'version': self.var_version.get(),
            'versionurl': self.var_versionUrl.get(),
            'catalogurl': self.var_catalogUrl.get(),
            'cdn1': self.var_cdn1.get(),
            'cdn2': self.var_cdn2.get(),
            'serverlist': self.var_serverlist.get(),
            'announcement': self.var_announcement.get(),
        }
        self.differences = []
        for root, dirs, files in os.walk(self.var_localRootPath.get()):
            for file in files:
                if not self.isAsset(file):
                    continue
                key = os.path.relpath(os.path.join(root, file), self.var_localRootPath.get()).replace('\\', '/')
                md5str = self.getMD5(os.path.join(root, file))
                passdataCatalog['assets'][key] = {}
                if self.lastCatalogData and (key in self.lastCatalogData['assets']) and self.lastCatalogData['assets'][key]['md5'] == md5str:
                    passdataCatalog['assets'][key] = {
                        'md5': md5str,
                        'url1': self.lastCatalogData['assets'][key]['url1'],
                        'url2': self.lastCatalogData['assets'][key]['url2'],
                        'size': self.lastCatalogData['assets'][key]['size'],
                    }
                else:
                    passdataCatalog['assets'][key] = {
                        'md5': md5str,
                        'url1': os.path.join(self.var_cdn1.get(), key).replace('\\', '/'),
                        'url2': os.path.join(self.var_cdn2.get(), key).replace('\\', '/'),
                        'size': os.path.getsize(os.path.join(root, file)),
                    }
                    # 记录差异文件
                    self.differences.append(key)
        targetdir = os.path.abspath(os.path.join(self.var_localRootPath.get(), os.path.pardir, '{0}_{1}'.format(self.var_platform.get(), self.var_version.get())))
        try:
            os.mkdir(targetdir)
        except Exception as e:
            self.debugLog(e)
        catalogFileName = '{0}_{1}.catalog'.format(self.var_platform.get(), self.var_version.get())
        versionFileName = 'version'
        # 写入catalog文件
        with open(os.path.join(targetdir, catalogFileName), 'w') as f:
            json.dump(passdataCatalog, f)
        # 写入version文件
        with open(os.path.join(targetdir, versionFileName), 'w') as f:
            json.dump(passdataVersion, f)
        self.bar_copyProgress.grid(row=15, columnspan=4, sticky=W+E)
        copyCount = 0
        for src, dst in map(lambda rel: (os.path.join(self.var_localRootPath.get(), rel), os.path.join(targetdir, rel)), self.differences):
            try:
                if not os.path.isdir(os.path.dirname(dst)):
                    os.makedirs(os.path.dirname(dst))
                shutil.copyfile(src, dst)
                self.debugLog('copy from {0} to {1}'.format(src, dst))
                copyCount = copyCount + 1
                self.var_copyProgress.set(copyCount / len(self.differences) * 100)
            except Exception as e:
                self.debugLog(e)
                self.bar_copyProgress.grid_remove()
                return
        self.bar_copyProgress.grid_remove()
        messagebox.showinfo('work done', 'asset preparing done')

    def onClickCommit(self):
        messagebox.showinfo('commit on svn', 'Coming soon')

    def onClickCopy(self):
        self.window.clipboard_clear()
        self.window.clipboard_append(self.txt_showlog.get(1.0, END))
        messagebox.showinfo('Copy log', 'Log has been copied on clipboard')

    def onClickClear(self):
        self.txt_showlog.delete(1.0, END)

    def checkPlatform(self):
        if self.var_platform.get():
            if self.lastCatalogData:
                if self.lastCatalogData['platform'] == self.var_platform.get():
                    return True
                else:
                    messagebox.showerror('platform error', 'platform can not be changed')
                    return False
            else:
                return True
        else:
            messagebox.showerror('platform error', 'must input platform')
            return False

    def checkVersion(self):
        if self.var_version.get():
            vers = self.parseVersion(self.var_version.get())
            if not vers['valid']:
                messagebox.showerror('version error', 'version invalid')
                return False
            lvers = {'valid': True, 'data': [-1, -1, -1]}
            if self.lastCatalogData:
                lvers = self.parseVersion(self.lastCatalogData['version'])
                if not lvers['valid']:
                    messagebox.showerror('version error', 'last version invalid')
                    return False
            for idx in range(3):
                if vers['data'][idx] == lvers['data'][idx]:
                    continue
                else:
                    if vers['data'][idx] < lvers['data'][idx]:
                        messagebox.showerror('version error', 'last version large than current')
                    return vers['data'][idx] > lvers['data'][idx]
            messagebox.showerror('version error', 'last version large than current')
            return False
        else:
            messagebox.showerror('version error', 'must input version')
            return False

    def checkVersionUrl(self):
        if self.var_versionUrl.get():
            return True
        else:
            messagebox.showerror('version url error', 'must input version url')
            return False
    
    def checkCatalogUrl(self):
        if self.var_catalogUrl.get():
            return True
        else:
            messagebox.showerror('catalog url error', 'must input catalog url')
            return False

    def checkcdn(self, index):
        cdnstr = ''
        if index == 1:
            cdnstr = self.var_cdn1.get()
        else:
            cdnstr = self.var_cdn2.get()
        if cdnstr:
            if cdnstr.endswith('/'):
                messagebox.showerror('cdn error', 'input cdn must not end with slash')
                return False
            else:
                return True
        else:
            messagebox.showerror('cdn error', 'must input cdn')
            return False

    def checkServerlist(self):
        if self.var_serverlist.get():
            return True
        else:
            messagebox.showerror('serverlist error', 'must input serverlist')
            return False

    def checkAnnouncement(self):
        if self.var_announcement.get():
            return True
        else:
            messagebox.showerror('announcement error', 'must input announcement')
            return False

    def checkLocalRoot(self):
        if self.var_localRootPath.get():
            if not os.path.isdir(self.var_localRootPath.get()):
                messagebox.showerror('local root error', 'input local root path is not a directory')
                return False
            else:
                return True
        else:
            messagebox.showerror('local root error', 'must input local root')
            return False
    
    def parseVersion(self, versionStr):
        ret = []
        for verStr in versionStr.split('.'):
            try:
                temp = int(verStr)
                ret.append(temp)
            except:
                return {'valid':False, 'data':ret}
        if len(ret) != 3:
            return {'valid':False, 'data':ret}
        else:
            return {'valid':True, 'data':ret}

    def checkForGenerate(self):
        checklist = [self.checkPlatform, self.checkVersion, self.checkVersionUrl, self.checkCatalogUrl, lambda: self.checkcdn(1), lambda: self.checkcdn(2), self.checkServerlist, self.checkAnnouncement, self.checkLocalRoot]
        for result in map(lambda handler: handler(), checklist):
            if not result:
                return False
        return True

    def getMD5(self, fileFullname):
        hash_md5 = hashlib.md5()
        with open(fileFullname, 'rb') as f:
            for chunk in iter(lambda: f.read(4096), b''):
                hash_md5.update(chunk)
        return hash_md5.hexdigest()

    def isAsset(self, filename):
        return not filename.endswith('.meta') and not filename.startswith('.') and not filename.endswith('.svn')

    def debugLog(self, content):
        if self.txt_showlog:
            self.txt_showlog.insert(END, str(content))
            self.txt_showlog.insert(END, '\n')


app = GaeaCatalogMain()
app.window.mainloop()