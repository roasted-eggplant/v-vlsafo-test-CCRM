function hideStatusforDispatcherRole() {
    if (Xrm.Page.ui.getFormType() == 2) {
        var hasCSRRole = getUerRoles();
        if (hasCSRRole == false) {
            Xrm.Page.ui.controls.get("statuscode").setDisabled(true);
        }
        else {
            Xrm.Page.ui.controls.get("statuscode").setDisabled(false);
        }
    }
}
function getUerRoles() {
    var roleid = Xrm.Page.context.getUserRoles();
    var hasRole = false;
    for (var i = 0; i < roleid.length; i++) {
        var roleID = roleid[i];
        var RoleName = getRoleName(roleID);
        RoleName = RoleName.replace(/\s+/g, '');
        if (RoleName.toUpperCase() == 'CSR' || RoleName.toUpperCase() == "SYSTEMADMINISTRATOR" || RoleName.toUpperCase() == "REFADMIN") {
            hasRole = true;
        }
    }
    return hasRole;
}
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
        }
    });
    return roleName;

}