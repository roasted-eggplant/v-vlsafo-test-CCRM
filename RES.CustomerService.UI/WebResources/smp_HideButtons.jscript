////this function is used to disable Deactivate Button
function DisableDeactivateButton() {
    var enableordisable = getUerRoles();
    return enableordisable;
}
////this function is used to disable Deactivate Button on the form
function DisableDeactivateButtonOnForm() {
    var enableordisable = getUerRoles();
    return enableordisable;
}
////this function is used to identify the user is having refdispatcher role or not
function findREFDispatcher(currentUserRoles, dispatcherid) {
    var returnvalue = true;

    if (currentUserRoles.length === 1 && currentUserRoles[0] === dispatcherid) {
        returnvalue = false;
    }
    else if (currentUserRoles.length === 2) {
        var providerid = "2e2a0e22-8c29-e211-a8ab-00155dc83f23";
        for (var i = 0; i < currentUserRoles.length; i++) {
            if (currentUserRoles[i].toLocaleUpperCase() === dispatcherid) {
                returnvalue = true;
            }
        }
    }
    return returnvalue;
}
////this function is used to get the current user Roles
function getUerRoles() {
    var roleid = Xrm.Page.context.getUserRoles();
    var hasRole = false;
    for (var i = 0; i < roleid.length; i++) {
        var roleID = roleid[i];
        var RoleName = getRoleName(roleID);
        RoleName = RoleName.replace(/\s+/g, '');
        if (RoleName.toLocaleUpperCase() === 'REFPMADMIN' || RoleName.toLocaleUpperCase() === "SYSTEMADMINISTRATOR") {
            hasRole = true;
        }
    }
    return hasRole;
}
////this function is used to get the Role name
function getRoleName(roleID) {
    var serverUrl = Xrm.Page.context.getClientUrl();
    var OdataURL = serverUrl + "/XRMServices/2011/OrganizationData.svc" + "/" + "RoleSet?$filter=RoleId eq guid'" + roleID + "'";
    var roleName = null;

    $.ajax({
        type: "GET",
        async: false,
        contentType: "application/json; charset=utf-8", datatype: "json",
        url: OdataURL,
        beforeSend:
            function (XMLHttpRequest) {
                XMLHttpRequest.setRequestHeader("Accept", "application/json");
            },
        success:
            function (data, textStatus, XmlHttpRequest) {
                var result = data.d;
                roleName = result.results[0].Name;
            },
        error:
            function (XmlHttpRequest, textStatus, errorThrown) {
                // alert('OData Select Failed: ' + odataSelect);
            }
    });
    return roleName;

}
////this function is called from the Ribbon to hide Buttons (save, activate and save and Close and etc) in Work Order Entity except System Admins,REFADMIN and REFSCHEDULEMANAGERS.
////this function is called from the Ribbon to hide Buttons (save, activate and save and Close and etc) in Work Order Entity except System Admins,REFADMIN and REFSCHEDULEMANAGERS.
function hideWoButtons(executionContext) {
    //Modified:User story 4102317: Eng Hygenie - OData changes to Web API call for ajax
    var formContext = executionContext.getFormContext();
    var roleid = formContext.context.getUserRoles();
    var hasRole = false;
    hasRole = GetGuidForAdmins(executionContext, roleid);
    return hasRole;
}
////this function is called from the Ribbon to hide the New Button in Work Order Entity except System Admins.
function hideNewButtononWO(executionContext) {
    //Modified:User story 4102317: Eng Hygenie - OData changes to Web API call for ajax
    var formContext = executionContext.getFormContext();
    var roleid = formContext.context.getUserRoles();
    var hasRole = false;
    hasRole = GetGuidForAdmins(executionContext, roleid);
    return hasRole;
}

function GetGuidForAdmins(executionContext, currentUserRoles) {
    //Added as user story 4102317: Eng Hygenie - OData changes to Web API call for ajax
    var results;
    var formContext = executionContext.getFormContext();
    var OdataURL = formContext.context.getClientUrl() + "/api/data/v9.0/roles?$select=name,roleid&$filter=name eq 'System Administrator' or name eq 'REF Admin' or name eq 'REF Schedule Managers'";

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
            if (this.status === 200) {
                results = JSON.parse(this.response);

            } else {
                Xrm.Utility.alertDialog(this.statusText);
            }
        }
    };
    req.send();

    CurrentUserRolesLoop: // Current User Roles
    for (var i = 0; i < currentUserRoles.length; i++) {
        var userRole = currentUserRoles[i];
        for (var j = 0; j < results.value.length; j++) {
            var item = results.value[j];
            if (GuidsAreEqual(item.roleid, userRole)) {
                console.log("True for :" + item.roleid);
                return true;
            }
            else {
                console.log("False for :" + item.roleid);
            }
        }

    }
    return false;
}