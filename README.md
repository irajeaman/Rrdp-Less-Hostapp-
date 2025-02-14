# **Opening RDP with a Less Screen for HostApp using RDPAPP.exe**

This guide provides detailed instructions on using **RDPAPP.exe** to establish a remote desktop session with minimal UI (less screen) for a specific application (`HostApp`).

---

## **1. Command Usage**
```
RDPAPP.exe <IP_Address> <ApplicationName> <Username> <Password> <Module> <SessionID>
```

### **Example:**
```
RDPAPP.exe 10.137.183.187 domain username password  application  application_agr
```

#### **Parameters Explained:**
| Parameter      | Description |
|---------------|-------------|
| `RDPAPP.exe`  | Executable used to open an RDP session with a specific application. |
| `Ip ` | Target remote machine's IP address. |
| `Domain`      | RDP Domain Name . |
| `Username`         | Username for remote authentication. |
| `Password`  | Password for the user account. |
| `Application`         |Application (can vary based on configuration). |
| `Application_Parameters`        | Application Parameters. |

---

## **2. Steps to Use RDPAPP.exe for a Less-Screen RDP Session**
### **Step 1: Prepare the Remote Desktop Application**
Ensure `RDPAPP.exe` is available on your local machine.

### **Step 2: Run the Command**
- Open **Command Prompt** (`cmd.exe`) with administrative privileges.
- Navigate to the folder containing `RDPAPP.exe`:
  ```
  cd C:\Path\To\RDPAPP\
  ```
- Execute the command:
  ```
 RDPAPP.exe 10.137.183.187 domain username password  application  application_agr
  ```

### **Step 3: Establish the RDP Connection**
- The session will start with **minimal UI (less screen)**.
- Only `ipam01` will be launched in the session instead of a full desktop environment.

### **Step 4: Verify Connection**
- Ensure the application is running properly.
- Check session logs if any errors occur.

---

## **3. Connecting via RDP (GUI Method)**
If you want to connect manually:
1. Open **Run** (`Win + R`).
2. Type:
   ```
   mstsc /v:10.137.183.187
   ```
3. Enter credentials:
   - Username: `username`
   - Password: `password`
4. Click **Connect**.

---

## **4. Running RDP with Limited UI (Less-Screen Mode)**
- Use the `/admin` or `/restrictedAdmin` flag if applicable.
- To limit UI components further, use:
  ```
  mstsc /v:10.137.183.187 /restrictedAdmin
  ```

---

## **5. Security Considerations**
- **Avoid storing passwords in scripts**; use a credential manager.
- **Ensure firewall settings allow RDP connections**.
- **Use VPN or SSH tunneling** for added security.

---

## **6. Troubleshooting**
| Issue | Solution |
|-------|----------|
| Unable to connect | Check network connectivity and firewall settings. |
| Incorrect credentials | Verify username and password. |
| Application not launching | Ensure `domain` is installed on the remote machine. |

---

## **7. Automation**
To automate RDP sessions, use a **batch script**:
```batch
@echo off
RDPAPP.exe 10.137.183.187 domain username password  application  application_agr
exit
```
Save this as `rdp_connect.bat` and execute when needed.

---

Let me know if you need further details! 🚀
