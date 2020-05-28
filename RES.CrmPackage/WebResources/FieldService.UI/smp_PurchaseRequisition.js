////this function is called onchange of resource to update the requestor email details.
function OnResourceChange(executionContext) {
    var formContext = executionContext.getFormContext();
    var resource = formContext.getAttribute("msdyn_requestedbyresource").getValue();
    if (resource !== null) {
        var resourceid = resource[0].id.replace("{", "");
        resourceid = resourceid.replace("}", "");
        var userresults = GetRecordData(formContext, "bookableresources", resourceid, "_userid_value");
        if (userresults._userid_value !== null) {
            var emailresults = GetRecordData(formContext, "systemusers", userresults._userid_value, "internalemailaddress");
            var emailaddress = emailresults.internalemailaddress;
            formContext.getAttribute("smp_requestoremail").setValue(emailaddress);
        }
        else {
            formContext.getAttribute("smp_requestoremail").setValue("");
        }
        var wareHouseResults = GetRecordData(formContext, "bookableresources", resourceid, "_msdyn_warehouse_value");
        var resourceWareHouse = wareHouseResults._msdyn_warehouse_value;
        if (resourceWareHouse != null) {
            var currentWareHouse = formContext.getAttribute("msdyn_receivetowarehouse").getValue();
            if (formContext.getAttribute("msdyn_receivetowarehouse").getValue() != null) {
                var CurrentwareHouseId = formContext.getAttribute("msdyn_receivetowarehouse").getValue()[0].id;
                CurrentwareHouseId = CurrentwareHouseId.replace("{", "").replace("}", "");
                if (CurrentwareHouseId.toUpperCase() != resourceWareHouse.toUpperCase()) {
                    OnloadofWarehouse(formContext, resourceWareHouse);
                }
            }
            else {
                OnloadofWarehouse(formContext, resourceWareHouse);

            }
        }
        else {
            formContext.getAttribute("msdyn_receivetowarehouse").setValue("");
        }


    }
}


////this is common function, used to get the record data.
function GetRecordData(formContext, entityset, recordid, selectcolumns) {
    var results = null;
    var OdataURL = formContext.context.getClientUrl() + "/api/data/v9.1/" + entityset + "(" + recordid + ")?$select=" + selectcolumns;
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

////this function is called onload of PR form to set the vendor and warehouse.
function PurchaseRequisitionOnLoad(executionContext) {
    var formContext = executionContext.getFormContext();
    if (formContext.getAttribute("msdyn_vendor").getValue() === null) {
        GetVendor(formContext);
        if (formContext.ui.getFormType() !== 1) {
            formContext.data.entity.save();
        }
    }
    if (formContext.getAttribute("msdyn_requestedbyresource").getValue() === null) {
        var wareHouse = GetWarehousefromResource(formContext);
        if (wareHouse != null) {
            OnloadofWarehouse(formContext, wareHouse);
            if (formContext.ui.getFormType() !== 1) {
                formContext.data.entity.save();
            }
        }
    }
}

////this function is used to get the "Other" Vendor record.
function GetVendor(formContext) {
    var OdataURL = formContext.context.getClientUrl() + "/api/data/v9.1/accounts?$select=accountid,name&$filter=name eq 'Other'";
    var req = new XMLHttpRequest();
    req.open("GET", OdataURL, true);
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
                if (results !== null) {
                    var vendor = new Array();
                    vendor[0] = new Object();
                    vendor[0].id = results.value[0].accountid;
                    vendor[0].name = results.value[0].name;
                    vendor[0].entityType = "account";
                    formContext.getAttribute("msdyn_vendor").setValue(vendor);

                }
            }
            else {
                Xrm.Utility.alertDialog(this.statusText);
            }
        }
    };
    req.send();

}

////this function is used to get the "Main" warehouse record.
function GetWarehousefromResource(formContext) {
    var currentUserId = formContext.context.getUserId();
    currentUserId = currentUserId.replace("{", "").replace("}", "");
    var msdyn_warehouse_value = null;
    var req = new XMLHttpRequest();
    req.open("GET",formContext.context.getClientUrl() + "/api/data/v9.1/bookableresources?$select=bookableresourceid,_msdyn_warehouse_value,name&$filter=statecode eq 0 and _userid_value eq " + currentUserId, false);
    req.setRequestHeader("OData-MaxVersion", "4.0");
    req.setRequestHeader("OData-Version", "4.0");
    req.setRequestHeader("Accept", "application/json");
    req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
    req.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
    req.onreadystatechange = function () {
        if (this.readyState === 4) {
            req.onreadystatechange = null;
            if (this.status === 200) {
                var results = JSON.parse(this.response);
                for (var i = 0; i < results.value.length; i++) {
                    var resource = new Array();
                    resource[0] = new Object();
                    resource[0].id = results.value[i]["bookableresourceid"];
                    resource[0].name = results.value[i]["name"];
                    resource[0].entityType = "bookableresource";
                    formContext.getAttribute("msdyn_requestedbyresource").setValue(resource);
                    var emailresults = GetRecordData(formContext, "systemusers", currentUserId, "internalemailaddress");
                    var emailaddress = emailresults.internalemailaddress;
                    formContext.getAttribute("smp_requestoremail").setValue(emailaddress);
                    msdyn_warehouse_value = results.value[i]["_msdyn_warehouse_value"];
                }
            }
            else {
                Xrm.Utility.alertDialog(this.statusText);
            }
        }
    };
    req.send();

    return msdyn_warehouse_value;
}


////this function is called onchange of system status on PR form to set the current date as PR Date.
function setPRDate(formContext) {
    var systemStatus = formContext.getFormContext().getAttribute("msdyn_systemstatus").getValue();
    if (systemStatus === 690970000 || systemStatus === 690970001) {//Draft or Submitted      
        formContext.getFormContext().getAttribute("msdyn_purchaseorderdate").setValue(new Date());
    }
}
////this function is used to get the "Main" warehouse record.
function OnloadofWarehouse(formContext, wareHouseId) {
    var req = new XMLHttpRequest();
    req.open("GET", formContext.context.getClientUrl() + "/api/data/v9.1/msdyn_warehouses(" + wareHouseId + ")?$select=msdyn_name,msdyn_warehouseid", false);
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
                var warehouses = new Array();
                warehouses[0] = new Object();
                warehouses[0].id = result["msdyn_warehouseid"];
                warehouses[0].name = result["msdyn_name"];
                warehouses[0].entityType = "msdyn_warehouse";
                formContext.getAttribute("msdyn_receivetowarehouse").setValue(warehouses);
            }
            else {
                Xrm.Utility.alertDialog(this.statusText);
            }
        }
    };
    req.send();
}

