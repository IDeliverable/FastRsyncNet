# FastRsyncNet - C# delta syncing library

The Fast Rsync .NET library is Rsync implementation derived from [Octodiff](https://github.com/OctopusDeploy/Octodiff) tool.

Unlike the Octodiff which is based on SHA1 algorithm, the FastRsyncNet uses xxHash64 as a default algorithm.
Usage of xxHash64 allows for significant faster calculations and smaller signature size.

FastRsyncNet 1.x.x supports also SHA1 and is 100% compatible with Octodiff.

## Install [![NuGet](https://img.shields.io/nuget/v/FastRsyncNet.svg?style=flat)](https://www.nuget.org/packages/FastRsyncNet/)
Add To project via NuGet:  
1. Right click on a project and click 'Manage NuGet Packages'.  
2. Search for 'FastRsyncNet' and click 'Install'.  

## Examples

### Calculating signature

```csharp
using FastRsync.Signature;

...

var signatureBuilder = new SignatureBuilder();
using (var basisStream = new FileStream(basisFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
using (var signatureStream = new FileStream(signatureFilePath, FileMode.Create, FileAccess.Write, FileShare.Read))
{
    signatureBuilder.Build(basisStream, new SignatureWriter(signatureStream));
}
```

### Calculating delta

```csharp
using FastRsync.Delta;

...

var delta = new DeltaBuilder();
builder.ProgressReport = new ConsoleProgressReporter();
using (var newFileStream = new FileStream(newFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
using (var signatureStream = new FileStream(signatureFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
using (var deltaStream = new FileStream(deltaFilePath, FileMode.Create, FileAccess.Write, FileShare.Read))
{
    delta.BuildDelta(newFileStream, new SignatureReader(signatureStream, delta.ProgressReporter), new AggregateCopyOperationsDecorator(new BinaryDeltaWriter(deltaStream)));
}
```

### Patching (applying delta)

```csharp
using FastRsync.Delta;

...

var delta = new DeltaApplier
        {
            SkipHashCheck = true
        };
using (var basisStream = new FileStream(basisFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
using (var deltaStream = new FileStream(deltaFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
using (var newFileStream = new FileStream(newFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
{
    delta.Apply(basisStream, new BinaryDeltaReader(deltaStream, progressReporter), newFileStream);
}
```
### Calculating signature on Azure blobs

FastRsyncNet might not work on Azure Storage emulator due to issues with stream seeking.

```csharp
using FastRsync.Signature;

...

var storageAccount = CloudStorageAccount.Parse("azure storage connectionstring");
var blobClient = storageAccount.CreateCloudBlobClient();
var blobsContainer = blobClient.GetContainerReference("containerName");
var basisBlob = blobsContainer.GetBlockBlobReference("blobName");

var signatureBlob = container.GetBlockBlobReference("blob_signature");

var signatureBuilder = new SignatureBuilder();
using (var signatureStream = await signatureBlob.OpenWriteAsync())
using (var basisStream = await basisBlob.OpenReadAsync())
{
    await signatureBuilder.BuildAsync(basisStream, new SignatureWriter(signatureStream));
}
```
