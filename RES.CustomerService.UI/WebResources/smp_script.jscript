/// <summary>
///  SR Script to handle for multiple entities.
/// </summary>

var onChangeStatus = false;

/*Start Page Load Java script functions*/

/* Declaring Service Request Status Values */
var Approved = 4;
var Backordered = 180620018;
var CallBack = 180620001;
var Declined = 180620000;
var Dispatched = 180620002;
var Draft = 1;
var OnHoldPendingCustomerScheduling = 180620017;
var Open = 2;
var PendingApproval = 3;
var PendingCSRDispatch = 180620012;
var PendingTransmittal = 180620003;
var Referred = 180620010;
var Revised = 180620006;
var WaitingForApproval = 180620013;
var Inprogress = 180620005;
var Acknowledged = 180620004;
var InformationProvider = 1000;
var Closed = 5;
var Completed = 180620011;
var OnHoldpendingvendorArrival = 180620008;
var OnHoldpendinparts = 180620007;
var OnHoldpendingvendorScheduling = 180620009;


/*
 * function to update provider details
 * @param {any} executionContext: Execution Context
 */
function UpdateProvider(executionContext) {
    onChangeStatus = false;
    var formContext = executionContext.getFormContext();
    if (formContext.getAttribute("customerid").getValue() === null && formContext.ui.getFormType() === 1) {
        var retrieveRecordsReq = new XMLHttpRequest();
        var ODataPath = formContext.context.getClientUrl() + "/api/data/v9.0/accounts?$select=accountid&$filter=name eq 'Not Assigned'";
        retrieveRecordsReq.open('GET', ODataPath, false);
        retrieveRecordsReq.setRequestHeader("OData-MaxVersion", "4.0");
        retrieveRecordsReq.setRequestHeader("OData-Version", "4.0");
        retrieveRecordsReq.setRequestHeader("Accept", "application/json");
        retrieveRecordsReq.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        retrieveRecordsReq.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
        retrieveRecordsReq.onreadystatechange = function () {
            if (this.readyState === 4) {
                retrieveRecordsReq.onreadystatechange = null;
                if (this.status === 200 || this.status === 204) {
                    SetDefaultProvider(formContext, this);
                }
                else {
                    showAlertDialog(this.statusText, "UpdateProvider");
                }
            }
        };
        retrieveRecordsReq.send(null);
    }
}

/*
 * function to supress mandantory fields
 * @param {any} executionContext: Execution Context
 * @param {any} fieldname: Field Name
 */
function SupressMandatoryFields(executionContext, fieldname) {
    var formContext = executionContext.getFormContext();
    if (formContext.ui.getFormType() === 1) { //Create case
        var field = formContext.getAttribute(fieldname);
        if (field !== null) {
            field.setRequiredLevel("none");
        }
    }
}

/*
 * Hide Request Cancelled Reason and Reclassified Reason if Status is not equals cancelled
 * @param {any} executionContext: Execution Context
 */
function Load(executionContext) {
    var formContext = executionContext.getFormContext();
    if (formContext.ui.getFormType() !== 1) {
        var statusCodeOptionSet = formContext.getAttribute("statuscode").getValue();
        if (statusCodeOptionSet === 6) {
            formContext.ui.controls.get("smp_requestcancelledreason").setVisible(true);
            if (formContext.getAttribute("smp_reclassifiedsr").getValue() !== null) {
                formContext.ui.controls.get("smp_reasonforreclassifyingtherecordmemo").setVisible(true);
            }
            else {
                formContext.ui.controls.get("smp_reasonforreclassifyingtherecordmemo").setVisible(false);
            }
        }
        else {

            formContext.ui.controls.get("smp_requestcancelledreason").setVisible(false);
            formContext.ui.controls.get("smp_reasonforreclassifyingtherecordmemo").setVisible(false);
        }
    }

    var type = formContext.ui.getFormType();
    if (type !== null && type === 2) {
        var ODataPath = formContext.context.getClientUrl() + "/api/data/v9.0/roles?$select=name,roleid&$filter=name eq 'Provider User' or  name eq 'CSR' or name eq 'CSR Supervisor' or name eq 'System Administrator'&$orderby=name asc";
        var service = new XMLHttpRequest();
        if (service !== null) {
            service.open("GET", ODataPath, false);
            service.setRequestHeader("OData-MaxVersion", "4.0");
            service.setRequestHeader("OData-Version", "4.0");
            service.setRequestHeader("Accept", "application/json");
            service.setRequestHeader("Content-Type", "application/json; charset=utf-8");
            service.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
            service.onreadystatechange = function () {
                if (this.readyState === 4) {
                    service.onreadystatechange = null;
                    if (this.status === 200 || this.status === 204) {
                        ProviderUserRole(executionContext, this);
                    }
                    else {
                        showAlertDialog(this.statusText, "Load");
                    }
                }
            };
            service.send(null);
        }
    }

    formContext.getControl("smp_priorityescalationpopupnotes").setVisible(false);
    SetCSROnLoad(formContext);
    //Adding User Story : 3949201
    //var providerRole = GetConfigurationData(executionContext, 'ProviderUser_Role_Guid');
    var currentUserRoles = executionContext.getContext().userSettings.securityRoles;
    SetPriorityID(executionContext, currentUserRoles);
}

/*
 * function to Set Problem DateTime
 * @param {any} executionContext: Execution Context
 */
function SetProblemDateTime(executionContext) {
    var formContext = executionContext.getFormContext();
    if (formContext.ui.getFormType() === 1) {
        var problemOccuredDate = formContext.data.entity.attributes.get("smp_problemoccureddatetime");
        var today = new Date();
        problemOccuredDate.setValue(today);
    }
}

/*
 * function to find current logged in user security role and hide status
 * @param {any} executionContext: Execution Context
 */
function HideStatusBasedOnSecurityRole(executionContext) {
    var formContext = executionContext.getFormContext();
    if (formContext.ui.getFormType() === 1) {
        var serverUrl = formContext.context.getClientUrl();
        var oDataEndpointUrl = serverUrl + "/api/data/v9.0/roles?$select=roleid&$filter=name eq 'System%20Administrator' or name eq 'Application%20Admin'";
        var service = new XMLHttpRequest();
        if (service !== null) {
            service.open("GET", oDataEndpointUrl, true);
            service.setRequestHeader("OData-MaxVersion", "4.0");
            service.setRequestHeader("OData-Version", "4.0");
            service.setRequestHeader("Accept", "application/json");
            service.setRequestHeader("Content-Type", "application/json; charset=utf-8");
            service.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
            service.onreadystatechange = function () {
                if (this.readyState === 4) {
                    service.onreadystatechange = null;
                    if (this.status === 200 || this.status === 204) {
                        UserHasRole(executionContext, this);
                    }
                    else {
                        showAlertDialog(this.statusText, "HideStatusBasedOnSecurityRole");
                    }
                }
            };
            service.send(null);
        }
    }
}

/*
 * Make CostCenter invisible when IsApprovalRequired is false
 * @param {any} executionContext: Execution Context
 */
function HideCostCenter(executionContext) {
    var formContext = executionContext.getFormContext();
    if (formContext.ui.getFormType() === 1) {
        var ApprovalRequired = formContext.getAttribute("smp_isapprovalrequired").getValue();
        if (ApprovalRequired === true) {
            formContext.getAttribute("smp_costcentercode").setRequiredLevel("required");
            formContext.getControl("smp_iocode").setVisible(true);
        }
        else {
            formContext.getAttribute("smp_costcentercode").setRequiredLevel("none");
            formContext.getControl("smp_iocode").setVisible(false);
        }
    }
}

/*
 * function to handle Refresh Escalation
 * @param {any} executionContext: Execution Context
 */
function RefreshEscalation(executionContext) {
    var formContext = executionContext.getFormContext();
    if (formContext.ui.getFormType() === 1) {
        var wrControl = formContext.ui.controls.get("WebResource_escalationcontacts");
        wrControl.setSrc(wrControl.getSrc());
    }
}

/*
 * function to handle reclassification status
 * @param {any} executionContext: Execution Context
 */
function HideReclassificationStatus(executionContext) {
    var formContext = executionContext.getFormContext();
    if (formContext.ui.getFormType() === 1) {
        var PickListControl = formContext.getControl("caseorigincode");
        var CaseOrigin = formContext.data.entity.attributes.get("caseorigincode").getValue();
        if (CaseOrigin !== PendingTransmittal) {
            PickListControl.removeOption(PendingTransmittal);
        }
    }
}

/*Calling on below scenarioa
On Page Load
Problem Type OnChange
Problem Class OnChange
Problem Building OnChange */

/*
 * function to handle requested due date
 * @param {any} executionContext: Execution Context
 */
function HideRequestedDueDate(executionContext) {
    if (onChangeStatus === true) {
        return;
    }
    var formContext = executionContext.getFormContext();
    if (formContext.ui.getFormType() === 2) {
        var building = formContext.getAttribute("smp_problembuilding");
        var problemclass = formContext.getAttribute("smp_problemclassid");
        var problemtype = formContext.getAttribute("smp_problemtypeid");
        var roomtype = formContext.getAttribute("smp_problemroomtype");
        var isFinancialStateCampus = false;
        var zone;
        var priorityconfigurables;
        var priorityName;
        var Odata = null, results = null;
        var ispriorityConfigurables = false;

        if (building !== null && problemclass !== null && problemtype !== null && roomtype !== null) {
            var buildinglookup = building.getValue();
            var problemclasslookup = problemclass.getValue();
            var problemtypelookup = problemtype.getValue();
            var roomtypelookup = roomtype.getValue();

            if (buildinglookup !== null && buildinglookup !== undefined) {
                Odata = "/api/data/v9.0/smp_buildings(" + buildinglookup[0].id.replace('{', "").replace('}', "") + ")?$select=smp_addressline2,smp_isfinancialstatecampus";
                results = RetrieveODataResults(formContext, Odata);
                if (results !== null && results !== undefined) {
                    if (results.smp_isfinancialstatecampus !== null) {
                        isFinancialStateCampus = results.smp_isfinancialstatecampus;
                    }
                    else {
                        isFinancialStateCampus = false;
                    }
                }
            }

            if (roomtypelookup !== null && roomtypelookup !== undefined) {
                Odata = "/api/data/v9.0/smp_roomtypes(" + roomtypelookup[0].id.replace('{', "").replace('}', "") + ")?$select=smp_zone";
                results = RetrieveODataResults(formContext, Odata);
                if (results !== null && results !== undefined) {
                    if (results.smp_zone !== null) {
                        zone = results["smp_zone"];
                    }
                }
            }

            if (problemclasslookup !== null && problemtypelookup !== null && isFinancialStateCampus !== null && zone !== null) {
                Odata = "/api/data/v9.0/smp_slamatrixes?$select=_smp_priorityid_value&$filter=smp_zone eq " + zone + " and  smp_isfinancialstatecampus eq " + isFinancialStateCampus + " and  statuscode eq 1 and  _smp_problemclassid_value eq " + problemclasslookup[0].id.replace('{', "").replace('}', "") + " and  _smp_problemtypeid_value eq " + problemtypelookup[0].id.replace('{', "").replace('}', "") + "";
                results = RetrieveODataResults(formContext, Odata);
                if (results !== null && results !== undefined && results.value !== null && results.value.length >= 1) {
                    priority = results.value[0]["_smp_priorityid_value"];
                    priorityName = results.value[0]["_smp_priorityid_value@OData.Community.Display.V1.FormattedValue"];
                }
            }
            if (priorityName !== null && priorityName !== undefined) {
                /* To make configurable P6, P013 and P004 on portal dynamic as part of User Story 3919693 */
                Odata = "/api/data/v9.0/smp_configurations?$select=smp_value&$filter=smp_title eq 'PortalPriorityConfigurables'";
                results = RetrieveODataResults(formContext, Odata);
                if (results !== null && results !== undefined && results.value !== null && results.value.length >= 1) {
                    priorityconfigurables = results.value[0]["smp_value"];
                    if (priorityconfigurables !== null && priorityconfigurables !== undefined && priorityconfigurables !== "") {
                        priorityconfigurables = priorityconfigurables.toLowerCase();
                        priorityName = priorityName.toLowerCase();
                        var arrpriorityConfigurables = priorityconfigurables.split(',');
                        if (arrpriorityConfigurables.length >= 0) {
                            arrpriorityConfigurables.forEach(function (priorityConfigurable) {
                                priorityConfigurable = priorityConfigurable.trim();
                                if (priorityConfigurable === priorityName) {
                                    ispriorityConfigurables = true;
                                }
                            });
                        }
                    }
                }
                if (ispriorityConfigurables) {
                    formContext.ui.controls.get("smp_requestedduedate").setVisible(true);
                }
                else {
                    formContext.data.entity.attributes.get("smp_requestedduedate").setValue(null);
                    formContext.ui.controls.get("smp_requestedduedate").setVisible(false);
                    if (formContext.data.entity.getIsDirty()) {
                        formContext.data.entity.save();
                    }
                }
            }
        }
        else {
            formContext.data.entity.attributes.get("smp_requestedduedate").setValue(null);
            formContext.ui.controls.get("smp_requestedduedate").setVisible(false);
            if (formContext.data.entity.getIsDirty()) {
                formContext.data.entity.save();
            }
        }
    }
}

/*
 * function to retrieve escalations
 * @param {any} executionContext: Execution Context
 */
