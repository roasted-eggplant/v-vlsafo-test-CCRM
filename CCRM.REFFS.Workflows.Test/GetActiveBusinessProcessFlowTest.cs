using Microsoft.VisualStudio.TestTools.UnitTesting;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace CCRM.REFFS.Workflows.Test
{
    public enum StatusCode
    {
        Active = 1,
        Finished = 2,
        Aborted = 3
    }
    [TestClass]
    public class GetActiveBusinessProcessFlowTest
    {
        [TestMethod]
        public void When_Passing_CorrectIotAlertId_Should_Return_BusinessProcessFlowRecord()
        {
            XrmFakedContext fakedContext = new XrmFakedContext();
            Entity iotAlert = new Entity("msdyn_iotalert", Guid.NewGuid());
            Entity businessProcessFlow = new Entity("msdyn_bpf_477c16f59170487b8b4dc895c5dcd09b", Guid.NewGuid());
            businessProcessFlow["bpf_msdyn_iotalertid"] = iotAlert.ToEntityReference();
            businessProcessFlow["statuscode"] = new OptionSetValue((int)StatusCode.Active);
            fakedContext.Initialize(new List<Entity>()
                {
                    iotAlert, businessProcessFlow
                });
            var inputs = new Dictionary<string, object>() {
                { "IotAlertId", iotAlert.ToEntityReference() }
            };
            var outputParameter = fakedContext.ExecuteCodeActivity<GetActiveBPFForIotAlert>(inputs);
            var activeBusinessProcessFlowId = outputParameter["ActiveBusinessProcessFlowId"] as EntityReference;
            Assert.AreEqual(activeBusinessProcessFlowId.Id, businessProcessFlow.ToEntityReference().Id);
        }
        [TestMethod]
        public void When_No_BusinessProcessFlow_Exist_RelatedTo_IotAlertRecord_Should_Return_Null_BusinessProcessFlow()
        {
            XrmFakedContext fakedContext = new XrmFakedContext();
            Entity iotAlert = new Entity("msdyn_iotalert", Guid.NewGuid());
            fakedContext.Initialize(new List<Entity>()
                {
                    iotAlert
                });
            Entity wrongIotAlert = new Entity("msdyn_iotalert", Guid.NewGuid());
            var inputs = new Dictionary<string, object>() {
                { "IotAlertId", wrongIotAlert.ToEntityReference() }
            };
            var outputParameter = fakedContext.ExecuteCodeActivity<GetActiveBPFForIotAlert>(inputs);
            var activeBusinessProcessFlowId = outputParameter["ActiveBusinessProcessFlowId"] as EntityReference;
            Assert.IsNull(activeBusinessProcessFlowId);
        }
        [TestMethod]
        public void When_Passing_Wrong_IotAlertId_Should_Return_Null_BusinessProcessFlow()
        {
            XrmFakedContext fakedContext = new XrmFakedContext();
            Entity iotAlert = new Entity("msdyn_iotalert", Guid.NewGuid());
            Entity businessProcessFlow = new Entity("msdyn_bpf_477c16f59170487b8b4dc895c5dcd09b", Guid.NewGuid());
            businessProcessFlow["bpf_msdyn_iotalertid"] = iotAlert.ToEntityReference();
            businessProcessFlow["statuscode"] = new OptionSetValue((int)StatusCode.Active);
            fakedContext.Initialize(new List<Entity>()
                {
                    iotAlert, businessProcessFlow
                });
            Entity wrongIotAlert = new Entity("msdyn_iotalert", Guid.NewGuid());
            var inputs = new Dictionary<string, object>() {
                { "IotAlertId", wrongIotAlert.ToEntityReference() }
            };
            var outputParameter = fakedContext.ExecuteCodeActivity<GetActiveBPFForIotAlert>(inputs);
            var activeBusinessProcessFlowId = outputParameter["ActiveBusinessProcessFlowId"] as EntityReference;
            Assert.IsNull(activeBusinessProcessFlowId);
        }

        [TestMethod]
        public void When_OnlyAborted_BusinessProcessExist_And_Passing_Correct_IotAlertId_Should_Return_Null()
        {
            XrmFakedContext fakedContext = new XrmFakedContext();
            Entity iotAlert = new Entity("msdyn_iotalert", Guid.NewGuid());
            Entity businessProcessFlow = new Entity("msdyn_bpf_477c16f59170487b8b4dc895c5dcd09b", Guid.NewGuid());
            businessProcessFlow["bpf_msdyn_iotalertid"] = iotAlert.ToEntityReference();
            businessProcessFlow["statuscode"] = new OptionSetValue((int)StatusCode.Aborted);
            fakedContext.Initialize(new List<Entity>()
                {
                    iotAlert, businessProcessFlow
                });
            var inputs = new Dictionary<string, object>() {
                { "IotAlertId", iotAlert.ToEntityReference() }
            };
            var outputParameter = fakedContext.ExecuteCodeActivity<GetActiveBPFForIotAlert>(inputs);
            var activeBusinessProcessFlowId = outputParameter["ActiveBusinessProcessFlowId"] as EntityReference;
            Assert.IsNull(activeBusinessProcessFlowId);
        }

        [TestMethod]
        public void When_OnlyFinished_BusinessProcessExist_And_Passing_Correct_IotAlertId_Should_Return_Null()
        {
            XrmFakedContext fakedContext = new XrmFakedContext();
            Entity iotAlert = new Entity("msdyn_iotalert", Guid.NewGuid());
            Entity businessProcessFlow = new Entity("msdyn_bpf_477c16f59170487b8b4dc895c5dcd09b", Guid.NewGuid());
            businessProcessFlow["bpf_msdyn_iotalertid"] = iotAlert.ToEntityReference();
            businessProcessFlow["statuscode"] = new OptionSetValue((int)StatusCode.Finished);
            fakedContext.Initialize(new List<Entity>()
                {
                    iotAlert, businessProcessFlow
                });
            var inputs = new Dictionary<string, object>() {
                { "IotAlertId", iotAlert.ToEntityReference() }
            };
            var outputParameter = fakedContext.ExecuteCodeActivity<GetActiveBPFForIotAlert>(inputs);
            var activeBusinessProcessFlowId = outputParameter["ActiveBusinessProcessFlowId"] as EntityReference;
            Assert.IsNull(activeBusinessProcessFlowId);
        }
    }
}
