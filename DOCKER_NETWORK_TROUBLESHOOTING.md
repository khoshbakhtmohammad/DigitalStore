# Docker Network Troubleshooting Guide

## Issue: Cannot reach mcr.microsoft.com from Docker

### Quick Fixes (try in order):

1. **Restart Docker Desktop**
   - Close Docker Desktop completely
   - Restart Docker Desktop
   - Wait for it to fully start
   - Try again: `docker-compose up -d --build`

2. **Check Docker Desktop Network Settings**
   - Open Docker Desktop
   - Go to Settings → Resources → Network
   - Ensure "Use kernel networking for UDP" is enabled
   - Click "Apply & Restart"

3. **Reset Docker Network**
   ```powershell
   docker network prune -f
   docker-compose down
   docker-compose up -d --build
   ```

4. **Check DNS Settings in Docker Desktop**
   - Open Docker Desktop
   - Go to Settings → Docker Engine
   - Add DNS servers if needed:
     ```json
     {
       "dns": ["8.8.8.8", "8.8.4.4"]
     }
     ```
   - Click "Apply & Restart"

5. **Use Alternative Registry (if Microsoft Container Registry is blocked)**
   - Try using Docker Hub mirror or alternative registry
   - Or download images manually first:
     ```powershell
     docker pull mcr.microsoft.com/dotnet/sdk:8.0
     docker pull mcr.microsoft.com/dotnet/aspnet:8.0
     ```

6. **Check Windows Firewall/Proxy**
   - Ensure Docker Desktop is allowed through Windows Firewall
   - If behind a corporate proxy, configure Docker Desktop proxy settings:
     - Settings → Resources → Proxies
     - Enter your proxy settings

7. **Restart Windows Network Stack**
   ```powershell
   # Run as Administrator
   ipconfig /flushdns
   netsh winsock reset
   netsh int ip reset
   # Then restart your computer
   ```

### Alternative: Pre-download Images

If network issues persist, you can pre-download the images:

```powershell
docker pull mcr.microsoft.com/dotnet/sdk:8.0
docker pull mcr.microsoft.com/dotnet/aspnet:8.0
docker pull mcr.microsoft.com/mssql/server:2022-latest
docker pull rabbitmq:3-management-alpine
docker pull redis:7-alpine
docker pull mongo:7
```

Then run: `docker-compose up -d --build`