function RetrieveEscalations(executionContext) {
    if (onChangeStatus === true) {
        return;
    }
    var formContext = executionContext.getFormContext();
    var problemClassId = null;
    var buildingId = null;
    var problemTypeId = null;
    var providerId = null;

    // Problem Class
    var lookupProblemClass = formContext.getAttribute("smp_problemclassid");
    if (lookupProblemClass !== null) {
        var lookupProblemClassValue = lookupProblemClass.getValue();
        if (lookupProblemClassValue !== null) {
            problemClassId = lookupProblemClassValue[0].id;
        }
    }

    //Building
    var lookupBuilding = formContext.getAttribute("smp_problembuilding");
    if (lookupBuilding !== null) {
        var lookupBuildingValue = lookupBuilding.getValue();
        if (lookupBuildingValue !== null) {
            buildingId = lookupBuildingValue[0].id;
        }
    }

    //Problem Type
    var lookupProblemType = formContext.getAttribute("smp_problemtypeid");
    if (lookupProblemType !== null) {
        var lookupProblemTypeValue = lookupProblemType.getValue();
        if (lookupProblemTypeValue !== null) {
            problemTypeId = lookupProblemTypeValue[0].id;
        }
    }

    //Provider
    var lookupProvider = formContext.getAttribute("customerid");
    if (lookupProvider !== null) {
        var lookupProviderValue = lookupProvider.getValue();

        if (lookupProviderValue !== null) {
            providerId = lookupProviderValue[0].id;
        }
    }

    // Draft
    var statusCode = formContext.getAttribute("statuscode").getValue();

    // Get escaltion contact details
    if (formContext.ui.getFormType() !== 1 && statusCode !== 1 && problemClassId !== null && buildingId !== null && problemTypeId !== null && providerId !== null) {
        var retrieveReq = new XMLHttpRequest();
        var ODataPath = formContext.context.getClientUrl() + "/api/data/v9.0/smp_providermatrixes()?$select=_smp_primaryescalationid_value,_smp_secondaryescalationid_value,_smp_tertiaryescalationid_value,_smp_primaryproviderid_value&$expand=smp_primaryproviderid($select=name,primarycontactid,telephone1,accountid),smp_primaryescalationid($select=emailaddress,smp_escalationid,smp_information,smp_name,smp_otherphone,smp_phone),smp_secondaryescalationid($select=emailaddress,smp_escalationid,smp_information,smp_name,smp_otherphone,smp_phone),smp_tertiaryescalationid($select=emailaddress,smp_information,smp_name,smp_otherphone,smp_phone)&$filter=_smp_buildingid_value eq " + buildingId.replace('{', "").replace('}', "") + " and  _smp_problemclassid_value eq " + problemClassId.replace('{', "").replace('}', "") + " and  _smp_problemtypeid_value eq " + problemTypeId.replace('{', "").replace('}', "") + " and  _smp_primaryproviderid_value eq " + providerId.replace('{', "").replace('}', "") + "";
        retrieveReq.open("GET", ODataPath, false);
        retrieveReq.setRequestHeader("OData-MaxVersion", "4.0");
        retrieveReq.setRequestHeader("OData-Version", "4.0");
        retrieveReq.setRequestHeader("Accept", "application/json");
        retrieveReq.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        retrieveReq.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
        retrieveReq.onreadystatechange = function () {
            if (this.readyState === 4) {
                retrieveReq.onreadystatechange = null;
                if (this.status === 200 || this.status === 204) {
                    RetrieveEscalationsCallBack(formContext, this);
                }
                else {
                    showAlertDialog(this.statusText, "RetrieveEscalations");
                }
            }
        };
        retrieveReq.send();
    }
}

/*
 * SetOriginForSR
 * @param {any} executionContext: Execution Context
 */
function SetOriginForSR(executionContext) {
    var formContext = executionContext.getFormContext();
    if ((formContext.getAttribute("caseorigincode").getValue() === 3) || (formContext.getAttribute("caseorigincode").getValue() === 100008999)) {
        formContext.getControl("caseorigincode").setDisabled(true);
    }
    else {
        formContext.getControl("caseorigincode").removeOption(2483); // Facebook
        formContext.getControl("caseorigincode").removeOption(3986); // Twitter
        formContext.getControl("caseorigincode").removeOption(100009000); // GarconBadgeProject
        //formContext.getControl("caseorigincode").removeOption(100008999); // IOT
        formContext.getControl("caseorigincode").removeOption(180620004); // Integration
        formContext.getControl("caseorigincode").removeOption(180620005); // Mobile
        formContext.getControl("caseorigincode").removeOption(180620003); // Reclassified
        formContext.getControl("caseorigincode").removeOption(180620000); // Fax
        formContext.getControl("caseorigincode").removeOption(3);         // Web
        formContext.getControl("caseorigincode").removeOption(180620002); // PDA
        formContext.getControl("caseorigincode").removeOption(2);         // Email
        formContext.getControl("caseorigincode").removeOption("");
    }
}


/* Calling on below scenarioa
On Page Load
Status OnChange */

/*
 * StatusOptionsForCSR
 * @param {any} executionContext: Execution Context
 */
function StatusOptionsForCSR(executionContext) {
    //var formContext = executionContext.getFormContext();
    //var PickListControl = formContext.getControl("statuscode");
    //var SRStatus = formContext.getAttribute("statuscode").getValue();
    //var IsCSR = false;
    var currentUserRoles = executionContext.getContext().userSettings.securityRoles;
    StatusOptionsIsProviderUser(executionContext, currentUserRoles);
    StatusOptionsIsCSR(executionContext, currentUserRoles);
    //Adding User Story : 3949201
}

/*
 * PortalOnLoad
 * @param {any} executionContext: Execution Context
 */
function PortalOnLoad(executionContext) {
    var formContext = executionContext.getFormContext();
    formContext.getAttribute("smp_contact").setRequiredLevel("required");
    formContext.getAttribute("smp_contactphone").setRequiredLevel("required");
}

/*
 * CheckUserRole
 * @param {any} executionContext: Execution Context
 */
function CheckUserRole(executionContext) {
    var currentUserRoles = executionContext.getContext().userSettings.securityRoles;
    //Adding User Story : 3949201
    var csrRoleGuid = GetConfigurationData(executionContext, 'CSR_Role_Guid');
    for (var i = 0; i < currentUserRoles.length; i++) {
        var userRoleId = currentUserRoles[i];
        //User Story : 3949201
        //var userRoleName = GetRoleName(formContext, userRoleId);
        if (GuidsAreEqual(userRoleId, csrRoleGuid)) {
            return false;
        }
        else {
            return true;
        }
    }
}

/*
 * RoomNumberOtherOnLoad
 * @param {any} executionContext: Execution Context
 */
function RoomNumberOtherOnLoad(executionContext) {
    var formContext = executionContext.getFormContext();
    if (formContext.getAttribute("new_problemroomnumber").getValue() !== null) {
        var roomNumberObject = formContext.getAttribute("new_problemroomnumber");
        var roomNumberLookupField = roomNumberObject.getValue();
        var roomNumberLookupName = roomNumberLookupField[0].name;
        if (roomNumberLookupName === 'Other') {
            formContext.ui.controls.get("smp_problemroom").setVisible(true);
            formContext.getAttribute("smp_problemroom").setRequiredLevel("required");
            formContext.getAttribute("new_problemroomnumber").setRequiredLevel("none");
        }
        else {
            formContext.ui.controls.get("smp_problemroom").setVisible(false);
        }
    }
    else {
        formContext.ui.controls.get("smp_problemroom").setVisible(false);
    }
}

//Filter Problem Room Lookup - Calling on below scenarioa
//On Page Load
//Requestor Name OnChange
//Problem Building OnChange
//Contact Name OnChange

/**
 * FilteredRoomNumberLookupFromBuilding
 * @param {any} executionContext: Execution Context
 */
function FilteredRoomNumberLookupFromBuilding(executionContext) {
    if (onChangeStatus === true) {
        return;
    }
    var formContext = executionContext.getFormContext();
    if (formContext.getAttribute("smp_problembuilding").getValue() !== null) {
        var building = formContext.getAttribute("smp_problembuilding");
        if (building.getValue() !== null) {
            var viewId = "{FBC39F3A-0103-4898-A74D-8540E15F4324}";
            var entityName = "smp_room"; // Entity to be filtered
            var viewDisplayName = "Custom Room Number View"; // Custom name for the lookup window.

            var fetchXml = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'><entity name='smp_room'><attribute name='smp_roomid' />" +
                "<attribute name='smp_name' /><attribute name='createdon' /><order attribute='smp_name' descending='false' />" +
                "<filter type='or'>" +
                "<condition attribute='smp_building' operator='eq' value='" + building.getValue()[0].id + "' />" +
                "<condition attribute='smp_name' value='Other' operator='eq'/>" +
                "</filter>" +
                "</entity></fetch>";

            var layoutXml = '<grid name="resultset" object="1" jump="smp_roomid" select="1" icon="1" preview="1">' +
                '<row name="result" id="smp_roomid"><cell name="smp_name" width="200" /></row></grid>';

            formContext.getControl("new_problemroomnumber").addCustomView(viewId, entityName, viewDisplayName, fetchXml, layoutXml, true);
        }
    }
}

/*Filter Problem Class Lookup - Calling on below scenarioa
On Page Load
Requestor Name OnChange
Problem Building OnChange
Contact Name OnChange */

/**
 * Filter Problem Class Lookup
 * @param {any} executionContext: Execution Context
 */
function FilteredProblemClassLookup(executionContext) {
    if (onChangeStatus === true) {
        return;
    }
    var formContext = executionContext.getFormContext();
    if (formContext.getAttribute("smp_problembuilding").getValue() !== null) {
        var building = formContext.getAttribute("smp_problembuilding");
        if (building.getValue() !== null) {
            var viewId = "{76FBBA25-7FF9-4EBE-A154-1D5B75ED991E}"; //Any Guid is fine.
            var entityName = "smp_problemclass"; // Entity to be filtered
            var viewDisplayName = "Custom Problem Class View"; // Custom name for the lookup window.

            var fetchXml = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'><entity name='smp_problemclass'><attribute name='smp_problemclassid' />" +
                "<attribute name='smp_problemclassname' /><attribute name='createdon' /><order attribute='smp_problemclassname' descending='false' />" +
                "<link-entity name='smp_providermatrix' from='smp_problemclassid' to='smp_problemclassid' alias='ac'>" +
                "<filter type='and'>" +
                "<condition attribute='smp_buildingid' operator='eq' value='" + building.getValue()[0].id + "' />" +
                "<condition attribute='statecode' value='0' operator='eq'/>" +
                "</filter>" +
                "</link-entity>" +
                "</entity></fetch>";

            var layoutXml = '<grid name="resultset" object="1" jump="smp_problemclassid" select="1" icon="1" preview="1">' +
                '<row name="result" id="smp_problemclassid"><cell name="smp_problemclassname" width="200" /></row></grid>';

            formContext.getControl("smp_problemclassid").addCustomView(viewId, entityName, viewDisplayName, fetchXml, layoutXml, true);
        }
    }
}

//Filter Problem type Lookup based on building - Calling on below scenarioa
//On Page Load
//Requestor Name OnChange
//Problem Type OnChange
//Problem Building OnChange
//Contact Name

/*
 * Filter Problem type Lookup based on building
 * @param {any} executionContext: Execution Context
 */
function FilteredProblemTypeLookupFromBuilding(executionContext) {
    if (onChangeStatus === true) {
        return;
    }
    var formContext = executionContext.getFormContext();
    if (formContext.getAttribute("smp_problembuilding").getValue() !== null) {
        var building = formContext.getAttribute("smp_problembuilding");
        if (building.getValue() !== null) {
            var viewId = "{641AC93E-F9E7-45B3-AFCC-B7F23219793B}";
            var entityName = "smp_problemtype"; // Entity to be filtered
            var viewDisplayName = "Custom Problem Type View"; // Custom name for the lookup window.

            var fetchXml = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'><entity name='smp_problemtype'><attribute name='smp_problemtypeid' />" +
                "<attribute name='smp_problemtypename' /><attribute name='createdon' /><order attribute='smp_problemtypename' descending='false' />" +
                "<link-entity name='smp_providermatrix' from='smp_problemtypeid' to='smp_problemtypeid' alias='ac'>" +
                "<filter type='and'>" +
                "<condition attribute='smp_buildingid' operator='eq' value='" + building.getValue()[0].id + "' />" +
                "<condition attribute='statecode' value='0' operator='eq'/>" +
                "</filter>" +
                "</link-entity>" +
                "</entity></fetch>";

            var layoutXml = '<grid name="resultset" object="1" jump="smp_problemtypeid" select="1" icon="1" preview="1">' +
                '<row name="result" id="smp_problemtypeid"><cell name="smp_problemtypename" width="200" /></row></grid>';

            formContext.getControl("smp_problemtypeid").addCustomView(viewId, entityName, viewDisplayName, fetchXml, layoutXml, true);
        }
    }
}

/* User Story 3740189: To lock fields for all the role except system administartor */
function LockFields(executionContext) {
    var formContext = executionContext.getFormContext();
    var hasRole = getUserRoles(executionContext);
    if (hasRole === false) {
        formContext.ui.controls.get("smp_duedate").setDisabled(true);
        formContext.ui.controls.get("smp_problemoccureddatetime").setDisabled(true);
    }
    else {
        formContext.ui.controls.get("smp_duedate").setDisabled(false);
        formContext.ui.controls.get("smp_problemoccureddatetime").setDisabled(false);
    }
}


/*Completed Page Load Java script functions*/
/*Start of Page OnSave Java script functions*/

/*
 * function to handle set escalation
 * @param {any} executionContext: Execution Context
 */
