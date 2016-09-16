The fast Rsync .NET library based on [Octodiff](https://github.com/OctopusDeploy/Octodiff) Rsync implementation.
Unlike the Octodiff which is based on Adler32 and SHA1 algorithms, the FastRsyncNet uses Adler32 and xxHash64 as a default algorithms.
The SHA1 is also supported so FastRsyncNet is 100% compatible with Octodiff.
