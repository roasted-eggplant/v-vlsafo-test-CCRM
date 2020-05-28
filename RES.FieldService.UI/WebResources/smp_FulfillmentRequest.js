////this function is called on change of Inventory in FulFillment Request Form to fill the Product and Warehouse values.
function OnchangeofInventory(executionContext) {
    var formContext = executionContext.getFormContext();
    var selectedinventory = formContext.getAttribute("smp_inventory").getValue();
    if (selectedinventory !== null) {
        var inventoryid = selectedinventory[0].id.replace("{", "");
        inventoryid = inventoryid.replace("}", "");
        var results = GetRecordData("smp_inventories", inventoryid, "_smp_product_value,_smp_warehouse_value", formContext);
        if (results !== null && results._smp_product_value !== null) {
            var productresults = GetRecordData("products", results._smp_product_value, "name", formContext);
            if (productresults !== null) {
                var product = new Array();
                product[0] = new Object();
                product[0].id = productresults.productid;
                product[0].name = productresults.name;
                product[0].entityType = "product";
                formContext.getAttribute("smp_product").setValue(product);
            }
        }
        if (results !== null && results._smp_warehouse_value !== null) {
            var warehouseresults = GetRecordData("msdyn_warehouses", results._smp_warehouse_value, "msdyn_name", formContext);
            if (warehouseresults !== null) {
                var warehouse = new Array();
                warehouse[0] = new Object();
                warehouse[0].id = warehouseresults.msdyn_warehouseid;
                warehouse[0].name = warehouseresults.msdyn_name;
                warehouse[0].entityType = "msdyn_warehouse";
                formContext.getAttribute("smp_warehouse").setValue(warehouse);
            }
        }
    }
    else {
        formContext.getAttribute("smp_product").setValue(null);
        formContext.getAttribute("smp_warehouse").setValue(null);
    }
}

////this function is used to get the Record Data.
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

////this function is called on Load Fulfillment Request to filter the Inventory based on ItemGroup.
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