function SetEscalationPopUp(executionContext) {
    onChangeStatus = true;
    var formContext = executionContext.getFormContext();
    if (!formContext.getAttribute("smp_ispopupshown").getValue() && formContext.getAttribute("statuscode").getValue() === 2) {
        var building = formContext.getAttribute("smp_problembuilding");
        var problemclass = formContext.getAttribute("smp_problemclassid");
        var problemtype = formContext.getAttribute("smp_problemtypeid");
        var roomtype = formContext.getAttribute("smp_problemroomtype");
        var isFinancialStateCampus = false;
        var zone;
        var priorityName;
        var Odata = null, results = null;
        var highPriorities;
        var isHighPriority = false;
        if (building !== null && problemclass !== null && problemtype !== null && roomtype !== null) {
            var buildinglookup = building.getValue();
            var problemclasslookup = problemclass.getValue();
            var problemtypelookup = problemtype.getValue();
            var roomtypelookup = roomtype.getValue();

            if (buildinglookup !== null && buildinglookup !== undefined) {
                Odata = "/api/data/v9.0/smp_buildings(" + buildinglookup[0].id.replace('{', "").replace('}', "") + ")?$select=smp_addressline2,smp_isfinancialstatecampus";
                results = RetrieveODataResults(formContext, Odata);
                if (results !== null && results !== undefined) {
                    if (results.smp_isfinancialstatecampus !== null) {
                        isFinancialStateCampus = results.smp_isfinancialstatecampus;
                    }
                    else {
                        isFinancialStateCampus = false;
                    }
                }
            }

            if (roomtypelookup !== null && roomtypelookup !== undefined) {
                Odata = "/api/data/v9.0/smp_roomtypes(" + roomtypelookup[0].id.replace('{', "").replace('}', "") + ")?$select=smp_zone";
                results = RetrieveODataResults(formContext, Odata);
                if (results !== null && results !== undefined) {
                    if (results.smp_zone !== null) {
                        zone = results["smp_zone"];
                    }
                }
            }

            if (problemclasslookup !== null && problemtypelookup !== null && isFinancialStateCampus !== null && zone !== null && buildinglookup !== null) {
                Odata = "/api/data/v9.0/smp_slamatrixes?$select=_smp_priorityid_value&$filter=smp_zone eq " + zone + " and  smp_isfinancialstatecampus eq " + isFinancialStateCampus + " and  statuscode eq 1 and  _smp_problemclassid_value eq " + problemclasslookup[0].id.replace('{', "").replace('}', "") + " and  _smp_problemtypeid_value eq " + problemtypelookup[0].id.replace('{', "").replace('}', "") + "";

                results = RetrieveODataResults(formContext, Odata);
                if (results !== null && results !== undefined && results.value !== null && results.value.length >= 1) {
                    priority = results.value[0]["_smp_priorityid_value"];
                    priorityName = results.value[0]["_smp_priorityid_value@OData.Community.Display.V1.FormattedValue"];
                }
                if (priorityName !== null) {
                    Odata = "/api/data/v9.0/smp_configurations?$select=smp_value&$filter=smp_title eq '8/8RoutingPriorities'";
                    results = RetrieveODataResults(formContext, Odata);
                    if (results !== null && results !== undefined && results.value !== null && results.value.length >= 1) {
                        highPriorities = results.value[0]["smp_value"];
                        if (highPriorities !== null && highPriorities !== undefined && highPriorities !== "") {
                            highPriorities = highPriorities.toLowerCase();
                            priorityName = priorityName.toLowerCase();
                            var arrhighPriorities = highPriorities.split(',');
                            if (arrhighPriorities.length >= 0) {
                                arrhighPriorities.forEach(function (highPriority) {
                                    highPriority = highPriority.trim();
                                    if (highPriority === priorityName) {
                                        isHighPriority = true;
                                    }
                                });

                                if (isHighPriority) {
                                    executionContext.getEventArgs().preventDefault();
                                    var url = formContext.context.getClientUrl() + "/WebResources/smp_priorityescalations";
                                    var win = window.open(url, "Escalation Contacts", "toolbar=yes, scrollbars=yes, resizable=no, width=800, height=400");
                                    var timer = setInterval(function () {
                                        if (win.closed) {
                                            clearInterval(timer);
                                            ReloadOnsave(executionContext);
                                        }
                                    }, 500);
                                }
                            }
                        }
                    }
                }
                else {
                    ReloadOnsave(executionContext);
                }
            }
        }
    }
}
/*
 * Cancel Service Request
 * @param {any} executionContext: execution Context
 * @param {any} executionObj: execution object
 */
function CancelSR(executionContext, executionObj) {
    var formContext = executionContext.getFormContext();
    var wod_SaveMode = executionContext.getEventArgs().getSaveMode();
    var PickListControl = formContext.getControl("statuscode");
    PickListControl.addOption({ value: 6, text: "Cancelled" });
    if (wod_SaveMode !== "40") {
        formContext.getAttribute("smp_requestcancelledreason").setRequiredLevel("none");
    }
}

/**
 * ReloadOnsave
 * @param {any} executionContext: execution Context
 */
function ReloadOnsave(executionContext) {
    var formContext = executionContext.getFormContext();
    if (formContext.data.entity.getIsDirty()) {
        formContext.data.entity.save();
    }
    setTimeout(function () {
        // Call the Open Entity Form method and pass through the current entity name and ID after opening the form perform the function you were performing earlier
        var entityFormOptions = {};
        //formContext.data.entity.save();
        entityFormOptions["entityName"] = formContext.data.entity.getEntityName();
        if (formContext.data.entity.getId() !== "" || formContext.data.entity.getId() !== null) {
            entityFormOptions["entityId"] = formContext.data.entity.getId();
        }
        Xrm.Navigation.openForm(entityFormOptions).then(
            function (success) {
                console.log(success);
            },
            function (error) {
                console.log(error);
            });
    }, 4000);
}

// Created By Manoj
/*
 * function to handle save confirmation
 * @param {any} executionContext: executionContext
 */
function OnSaveConfirmaion(executionContext) {
    var formContext = executionContext.getFormContext();
    var status = formContext.getAttribute("statuscode").getText();
    var isDirty = formContext.getAttribute("statuscode").getIsDirty();
    if (status !== null && status !== "undefined") {
        if (status === "Completed" && isDirty) {
            var isConfirmed = confirm("Please add a Note explaining why you are manually completing this SR.");
            if (isConfirmed === false) {
                executionContext.getEventArgs().preventDefault();
            }
        }
    }
}
/*Completed of Page OnSave Java script functions*/
/*Start of Requestor Name OnChange Java script functions*/

//Calling on below scenarioa
//Requestor Name OnChange
//Problem Building OnChange
//Contact Name OnChange

/**
 * ClearBuildingRelatedFields
 * @param {any} executionContext: Execution Context
 */
function ClearBuildingRelatedFields(executionContext) {
    if (onChangeStatus === true) {
        return;
    }
    var formContext = executionContext.getFormContext();
    //set room type, problem type and class to null    
    formContext.data.entity.attributes.get("smp_problemroomtype").setValue(null);
    formContext.data.entity.attributes.get("smp_problemclassid").setValue(null);
    formContext.data.entity.attributes.get("smp_problemtypeid").setValue(null);
}

/*
 * function to handle on change event for requestor
 * @param {any} executionContext: Execution Context
 */
function SelectRequestorDetailsOnChange(executionContext) {
    var formContext = executionContext.getFormContext();
    var requestorLookupObject = formContext.getAttribute("smp_requestorid");
    if (requestorLookupObject !== null) {
        var lookupField = requestorLookupObject.getValue();
        if (lookupField !== null && lookupField !== undefined) {
            var serverUrl = formContext.context.getClientUrl();
            formContext.getAttribute("smp_contact").setValue([{ id: lookupField[0].id, name: lookupField[0].name, entityType: "contact" }]);
            var oDataPath = serverUrl;
            var retrieveReq = new XMLHttpRequest();
            //code change for room number lookup by Mihika 
            var Odata = oDataPath + "/api/data/v9.0/contacts(" + lookupField[0].id.replace('{', "").replace('}', "") + ")?$select=emailaddress1,smp_alias,smp_costcenter,smp_roomno,telephone1,_smp_buildingid_value,_smp_roomnumber_value";
            //end of code change for room number lookup by Mihika 
            retrieveReq.open("GET", Odata, false);
            retrieveReq.setRequestHeader("OData-MaxVersion", "4.0");
            retrieveReq.setRequestHeader("OData-Version", "4.0");
            retrieveReq.setRequestHeader("Accept", "application/json");
            retrieveReq.setRequestHeader("Content-Type", "application/json; charset=utf-8");
            retrieveReq.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
            retrieveReq.onreadystatechange = function () {
                if (this.readyState === 4) {
                    retrieveReq.onreadystatechange = null;
                    if (this.status === 200 || this.status === 204) {
                        SetRequestorDetails(executionContext, retrieveReq);
                    }
                    else {
                        showAlertDialog(this.statusText, "SelectRequestorDetailsOnChange");
                    }
                }
            };
            retrieveReq.send();
        }
    }
}

//Calling on below scenarioa
//Requestor Name OnChange
//Problem Room Number OnChange
//Contact Name OnChange

/*
 * SelectRoomTypeonRoomNumber
 * @param {any} executionContext: Execution Context
 */
function SelectRoomTypeonRoomNumber(executionContext) {
    if (onChangeStatus === true) {
        return;
    }
    var formContext = executionContext.getFormContext();
    var buildingLookupObject = formContext.getAttribute("smp_problembuilding");
    var roomnumberLookupObject = formContext.getAttribute("new_problemroomnumber");
    if (buildingLookupObject !== null && roomnumberLookupObject !== null) {
        var buildinglookupField = buildingLookupObject.getValue();
        var roomnumberlookupField = roomnumberLookupObject.getValue();

        if (buildinglookupField !== null) {
            var buildingId = buildinglookupField[0].id;
        }
        if (roomnumberlookupField !== null) {
            var roomnumberId = roomnumberlookupField[0].id;
            var roomnumbername = roomnumberlookupField[0].name;
        }

        if (buildingId !== null && roomnumberId !== null && roomnumberId !== undefined) {
            if (roomnumbername !== 'Other') {
                formContext.ui.controls.get("smp_problemroom").setVisible(false);
                formContext.getAttribute("smp_problemroom").setRequiredLevel("none");
                formContext.getAttribute("new_problemroomnumber").setRequiredLevel("required");
                var serverUrl = formContext.context.getClientUrl();
                var oDataPath = serverUrl;
                var retrieveReq = new XMLHttpRequest();
                var Odata = oDataPath + "/api/data/v9.0/smp_rooms(" + roomnumberId.replace('{', "").replace('}', "") + ")?$select=_new_roomtype_value";
                retrieveReq.open("GET", Odata, false);
                retrieveReq.setRequestHeader("OData-MaxVersion", "4.0");
                retrieveReq.setRequestHeader("OData-Version", "4.0");
                retrieveReq.setRequestHeader("Accept", "application/json");
                retrieveReq.setRequestHeader("Content-Type", "application/json; charset=utf-8");
                retrieveReq.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
                retrieveReq.onreadystatechange = function () {
                    if (this.readyState === 4) {
                        retrieveReq.onreadystatechange = null;
                        if (this.status === 200 || this.status === 204) {
                            SetRoomTypeonRoomNumber(formContext, this);
                        }
                        else {
                            showAlertDialog(this.statusText, "SelectRoomTypeonRoomNumber");
                        }
                    }
                };
                retrieveReq.send();
            }
            else {
                if (formContext.getAttribute("smp_problemroomtype") !== null) {
                    formContext.getAttribute("smp_problemroomtype").setValue(null);
                }
                formContext.getAttribute("smp_problemroom").setValue(null);
                formContext.ui.controls.get("smp_problemroom").setVisible(true);
                formContext.getAttribute("smp_problemroom").setRequiredLevel("required");
                formContext.getAttribute("new_problemroomnumber").setRequiredLevel("none");
            }
        }
        else {
            formContext.ui.controls.get("smp_problemroom").setVisible(false);
        }
    }
    formContext.getControl("smp_problemclassid").setFocus();
}

/*
 * RequestorTab
 * @param {any} executionContext: Execution Context
 */
function RequestorTab(executionContext) {
    var formContext = executionContext.getFormContext();
    var requestorPhone = null;
    if (formContext.getAttribute("smp_requestorphone").getValue() !== null) {
        requestorPhone = formContext.getAttribute("smp_requestorphone").getValue();
        formContext.getAttribute("smp_requestorphone").setValue(requestorPhone);
    }
    else {
        formContext.getAttribute("smp_requestorphone").setValue(null);
    }
    formContext.getControl("smp_requestorphone").setFocus();
}

/*Completed of Requestor Name OnChange Java script functions*/
/*Start of Requestor Cancelled Reason OnChange Java script functions*/

/**
 * FormSave
 * @param {any} executionContext: Execution Context
 */
function FormSave(executionContext) {
    var formContext = executionContext.getFormContext();
    if (formContext.data.entity.getIsDirty()) {
        formContext.data.entity.save();
    }
}

/*Completed of Requestor Cancelled Reason OnChange Java script functions*/
/*Start of Status OnChange Java script functions*/

/*
 * ReloadServiceRequest
 * @param {any} executionContext: Execution Context
 */
function ReloadServiceRequest(executionContext) {
    var formContext = executionContext.getFormContext();
    var SRStatus = formContext.getAttribute("statuscode").getValue();
    if (SRStatus === Dispatched || SRStatus === Open) {
        if (formContext.data.entity.getIsDirty()) {
            formContext.data.entity.save();
        }
        var IsCSR = false;
        var currentUserRoles = executionContext.getContext().userSettings.securityRoles;
        //Adding User Story : 3949201
        //var csrRoleGuid = GetConfigurationData(executionContext, 'CSR_Role_Guid');
        ReloadIsCSR(executionContext, currentUserRoles);
    }
}

/*Completed of Status OnChange Java script functions*/
/*Start of Provider OnChange Java script functions*/

/*
 * RetrieveEscalationsCallBack
 * @param {any} retrieveReq: Retrieve results
 */
