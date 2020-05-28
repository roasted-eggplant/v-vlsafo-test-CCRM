using FakeXrmEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;

namespace CCRM.REFFS.Workflows.Test
{

    [TestClass]
    public class GetContactByUserTest
    {
        XrmFakedContext fakedContext = new XrmFakedContext();
        public Entity User = new Entity("systemuser", Guid.NewGuid());
        public Entity Contact = new Entity("contact", Guid.NewGuid());

        [TestInitialize]
        public void TestInitialize()
        {
            User["internalemailaddress"] = "mark@microsoft.com";
            Contact["emailaddress1"] = "mark@microsoft.com";
            fakedContext.CallerId = User.ToEntityReference();
            fakedContext.Initialize(new List<Entity>()
            {
                User, Contact
            });
        }
        [TestMethod]
        public void When_Passing_WrongUser_NullContactReturned()
        {
            var userId = new EntityReference
            {
                Id = new Guid("A97F07A5-0004-419D-A930-B2828EE2FEAD"),
                Name = "Steave",
                LogicalName = "systemuser"
            };

 
            var inputs = new Dictionary<string, object>() {
                { "CurrentUserId", userId }
            };
            var outputParameter = fakedContext.ExecuteCodeActivity<GetContactByUser>(inputs);
            var relatedContact = outputParameter["ContactRelatedToUser"] as EntityReference;
            Assert.IsNull(relatedContact);
        }

        [TestMethod]
        public void When_Passing_CorrectUser_ValidContactReturned()
        {
            var inputs = new Dictionary<string, object>() {
                { "CurrentUserId", User.ToEntityReference() }
            };
            var outputParameter = fakedContext.ExecuteCodeActivity<GetContactByUser>(inputs);
            var relatedContact = outputParameter["ContactRelatedToUser"] as EntityReference;
            Assert.AreEqual(relatedContact.Id, Contact.Id);
        }

        [TestMethod]
        public void When_Passing_UserWithMisMatchEmailAddress_NullContactReturned()
        {
            XrmFakedContext fakedContextLocal = new XrmFakedContext();
            var user = new Entity("systemuser");
            user.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
            user["internalemailaddress"] = "markmismatched@microsoft.com";

            var contact = new Entity("contact");
            contact.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
            contact["emailaddress1"] = "mark@microsoft.com";

            fakedContextLocal.Initialize(new List<Entity>()
            {
                user, contact
            });
            var inputs = new Dictionary<string, object>() {
                { "CurrentUserId", user.ToEntityReference() }
            };
            var outputParameter = fakedContextLocal.ExecuteCodeActivity<GetContactByUser>(inputs);
            var relatedContact = outputParameter["ContactRelatedToUser"] as EntityReference;
            Assert.IsNull(relatedContact);
        }       
    }
}
