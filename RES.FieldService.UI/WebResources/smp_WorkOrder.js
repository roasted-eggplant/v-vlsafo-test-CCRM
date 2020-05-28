////this function is called on Work Order Form Onload to register the Onchange Events for Work Order Type and Incident Type.
////this function is used to show or hide problem Room number and Room
function WOOnLoad(executionContext) {
    var formContext = executionContext.getFormContext();
    var activeProcess = formContext.data.process.getActiveProcess();
    if (activeProcess !== null) {
        HideBPFForWebClient();
    }
    if (formContext.getAttribute("smp_room").getValue() !== null) {
        var roomNumberLookupName = formContext.getAttribute("smp_room").getValue()[0].name;
        if (roomNumberLookupName !== null) {
            if (roomNumberLookupName === 'Other') {
                formContext.getControl("smp_problemroom").setVisible(true);
            }
            else {
                formContext.getControl("smp_problemroom").setVisible(false);
            }
        }
    }
    if (formContext.getAttribute("msdyn_agreement").getValue() !== null) {
        formContext.ui.controls.get("msdyn_workordertype").setDisabled(true);
        formContext.ui.controls.get("msdyn_primaryincidenttype").setDisabled(true);
        formContext.ui.controls.get("smp_room").setDisabled(true);
    }

    else {
        formContext.ui.controls.get("msdyn_workordertype").setDisabled(false);
        formContext.ui.controls.get("msdyn_primaryincidenttype").setDisabled(false);
        formContext.ui.controls.get("smp_room").setDisabled(false);
    }
}

////this function is called OnLoad of Work Order to hide the BPF for Web Client
function HideBPFForWebClient() {
    var globalContext = Xrm.Utility.getGlobalContext();
    if (globalContext.getCurrentAppUrl() === globalContext.getClientUrl()) {
        Xrm.Page.ui.process.setVisible(false);
    }
}

////this function is called on Onchange of Work Order Type to show the reclassification reason and make the field is required.
function WOTypeChange(executionContext) {
    var formContext = executionContext.getFormContext();
    if (formContext.ui.getFormType() === 2) {
        formContext.getControl("smp_reclassificationreason").setVisible(true);
        formContext.getAttribute("smp_reclassificationreason").setRequiredLevel("required");
        formContext.getAttribute("msdyn_primaryincidenttype").setValue(null);
    }
}

////this function is called on Onchange of Incident Type to show the reclassification reason and make the field is required.
function IncidentTypeChange(executionContext) {
    var formContext = executionContext.getFormContext();
    if (formContext.ui.getFormType() === 2) {
        formContext.getControl("smp_reclassificationreason").setVisible(true);
        formContext.getAttribute("smp_reclassificationreason").setRequiredLevel("required");
    }
}

////this function is called on Onchange of SubStatusChange to show the targetcompletiondate for Onhold Status.
function SubStatusChange(executionContext) {
    var formContext = executionContext.getFormContext();
    if (formContext.ui.getFormType() === 2) {
        var Substatus = formContext.data.entity.attributes.get("msdyn_substatus").getValue()[0].name;
        Substatus = Substatus.replace(/\s+/g, '');
        if (Substatus === "OnHoldPendingCustomer/Scheduling" || Substatus === "OnHoldPendingParts" ||
            Substatus === "OnHoldPendingVendor" || Substatus === "OnHoldPendingVendorArrival" ||
            Substatus === "OnHoldPendingVendorScheduling") {
            formContext.getControl("smp_targetcompletion").setVisible(true);
        }
    }
}

////this function is used to filter the room lookup based on Building
function FilteredRoomNumberLookupFromBuilding(executionContext) {
    var formContext = executionContext.getFormContext();
    formContext.getControl("smp_room").addPreSearch(function () {
        if (formContext.getAttribute("smp_building").getValue() !== null) {
            var building = formContext.getAttribute("smp_building");
            if (building.getValue() !== null) {
                var fetchquery = "<filter type='or'><condition attribute= 'smp_building' operator= 'eq' value= '" + building.getValue()[0].id + "' /><condition attribute='smp_name' operator='eq' value='Other' /></filter>";
                formContext.getControl("smp_room").addCustomFilter(fetchquery);
            }
        }
    });
}