function RetrieveEscalationsCallBack(retrieveReq) {
    if (retrieveReq.readyState === 4 /* complete */) {
        var primaryescaltion = null, primarycontact = null;
        var retrieved = JSON.parse(retrieveReq.responseText);
        for (var i = 0; i < retrieved.value.length; i++) {

            if (retrieved.value[0].smp_primaryproviderid !== null) {
                primaryescaltion = retrieved.value[i].smp_primaryproviderid.accountid;
                primarycontact = retrieved.value[i].smp_primaryproviderid;
                CreateProviderRow(primaryescaltion, primarycontact, "primary provider");
            }

            if (retrieved.value[0].smp_primaryescalationid !== null) {
                primaryescaltion = retrieved.value[i].smp_primaryescalationid.smp_escalationid;
                primarycontact = retrieved.value[i].smp_primaryescalationid;
                CreateRow(primaryescaltion, primarycontact, "primary escalation");
            }

            if (retrieved.value[0].smp_secondaryescalationid !== null) {
                var secondaryescalation = retrieved.value[i].smp_secondaryescalationid.smp_escalationid;
                var secondarycontact = retrieved.value[i].smp_secondaryescalationid;
                CreateRow(secondaryescalation, secondarycontact, "secondary escalation");
            }
        }
    }
}

/*Completed of Provider OnChange Java script functions*/
/*Start of Priority OnChange Java script functions*/

/*
 * function to Set Priority Override reason Mandatory
 * @param {any} executionContext: Execution Context
 */
function SetPriorityOverridereasonMandatory(executionContext) {
    if (onChangeStatus === true) {
        return;
    }
    var formContext = executionContext.getFormContext();
    var isDirty = formContext.getAttribute("smp_priorityid").getIsDirty();
    if (formContext.ui.getFormType() !== 1) {
        if (isDirty) {
            console.log("SetPriorityOverridereasonMandatory3");
        }
    }
}

/*Completed of Priority OnChange Java script functions*/
/*Start of Problem Type OnChange Java script functions*/

/*
 * function to handle on change on problem class
 * @param {any} executionContext: Execution Context
 */
function SelectProblemClass(executionContext) {
    var formContext = executionContext.getFormContext();
    var requestorLookupObject = formContext.getAttribute("smp_problemtypeid");
    if (requestorLookupObject !== null) {
        var lookupField = requestorLookupObject.getValue();
        if (lookupField !== null && lookupField !== undefined) {
            var serverUrl = formContext.context.getClientUrl();
            var oDataPath = serverUrl;
            var retrieveReq = new XMLHttpRequest();
            var Odata = oDataPath + "/api/data/v9.0/smp_problemtypes(" + lookupField[0].id.replace('{', "").replace('}', "") + ")?$select=_smp_problemclass_value";
            retrieveReq.open("GET", Odata, false);
            retrieveReq.setRequestHeader("OData-MaxVersion", "4.0");
            retrieveReq.setRequestHeader("OData-Version", "4.0");
            retrieveReq.setRequestHeader("Accept", "application/json");
            retrieveReq.setRequestHeader("Content-Type", "application/json; charset=utf-8");
            retrieveReq.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
            retrieveReq.onreadystatechange = function () {
                if (this.readyState === 4) {
                    retrieveReq.onreadystatechange = null;
                    if (this.status === 200 || this.status === 204) {
                        SetProblemClass(formContext, this);
                    }
                    else {
                        showAlertDialog(this.statusText, "SelectProblemClass");
                    }
                }
            };
            retrieveReq.send();
        }
    }
}

/**
 * ProblemTypeTab
 * @param {any} executionContext: Execution Context
 */
function ProblemTypeTab(executionContext) {
    var formContext = executionContext.getFormContext();
    formContext.getControl("smp_problemtypedescription").setFocus();
}

/*Completed of Problem Type OnChange Java script functions*/
/*Start of Problem Class OnChange Java script functions*/

/*
 * Clear Problem Class Related Lookup fields
 * @param {any} executionContext: Execution Context
 */
function ProblemClassOnchange(executionContext) {
    var formContext = executionContext.getFormContext();
    //set problem type to null
    formContext.data.entity.attributes.get("smp_problemtypeid").setValue(null);
}

/*
 * RoomTypeSetFocus
 * @param {any} executionContext: Execution Context
 */
function RoomTypeSetFocus(executionContext) {
    if (onChangeStatus === true) {
        return;
    }
    var formContext = executionContext.getFormContext();
    formContext.getControl("smp_problemclassid").setFocus();
}

/*Completed of Problem Class OnChange Java script functions*/
/*Start of Problem Building OnChange Java script functions*/

/*
 * function to handle on change ebvent for building
 @param {any} formContext: Form Context
 */
function SelectBuildingDetailsOnChange(executionContext) {
    if (onChangeStatus === true) {
        return;
    }
    var formContext = executionContext.getFormContext();
    //set room no and room no lookup to null
    if (formContext.ui.getFormType() === 1) {
        SetLookupNull(formContext, 'smp_problemroom');
    }

    var requestorLookupObject = formContext.getAttribute("smp_problembuilding");
    if (requestorLookupObject !== null) {
        var lookupField = requestorLookupObject.getValue();
        if (lookupField !== null && lookupField !== undefined) {
            var serverUrl = formContext.context.getClientUrl();
            var oDataPath = serverUrl;
            var retrieveReq = new XMLHttpRequest();
            var Odata = oDataPath + "/api/data/v9.0/smp_buildings(" + lookupField[0].id.replace('{', "").replace('}', "") + ")?$select=smp_addressline1,smp_addressline2,smp_state,smp_country,smp_city,smp_zipcode,_smp_timezoneid_value";
            retrieveReq.open("GET", Odata, false);
            retrieveReq.setRequestHeader("OData-MaxVersion", "4.0");
            retrieveReq.setRequestHeader("OData-Version", "4.0");
            retrieveReq.setRequestHeader("Accept", "application/json");
            retrieveReq.setRequestHeader("Content-Type", "application/json; charset=utf-8");
            retrieveReq.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
            retrieveReq.onreadystatechange = function () {
                if (this.readyState === 4) {
                    retrieveReq.onreadystatechange = null;
                    if (this.status === 200 || this.status === 204) {
                        SetBuildingDetails(formContext, retrieveReq);
                    }
                    else {
                        showAlertDialog(this.statusText, "SelectBuildingDetailsOnChange");
                    }
                }
            };
            retrieveReq.send();
        }
    }
}

/*
 * ClearRoomNumberonBuildingChange
 * @param {any} executionContext: Execution Context
 */
function ClearRoomNumberonBuildingChange(executionContext) {
    if (onChangeStatus === true) {
        return;
    }
    var formContext = executionContext.getFormContext();
    //set room number to null on change of building
    formContext.data.entity.attributes.get("new_problemroomnumber").setValue(null);
    formContext.data.entity.attributes.get("smp_problemroom").setValue(null);
    formContext.ui.controls.get("smp_problemroom").setVisible(false);
    formContext.getAttribute("smp_problemroom").setRequiredLevel("none");
    formContext.getAttribute("new_problemroomnumber").setRequiredLevel("required");
}

/*Completed of Problem Building OnChange Java script functions*/
/*Start of Contact Name OnChange Java script functions*/

/*
 * function to handle on change event for contact
 * @param {any} executionContext: Execution Context
 */
function SelectContactDetailsOnChange(executionContext) {
    var formContext = executionContext.getFormContext();
    var requestorLookupObject = formContext.getAttribute("smp_contact");
    if (requestorLookupObject !== null) {
        var lookupField = requestorLookupObject.getValue();
        if (lookupField !== null && lookupField !== undefined) {
            var serverUrl = formContext.context.getClientUrl();
            var oDataPath = serverUrl;
            var retrieveReq = new XMLHttpRequest();
            //code change for room number lookup by Mihika 
            var Odata = oDataPath + "/api/data/v9.0/contacts(" + lookupField[0].id.replace('{', "").replace('}', "") + ")?$select=emailaddress1,smp_alias,smp_costcenter,smp_roomno,telephone1,_smp_buildingid_value,_smp_roomnumber_value";
            //end of code change for room number lookup by Mihika 
            retrieveReq.open("GET", Odata, false);
            retrieveReq.setRequestHeader("OData-MaxVersion", "4.0");
            retrieveReq.setRequestHeader("OData-Version", "4.0");
            retrieveReq.setRequestHeader("Accept", "application/json");
            retrieveReq.setRequestHeader("Content-Type", "application/json; charset=utf-8");
            retrieveReq.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
            retrieveReq.onreadystatechange = function () {
                if (this.readyState === 4) {
                    retrieveReq.onreadystatechange = null;
                    if (this.status === 200 || this.status === 204) {
                        SetContactDetails(executionContext, retrieveReq);
                    }
                    else {
                        showAlertDialog(this.statusText, "SelectContactDetailsOnChange");
                    }
                }
            };
            retrieveReq.send();
        }
    }
}

/*
 * ContactTab
 * @param {any} executionContext: Execution Context
 */
function ContactTab(executionContext) {
    var formContext = executionContext.getFormContext();
    var requestorPhone = null;
    if (formContext.getAttribute("smp_contactphone").getValue() !== null) {
        requestorPhone = formContext.getAttribute("smp_contactphone").getValue();
        formContext.getAttribute("smp_contactphone").setValue(requestorPhone);
    }
    else {
        formContext.getAttribute("smp_contactphone").setValue(null);
    }
    formContext.getControl("smp_contactphone").setFocus();
}

/*Completed of Contact Name OnChange Java script functions*/

/* Calling from "Load" Function */
/*
 *  function to handle provider user role
 * @param {any} formContext : Form Context
 * @param {any} retrieveReq: Retrived results
 */
function ProviderUserRole(executionContext, retrieveReq) {
    var formContext = executionContext.getFormContext();
    if (retrieveReq.readyState === 4 /* complete */) {
        var requestResults = JSON.parse(retrieveReq.responseText);
        if (requestResults !== null && requestResults.value.length >= 1) {
            for (var i = 0; i < requestResults.value.length; i++) {
                var role = requestResults.value[i];
                var id = role.roleid;
                var userName = role.name;
                if (userName === "Provider User") {
                    var owner = new Array();
                    owner = formContext.getAttribute("ownerid").getValue();
                    var ownerId = owner[0].id;
                    var userId = formContext.context.getUserId();
                    var ODataPath = formContext.context.getClientUrl() + "/api/data/v9.0/teammemberships?$filter=systemuserid eq " + userId.replace('{', "").replace('}', "") + " and  teamid eq " + ownerId.replace('{', "").replace('}', "") + "";
                    var service = new XMLHttpRequest();
                    if (service !== null) {
                        service.open("GET", ODataPath, false);
                        service.setRequestHeader("OData-MaxVersion", "4.0");
                        service.setRequestHeader("OData-Version", "4.0");
                        service.setRequestHeader("Accept", "application/json");
                        service.setRequestHeader("Content-Type", "application/json; charset=utf-8");
                        service.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
                        service.onreadystatechange = function () {
                            if (this.readyState === 4) {
                                service.onreadystatechange = null;
                                if (this.status === 200 || this.status === 204) {
                                    UserHasTeam(formContext, this);
                                }
                                else {
                                    showAlertDialog(this.statusText, "ProviderUserRole");
                                }
                            }
                        };
                        service.send(null);
                    }
                }
                else {
                    var currentUserRoles = executionContext.getContext().userSettings.securityRoles;
                    for (var j = 0; j < currentUserRoles.length; j++) {
                        var userRole = currentUserRoles[j];
                        if (GuidsAreEqual(userRole, id)) {
                            formContext.ui.controls.get("smp_tier1workcompletiondatebybuildingtimezone").setDisabled(false);
                            formContext.ui.controls.get("smp_providerduedatebybuildingtimezone").setDisabled(false);
                        }
                    }
                }
            }
        }
    }
}

/* Calling from "ProviderUserRole" Function */
/*
 *  function to check whether user has team or not
 * @param {any} formContext: Form Context
 * @param {any} retrieveReq: Retrieve results
 */
function UserHasTeam(formContext, retrieveReq) {
    if (retrieveReq.readyState === 4 /* complete */) {
        var requestResults = JSON.parse(retrieveReq.responseText);
        if (requestResults !== null && requestResults.value.length >= 1) {
            formContext.ui.controls.get("smp_tier1workcompletiondatebybuildingtimezone").setDisabled(false);
            formContext.ui.controls.get("smp_providerduedatebybuildingtimezone").setDisabled(false);
        }
    }
}

/* Calling from "Load" Function */
/*
 * function to set CSR on load
 * @param {any} formContext: Form Context
 */
function SetCSROnLoad(formContext) {
    var mode = formContext.ui.getFormType();
    if (mode !== null && (mode === 1 || mode === 5)) {
        var owner = new Array();
        owner = formContext.getAttribute("ownerid").getValue();
        if (owner === null) {
            formContext.getAttribute("ownerid").setValue([{ id: formContext._globalContext.getUserId(), name: formContext._globalContext.getUserName(), entityType: "systemuser" }]);
            formContext.getAttribute("smp_csr").setValue([{ id: formContext._globalContext.getUserId(), name: formContext._globalContext.getUserName(), entityType: "systemuser" }]);
        }
        if (owner !== null && owner.length > 0) {
            formContext.getAttribute("smp_csr").setValue([{ id: owner[0].id, name: owner[0].name, entityType: "systemuser" }]);
        }
    }
}

/* Calling from "UpdateProvider" Function */
/*
 * function to set default provider
 * @param {any} formContext: Form Context
 * @param {any} retrieveReq: Retrieve results
 */
