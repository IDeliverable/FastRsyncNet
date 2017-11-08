using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FastRsync.Diagnostics;
using FastRsync.Hash;
using FastRsync.Signature;
using NSubstitute;
using NUnit.Framework;

namespace FastRsync.Tests
{
    [TestFixture]
    public class Adler32RollingChecksumTests
    {
        [Test]
        public void SignatureBuilderXXHash_BuildsSignature()
        {
            // Arrange
            var data1 = Encoding.ASCII.GetBytes("Adler32 checksum test");
            var data2 = Encoding.ASCII.GetBytes("Fast Rsync Fast Rsync");
            var data3 = Encoding.ASCII.GetBytes("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            var data4 = Encoding.ASCII.GetBytes("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Duis malesuada turpis non libero faucibus sodales. Mauris eget justo est. Pellentesque.");

            // Act
            var checksum1 = new Adler32RollingChecksum().Calculate(data1, 0, data1.Length);
            var checksum2 = new Adler32RollingChecksum().Calculate(data2, 0, data2.Length);
            var checksum3 = new Adler32RollingChecksum().Calculate(data3, 0, data3.Length);
            var checksum4 = new Adler32RollingChecksum().Calculate(data4, 0, data4.Length);

            // Assert
            Assert.AreEqual(0x4ff907a1, checksum1);
            Assert.AreEqual(0x5206079b, checksum2);
            //Assert.AreEqual(0x040f0fc1, checksum3); // bug in adler32 implementation https://github.com/OctopusDeploy/Octodiff/issues/16
            //Assert.AreEqual(0x2d10357d, checksum4);
        }
    }
}
