# 🔐 Sudo Proxy Shell (.NET 9 AOT)

A minimal Linux utility built in **.NET 9** that launches a shell or specified binary with elevated root privileges. It leverages **Ahead-of-Time (AOT)** compilation to produce a single native executable. The build process is fully containerized for consistency and simplicity.

---

## 🚀 Features

- ✅ **Runs with setuid root** to enable privilege escalation
- 🧊 **.NET 9 AOT native compilation**
- 🐳 **Docker-based build process** for portability
- 📂 **Single native binary output** with no runtime dependency on .NET
- 🔧 Optionally run any binary or script as root

---

## 📦 Build Instructions

Run the following to build and extract the binary:

```bash
./build.sh
```

This will:

- Build the Docker image
- Publish the app with AOT
- Copy the binary to `./out`
- Apply `setuid` root permission if run with `sudo`

---

## 🧪 Usage

To run the binary (default: `/bin/bash`):

```bash
./out/sudo
```

To run a specific binary:

```bash
./out/sudo /usr/bin/id
```

---

## 🛠 Project Structure

- `Program.cs`: Core logic to elevate privileges and spawn the target process
- `build.sh`: Automates build and permission setup
- `Dockerfile`: Containerized build environment using .NET 9 SDK

---

## ⚠️ Notes

- The binary must have root ownership and the `setuid` bit set for privilege escalation to work:

  ```bash
  sudo chown root:root ./out/sudo
  sudo chmod u+s ./out/sudo
  ```

- You should not use this in production or on shared systems without understanding the security implications.

---

## 📝 TODO

- [ ] Add sudoers-like authentication or integrate with system sudoers for secure access control

---

## 📄 License

MIT License
