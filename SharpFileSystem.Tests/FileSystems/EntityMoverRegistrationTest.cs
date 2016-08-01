using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpFileSystem.Collections;
using SharpFileSystem.FileSystems;

namespace SharpFileSystem.Tests.FileSystems
{
    [TestClass]
    public class EntityMoverRegistrationTest
    {
        private TypeCombinationDictionary<IEntityMover> Registration;
        private IEntityMover physicalEntityMover = new PhysicalEntityMover();
        private IEntityMover standardEntityMover = new StandardEntityMover();

        [TestInitialize]
        public void Initialize()
        {
            Registration = new TypeCombinationDictionary<IEntityMover>();
            Registration.AddLast(typeof(PhysicalFileSystem), typeof(PhysicalFileSystem), physicalEntityMover);
            Registration.AddLast(typeof(IFileSystem), typeof(IFileSystem), standardEntityMover);
        }

        [TestMethod]
        public void When_MovingFromPhysicalToGenericFileSystem_Expect_StandardEntityMover()
        {
            Assert.AreEqual(
                Registration.GetSupportedRegistration(typeof(PhysicalFileSystem), typeof(IFileSystem)).Value,
                standardEntityMover
                );
        }

        [TestMethod]
        public void When_MovingFromOtherToPhysicalFileSystem_Expect_StandardEntityMover()
        {
            Assert.AreEqual(
                Registration.GetSupportedRegistration(typeof(IFileSystem), typeof(PhysicalFileSystem)).Value,
                standardEntityMover
                );
        }

        [TestMethod]
        public void When_MovingFromGenericToGenericFileSystem_Expect_StandardEntityMover()
        {
            Assert.AreEqual(
                Registration.GetSupportedRegistration(typeof(IFileSystem), typeof(IFileSystem)).Value,
                standardEntityMover
                );
        }

        [TestMethod]
        public void When_MovingFromPhysicalToPhysicalFileSystem_Expect_PhysicalEntityMover()
        {
            Assert.AreEqual(
                Registration.GetSupportedRegistration(typeof(PhysicalFileSystem), typeof(PhysicalFileSystem)).Value,
                physicalEntityMover
                );
        }
    }
}
