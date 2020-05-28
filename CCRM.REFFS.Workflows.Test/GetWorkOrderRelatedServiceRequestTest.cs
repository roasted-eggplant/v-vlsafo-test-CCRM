using FakeXrmEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCRM.REFFS.Workflows.Test
{
    [TestClass]
    public class GetWorkOrderRelatedServiceRequestTest
    {
        [TestMethod]
        public void When_Passing_CorrectSeriviceRequestId_Should_Return_WorkOrderRecord()
        {
            XrmFakedContext fakedContext = new XrmFakedContext();
            Entity serviceRequest = new Entity("incident", Guid.NewGuid());
            Entity workOrder = new Entity("msdyn_workorder", Guid.NewGuid());
            workOrder["msdyn_servicerequest"] = serviceRequest.ToEntityReference();
            workOrder["statuscode"] = new OptionSetValue((int)StatusCode_WO.Active);
            fakedContext.Initialize(new List<Entity>()
                {
                    serviceRequest, workOrder
                });
            var inputs = new Dictionary<string, object>() {
                { "ServiceRequestId", serviceRequest.ToEntityReference() }
            };
            var outputParameter = fakedContext.ExecuteCodeActivity<GetWorkOrderRelatedServiceRequest>(inputs);
            var isExists = (bool)outputParameter["IsExists"];
            Assert.IsTrue(isExists);
            var workOrderId = outputParameter["WorkOrderId"] as EntityReference;
            Assert.AreEqual(workOrderId.Id, workOrder.ToEntityReference().Id);

        }
        [TestMethod]
        public void When_No_WorkOrder_Exist_RelatedTo_ServiceRequestRecord_Should_Return_Null_WorkOrder()
        {
            XrmFakedContext fakedContext = new XrmFakedContext();
            Entity serviceRequest = new Entity("incident", Guid.NewGuid());
            fakedContext.Initialize(new List<Entity>()
                {
                    serviceRequest
                });
            var inputs = new Dictionary<string, object>() {
                { "ServiceRequestId", serviceRequest.ToEntityReference() }
            };
            var outputParameter = fakedContext.ExecuteCodeActivity<GetWorkOrderRelatedServiceRequest>(inputs);
            var workOrderId = outputParameter["WorkOrderId"] as EntityReference;
            var isExists = (bool)outputParameter["IsExists"];
            Assert.IsFalse(isExists);
            Assert.IsNull(workOrderId);
        }
        [TestMethod]
        public void When_Passing_Wrong_Service_RequestId_Should_Return_Null_WorkOrder()
        {
            XrmFakedContext fakedContext = new XrmFakedContext();
            Entity serviceRequest = new Entity("incident", Guid.NewGuid());
            Entity wrongServiceRequest = new Entity("incident", Guid.NewGuid());
            Entity workOrder = new Entity("msdyn_workorder", Guid.NewGuid());
            workOrder["msdyn_servicerequest"] = serviceRequest.ToEntityReference();
            workOrder["statuscode"] = new OptionSetValue((int)StatusCode_WO.Active);
            fakedContext.Initialize(new List<Entity>()
                {
                    serviceRequest, workOrder
                });
            var inputs = new Dictionary<string, object>() {
                { "ServiceRequestId", wrongServiceRequest.ToEntityReference() }
            };
            var outputParameter = fakedContext.ExecuteCodeActivity<GetWorkOrderRelatedServiceRequest>(inputs);
            var isExists = (bool)outputParameter["IsExists"];
            Assert.IsFalse(isExists);
            var workOrderId = outputParameter["WorkOrderId"] as EntityReference;
            Assert.IsNull(workOrderId);
        }
        [TestMethod]
        public void When_OnlyDeactivated_WorkOrderExist_And_Passing_Correct_IotAlertId_Should_Return_Null()
        {
            XrmFakedContext fakedContext = new XrmFakedContext();
            Entity serviceRequest = new Entity("incident", Guid.NewGuid());
            Entity workOrder = new Entity("msdyn_workorder", Guid.NewGuid());
            workOrder["msdyn_servicerequest"] = serviceRequest.ToEntityReference();
            workOrder["statuscode"] = new OptionSetValue((int)StatusCode_WO.Inactive);
            fakedContext.Initialize(new List<Entity>()
                {
                    serviceRequest, workOrder
                });
            var inputs = new Dictionary<string, object>() {
                { "ServiceRequestId", serviceRequest.ToEntityReference() }
            };
            var outputParameter = fakedContext.ExecuteCodeActivity<GetWorkOrderRelatedServiceRequest>(inputs);
            var isExists = (bool)outputParameter["IsExists"];
            Assert.IsFalse(isExists);
            var workOrderId = outputParameter["WorkOrderId"] as EntityReference;
            Assert.IsNull(workOrderId);
        }
    }
}
