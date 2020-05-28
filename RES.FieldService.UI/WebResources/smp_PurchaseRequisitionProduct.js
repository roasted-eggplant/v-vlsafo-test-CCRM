////this function is called onchange of inventory to auto populate the product and warehouse.
function OnchangeofInventory(executionContext) {
    var formContext = executionContext.getFormContext();
    var selectedinventory = formContext.getAttribute("smp_inventory").getValue();
    if (selectedinventory !== null) {
        var inventoryid = selectedinventory[0].id.replace("{", "");
        inventoryid = inventoryid.replace("}", "");
        var results = GetRecordData(formContext,"smp_inventories", inventoryid, "_smp_product_value,_smp_warehouse_value");
        if (results !== null && results._smp_product_value !== null) {
            var productresults = GetRecordData(formContext,"products", results._smp_product_value, "name");
            if (productresults !== null) {
                var product = new Array();
                product[0] = new Object();
                product[0].id = productresults.productid;
                product[0].name = productresults.name;
                product[0].entityType = "product";
                formContext.getAttribute("msdyn_product").setValue(product);
                formContext.getAttribute("msdyn_name").setValue(productresults.name);
            }
        }
        if (results !== null && results._smp_warehouse_value !== null) {
            var warehouseresults = GetRecordData(formContext,"msdyn_warehouses", results._smp_warehouse_value, "msdyn_name");
            if (warehouseresults !== null) {
                var warehouse = new Array();
                warehouse[0] = new Object();
                warehouse[0].id = warehouseresults.msdyn_warehouseid;
                warehouse[0].name = warehouseresults.msdyn_name;
                warehouse[0].entityType = "msdyn_warehouse";
                formContext.getAttribute("msdyn_associatetowarehouse").setValue(warehouse);
            }
        }
    }
    else {
        formContext.getAttribute("msdyn_product").setValue(null);
    }
}

////this is common function to get the record data.
function GetRecordData(formContext,entityset, recordid, selectcolumns) {
    var results = null;
      var OdataURL = formContext.context.getClientUrl() + "/api/data/v9.0/"+ entityset + "(" + recordid + ")?$select=" + selectcolumns;
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

////this function is called Onload of PR Product to apply the filter for inventory lookup based on item group.
function OnLoadFilterInventory(executionContext) {
    var formContext = executionContext.getFormContext();
    formContext.getControl("smp_inventory").addPreSearch(function () {
        var itemgroup = formContext.getAttribute("smp_itemgroup").getValue();

        if (itemgroup !== null) {
            var fetchquery = "<filter type='and'><condition attribute= 'smp_itemgroup' operator= 'eq' value= '" + itemgroup + "' /></filter>";
            formContext.getControl("smp_inventory").addCustomFilter(fetchquery);
        }
    });
}
////this function is used to set the unitcost.
function contSetValue(executionContext) {
    var formContext = executionContext.getFormContext();
    var formtype = formContext.ui.getFormType();
    if (formtype == 1) {
        formContext.getAttribute("msdyn_unitcost").setValue(0.00);
    }

}