////this function is used to set room number based on selected room
function SetRoomNumberTextFieldFromRoomNumberLookup(executionContext) {
    var formContext = executionContext.getFormContext();
    if (formContext.ui.getFormType() === 2) {
        if (formContext.getAttribute("smp_room").getValue() !== null) {
            var roomNumberLookupName = formContext.getAttribute("smp_room").getValue()[0].name;
            var roomtype = GetRecordData("smp_rooms", formContext.getAttribute("smp_room").getValue()[0].id.replace("{", "").replace("}", ""), "_new_roomtype_value", formContext);

            if (roomNumberLookupName !== null) {
                if (roomNumberLookupName === 'Other') {
                    formContext.getControl("smp_problemroom").setVisible(true);
                    formContext.getAttribute("smp_problemroom").setValue(null);
                    formContext.getAttribute("smp_roomtype").setRequiredLevel("required");
                    formContext.getAttribute("smp_roomtype").setValue(null);
                }
                else {
                    formContext.getAttribute("smp_problemroom").setValue(roomNumberLookupName);
                    formContext.getAttribute("smp_roomtype").setValue(null);
                    formContext.getControl("smp_problemroom").setVisible(false);
                    if (roomtype !== null) {
                        var roomtypeid = roomtype._new_roomtype_value;
                        var getroomtypedata = GetRecordData("smp_roomtypes", roomtypeid, "smp_roomtype", formContext);
                        var roomtypelookup = new Array();
                        roomtypelookup[0] = new Object();
                        roomtypelookup[0].id = getroomtypedata.smp_roomtypeid;
                        roomtypelookup[0].name = getroomtypedata.smp_roomtype;
                        roomtypelookup[0].entityType = "smp_roomtype";
                        formContext.getAttribute("smp_roomtype").setValue(roomtypelookup);
                    }
                }
            }
        }
    }
}

////this is common method to get the record data.
function GetRecordData(entityset, recordid, selectcolumns, formContext) {
    var results = null;
    var OdataURL = formContext.context.getClientUrl() + "/api/data/v9.0/" + entityset + "(" + recordid + ")?$select=" + selectcolumns;
    var req = new XMLHttpRequest();
    req.open("GET", OdataURL, false);
    req.setRequestHeader("OData-MaxVersion", "4.0");
    req.setRequestHeader("OData-Version", "4.0");
    req.setRequestHeader("Accept", "application/json");
    req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
    req.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
    req.onreadystatechange = function () {
        if (this.readyState === 4) {
            req.onreadystatechange = null;
            if (this.status === 200 || this.status === 204) {
                results = JSON.parse(this.response);
            }
            else {
                Xrm.Utility.alertDialog(this.statusText);
            }
        }
    };
    req.send();
    return results;
}

////This is set Target completed filed time to 11.59 PM
function SetTargetCompleteTime(executionContext) {
    var formContext = executionContext.getFormContext();
    var targetCompletion = formContext.getAttribute("smp_targetcompletion");

    if (targetCompletion.getValue() !== null) {
        var targetCompletionValue = new Date(targetCompletion.getValue().setHours(23, 59, 0));
        targetCompletion.setValue(targetCompletionValue);
    }
}

////This is used to set Pricelist and Currency on the WO form
function Pricelist(executionContext) {
    var formContext = executionContext.getFormContext();
    var buildingLookup = formContext.getAttribute("msdyn_serviceaccount");
    if (buildingLookup != null) {
        var building = buildingLookup.getValue();
        var currencyData = GetRecordData("accounts", building[0].id.replace('{', "").replace('}', ""), "_transactioncurrencyid_value", formContext);
        var currency = new Array();
        currency[0] = new Object();
        currency[0].id = currencyData["_transactioncurrencyid_value"];
        currency[0].name = currencyData["_transactioncurrencyid_value@OData.Community.Display.V1.FormattedValue"];
        currency[0].entityType = currencyData["_transactioncurrencyid_value@Microsoft.Dynamics.CRM.lookuplogicalname"];
        formContext.getAttribute("transactioncurrencyid").setValue(currency);
        var currencyid = currencyData._transactioncurrencyid_value;
        Xrm.WebApi.retrieveMultipleRecords("pricelevel", "?$select=name,pricelevelid&$filter=_transactioncurrencyid_value eq " + currencyid).then(
            function success(results) {
                for (var i = 0; i < results.entities.length; i++) {
                    var priceList = new Array();
                    priceList[i] = new Object();
                    priceList[i].id = results.entities[i].pricelevelid;
                    priceList[i].name = results.entities[i].name;
                    priceList[i].entityType = "pricelevel";
                    formContext.getAttribute("msdyn_pricelist").setValue(priceList);
                }
            },
            function (error) {
                Xrm.Utility.alertDialog(error.message);
            }
        );
    }
}