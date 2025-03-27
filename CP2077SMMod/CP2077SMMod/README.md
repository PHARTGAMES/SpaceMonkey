# RED4ext.Example using Visual Studio

An example of a RED4ext plugin using Visual Studio.

## Build instructions

### Steps

1. Download and install [Visual Studio 2022 Community Edition](https://www.visualstudio.com/) or a higher version.
2. Clone this repository.
3. Clone the dependencies (`git submodule update --init --recursive`).
4. Open the solution (**RED4ext.Example.VisualStudio.sln**).
5. Build the project (the artifacts are located in `build/{debug|release}` directory).

**Notes**:

* The plugin has to be a 64-bit library. That means all required libraries have to be compiled in 64-bit and the compiler has to support 64-bit.
* Make sure you have the latest SDK by updating it using the following commands:
  * `cd deps/red4ext.sdk`
  * `git pull` / `git fetch`
  * `git checkout master`
* You can also generate the projects from command line, see the **build.yml** in **.github/workflows/build.yml**.
