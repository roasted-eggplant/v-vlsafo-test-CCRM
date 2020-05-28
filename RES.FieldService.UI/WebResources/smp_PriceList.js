//// this function is used to auto populate the US Pricelist
function OnloadofPriceList(executionContext) {
    var formContext = executionContext.getFormContext();
    if (formContext.getAttribute("msdyn_pricelist").getValue() === null) {
        getpricelist(formContext);
    }
}

////this function is used to get the US Pricelist record.
function getpricelist(formContext) {
    var OdataURL = formContext.context.getClientUrl() + "/api/data/v9.0/pricelevels?$select=pricelevelid,name&$filter=name eq 'US'";
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
                    var pricelist = new Array();
                    pricelist[0] = new Object();
                    pricelist[0].id = results.value[0].pricelevelid;
                    pricelist[0].name = results.value[0].name;
                    pricelist[0].entityType = "pricelevel";
                    formContext.getAttribute("msdyn_pricelist").setValue(pricelist);
                }

            }
            else {
                Xrm.Utility.alertDialog(this.statusText);
            }
        }
    };
    req.send();
}

