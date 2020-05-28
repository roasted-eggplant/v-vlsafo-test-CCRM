/// <summary>
///  ReClassify ServiceRequest Js
/// </summary>

/*
 * ReClassfymethod
 * @param {any} executionContext
 */
function ReClassfymethod(executionContext) {
    var formContext = executionContext;
    var entityName = Xrm.Page.data.entity.getEntityName();
    var id = Xrm.Page.data.entity.getId();
    var customParameters = encodeURIComponent("entitytypename=" + entityName + "&entityId=" + id);
    var windowOptions = { height: 260, width: 750, toolbar: 0, menubar: 0, location: 0, status: 0, scrollbars: 0, resizable: 1, left: 400, top: 100 };
    Xrm.Navigation.openWebResource("new_ReClassifyServiceRequest", windowOptions, customParameters);
}