function SetDefaultProvider(formContext, retrieveReq) {
    if (retrieveReq.readyState === 4 /* complete */) {
        var retrieved = JSON.parse(retrieveReq.responseText);
        if (retrieved.value !== null) {
            var lookupField = new Array();
            var lookupData = new Object();
            lookupData.id = retrieved.value[0].accountid;
            lookupData.name = "Not Assigned";
            lookupData.entityType = 'account';
            lookupField[0] = lookupData;
            formContext.getAttribute("customerid").setValue(lookupField);
            formContext.getAttribute("customerid").setSubmitMode("always");
        }
    }
}

/* Calling from "SelectRequestorDetailsOnChange" Function */
/*
 * function to handle on change event for requestor
 * @param {any} formContext: Form Context
 * @param {any} retrieveReq: Retrieve results
 */
function SetRequestorDetails(executionContext, retrieveReq) {
    var formContext = executionContext.getFormContext();
    var retrieved = JSON.parse(retrieveReq.responseText);
    if (retrieved !== null) {
        if (retrieved._smp_buildingid_value !== null) {
            var buildingId = retrieved["_smp_buildingid_value"];
            var buildingName = retrieved["_smp_buildingid_value@OData.Community.Display.V1.FormattedValue"];
            formContext.getAttribute("smp_problembuilding").setValue([{ id: buildingId, name: buildingName, entityType: "smp_building" }]);
            formContext.getAttribute("smp_buildingid").setValue([{ id: buildingId, name: buildingName, entityType: "smp_building" }]);
            formContext.getAttribute("smp_contactbuilding").setValue([{ id: buildingId, name: buildingName, entityType: "smp_building" }]);
        }
        else {
            SetLookupNull(formContext, 'smp_problembuilding');
            SetLookupNull(formContext, 'smp_buildingid');
            SetLookupNull(formContext, 'smp_contactbuilding');
        }

        //code change for room number lookup by Mihika 
        if (retrieved._smp_roomnumber_value !== null) {
            var roomnumberId = retrieved["_smp_roomnumber_value"];
            var roomnumberName = retrieved["_smp_roomnumber_value@OData.Community.Display.V1.FormattedValue"];
            formContext.getAttribute("new_problemroomnumber").setValue([{ id: roomnumberId, name: roomnumberName, entityType: "smp_room" }]);
            formContext.getAttribute("new_requestorroomnumber").setValue([{ id: roomnumberId, name: roomnumberName, entityType: "smp_room" }]);
            formContext.getAttribute("new_contactroomnumber").setValue([{ id: roomnumberId, name: roomnumberName, entityType: "smp_room" }]);
        }
        else {
            SetLookupNull(formContext, 'new_requestorroomnumber');
            SetLookupNull(formContext, 'new_contactroomnumber');
            SetLookupNull(formContext, 'new_problemroomnumber');
        }
        //end of code change for room number lookup by Mihika 

        if (retrieved.smp_alias !== null) {
            formContext.getAttribute("smp_requestoralias").setValue(retrieved.smp_alias);
            formContext.getAttribute("smp_contactalias").setValue(retrieved.smp_alias);
        }
        else {
            SetLookupNull(formContext, 'smp_requestoralias');
            SetLookupNull(formContext, 'smp_contactalias');
        }

        if (retrieved.emailaddress1 !== null) {
            formContext.getAttribute("smp_requestoremail").setValue(retrieved.emailaddress1);
            formContext.getAttribute("smp_contactemail").setValue(retrieved.emailaddress1);
        }
        else {
            SetLookupNull(formContext, 'smp_requestoremail');
            SetLookupNull(formContext, 'smp_contactemail');
        }

        if (retrieved.smp_costcenter !== null) {
            formContext.getAttribute("smp_costcentercode").setValue(retrieved.smp_costcenter);
        }
        else {
            SetLookupNull(formContext, 'smp_costcentercode');
        }
        SelectBuildingDetailsOnChange(executionContext);

        if (retrieved.smp_roomno !== null) {
            formContext.getAttribute("smp_requestorroomno").setValue(retrieved.smp_roomno);
            formContext.getAttribute("smp_problemroom").setValue(retrieved.smp_roomno);
            formContext.getAttribute("smp_contactroom").setValue(retrieved.smp_roomno);
        }
        else {
            SetLookupNull(formContext, 'smp_requestorroomno');
            SetLookupNull(formContext, 'smp_problemroom');
            SetLookupNull(formContext, 'smp_contactroom');
        }

        if (retrieved.telephone1 !== null) {
            formContext.getAttribute("smp_contactphone").setValue(retrieved.telephone1);
            formContext.getAttribute("smp_requestorphone").setValue(retrieved.telephone1);
        }
        else {
            SetLookupNull(formContext, 'smp_contactphone');
            SetLookupNull(formContext, 'smp_requestorphone');
        }
    }
}

/* Calling from "SelectContactDetailsOnChange" Function */
/*
 * function to handle on change event for contact
 * @param {any} formContext: Form Context
 * @param {any} retrieveReq
 */
function SetContactDetails(executionContext, retrieveReq) {
    var formContext = executionContext.getFormContext();
    var retrieved = JSON.parse(retrieveReq.responseText);
    if (retrieved !== null) {
        if (retrieved._smp_buildingid_value !== null) {
            var buildingId = retrieved["_smp_buildingid_value"];
            var buildingName = retrieved["_smp_buildingid_value@OData.Community.Display.V1.FormattedValue"];
            formContext.getAttribute("smp_contactbuilding").setValue([{ id: buildingId, name: buildingName, entityType: "smp_building" }]);
            formContext.getAttribute("smp_problembuilding").setValue([{ id: buildingId, name: buildingName, entityType: "smp_building" }]);
        }
        else {
            SetLookupNull(formContext, 'smp_contactbuilding');
        }

        //code change for room number lookup by Mihika 
        if (retrieved._smp_roomnumber_value !== null) {
            var roomnumberId = retrieved["_smp_roomnumber_value"];
            var roomnumberName = retrieved["_smp_roomnumber_value@OData.Community.Display.V1.FormattedValue"];
            formContext.getAttribute("new_contactroomnumber").setValue([{ id: roomnumberId, name: roomnumberName, entityType: "smp_room" }]);
            formContext.getAttribute("new_problemroomnumber").setValue([{ id: roomnumberId, name: roomnumberName, entityType: "smp_room" }]);
        }
        else {
            SetLookupNull(formContext, 'new_contactroomnumber');
        }
        //end of code change for room number lookup by Mihika 

        if (retrieved.smp_alias !== null) {
            formContext.getAttribute("smp_contactalias").setValue(retrieved.smp_alias);
        }
        else {
            SetLookupNull(formContext, 'smp_contactalias');
        }

        if (retrieved.emailaddress1 !== null) {
            formContext.getAttribute("smp_contactemail").setValue(retrieved.emailaddress1);
        }
        else {
            SetLookupNull(formContext, 'smp_contactemail');
        }

        if (retrieved.smp_costcenter !== null) {
            formContext.getAttribute("smp_costcentercode").setValue(retrieved.smp_costcenter);
        }
        else {
            SetLookupNull(formContext, 'smp_costcentercode');
        }
        SelectBuildingDetailsOnChange(executionContext);

        if (retrieved.smp_roomno !== null) {
            formContext.getAttribute("smp_contactroom").setValue(retrieved.smp_roomno);
            formContext.getAttribute("smp_problemroom").setValue(retrieved.smp_roomno);
        }
        else {
            SetLookupNull(formContext, 'smp_contactroom');
            SetLookupNull(formContext, 'smp_problemroom');
        }

        if (retrieved.telephone1 !== null) {
            formContext.getAttribute("smp_contactphone").setValue(retrieved.telephone1);
        }
        else {
            SetLookupNull(formContext, 'smp_contactphone');
        }
    }
}

/* Calling from "SelectBuildingDetailsOnChange" Function */
/*
 * function to handle on change ebvent for building
 * @param {any} formContext: Form Context
 * @param {any} retrieveReq: Retrieve results
 */
function SetBuildingDetails(formContext, retrieveReq) {
    if (onChangeStatus === true) {
        return;
    }
    var retrieved = JSON.parse(retrieveReq.responseText);
    if (retrieved !== null) {
        if (retrieved._smp_timezoneid_value !== null) {
            var timezoneId = retrieved["_smp_timezoneid_value"];
            var timezoneName = retrieved["_smp_timezoneid_value@OData.Community.Display.V1.FormattedValue"];
            if (formContext.getAttribute("smp_problembuildingtimezone") !== null) {
                if (formContext.getAttribute("smp_problembuildingtimezone").getValue() !== null) {
                    var previousTimezoneId = formContext.getAttribute("smp_problembuildingtimezone").getValue()[0].id.replace("{", "").replace("}", "");
                    if (previousTimezoneId.toLowerCase() !== timezoneId.toLowerCase()) { formContext.getAttribute("smp_problembuildingtimezone").setValue([{ id: timezoneId, name: timezoneName, entityType: "smp_timezone" }]); }
                    else { return; }
                }
                else if (formContext.ui.getFormType() === 1) { formContext.getAttribute("smp_problembuildingtimezone").setValue([{ id: timezoneId, name: timezoneName, entityType: "smp_timezone" }]); }
            }
        }
        else {
            SetLookupNull(formContext, 'smp_problembuildingtimezone');
        }

        if (retrieved.smp_addressline1 !== null) {
            formContext.getAttribute("smp_problembuildingaddressline1").setValue(retrieved.smp_addressline1);
        }
        else {
            if (formContext.getAttribute("smp_problembuildingaddressline1").getValue() !== null)
                SetLookupNull(formContext, 'smp_problembuildingaddressline1');
        }

        if (retrieved.smp_addressline2 !== null) {
            formContext.getAttribute("smp_problembuildingaddressline2").setValue(retrieved.smp_addressline2);
        }
        else {
            if (formContext.getAttribute("smp_problembuildingaddressline2").getValue() !== null)
                SetLookupNull(formContext, 'smp_problembuildingaddressline2');
        }

        if (retrieved.smp_state !== null) {
            formContext.getAttribute("smp_problembuildingstate").setValue(retrieved.smp_state);
        }
        else {
            if (formContext.getAttribute("smp_problembuildingaddressline2").getValue() !== null)
                SetLookupNull(formContext, 'smp_problembuildingstate');
        }
        if (retrieved.smp_zipcode !== null) {
            formContext.getAttribute("smp_problembuildingzipcode").setValue(retrieved.smp_zipcode);
        }
        else {
            if (formContext.getAttribute("smp_problembuildingzipcode").getValue() !== null)
                SetLookupNull(formContext, 'smp_problembuildingzipcode');
        }

        if (retrieved.smp_country !== null) {
            formContext.getAttribute("smp_problembuildingcountry").setValue(retrieved.smp_country);
        }
        else {
            if (formContext.getAttribute("smp_problembuildingcountry").getValue() !== null)
                SetLookupNull(formContext, 'smp_problembuildingcountry');
        }
        if (retrieved.smp_city !== null) {
            formContext.getAttribute("smp_problembuildingcity").setValue(retrieved.smp_city);
        }
        else {
            if (formContext.getAttribute("smp_problembuildingcity").getValue() !== null)
                SetLookupNull(formContext, 'smp_problembuildingcity');
        }
    }
}

/*
 * function to handle on change on problem type
 * @param {any} formContext: Form Context
 * @param {any} retrieveReq: Retrieve results
 */
function SetProblemClass(formContext, retrieveReq) {
    if (retrieveReq.readyState === 4 /* complete */) {
        var retrieved = JSON.parse(retrieveReq.responseText);
        if (retrieved !== null) {
            var problemClassId = retrieved["_smp_problemclass_value"];
            var problemClassName = retrieved["_smp_problemclass_value@OData.Community.Display.V1.FormattedValue"];
            if (formContext.getAttribute("smp_problemclassid").getValue() !== null) {
                if (formContext.getAttribute("smp_problemclassid").getValue()[0].id.replace('{', "").replace('}', "").toLowerCase() === problemClassId.toLowerCase()) return;
            }
            formContext.getAttribute("smp_problemclassid").setValue([{ id: problemClassId, name: problemClassName, entityType: "smp_problemclass" }]);
        }
    }
}

/* Calling from Multiple Function like "SelectBuildingDetailsOnChange" */
/*
 * function to set look up values null
 * @param {any} formContext: Form Context
 * @param {any} lookupAttribute: lookup Attribute
 */
function SetLookupNull(formContext, lookupAttribute) {
    if (formContext.ui.getFormType() === 1) {
        var lookupObj = formContext.getAttribute(lookupAttribute);
        if (lookupObj !== null) {
            lookupObj.setValue(null);
        }
    }
}

/* Calling from "HideStatusBasedOnSecurityRole" Function */

/*
 * function to check whether user has role
 * @param {any} formContext: Form Context
 * @param {any} retrieveReq: Retrieve results
 */
