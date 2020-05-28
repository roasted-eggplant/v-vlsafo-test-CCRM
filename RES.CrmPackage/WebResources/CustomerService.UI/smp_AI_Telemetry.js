var UserName;
var Alias;
var serverUrl;
var tel_entity;

function Getinfo() {
    try {
        var context;
        var UserID;
        var ODataPath;
        var selectFields = "$select=DomainName,FullName";

        tel_entity = Xrm.Page.data.entity.getEntityName();
        context = Xrm.Page.context;
        serverUrl = context.getClientUrl();
        UserID = context.getUserId();
        ODataPath = serverUrl + "/XRMServices/2011/OrganizationData.svc";

        var retrieveUserReq = new XMLHttpRequest();
        retrieveUserReq.open("GET", ODataPath + "/SystemUserSet(guid'" + UserID + "')?" + selectFields, false);
        retrieveUserReq.setRequestHeader("Accept", "application/json");
        retrieveUserReq.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        retrieveUserReq.onreadystatechange = function () {
            retrieveUserReqCallBack(this);
        };
        retrieveUserReq.send();
    }
    catch (err) {
        //
    }
}

function retrieveUserReqCallBack(retrieveUserReq) {
    if (retrieveUserReq.readyState == 4 /* complete */) {
        if (retrieveUserReq.status == 200) {
            var retrievedUser = JSON.parse(retrieveUserReq.responseText).d;
            if (retrievedUser.FullName != null) {
                UserName = retrievedUser.FullName;
                Alias = retrievedUser.DomainName;
            }
        }
    }
}

function captureTelemetry() {
    try {
        var appInsights = window.appInsights || function (config) {
            function r(config) { t[config] = function () { var i = arguments; t.queue.push(function () { t[config].apply(t, i) }) } } var t = { config: config }, u = document, e = window, o = "script", s = u.createElement(o), i, f; for (s.src = config.url || "//az416426.vo.msecnd.net/scripts/a/ai.0.js", u.getElementsByTagName(o)[0].parentNode.appendChild(s), t.cookie = u.cookie, t.queue = [], i = ["Event", "Exception", "Metric", "PageView", "Trace"]; i.length;) r("track" + i.pop()); return r("setAuthenticatedUserContext"), r("clearAuthenticatedUserContext"), config.disableExceptionTracking || (i = "onerror", r("_" + i), f = e[i], e[i] = function (config, r, u, e, o) { var s = f && f(config, r, u, e, o); return s !== !0 && t["_" + i](config, r, u, e, o), s }), t
        }({
            instrumentationKey: "ed8d3fe0-685a-46e1-b780-e984132e4e91"
        });

        window.appInsights = appInsights;
        appInsights.trackPageView(tel_entity, serverUrl, { User: UserName, DomainName: Alias });
    }
    catch (err) {
        appInsights.trackException(err);
    }
}

Getinfo();
captureTelemetry();
