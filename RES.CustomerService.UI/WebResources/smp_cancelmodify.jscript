/// <summary>
///   Cancel and modify
/// </summary>

/*
 * DuplicateSR1
 * @param {any} executionContext
 */
function DuplicateSR1(executionContext) {
    var formContext = executionContext.getFormContext();
    var r = confirm("Do you want to Cancel the Service Request and Create new Service Request ?");
    if (r === true) {

        // Fetch Current Service Request Details
        var context = formContext.context;
        var serverUrl = context.getClientUrl();
        var guid = formContext.data.entity.getId();

        var ODATA_ENDPOINT = "/XRMServices/2011/OrganizationData.svc";
        // Specify the ODATA entity collection 
        var ODATA_EntityCollection = "/IncidentSet";
        var ODATA_SELECT = "smp_RequestorPhone,smp_RequestorAlias,smp_RequestorRoomNo,smp_RequestorEmail,smp_ContactPhone,smp_ContactAlias,smp_ContactRoom,smp_CostCenterCode,smp_ContactEmail,smp_ProblemBuildingAddressLine1,smp_ProblemBuildingAddressLine2,smp_ProblemBuildingCity,smp_ProblemBuildingState,smp_ProblemBuildingCountry,smp_ProblemBuildingZipcode,smp_ProblemRoom,smp_ProblemTypeDescription,smp_ServiceRequestDeviceType,CaseOriginCode,smp_RequestorId,smp_BuildingId,smp_Contact,smp_ContactBuilding,smp_ProblemBuilding,smp_ProblemRoomType,smp_ProblemBuildingTimeZone,smp_ProblemTypeId,smp_ProblemClassId,smp_Problemoccureddatetime";
        //debugger;

        //Asynchronous AJAX function to Create a CRM record using OData
        $.ajax({
            type: "GET",
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            url: serverUrl + ODATA_ENDPOINT + ODATA_EntityCollection + "(guid'" + guid + "')?$select=" + ODATA_SELECT,
            beforeSend: function (XMLHttpRequest) {
                //Specifying this header ensures that the results will be returned as JSON. 
                XMLHttpRequest.setRequestHeader("Accept", "application/json");
            },
            success: function (data, textStatus, XmlHttpRequest) {
                //alert("success");
                var incidentObject = data["d"];
                if (incidentObject !== null) {
                    CreateIncident(formContext, incidentObject);
                }
                else {
                    alert("Unexpected error has occurred, please try after some time.");
                    return;
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert("Unexpected error has occurred, please try after some time.");
                return;
            }
        });

    }
}

/*
 * CreateIncident
 * @param {any} formContext
 * @param {any} incidentObject
 */
function CreateIncident(formContext, incidentObject) {

    var sRId = formContext.data.entity.getId();
    var sRName = formContext.getAttribute("title").getValue();

    //Create Incident Record
    var incident = new Object();

    var lookupField = new Array();
    var lookupData = new Object();
    lookupData.id = RetrieveRecordsSynchronously(formContext);
    lookupData.name = "Not Assigned";
    lookupData.entityType = 'account';
    lookupField[0] = lookupData;

    var provider = new Object();
    provider.AccountId = lookupData.id;
    provider.Name = "Not Assigned";
    if (provider !== null) {
        incident.CustomerId = { Id: provider.AccountId, LogicalName: "account", Name: provider.Name };
    }

    incident.smp_RequestorPhone = incidentObject.smp_RequestorPhone;
    incident.smp_RequestorAlias = incidentObject.smp_RequestorAlias;
    incident.smp_RequestorRoomNo = incidentObject.smp_RequestorRoomNo;
    incident.smp_RequestorEmail = incidentObject.smp_RequestorEmail;
    incident.smp_ContactPhone = incidentObject.smp_ContactPhone;
    incident.smp_ContactAlias = incidentObject.smp_ContactAlias;
    incident.smp_ContactRoom = incidentObject.smp_ContactRoom;
    incident.smp_CostCenterCode = incidentObject.smp_CostCenterCode;
    incident.smp_ContactEmail = incidentObject.smp_ContactEmail;
    incident.smp_ProblemBuildingAddressLine1 = incidentObject.smp_ProblemBuildingAddressLine1;
    incident.smp_ProblemBuildingAddressLine2 = incidentObject.smp_ProblemBuildingAddressLine2;
    incident.smp_ProblemBuildingCity = incidentObject.smp_ProblemBuildingCity;
    incident.smp_ProblemBuildingState = incidentObject.smp_ProblemBuildingState;
    incident.smp_ProblemBuildingCountry = incidentObject.smp_ProblemBuildingCountry;
    incident.smp_ProblemBuildingZipcode = incidentObject.smp_ProblemBuildingZipcode;
    incident.smp_ProblemRoom = incidentObject.smp_ProblemRoom;
    incident.smp_ProblemTypeDescription = incidentObject.smp_ProblemTypeDescription;
    incident.smp_ServiceRequestDeviceType = { Value: incidentObject.smp_ServiceRequestDeviceType.Value };
    incident.CaseOriginCode = { Value: 180620003 };
    incident.smp_CreatedFrom = { Value: 3 };

    var dt = incidentObject.smp_Problemoccureddatetime;
    dt = dt.replace("/Date(", "");
    dt = dt.replace(")/", "");
    var dateValue = new Date(parseInt(dt, 10));
    dateValue.setDate(dateValue.getDate());

    incident.smp_Problemoccureddatetime = dateValue;

    var referenceSR = new Object();
    referenceSR.IncidentId = sRId;
    referenceSR.Title = sRName;
    if (referenceSR !== null) {
        incident.smp_ReferenceSR = { Id: referenceSR.IncidentId, LogicalName: "incident", Name: referenceSR.Title };
    }

    var Requestor = new Object();
    Requestor.ContactId = incidentObject.smp_RequestorId.Id;
    Requestor.FullName = incidentObject.smp_RequestorId.Name;
    if (Requestor.ContactId !== null) {
        incident.smp_RequestorId = { Id: Requestor.ContactId, LogicalName: "contact", Name: Requestor.FullName };
    }

    var RequestorBuilding = new Object();
    RequestorBuilding.smp_buildingId = incidentObject.smp_BuildingId.Id;
    RequestorBuilding.smp_buildingname = incidentObject.smp_BuildingId.Name;
    if (RequestorBuilding.smp_buildingId !== null) {
        incident.smp_BuildingId = { Id: RequestorBuilding.smp_buildingId, LogicalName: "smp_building", Name: RequestorBuilding.smp_buildingname };
    }

    var Contact = new Object();
    Contact.ContactId = incidentObject.smp_Contact.Id;
    Contact.FullName = incidentObject.smp_Contact.Name;
    if (Contact.ContactId !== null) {
        incident.smp_Contact = { Id: Contact.ContactId, LogicalName: "contact", Name: Contact.FullName };
    }

    var ContactBuilding = new Object();
    ContactBuilding.smp_buildingId = incidentObject.smp_ContactBuilding.Id;
    ContactBuilding.smp_buildingname = incidentObject.smp_ContactBuilding.Name;
    if (ContactBuilding.smp_buildingId !== null) {
        incident.smp_ContactBuilding = { Id: ContactBuilding.smp_buildingId, LogicalName: "smp_building", Name: ContactBuilding.smp_buildingname };
    }

    var ProblemBuilding = new Object();
    ProblemBuilding.smp_buildingId = incidentObject.smp_ProblemBuilding.Id;
    ProblemBuilding.smp_buildingname = incidentObject.smp_ProblemBuilding.Name;
    if (ProblemBuilding !== null) {
        incident.smp_ProblemBuilding = { Id: ProblemBuilding.smp_buildingId, LogicalName: "smp_building", Name: ProblemBuilding.smp_buildingname };
    }

    var ProblemRoomType = new Object();
    ProblemRoomType.smp_roomtypeid = incidentObject.smp_ProblemRoomType.Id;
    ProblemRoomType.smp_roomtype = incidentObject.smp_ProblemRoomType.Name;
    if (ProblemRoomType !== null) {
        incident.smp_ProblemRoomType = { Id: ProblemRoomType.smp_roomtypeid, LogicalName: "smp_roomtype", Name: ProblemRoomType.smp_roomtype };
    }

    var ProblemBuildingTimeZone = new Object();
    ProblemBuildingTimeZone.smp_timezoneId = incidentObject.smp_ProblemBuildingTimeZone.Id;
    ProblemBuildingTimeZone.smp_timezonename = incidentObject.smp_ProblemBuildingTimeZone.Name;
    if (ProblemBuildingTimeZone.smp_timezoneId !== null) {
        incident.smp_ProblemBuildingTimeZone = { Id: ProblemBuildingTimeZone.smp_timezoneId, LogicalName: "smp_timezone", Name: ProblemBuildingTimeZone.smp_timezonename };
    }

    var ProblemType = new Object();
    ProblemType.smp_problemtypeid = incidentObject.smp_ProblemTypeId.Id;
    ProblemType.smp_problemtypename = incidentObject.smp_ProblemTypeId.Name;
    if (ProblemType !== null) {
        incident.smp_ProblemTypeId = { Id: ProblemType.smp_problemtypeid, LogicalName: "smp_problemtype", Name: ProblemType.smp_problemtypename };
    }

    var ProblemClass = new Object();
    ProblemClass.smp_problemclassId = incidentObject.smp_ProblemClassId.Id;
    ProblemClass.smp_problemclassname = incidentObject.smp_ProblemClassId.Name;
    if (ProblemClass !== null) {
        incident.smp_ProblemClassId = { Id: ProblemClass.smp_problemclassId, LogicalName: "smp_problemclass", Name: ProblemClass.smp_problemclassname };
    }

    var owner = formContext.context.getUserId();
    incident.smp_CSR =
        {
            Id: owner,
            LogicalName: "systemuser"
        };

    incident.OwnerId =
        {
            Id: owner,
            LogicalName: "systemuser"
        };

    var createdIncident = CreateRecordSync(formContext, incident, "IncidentSet");
    if (createdIncident) {
        var entityFormOptions = {};
        entityFormOptions["entityName"] = incident;
        entityFormOptions["entityId"] = createdIncident.IncidentId;
        Xrm.Navigation.openForm(entityFormOptions).then(
            function (success) {
                console.log(success);
            },
            function (error) {
                console.log(error);
            });
    }
}

/*
 * CreateRecordSync
 * @param {any} formContext
 * @param {any} entityObject
 * @param {any} odataSetName
 */
function CreateRecordSync(formContext, entityObject, odataSetName) {
    try {
        var jsonEntity = window.JSON.stringify(entityObject);
        // Get Server URL
        var serverUrl = formContext.context.getClientUrl();
        //The OData end-point
        var ODATA_ENDPOINT = "/XRMServices/2011/OrganizationData.svc";
        var createRecordReq = new XMLHttpRequest();
        var ODataPath = serverUrl + ODATA_ENDPOINT;
        createRecordReq.open('POST', ODataPath + "/" + odataSetName, false);
        createRecordReq.setRequestHeader("Accept", "application/json");
        createRecordReq.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        createRecordReq.send(jsonEntity);
        var newRecord = JSON.parse(createRecordReq.responseText).d;
        return newRecord;
    } catch (e) {
        alert(e.Message);
    }

}

/*
 * RetrieveRecordsSynchronously
 * @param {any} formContext
 */
function RetrieveRecordsSynchronously(formContext) {
    var retrieveRecordsReq = new XMLHttpRequest();
    var ODataPath = formContext.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/AccountSet?$filter=Name eq 'Not Assigned' ";

    retrieveRecordsReq.open('GET', ODataPath, false);
    retrieveRecordsReq.setRequestHeader("Accept", "application/json");
    retrieveRecordsReq.setRequestHeader("Content-Type", "application/json; charset=utf-8");
    retrieveRecordsReq.send(null);

    var records = JSON.parse(retrieveRecordsReq.responseText).d;
    //Read the first account name
    if (records.results !== null)
        return records.results[0].AccountId;
}

/*
 * UpdateCancelledForReclassification
 * @param {any} executionContext
 */
function UpdateCancelledForReclassification(executionContext) {
    var formContext = executionContext.getFormContext();
    var incidentId = formContext.data.entity.getId();
    var odataSetName = "IncidentSet";
    var objIncident = new Object();
    objIncident.smp_CancelledForReclassification = true;
    objIncident.smp_RequestCancelledReason = 'Reclassified';


    var jsonEntity = window.JSON.stringify(objIncident);

    // Get Server URL
    var serverUrl = formContext.context.getClientUrl();
    //The OData end-point
    var ODATA_ENDPOINT = "/XRMServices/2011/OrganizationData.svc";
    //Asynchronous AJAX function to Update a CRM record using OData
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        data: jsonEntity,
        url: serverUrl + ODATA_ENDPOINT + "/" + odataSetName + "(guid'" + incidentId + "')",
        beforeSend: function (XMLHttpRequest) {
            //Specifying this header ensures that the results will be returned as JSON.
            XMLHttpRequest.setRequestHeader("Accept", "application/json");
            //Specify the HTTP method MERGE to update just the changes you are submitting.
            XMLHttpRequest.setRequestHeader("X-HTTP-Method", "MERGE");
        },
        success: function (data, textStatus, XmlHttpRequest) {
            alert("Updated successfully");
        },
        error: function (XmlHttpRequest, textStatus, errorThrown) {
            if (XmlHttpRequest && XmlHttpRequest.responseText) {
                alert("Error while updating " + odataSetName + " ; Error – " + XmlHttpRequest.responseText);
            }
        }
    });
}

