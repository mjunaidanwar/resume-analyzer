const { app, BrowserWindow } = require('electron');
const path = require('path');
const { spawn } = require('child_process');

let mainWindow;
let backendProcess;

function createWindow() {
  mainWindow = new BrowserWindow({
    width: 1000,
    height: 800,
    webPreferences: {
      contextIsolation: true,
      nodeIntegration: false
    }
  });

  mainWindow.loadURL('http://localhost:5000');
}

function startBackend() {
  const isMac = process.platform === 'darwin';
  const backendPath = isMac
    ? path.join(app.getAppPath(), 'publish', 'api', 'ResumeAnalyzerAPI')
    : path.join(app.getAppPath(), 'publish', 'api', 'ResumeAnalyzerAPI.exe');

  backendProcess = spawn(backendPath, [], {
    cwd: path.join(app.getAppPath(), 'publish', 'api'),
    shell: true,
    env: {
      ...process.env,
      ASPNETCORE_ENVIRONMENT: 'Development'
    }
  });

  backendProcess.stdout.on('data', data => console.log(`🟢 Backend: ${data}`));
  backendProcess.stderr.on('data', data => console.error(`🔴 Backend error: ${data}`));
}

app.whenReady().then(() => {
  startBackend();

  setTimeout(() => {
    createWindow();
  }, 2000); // wait for backend to start before opening window
});

app.on('window-all-closed', () => {
  if (backendProcess) backendProcess.kill();
  if (process.platform !== 'darwin') app.quit();
});

app.on('activate', () => {
  if (BrowserWindow.getAllWindows().length === 0) createWindow();
});