function UserHasRole(executionContext, retrieveReq) {
    var formContext = executionContext.getFormContext();
    if (retrieveReq.readyState === 4 /* complete */) {
        var retrieved = JSON.parse(retrieveReq.responseText);
        if (retrieved !== null && retrieved.value.length >= 1) {
            var role1Id = retrieved.value[0].roleid;
            var role2Id = null;
            if (retrieved.value[1] !== null) {
                role2Id = retrieved.value[1].roleid;
            }

            //Get Current User Roles
            var currentUserRoles = executionContext.getContext().userSettings.securityRoles;
            var isAdminUser = false;

            //Check whether current user roles has the role passed as argument
            for (var i = 0; i < currentUserRoles.length; i++) {
                var userRole = currentUserRoles[i];
                if (GuidsAreEqual(userRole, role1Id) || GuidsAreEqual(userRole, role2Id)) {
                    return;
                }
                else {
                    isAdminUser = true;
                }
            }

            if (isAdminUser === true) {
                var PickListControl = formContext.getControl("statuscode");
                var SRStatus = formContext.data.entity.attributes.get("statuscode").getValue();
                if (SRStatus === Approved) {
                    PickListControl.removeOption(Declined);
                    PickListControl.removeOption(PendingApproval);
                    PickListControl.removeOption(WaitingForApproval);
                }
                else if (SRStatus === Declined) {
                    PickListControl.removeOption(Approved);
                    PickListControl.removeOption(PendingApproval);
                    PickListControl.removeOption(WaitingForApproval);
                }
                else if (SRStatus === PendingApproval) {
                    PickListControl.removeOption(Approved);
                    PickListControl.removeOption(Declined);
                    PickListControl.removeOption(WaitingForApproval);
                }
                else if (SRStatus === Inprogress) {
                    PickListControl.removeOption(Approved);
                    PickListControl.removeOption(Declined);
                    PickListControl.removeOption(PendingApproval);
                }
                else {
                    PickListControl.removeOption(Approved);
                    PickListControl.removeOption(Declined);
                    PickListControl.removeOption(PendingApproval);
                    PickListControl.removeOption(WaitingForApproval);
                }
            }
        }
    }
}

/* Calling from multiple function */
/*
 * function to check Guids are equal
 * @param {any} guid1
 * @param {any} guid2
 */
function GuidsAreEqual(guid1, guid2) {
    var isEqual = false;
    if (guid1 === null || guid2 === null || guid1 === undefined || guid2 === undefined) {
        isEqual = false;
    }
    else {
        isEqual = guid1.replace(/[{}]/g, "").toLowerCase() === guid2.replace(/[{}]/g, "").toLowerCase();
    }
    return isEqual;
}

/* Calling from "RetrieveEscalationsCallBack" Function */

/*
 * function to create a row
 * @param {any} contactId: contact Id
 * @param {any} contactDetails: contact Details
 * @param {any} contactType: contact Type
 */
function CreateRow(contactId, contactDetails, contactType) {
    var table = document.getElementById("tblContact");
    var row = table.insertRow(table.rows.length);
    row.ondblclick = function () {
        // open a existing form.
        var entityFormOptions = {};
        entityFormOptions["entityName"] = "smp_escalation";
        entityFormOptions["entityId"] = contactId;
        window.parent.Xrm.Navigation.openForm(entityFormOptions).then(
            function (success) {
                console.log(success);
            },
            function (error) {
                console.log(error);
            });
    };
    var tbclType = row.insertCell(0);
    tbclType.innerHTML = contactType;
    tbclType.className = "dataCell";

    var tbclName = row.insertCell(1);
    tbclName.innerHTML = contactDetails.smp_name;
    tbclName.className = "dataCell";

    var tbclPhone = row.insertCell(2);
    tbclPhone.innerHTML = contactDetails.smp_phone;
    tbclPhone.className = "dataCell";

    var tbclOtherPhone = row.insertCell(3);
    tbclOtherPhone.innerHTML = contactDetails.smp_otherphone;
    tbclOtherPhone.className = "dataCell";

    var tbclInformation = row.insertCell(4);
    if (contactDetails.smp_Information !== null) {
        tbclInformation.innerHTML = contactDetails.smp_information;
    }
    else {
        tbclInformation.innerHTML = '';
    }
    tbclInformation.className = "dataCell";
}

/* Calling from "RetrieveEscalationsCallBack" Function */

/*
 * function to create a row
 * @param {any} contactId: contact Id
 * @param {any} contactDetails: contact Details
 * @param {any} contactType: contact Type
 */
function CreateProviderRow(contactId, contactDetails, contactType) {
    var table = document.getElementById("tblContact");
    var row = table.insertRow(table.rows.length);
    row.ondblclick = function () {
        //open existing record
        var entityFormOptions = {};
        entityFormOptions["entityName"] = "account";
        entityFormOptions["entityId"] = contactId;
        window.parent.Xrm.Navigation.openForm(entityFormOptions).then(
            function (success) {
                console.log(success);
            },
            function (error) {
                console.log(error);
            });
    };
    var tbclType = row.insertCell(0);
    tbclType.innerHTML = contactType;
    tbclType.className = "dataCell";

    var tbclName = row.insertCell(1);
    tbclName.innerHTML = contactDetails.name;
    tbclName.className = "dataCell";

    var tbclPhone = row.insertCell(2);
    if (contactDetails.telephone1 !== null) {
        tbclPhone.innerHTML = contactDetails.telephone1;
    }
    else {
        tbclPhone.innerHTML = '';
    }
    tbclPhone.className = "dataCell";

    var tbclOtherPhone = row.insertCell(3);
    tbclOtherPhone.innerHTML = '';
    tbclOtherPhone.className = "dataCell";

    var tbclInformation = row.insertCell(4);
    tbclInformation.innerHTML = '';
    tbclInformation.className = "dataCell";
}

/* Calling from multiple function */

/*
 * function to Retrieve OData Results
 * @param {any} formContext: Form Context
 * @param {any} oDataQueryUrl: oData Url
 */
function RetrieveODataResults(formContext, oDataQueryUrl) {
    var serviceResults;
    // Getting the REST End point service URL
    var url = formContext.context.getClientUrl();
    url += oDataQueryUrl;
    // Get Windows XMLHTTP service request object
    var service = new XMLHttpRequest();
    if (service !== null) {
        try {
            service.open("GET", url, false);
            service.setRequestHeader("OData-MaxVersion", "4.0");
            service.setRequestHeader("OData-Version", "4.0");
            service.setRequestHeader("Accept", "application/json");
            service.setRequestHeader("Content-Type", "application/json; charset=utf-8");
            service.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
            service.onreadystatechange = function () {
                if (this.readyState === 4) {
                    service.onreadystatechange = null;
                    if (this.status === 200 || this.status === 204) {
                        if (service.responseText !== null && service.responseText !== "") {
                            // Get the result from service
                            var requestResults = JSON.parse(service.responseText);
                            if (requestResults !== null) {
                                serviceResults = requestResults;
                            }
                        }
                    }
                    else {
                        showAlertDialog(this.statusText, "RetrieveODataResults");
                        serviceResults = null;
                    }
                }
            };
            service.send(null);
        }
        catch (ex) {
            serviceResults = null;
        }
        return serviceResults;
    }
}

/* Calling from multiple function */

/*
 *  To get client url
 * @param {any} formContext: Form Context
 */
function GetClientUrl(formContext) {
    if (typeof formContext.context === "object") {
        clientUrl = formContext.context.getClientUrl();
    }
    var ServicePath = "/api/data/v9.0/";
    return clientUrl + ServicePath;
}

//Calling from "SelectRoomTypeonRoomNumber" Function

/*
 * SetRoomTypeonRoomNumber
 * @param {any} formContext
 * @param {any} retrieveReq
 */
function SetRoomTypeonRoomNumber(formContext, retrieveReq) {
    if (onChangeStatus === true) {
        return;
    }
    if (retrieveReq.readyState === 4 /* complete */) {
        var retrieved = JSON.parse(retrieveReq.responseText);
        if (retrieved !== null) {
            if (retrieved._new_roomtype_value !== null) {
                var roomtypeId = retrieved["_new_roomtype_value"];
                var roomtypeName = retrieved["_new_roomtype_value@OData.Community.Display.V1.FormattedValue"];
                formContext.getAttribute("smp_problemroomtype").setValue([{ id: roomtypeId, name: roomtypeName, entityType: "smp_roomtype" }]);
            } else {
                SetLookupNull(formContext, 'smp_problemroomtype');
            }
        }
    }
}


/* User Story 3740189: To lock fields for all the role except system administartor */
/*
 * getUserRoles
 * @param {any} formContext
 */
function getUserRoles(executionContext) {
    var formContext = executionContext.getFormContext();
    var roleid = executionContext.getContext().userSettings.securityRoles;
    var hasRole = false;
    for (var i = 0; i < roleid.length; i++) {
        var roleID = roleid[i];
        var rolename = getRoleNamewithApi(formContext, roleID);
        if (rolename !== null && rolename !== undefined) {
            rolename = rolename.replace(/\s+/g, '');
            if (rolename.toUpperCase() === 'SYSTEMADMINISTRATOR') {
                hasRole = true;
            }
        }
    }
    return hasRole;
}

//Calling from "getUserRoles" Function

/* User Story 3740189: To lock fields for all the role except system administartor */
/*
 * getRoleNamewithApi
 * @param {any} formContext
 * @param {any} userRoleId
 */
function getRoleNamewithApi(formContext, userRoleId) {
    var roleName = null;
    var req = new XMLHttpRequest();
    req.open("GET", formContext.context.getClientUrl() + "/api/data/v9.0/roles(" + userRoleId + ")?$select=name", false);
    req.setRequestHeader("OData-MaxVersion", "4.0");
    req.setRequestHeader("OData-Version", "4.0");
    req.setRequestHeader("Accept", "application/json");
    req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
    req.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
    req.onreadystatechange = function () {
        if (this.readyState === 4) {
            req.onreadystatechange = null;
            if (this.status === 200) {
                var result = JSON.parse(this.response);
                roleName = result["name"];

            } else {
                showAlertDialog(this.statusText, "getRoleNamewithApi");
            }
        }

    };
    req.send();
    return roleName;
}

//Calling from Multiple Function

/*
 * Function to show the Alert dialog
 * @param {any} message
 * @param {any} methodName
 */
function showAlertDialog(message, methodName) {
    if (message === null || message === "") {
        message = "Status Text is empty in a method " + methodName;
    }
    var alertStrings = { confirmButtonLabel: "Ok", text: message };
    Xrm.Navigation.openAlertDialog(alertStrings).then(
        function success(result) {
            console.log("Alert dialog closed");
        },
        function (error) {
            console.log(error.message);
        }
    );
}

//End of Dependent Functions

/*
 * Problem Type Lookup filter
 * @param {any} executionContext: Execution Context
 */
function FilteredProblemTypeLookup(executionContext) {
    var formContext = executionContext.getFormContext();
    var problemClass = formContext.getAttribute("smp_problemclassid");
    var problemBuilding = formContext.getAttribute("smp_problembuilding");
    if (problemClass.getValue() !== null && problemBuilding.getValue() !== null) {
        var viewId = "{a76b2c46-c28e-4e5e-9ddf-951b71202c9d}"; //Any Guid is fine.
        var entityName = "smp_problemtype"; // Entity to be filtered
        var viewDisplayName = "Custom Problem Type View"; // Custom name for the lookup window.
        var fetchXml = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'><entity name='smp_problemtype'><attribute name='smp_problemtypeid' />" +
            "<attribute name='smp_problemtypename' /><attribute name='createdon' /><order attribute='smp_problemtypename' descending='false' />" +
            "<link-entity name='smp_providermatrix' from='smp_problemtypeid' to='smp_problemtypeid' alias='ac'>" +
            "<filter type='and'>" +
            "<condition attribute='smp_problemclassid' operator='eq' value='" + problemClass.getValue()[0].id + "' />" +
            "<condition attribute='smp_buildingid' operator='eq' value='" + problemBuilding.getValue()[0].id + "' />" +
            "<condition attribute='statecode' value='0' operator='eq'/>" +
            "</filter>" +
            "</link-entity>" +
            "</entity></fetch>";

        var layoutXml = '<grid name="resultset" object="1" jump="smp_problemtypeid" select="1" icon="1" preview="1">' +
            '<row name="result" id="smp_problemtypeid"><cell name="smp_problemtypename" width="200" /></row></grid>';

        formContext.getControl("smp_problemtypeid").addCustomView(viewId, entityName, viewDisplayName, fetchXml, layoutXml, true);
    }
}

/*
 * Calling from ribbon on click of cancel button
 * @param {any} executionContext: Execution Context
 */
function MandateCancellationReason(executionContext) {
    var formContext = executionContext;
    if (formContext.getAttribute("smp_requestcancelledreason").getValue() === null) {
        formContext.ui.controls.get("smp_requestcancelledreason").setVisible(true);
        formContext.getAttribute("smp_requestcancelledreason").setRequiredLevel("required");
        alert("You must provide value for Request Cancelled Reason.");
        formContext.ui.controls.get("smp_requestcancelledreason").setFocus();
        formContext.ui.refreshRibbon();
    }
    else {
        // calls out of the box cancellation event
        //Mscrm.IncidentCommandBarActions.cancel();
        CrmService.IncidentRibbon.CommandBarActions.cancel();
    }
}

/*
 * function to set controls visible
 * @param {any} executionContext
 * @param {any} field : field name
 * @param {any} dependantField: dependent field name
 * @param {any} trueValue: flag
 */
function SetControlVisible(executionContext, field, dependantField, trueValue) {
    var formContext = executionContext.getFormContext();
    ShowHideSection(formContext);
    RetrieveAndSetCompanyName(executionContext);
    //get control for field
    var control = formContext.ui.controls.get(field);
    if (control !== "undefined" && control !== null) {
        //get control for dependant field
        var dependantControl = formContext.ui.controls.get(dependantField);
        if (dependantControl !== "undefined" && dependantControl !== null) {
            var attribute = dependantControl.getAttribute();
            var depandantFieldValue = attribute.getValue();
            //set visible based on depandant fields value
            if (depandantFieldValue === trueValue) {
                control.setVisible(true);
                formContext.getAttribute(field).setRequiredLevel("required");
            }
            else {
                control.setVisible(false);
                formContext.getAttribute(field).setRequiredLevel("none");
            }
        }
        else if (dependantControl === null) {
            control.setVisible(false);
        }
        else {
            alert('control not found ' + dependantField);
        }
    }
    else {
        alert("control not found " + field);
    }
}

/*
 * function to show and hide the sections
 * @param {any} formContext: Form Context
 */