/*
 * CancelIncident
 * @param {any} executionContext
 */
function CancelIncident(executionContext) {
    var formContext = executionContext.getFormContext();
    var request = "<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\">";
    request += "<s:Body>";
    request += "<Execute xmlns=\"http://schemas.microsoft.com/xrm/2011/Contracts/Services\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\">";
    request += "<request i:type=\"b:SetStateRequest\" xmlns:a=\"http://schemas.microsoft.com/xrm/2011/Contracts\" xmlns:b=\"http://schemas.microsoft.com/crm/2011/Contracts\">";
    request += "<a:Parameters xmlns:c=\"http://schemas.datacontract.org/2004/07/System.Collections.Generic\">";
    request += "<a:KeyValuePairOfstringanyType>";
    request += "<c:key>EntityMoniker</c:key>";
    request += "<c:value i:type=\"a:EntityReference\">";
    request += "<a:Id>" + formContext.data.entity.getId() + "</a:Id>";
    request += "<a:LogicalName>incident</a:LogicalName>";
    request += "<a:Name i:nil=\"true\" />";
    request += "</c:value>";
    request += "</a:KeyValuePairOfstringanyType>";
    request += "<a:KeyValuePairOfstringanyType>";
    request += "<c:key>State</c:key>";
    request += "<c:value i:type=\"a:OptionSetValue\">";
    request += "<a:Value>2</a:Value>";
    request += "</c:value>";
    request += "</a:KeyValuePairOfstringanyType>";
    request += "<a:KeyValuePairOfstringanyType>";
    request += "<c:key>Status</c:key>";
    request += "<c:value i:type=\"a:OptionSetValue\">";
    request += "<a:Value>6</a:Value>";
    request += "</c:value>";
    request += "</a:KeyValuePairOfstringanyType>";
    request += "</a:Parameters>";
    request += "<a:RequestId i:nil=\"true\" />";
    request += "<a:RequestName>SetState</a:RequestName>";
    request += "</request>";
    request += "</Execute>";
    request += "</s:Body>";
    request += "</s:Envelope>";

    //send set state request
    var req = new XMLHttpRequest();
    req.open("POST", formContext.context.getClientUrl() + "/XRMServices/2011/Organization.svc/web", true);
    // Responses will return XML. It isn't possible to return JSON.
    req.setRequestHeader("Accept", "application/xml, text/xml, */*");
    req.setRequestHeader("Content-Type", "text/xml; charset=utf-8");
    req.setRequestHeader("SOAPAction", "http://schemas.microsoft.com/xrm/2011/Contracts/Services/IOrganizationService/Execute");
    var response = req.send(request);
}

