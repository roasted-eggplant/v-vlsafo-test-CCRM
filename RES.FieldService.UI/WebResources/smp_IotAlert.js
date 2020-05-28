var CCRM_REF_Customization_IotAlert = window.CCRM_REF_Customization_IotAlert || {};
(function () {
    /*
     This function calls the async workflow to create service request/work order using IOT Alert Attributes
     */
    this.IOTALERT_PROCESS_INVOKE_MESSAGE =
        "Please wait while we convert IOT ALERT to WORK ORDER, page will refresh after completion.";

    this.invokeConvertToWorkOrderAction = function (primaryControl) {
        let formContext = primaryControl;
        var problemClass = formContext.getAttribute("smp_problemclassid").getValue()[0].id.replace('{', "").replace('}', "");
        var problemType = formContext.getAttribute("smp_problemtypeid").getValue()[0].id.replace('{', "").replace('}', "");
        var problemBuilding = formContext.getAttribute("smp_problembuildingid").getValue()[0].id.replace('{', "").replace('}', "");
        var zone = null;
        var isFinancialStateCampus = null;
        Xrm.WebApi.retrieveMultipleRecords("smp_building", "?$select=smp_isfinancialstatecampus&$filter=smp_buildingid eq " + problemBuilding).then(
            function success(result) {
                if (result !== null && result !== undefined && result.entities !== null && result.entities.length >= 1) {
                    isFinancialStateCampus = result.entities[0].smp_isfinancialstatecampus;
                }
                Xrm.WebApi.retrieveMultipleRecords("smp_roomtype", "?$select=smp_zone&$filter=smp_roomtype eq 'Mech/Elec'").then(
                    function success(result) {
                        if (result !== null && result !== undefined && result.entities !== null && result.entities.length >= 1) {
                            zone = result.entities[0].smp_zone;
                        }
                        if (problemClass !== null && problemType !== null && zone !== null && isFinancialStateCampus !== null) {
                            Xrm.WebApi.retrieveMultipleRecords("smp_slamatrix", "?$select=_smp_priorityid_value&$filter=statuscode eq 1 and _smp_problemtypeid_value eq " + problemType + " and _smp_problemclassid_value eq " + problemClass + " and smp_zone eq " + zone + " and  smp_isfinancialstatecampus eq " + isFinancialStateCampus).then(
                                function success(result) {
                                    if (result !== null && result !== undefined && result.entities !== null && result.entities.length >= 1) {
                                        let globalContext = Xrm.Utility.getGlobalContext();
                                        let parameters = {};
                                        let currentuser = {};
                                        currentuser.systemuserid = globalContext.userSettings.userId.replace(/[{}]/g, "");
                                        currentuser["@odata.type"] = "Microsoft.Dynamics.CRM.systemuser";
                                        parameters.CurrentUser = currentuser;
                                        let id = formContext.data.entity.getId().replace(/[{}]/g, "");
                                        let impersonateUserId = formContext.getAttribute("createdby").getValue()[0].id.replace(/[{}]/g, "");
                                        var message =
                                            formContext.ui.setFormNotification(
                                                CCRM_REF_Customization_IotAlert.IOTALERT_PROCESS_INVOKE_MESSAGE,
                                                "INFO",
                                                formContext.data.entity.getId());
                                        let req = new XMLHttpRequest();
                                        req.open("POST",
                                            Xrm.Page.context.getClientUrl() +
                                            "/api/data/v9.1/msdyn_iotalerts(" +
                                            id +
                                            ")/Microsoft.Dynamics.CRM.smp_ConvertIoTAlerttoWorkOrder",
                                            true);
                                        req.setRequestHeader("OData-MaxVersion", "4.0");
                                        req.setRequestHeader("OData-Version", "4.0");
                                        req.setRequestHeader("Accept", "application/json");
                                        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
                                        req.setRequestHeader("MSCRMCallerID", impersonateUserId);
                                        req.onreadystatechange = function () {
                                            if (this.readyState === 4) {
                                                req.onreadystatechange = null;
                                                if (this.status === 200) {
                                                    var results = JSON.parse(this.response);
                                                    CCRM_REF_Customization_IotAlert.successCallBack(formContext);
                                                } else {
                                                    CCRM_REF_Customization_IotAlert.errorCallBack(this.responseText, formContext);
                                                }
                                            }
                                        };
                                        req.send(JSON.stringify(parameters));
                                        //refreshing ribbon
                                        CCRM_REF_Customization_IotAlert.disableRibbonButton(primaryControl);
                                        formContext.ui.refreshRibbon(false);
                                    }
                                    else {
                                        alert("Priority is not available in SLA Matrix for the given combination");
                                    }
                                },
                                function (error) {
                                    console.log(error.message);
                                }
                            );
                        }
                    },
                    function (error) {
                        console.log(error.message);
                    }
                );
            },
            function (error) {
                console.log(error.message);
            }
        );


    };
    this.disableRibbonButton = function (primaryControl) {
        let formContext = primaryControl;
        if (formContext.ui.clearFormNotification(formContext.data.entity.getId())) {
            formContext.ui.setFormNotification(
                CCRM_REF_Customization_IotAlert.IOTALERT_PROCESS_INVOKE_MESSAGE,
                "INFO",
                formContext.data.entity.getId());
            return false;
        } else {
            return true;
        }
    };
    this.successCallBack = function (formContext) {
        //console.log('refreshing form');
        var recordId = formContext.data.entity.getId();
        formContext.ui.clearFormNotification(recordId);
        Xrm.Utility.openEntityForm("msdyn_iotalert", recordId);
    };
    this.errorCallBack = function (error, formContext) {
        console.log(error);
        var recordId = formContext.data.entity.getId();
        formContext.ui.clearFormNotification(recordId);
        Xrm.Navigation.openErrorDialog({ errorCode: 1234, message: error }).then(
            function (success) {
                console.log(success);
            },
            function (error) {
                console.log(error);
            });
    }

}).call(CCRM_REF_Customization_IotAlert);