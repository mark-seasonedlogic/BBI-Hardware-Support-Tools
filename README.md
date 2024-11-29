
# BBIHardwareSupportTools

## Overview
**BBIHardwareSupportTools** is a Visual Studio solution for managing and supporting hardware tools. The solution consists of a main project, `BBIHardwareSupportTools`, which depends on a submodule project, `AirWatchMDMClient`. 

This guide explains how to clone and work with the repository, ensuring the submodule code remains read-only for developers working on the main project.

---

## Prerequisites

Before cloning the repository, ensure you have the following installed:

- [Git](https://git-scm.com/)
- [Visual Studio](https://visualstudio.microsoft.com/) (with the required workloads for .NET projects)

---

## Cloning the Repository

Follow these steps to clone the repository, including its submodule:

1. Open a terminal or Git Bash.

2. Clone the main repository:
   ```bash
   git clone https://github.com/yourusername/BBIHardwareSupportTools.git
   ```

3. Navigate to the cloned repository:
   ```bash
   cd BBIHardwareSupportTools
   ```

4. Initialize and update the submodule:
   ```bash
   git submodule update --init --recursive
   ```
   This will download the `AirWatchMDMClient` submodule into the `Dependencies/AirWatchMDMClient` folder.

5. Make the submodule read-only to prevent accidental changes:
   - **Windows**:
     ```cmd
     attrib +r /s /d Dependencies\AirWatchMDMClient
     ```
   - **Linux/macOS**:
     ```bash
     chmod -R -w Dependencies/AirWatchMDMClient
     ```

---

## Working on the Main Project

You can now open the solution in Visual Studio and work on the main project:

1. Open the `BBIHardwareSupportTools.sln` file in Visual Studio.
2. Make your changes to the main project (`BBIHardwareSupportTools`).
3. Build and test your changes using Visual Studio's build tools.

---

## Important Notes About the Submodule

- The submodule code (`AirWatchMDMClient`) is read-only and should not be modified directly.
- If you need to make changes to the submodule, please work in a separate repository for `AirWatchMDMClient`.
- Any updates to the submodule in the main repository must be handled by authorized developers with write access to the submodule repository.

---

## Committing and Pushing Changes

1. **Stage your changes**:
   ```bash
   git add .
   ```

2. **Commit your changes**:
   ```bash
   git commit -m "Your commit message"
   ```

3. **Push your changes to the remote repository**:
   ```bash
   git push
   ```

*Note:* Ensure no changes have been made to the submodule before pushing. If changes to the submodule are detected, reset them:
   ```bash
   git submodule update --checkout
   ```

---

## Additional Resources

- [Git Documentation](https://git-scm.com/doc)
- [Visual Studio Documentation](https://learn.microsoft.com/en-us/visualstudio/)

If you have any questions or encounter issues, please contact the repository maintainer.