/*
 * DuplicateSR
 * @param {any} executionContext
 */
function DuplicateSR(executionContext) {
    var formContext = executionContext.getFormContext();
    var r = confirm("Do you want to Cancel the Service Request and Create new Service Request ?");
    if (r === true) {
        var requestName = "smp_Reclassify";
        var requestXML = "";
        requestXML += "<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\">";
        requestXML += "  <s:Body>";
        requestXML += "    <Execute xmlns=\"http://schemas.microsoft.com/xrm/2011/Contracts/Services\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\">";
        requestXML += "      <request xmlns:a=\"http://schemas.microsoft.com/xrm/2011/Contracts\">";
        requestXML += "        <a:Parameters xmlns:b=\"http://schemas.datacontract.org/2004/07/System.Collections.Generic\">";
        requestXML += "          <a:KeyValuePairOfstringanyType>";
        requestXML += "            <b:key>Target</b:key>";
        requestXML += "            <b:value i:type=\"a:EntityReference\">";
        requestXML += "              <a:Id>" + formContext.data.entity.getId() + "</a:Id>";
        requestXML += "              <a:LogicalName>" + formContext.data.entity.getEntityName() + "</a:LogicalName>";
        requestXML += "              <a:Name i:nil=\"true\" />";
        requestXML += "            </b:value>";
        requestXML += "          </a:KeyValuePairOfstringanyType>";
        requestXML += "        </a:Parameters>";
        requestXML += "        <a:RequestId i:nil=\"true\" />";
        requestXML += "        <a:RequestName>" + requestName + "</a:RequestName>";
        requestXML += "      </request>";
        requestXML += "    </Execute>";
        requestXML += "  </s:Body>";
        requestXML += "</s:Envelope>";
        var req = new XMLHttpRequest();
        req.open("POST", GetClientUrl(formContext), false);
        req.setRequestHeader("Accept", "application/xml, text/xml, */*");
        req.setRequestHeader("Content-Type", "text/xml; charset=utf-8");
        req.setRequestHeader("SOAPAction", "http://schemas.microsoft.com/xrm/2011/Contracts/Services/IOrganizationService/Execute");
        req.send(requestXML);
        //Get the Resonse from the CRM Execute method
        var response = req.responseXML.xml;

        var id = $(req.response).children(":first").children(":first").children(":first").children("a\\:Results").children("a\\:KeyValuePairOfstringanyType").children("b\\:value").children("a\\:Id").text();
        var logicalName = $(req.response).children(":first").children(":first").children(":first").children("a\\:Results").children("a\\:KeyValuePairOfstringanyType").children("b\\:value").children("a\\:LogicalName").text();
        var entityFormOptions = {};
        entityFormOptions["entityName"] = logicalName;
        entityFormOptions["entityId"] = id;
        Xrm.Navigation.openForm(entityFormOptions).then(
            function (success) {
                console.log(success);
            },
            function (error) {
                console.log(error);
            });
    }
}

/*
 * GetClientUrl
 * @param {any} formContext
 */
function GetClientUrl(formContext) {
    if (typeof formContext.context === "object") {
        clientUrl = formContext.context.getClientUrl();
    }
    var ServicePath = "/XRMServices/2011/Organization.svc/web";
    return clientUrl + ServicePath;
}