function ShowHideSection(formContext) {
    var generalTabNumber = 0;
    var sectionNumber = 3;
    var myOptionSet = formContext.data.entity.attributes.get("smp_contacttype");
    if (myOptionSet) {
        var optionSetText = myOptionSet.getText();
        if (optionSetText === "Provider") {
            formContext.ui.tabs.get(generalTabNumber).sections.get(sectionNumber).setVisible(false);
        }
        else {
            formContext.ui.tabs.get(generalTabNumber).sections.get(sectionNumber).setVisible(true);
        }
    }
}

/*
 * function to Retrieve And Set Company Name
 * @param {any} formContext: Form Context
 */
function RetrieveAndSetCompanyName(executionContext) {
    if (arguments.callee.caller === null || typeof formContext !== "function") {
        formContext = executionContext.getFormContext();
    }

    if (formContext.ui.getFormType() === 1) {
        var retrieveReq = new XMLHttpRequest();
        var ODataPath = formContext.context.getClientUrl() + "/api/data/v9.0/smp_companies?$select=smp_companyid,smp_name";
        retrieveReq.open("GET", ODataPath, false);
        retrieveReq.setRequestHeader("OData-MaxVersion", "4.0");
        retrieveReq.setRequestHeader("OData-Version", "4.0");
        retrieveReq.setRequestHeader("Accept", "application/json");
        retrieveReq.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        retrieveReq.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
        retrieveReq.onreadystatechange = function () {
            if (this.readyState === 4) {
                retrieveReq.onreadystatechange = null;
                if (this.status === 200 || this.status === 204) {
                    RetrieveReqCallBack(formContext, this);
                }
                else {
                    showAlertDialog(this.statusText, "RetrieveAndSetCompanyName");
                }
            }
        };
        retrieveReq.send();
    }
}

/*
 * RetrieveReqCallBack
 * @param {any} formContext: Form Context
 * @param {any} retrieveReq: Retrieve results
 */
function RetrieveReqCallBack(formContext, retrieveReq) {
    if (retrieveReq.readyState === 4 /* complete */) {
        var retrieved = JSON.parse(retrieveReq.responseText);
        for (var i = 0; i < retrieved.value.length; i++) {
            var companyName = retrieved.value[i].smp_name;
            if (companyName.toLowerCase() === "microsoft") {
                var companyid = retrieved.results[i].smp_companyid;
                formContext.getAttribute("smp_companyid").setValue([{ id: companyid, name: companyName, entityType: "smp_company" }]);
                formContext.ui.controls.get("smp_companyid").setDisabled(true);
                formContext.getAttribute("smp_companyid").setSubmitMode("always");
                break;
            }
        }
    }
}

/*
 * Company Onchange on Contact Entity
 * @param {any} formContext: Form Context
 */
function CompanyOnchange(executionContext) {
    var formContext = executionContext.getFormContext();
    //set problem type to null
    formContext.data.entity.attributes.get("smp_region").setValue(null);
}

/*
 * Region Onchange on Contact Entity
 * @param {any} formContext: Form Context
 */
function RegionOnchange(executionContext) {
    var formContext = executionContext.getFormContext();
    //set problem type to null
    formContext.data.entity.attributes.get("smp_subregion").setValue(null);
}

/*
 * SubRegion Onchange on Contact Entity
 * @param {any} formContext: Form Context
 */
function SubRegionOnchange(executionContext) {
    var formContext = executionContext.getFormContext();
    //set problem type to null
    formContext.data.entity.attributes.get("smp_country").setValue(null);
}

/*
 * SubRegion Onchange on Contact Entity
 * @param {any} formContext: Form Context
 */
function ContactBuildingOnchange(executionContext) {
    var formContext = executionContext.getFormContext();
    //set problem type to null
    formContext.data.entity.attributes.get("smp_floor").setValue(null);
}

/*
 * Dynamic Problem Type Problem Class Onchange
 * @param {any} formContext: Form Context
 */
function DynamicProblemTypeProblemClassOnchange(executionContext) {
    var formContext = executionContext.getFormContext();
    //set problem type to null
    formContext.data.entity.attributes.get("smp_problemtype").setValue(null);
}

/* To get Request Object */
function GetRequestObject() {
    if (window.XMLHttpRequest) {
        return new window.XMLHttpRequest;
    }
    else {
        try {
            return new ActiveXObject("MSXML2.XMLHTTP.3.0");
        }
        catch (ex) {
            return null;
        }
    }
}

/*
 * Copy Service Request
 * @param {any} executionContext: Execution Context
 */
function CopySR(executionContext) {
    var formContext = executionContext;
    var r = confirm("Do you want to copy and Create new Service Request ?");
    if (r === true) {
        var id = formContext.data.entity.getId();
        var name = formContext.data.entity.getEntityName();
        var logicalName = name + "s";
        var query = logicalName + "(" + id.replace('{', "").replace('}', "") + ")/Microsoft.Dynamics.CRM.smp_CopySR";
        var req = new XMLHttpRequest();
        req.open("POST", GetClientUrl(formContext) + query, false);
        req.setRequestHeader("OData-MaxVersion", "4.0");
        req.setRequestHeader("OData-Version", "4.0");
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
        req.onreadystatechange = function () {
            if (this.readyState === 4) {
                req.onreadystatechange = null;
                if (this.status === 200 || this.status === 204) {
                    var result = JSON.parse(req.responseText);
                    //open existing record.
                    var entityFormOptions = {};
                    entityFormOptions["entityName"] = name;
                    entityFormOptions["entityId"] = result.incidentid;
                    Xrm.Navigation.openForm(entityFormOptions).then(
                        function (success) {
                            console.log(success);
                        },
                        function (error) {
                            console.log(error);
                        });
                }
            }
        };
        req.send();
    }
}

/*
 * ClearRoomNumberonContactForm
 * @param {any} executionContext: Execution Context
 */
function ClearRoomNumberonContactForm(executionContext) {
    var formContext = executionContext.getFormContext();
    //set room number to null on change of building
    formContext.data.entity.attributes.get("smp_roomnumber").setValue(null);
}

/*
 * SelectServiceRequestDetails
 * @param {any} executionContext: Execution Context
 */
function SelectServiceRequestDetails(executionContext) {
    var formContext = executionContext.getFormContext();
    var servicerequestLookupObject = formContext.getAttribute("smp_incidentid");
    if (servicerequestLookupObject !== null) {
        var lookupField = servicerequestLookupObject.getValue();
        if (lookupField !== null && lookupField !== undefined) {
            var serverUrl = formContext.context.getClientUrl();
            var oDataPath = serverUrl;
            var retrieveReq = new XMLHttpRequest();
            var Odata = oDataPath + "/api/data/v9.0/incidents(" + lookupField[0].id.replace('{', "").replace('}', "") + ")?$select=_smp_problembuilding_value, _smp_problemclassid_value, _smp_problemtypeid_value, _customerid_value, _smp_priorityid_value";
            retrieveReq.open("GET", Odata, false);
            retrieveReq.setRequestHeader("OData-MaxVersion", "4.0");
            retrieveReq.setRequestHeader("OData-Version", "4.0");
            retrieveReq.setRequestHeader("Accept", "application/json");
            retrieveReq.setRequestHeader("Content-Type", "application/json; charset=utf-8");
            retrieveReq.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
            retrieveReq.onreadystatechange = function () {
                if (this.readyState === 4) {
                    retrieveReq.onreadystatechange = null;
                    if (this.status === 200 || this.status === 204) {
                        SetServiceRequestDetails(formContext, retrieveReq);
                    }
                    else {
                        showAlertDialog(this.statusText, "SelectServiceRequestDetails");
                    }
                }
            };
            retrieveReq.send();
            if (formContext.data.entity.getIsDirty()) {
                formContext.data.entity.save();
            }
        }
    }
}

/*
 * SetServiceRequestDetails
 * @param {any} formContext
 * @param {any} retrieveReq
 */
function SetServiceRequestDetails(formContext, retrieveReq) {
    var retrieved = JSON.parse(retrieveReq.responseText);
    if (retrieved !== null) {
        if (retrieved._smp_problembuilding_value !== null) {
            var buildingId = retrieved["_smp_problembuilding_value"];
            var buildingName = retrieved["_smp_problembuilding_value@OData.Community.Display.V1.FormattedValue"];
            formContext.getAttribute("new_building").setValue([{ id: buildingId, name: buildingName, entityType: "smp_building" }]);
        }
        else {
            SetLookupNull(formContext, 'new_building');
        }

        if (retrieved._smp_problemclassid_value !== null) {
            var problemClassId = retrieved["_smp_problemclassid_value"];
            var problemClassName = retrieved["_smp_problemclassid_value@OData.Community.Display.V1.FormattedValue"];
            formContext.getAttribute("new_problemclass").setValue([{
                id: problemClassId, name: problemClassName, entityType: "smp_problemclass"
            }]);
        }
        else {
            SetLookupNull(formContext, 'new_problemclass');
        }

        if (retrieved._smp_problemtypeid_value !== null) {
            var problemtypeId = retrieved["_smp_problemtypeid_value"];
            var problemtypeName = retrieved["_smp_problemtypeid_value@OData.Community.Display.V1.FormattedValue"];
            formContext.getAttribute("new_problemtype").setValue([{ id: problemtypeId, name: problemtypeName, entityType: "smp_problemtype" }]);
        }
        else {
            SetLookupNull(formContext, 'new_problemtype');
        }

        if (retrieved._customerid_value !== null) {
            var providerId = retrieved["_customerid_value"];
            var providerName = retrieved["_customerid_value@OData.Community.Display.V1.FormattedValue"];
            formContext.getAttribute("new_provider").setValue([{ id: providerId, name: providerName, entityType: "account" }]);
        }
        else {
            SetLookupNull(formContext, 'new_provider');
        }

        if (retrieved._smp_priorityid_value !== null) {
            var priorityId = retrieved["_smp_priorityid_value"];
            var priorityName = retrieved["_smp_priorityid_value@OData.Community.Display.V1.FormattedValue"];
            formContext.getAttribute("new_priority").setValue([{ id: priorityId, name: priorityName, entityType: "smp_priority" }]);
        }
        else {
            SetLookupNull(formContext, 'new_priority');
        }

        if (formContext.data.entity.getIsDirty()) {
            formContext.data.entity.save();
        }
    }
}

/*
 * TriggerSurvey
 * @param {any} executionContext: Execution Context
 */
function TriggerSurvey(executionContext) {
    var formContext = executionContext.getFormContext();
    formContext.getAttribute("new_triggersurvey").setValue(1);
    if (formContext.data.entity.getIsDirty()) {
        formContext.data.entity.save();
    }
}

/*
 * SaveSurveyResponse
 * @param {any} executionContext
 */
function SaveSurveyResponse(executionContext) {
    var formContext = executionContext.getFormContext();
    if (formContext.data.entity.getIsDirty()) {
        formContext.data.entity.save();
    }
}

/*
 * callRecordStatus
 * @param {any} executionContext
 */
function callRecordStatus(executionContext) {
    var formContext = executionContext.getFormContext();
    var recordGuid = formContext.data.entity.getId();
    setRecordStatus(formContext, "incident", recordGuid, "2", "6");
}

/*
 * setRecordStatus
 * @param {any} formContext
 * @param {any} entitySchemaName
 * @param {any} recordGuid
 * @param {any} stateCode
 * @param {any} statusCode
 */
function setRecordStatus(formContext, entitySchemaName, recordGuid, stateCode, statusCode) {
    // create the request
    var request = "";
    request += "";
    request += "";
    request += "";
    request += "";
    request += "";
    request += "EntityMoniker";
    request += "";
    request += "" + recordGuid + "";
    request += "" + entitySchemaName + "";
    request += "";
    request += "";
    request += "";
    request += "";
    request += "State";
    request += "";
    request += "" + stateCode + "";
    request += "";
    request += "";
    request += "";
    request += "Status";
    request += "";
    request += "" + statusCode + "";
    request += "";
    request += "";
    request += "";
    request += "";
    request += "SetState";
    request += "";
    request += "";
    request += "";
    request += "";
    //send set state request  
    $.ajax({
        type: "POST",
        contentType: "text/xml; charset=utf-8",
        datatype: "xml",
        url: formContext.context.getClientUrl() + "/XRMServices/2011/Organization.svc/web",
        data: request,
        beforeSend: function (XMLHttpRequest) {
            XMLHttpRequest.setRequestHeader("Accept", "application/xml, text/xml, */*");
            XMLHttpRequest.setRequestHeader("SOAPAction",
                "http://schemas.microsoft.com/xrm/2011/Contracts/Services/IOrganizationService/Execute");
        },
        success: function (data, textStatus, XmlHttpRequest) {
            //Add code after changing Status of the record
        },

        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(errorThrown);
        }
    });
}

/*
 * FormExpandOnLoad
 * @param {any} executionContext
 */
function FormExpandOnLoad(executionContext) {
    var formContext = executionContext.getFormContext();
    if (formContext.ui.getFormType() !== 1) {
        formContext.ui.tabs.get("tab_General").setDisplayState("expanded");
    }
}

/*
 * ShowCustomMessage
 * @param {any} executionContext
 */
function ShowCustomMessage(executionContext) {
    var formContext = executionContext.getFormContext();
    var isDirty = formContext.getAttribute("smp_showonportal").getIsDirty();
    var isChecked = formContext.getAttribute("smp_showonportal").getValue();
    if (isDirty) {
        if (isChecked === 1) {
            var confirmStrings = { text: "Are you sure? This will cause the building to be visible on the portal/end users", title: "Confirmation Dialog" };
            Xrm.Navigation.openConfirmDialog(confirmStrings).then(
                function (success) {
                    if (success.confirmed) {
                        if (formContext.data.entity.getIsDirty()) {
                            formContext.data.entity.save();
                        }
                    }
                    else {
                        formContext.getAttribute("smp_showonportal").setValue(0);
                    }
                });
        }
        else { formContext.getAttribute("smp_showonportal").setValue(0); }
    }
}

