# 🧰 VirtualGrandMA  
### Virtual USB Utility for GrandMA2 onPC

**GMA2Toolkit** is a lightweight Windows application that simplifies the creation and management of **virtual USB drives** for **MA Lighting’s GrandMA** consoles and onPC software.  
It provides a modern, intuitive UI and handles everything from `.img` creation to automatic mounting using the **ImDisk Toolkit** — no command-line work needed.

---

## 🚀 Features

✅ **Create Virtual USB Images**  
Quickly create new `.img` or `.vhd` files in any size (GB). Perfect for simulating USB drives for GrandMA show files.

✅ **Mount & Unmount Instantly**  
Mount your image as a removable USB drive with one click using **ImDisk**. Unmount cleanly when done.

✅ **Automatic FAT32 Preparation**  
Every mounted image is ready for GrandMA — ideal for fixture libraries, show backups, or exports.

✅ **Recent Images List**  
The 10 most recently used `.img` files are automatically tracked and displayed for fast access.

✅ **ImDisk Installer Integration**  
If ImDisk Toolkit isn’t found, GMA2Toolkit will prompt and install it automatically using the included `install.bat` and `files.cab`.

✅ **Integrated Console View**  
Optional console output for debugging or monitoring backend operations.  
The console can be shown or hidden dynamically — closing it won’t quit the main app.

✅ **Modern UI**  
Built with the **ReaLTaiizor** framework for a clean, dark, and modern Windows Forms experience.

---

## 📂 Included Files

| File | Description |
|------|--------------|
| `GMA2Toolkit.exe` | Main executable |
| `install.bat` | Installs ImDisk Toolkit automatically if missing |
| `files.cab` | Required by `install.bat` for ImDisk installation |
| `recent.txt` | Automatically generated list of last used `.img` files (stored in AppData) |

---

## 🧩 Usage

1. **Run `GMA2Toolkit.exe`**
2. Click **Browse** to select or create a `.img` file  
3. Click **Create Image** to make a new one, or **Mount** to attach an existing one  
4. Once mounted, Windows assigns a removable drive letter  
5. Copy your GrandMA show files or libraries there  
6. When done, click **Unmount** safely

💡 The app automatically remembers your last used image and keeps a list of recent ones.

---

## ⚙️ Requirements

- Windows 10 / 11 (x64)
- [.NET Framework 4.8](https://dotnet.microsoft.com/download/dotnet-framework/net48)
- [ImDisk Toolkit](https://sourceforge.net/projects/imdisk-toolkit/) (auto-installs if missing)
- Administrator privileges (required for mounting/unmounting)

---

## 🧠 Technical Details

- Language: **VB.NET (.NET Framework)**
- IDE: **Visual Studio 2022**
- UI Framework: **[ReaLTaiizor](https://github.com/ForeverZer0/ReaLTaiizor)**
- Supports both `.img` and `.vhd` backends (ImDisk + VHD test backend)
- Persists settings via `My.Settings` and stores MRU data in `%AppData%\GMA2Toolkit`

---

## 🧱 Build & Publish

To publish manually:
1. Open the project in **Visual Studio**
2. Ensure `install.bat` and `files.cab` are included with:
   - **Build Action:** `Content`  
   - **Copy to Output Directory:** `Copy always`
3. Right-click the project → **Publish**
4. Choose a **Folder** target (recommended first)
5. Run `GMA2Toolkit.exe` from the published output — all files will be included automatically.

---

## 📜 License

This project is provided **free of charge** for non-commercial and educational purposes.  
All trademarks and software names (e.g., *GrandMA2*, *MA Lighting*) are the property of their respective owners.

---

## 🧑‍💻 Credits

- **Developer:** Marvin Franssen 
- **UI Framework:** [ReaLTaiizor] (https://github.com/ForeverZer0/ReaLTaiizor)  
- **Virtual Disk Backend:** [ImDisk Toolkit] (https://sourceforge.net/projects/imdisk-toolkit/)

---

### 💾 Screenshot
<img width="424" height="570" alt="image" src="https://github.com/user-attachments/assets/038af04a-737c-441c-bcae-61c313f82876" />