/*
 * SetLoggedInUser
 * @param {any} executionContext
 */
function SetLoggedInUser(executionContext) {
    var formContext = executionContext.getFormContext();
    if (formContext.getAttribute("customerid").getValue() === null && formContext.ui.getFormType() === 1) {
        var userName = formContext.context.getUserName();
        var req = new XMLHttpRequest();
        req.open("GET", formContext.context.getClientUrl() + "/api/data/v9.0/contacts?$select=contactid&$filter=fullname eq '" + userName + "'", false);
        req.setRequestHeader("OData-MaxVersion", "4.0");
        req.setRequestHeader("OData-Version", "4.0");
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
        req.onreadystatechange = function () {
            if (this.readyState === 4) {
                req.onreadystatechange = null;
                if (this.status === 200 || this.status === 204) {
                    var results = JSON.parse(this.response);
                    for (var i = 0; i < results.value.length; i++) {
                        var contactid = results.value[i]["contactid"];
                        var lookupField = new Array();
                        var lookupData = {};
                        lookupData.id = contactid;
                        lookupData.name = userName;
                        lookupData.entityType = 'contact';
                        lookupField[0] = lookupData;
                        formContext.getAttribute("customerid").setValue(lookupField);
                    }
                } else {
                    showAlertDialog(this.statusText, "SetLoggedInUser");
                }
            }
        };
        req.send();
    }
}

function SRProcessBar(executionContext) {
    var formContext = executionContext.getFormContext();
    var activeProcess = formContext.data.process.getActiveProcess();
    var SRStatus = formContext.getAttribute("statuscode").getValue();
    var formtype = formContext.ui.getFormType();

    //function getStage() {
    //    var activeStage = Xrm.Page.data.process.getActiveStage();
    //    var stageId = activeStage.getId();
    //    var stagename = activeStage.getName();
    //}
    // **getActiveProcess

    // ** getStages
    var StageCollection = activeProcess.getStages();

    StageCollection.forEach(function (stage) {
        //stage index
        var stageId = stage.getId();
        //alert("Stage Id:" + stageId);

        var stageName = stage.getName();
        //alert("Stage Name :" + stageName);

        //stage status
        var stageStatus = stage.getStatus();
        //alert("Stage Status: " + stageStatus);

        if (SRStatus === Draft && formtype === 1) {
            if (stage.getName().toString().toLowerCase() === "draft") {
                formContext.data.process.setActiveStage(stage.getId());
            }
            else if (stage.getName().toString().toLowerCase() === "open" || stage.getName().toString().toLowerCase() === "dispactched") {
                //stage.setVisible(true);
            }
            else {
                //stage.setVisible(false);
            }
        }
    });
}

//show or hide the businss proces 
//5					Closed
//1					Draft
//180, 620, 002		Dispatched
//180, 620, 017		On Hold Pending Customer/Scheduling
//180, 620, 007		On Hold Pending Parts
//180, 620, 008		On Hold Pending Vendor Arrival
//180, 620, 009		On Hold Pending Vendor Scheduling
//2					Open
//180, 620, 011     Completed
function ToShowOrHideServiceRequestProcess(executionContext) {
    var formContext = executionContext.getFormContext();
    formContext.getControl("header_process_smp_servicerequestpriority").setVisible(false);
    var status = formContext.getAttribute("statuscode").getValue();
    if (status === Closed || status === Draft || status === Dispatched || status === OnHoldPendingCustomerScheduling || status === OnHoldpendinparts || status === OnHoldpendingvendorArrival || status === OnHoldpendingvendorScheduling || status === Open || status === Completed) {
        formContext.ui.process.setVisible(true);
    }
    else {
        formContext.ui.process.setVisible(false);
    }
}

function GetConfigurationData(executionContext, ConfigTitle) {
    var configValue;
    var formContext = executionContext.getFormContext();
    Odata = "/api/data/v9.0/smp_configurations?$select=smp_value&$filter=smp_title eq '" + ConfigTitle + "'";
    var results = RetrieveODataResults(formContext, Odata);
    if (results !== null && results !== undefined && results.value !== null && results.value.length >= 1) {
        configValue = results.value[0]["smp_value"];
        return configValue;
    }
}

function SetPriorityID(executionContext, currentUserRoles) {
    var formContext = executionContext.getFormContext();
    Xrm.WebApi.retrieveMultipleRecords("role", "?$select=name&$filter=name eq 'Provider User'").then(
        function success(result) {
            CurrentUserRolesLoop: // Current User Roles
            for (var i = 0; i < currentUserRoles.length; i++) {
                var userRole = currentUserRoles[i];
                //Condition Changed User Story : 3949201
                for (var j = 0; j < result.entities.length; j++) {
                    var item = result.entities[j];
                    if (GuidsAreEqual(item.roleid, userRole)) {
                        formContext.ui.controls.get("smp_priorityid").setDisabled(true);
                        console.log("Setting Priority As True. " + item.roleid);
                        break CurrentUserRolesLoop;
                    }
                    else {
                        console.log("Setting Priority As False. " + item.roleid);
                        formContext.ui.controls.get("smp_priorityid").setDisabled(false);
                    }
                }
                //Commented below line of code User Story : 3949201
                //IsProviderUser(formContext, userRole);
            }
        },
        function (error) {
            console.log(error.message);
        }
    );
}

function ReloadIsCSR(executionContext, currentUserRoles) {
    var formContext = executionContext.getFormContext();
    var IsCSR = false;
    Xrm.WebApi.retrieveMultipleRecords("role", "?$select=name&$filter=name eq 'CSR'").then(
        function success(result) {
            CurrentUserRolesLoop: // Current User Roles
            for (var i = 0; i < currentUserRoles.length; i++) {
                var userRoleId = currentUserRoles[i];
                for (var j = 0; j < result.entities.length; j++) {
                    var item = result.entities[j];
                    if (GuidsAreEqual(userRoleId, item.roleid)) {
                        IsCSR = true;
                        break CurrentUserRolesLoop;
                    }
                    else {
                        IsCSR = false;
                    }
                }
            }
            if (IsCSR) {
                setTimeout(function () {
                    //    Call the Open Entity Form method and pass through the current entity name and ID after opening the form perform the function you were performing earlier
                    var id = formContext.data.entity.getId();
                    //Open an entity form for existing record
                    var entityFormOptions = {};
                    entityFormOptions["entityName"] = "incident";
                    entityFormOptions["entityId"] = id;
                    Xrm.Navigation.openForm(entityFormOptions).then(
                        function (success) {
                            console.log(success);
                        },
                        function (error) {
                            console.log(error);
                        });
                }, 5000);
            }
        },
        function (error) {
            console.log(error.message);
        }
    );
}

function StatusOptionsIsCSR(executionContext, currentUserRoles) {
    var formContext = executionContext.getFormContext();
    var SRStatus = formContext.getAttribute("statuscode").getValue();
    var PickListControl = formContext.getControl("statuscode");
    var IsCSR = false;
    Xrm.WebApi.retrieveMultipleRecords("role", "?$select=name&$filter=name eq 'CSR'").then(
        function success(result) {
            CurrentUserRolesLoop: // Current User Roles
            for (var i = 0; i < currentUserRoles.length; i++) {
                var userRoleId = currentUserRoles[i];
                for (var j = 0; j < result.entities.length; j++) {
                    var item = result.entities[j];
                    if (GuidsAreEqual(userRoleId, item.roleid)) {
                        IsCSR = true;
                        break CurrentUserRolesLoop;
                    }
                    else {
                        IsCSR = false;
                    }
                }
            }
            if (IsCSR) {
                if (SRStatus === null || SRStatus === undefined) {
                    return;
                }
                if (SRStatus === Open) {
                    var openOptionsToRemove = [Acknowledged, Approved, Backordered, Declined, PendingApproval, PendingCSRDispatch, PendingTransmittal, WaitingForApproval, InformationProvider, Closed];
                    openOptionsToRemove.forEach(option => PickListControl.removeOption(option));
                    PickListControl.addOption({ value: Dispatched, text: "Dispatched" });
                }
                else if (SRStatus === Draft) {
                    var draftOptionsToRemove = [Acknowledged, Approved, Backordered, CallBack, Completed, Declined, Dispatched, OnHoldPendingCustomerScheduling, OnHoldpendinparts, OnHoldpendingvendorArrival, OnHoldpendingvendorScheduling, PendingApproval, PendingCSRDispatch, PendingTransmittal, Referred, Revised, WaitingForApproval, InformationProvider, Closed];
                    draftOptionsToRemove.forEach(option => PickListControl.removeOption(option));
                }
                else if (SRStatus === Dispatched) {
                    var dispatchedOptionsToRemove = [Acknowledged, Approved, Backordered, Declined, Open, PendingApproval, PendingCSRDispatch, PendingTransmittal, WaitingForApproval, InformationProvider, Closed];
                    dispatchedOptionsToRemove.forEach(option => PickListControl.removeOption(option));
                }
                else if (SRStatus === PendingCSRDispatch) {
                    var pendingCSRDispatchOptionsToRemove = [Acknowledged, Approved, Backordered, Completed, Declined, Dispatched, Draft, OnHoldPendingCustomerScheduling, OnHoldpendinparts, OnHoldpendingvendorArrival, OnHoldpendingvendorScheduling, PendingApproval, PendingTransmittal, WaitingForApproval, InformationProvider, Closed];
                    pendingCSRDispatchOptionsToRemove.forEach(option => PickListControl.removeOption(option));
                }
                else if (SRStatus === Inprogress) {
                    var inProgressOptionsToRemove = [Acknowledged, Approved, Backordered, Declined, Dispatched, Draft, Open, PendingApproval, PendingCSRDispatch, PendingTransmittal, WaitingForApproval, InformationProvider, Closed];
                    inProgressOptionsToRemove.forEach(option => PickListControl.removeOption(option));
                }
                else if (SRStatus === PendingApproval) {
                    formContext.data.refresh(true);
                    var pendingApprovalOptionsToRemove = [Acknowledged, Approved, Backordered, Declined, Dispatched, PendingApproval, PendingCSRDispatch, PendingTransmittal, WaitingforApproval, InformationProvider, Closed];
                    pendingApprovalOptionsToRemove.forEach(option => PickListControl.removeOption(option));
                }
                else if (SRStatus === CallBack || SRStatus === Revised) {
                    //adding dispatched option for status reasons revised = 180620006, call back ===180620001
                    var callBackAndRevisedOptionsToRemove = [Acknowledged, Approved, Backordered, Declined, PendingApproval, PendingCSRDispatch, PendingTransmittal, WaitingForApproval, InformationProvider, Closed];
                    callBackAndRevisedOptionsToRemove.forEach(option => PickListControl.removeOption(option));
                    PickListControl.addOption({ value: Dispatched, text: "Dispatched" });
                }
                else if (SRStatus === Acknowledged) {
                    PickListControl.addOption({ value: Acknowledged, text: "Acknowledged" });
                    var acknowledgedOptionsToRemove = [Approved, Backordered, Declined, Dispatched, PendingApproval, PendingCSRDispatch, PendingTransmittal, WaitingForApproval, InformationProvider, Closed];
                    acknowledgedOptionsToRemove.forEach(option => PickListControl.removeOption(option));
                }
                else {
                    var generalOptionsToRemove = [Acknowledged, Approved, Backordered, Declined, Dispatched, PendingApproval, PendingCSRDispatch, PendingTransmittal, WaitingForApproval, InformationProvider, Closed];
                    generalOptionsToRemove.forEach(option => PickListControl.removeOption(option));
                }
            }
        },
        function (error) {
            console.log(error.message);
        }
    );
}

function StatusOptionsIsProviderUser(executionContext, currentUserRoles) {
    var formContext = executionContext.getFormContext();
    var SRStatus = formContext.getAttribute("statuscode").getValue();
    var PickListControl = formContext.getControl("statuscode");
    var IsProviderUser = false;
    Xrm.WebApi.retrieveMultipleRecords("role", "?$select=name&$filter=name eq 'Provider User'").then(
        function success(result) {
            CurrentUserRolesLoop: // Current User Roles
            for (var i = 0; i < currentUserRoles.length; i++) {
                var userRoleId = currentUserRoles[i];
                for (var j = 0; j < result.entities.length; j++) {
                    var item = result.entities[j];
                    if (GuidsAreEqual(userRoleId, item.roleid)) {
                        IsProviderUser = true;
                        break CurrentUserRolesLoop;
                    }
                    else {
                        IsProviderUser = false;
                    }
                }
            }
            if (IsProviderUser) {
                var status = {
                    '4': 'Approved',
                    '180620018': 'Backordered',
                    '180620001': 'Call Back',
                    '180620000': 'Declined',
                    '180620002': 'Dispatched',
                    '1': 'Draft',
                    '180620017': 'On Hold Pending Customer/Scheduling',
                    '2': 'Open',
                    '3': 'Pending Approval',
                    '180620012': 'Pending CSR Dispatch',
                    '180620003': 'Pending Transmittal',
                    '180620010': 'Referred',
                    '180620006': 'Revised',
                    '180620013': 'Waiting For Approval'
                };
                var generalOptionsToRemove = [Approved, Backordered, CallBack, Declined, Dispatched, Draft, OnHoldPendingCustomerScheduling, Open, PendingApproval, PendingCSRDispatch, PendingTransmittal, Referred, Revised, WaitingForApproval];
                generalOptionsToRemove.forEach(option => PickListControl.removeOption(option));
                if (status[SRStatus] !== null) {
                    PickListControl.addOption({ value: SRStatus, text: status[SRStatus] });
                }
            }
        },
        function (error) {
            console.log(error.message);
        }
    );
}