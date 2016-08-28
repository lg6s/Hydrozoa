// Animations
function toggle_sscreen(t) {
    $(t).css("display", function (t, e) { return "none" === e ? "initial" : "none" });
}

function DisplayRangeValue(t) {
    document.getElementById(t).textContent = $('input[name=SDR]').val();
}

// API Section
function AddPlan(t, e, a) {
    $.ajax({ method: "PUT", url: RequestUrls[1] + "AddPlan", data: { NewPlan: new PlanTemplate(t, e, a) }, complete: function (t) { }, dataType: "json" })
}

function TogglePlan(t) {
    $.ajax({ method: "POST", url: RequestUrls[1] + "TogglePlan", data: { Name: t }, complete: function (t) { }, dataType: "json" })
}

function DeletePlan(t) {
    $.ajax({ method: "DELETE", url: RequestUrls[1].concat("DeletePlan"), data: { Name: t }, complete: function (t) { }, dataType: "json" })
}

function GetAllPlans() {
    $.ajax({ method: "GET", url: RequestUrls[1].concat("GetAllPlans"), data: {}, complete: function (t) { console.log(t), localStorage.setItem("Plans", t), Plans = new PlansData(JSON.parse(t)) }, dataType: "json" })
}

function AddAction(t, e, a, n, o) {
    $.ajax({ method: "PUT", url: RequestUrls[1] + "AddAction", data: { NewAction: new ActionTemplate(t, e, a, n, o) }, complete: function (t) { }, dataType: "json" })
}

function DeleteAction(t, e) {
    $.ajax({ method: "DELETE", url: RequestUrls[1] + "DeleteAction", data: { NewAction: new ActionTemplate(t, e) }, complete: function (t) { }, dataType: "json" })
}

function GetWarnings() {
    $.ajax({ method: "Get", url: RequestUrls[0] + "GetWarnings", data: {}, complete: function (t) { alert("Not Implemented !") }, dataType: "json" })
}

function GetValveStatus() {
    $.ajax({ method: "Get", url: RequestUrls[0] + "GetValveStatus", data: {}, complete: function (t) { alert("Not Implemented !") }, dataType: "json" })
}

function GetSystemStatus() {
    $.ajax({ method: "Get", url: RequestUrls[0] + "GetSystemStatus", data: {}, complete: function (t) { alert("Not Implemented !") }, dataType: "json" })
}

// GP functions
function SetDefaultValues() {
    document.getElementById('SDRD').textContent = 182;
    document.getElementById('DRD').textContent = 182;
}

// Objects
function PlanTemplate(t, e, a) {
    this.Name = t, this.StartCondition = e, this.Duration = a
}

function ActionTemplate(t, e, a, n, o) {
    this.Name = t, this.PlanName = e, this.Duration = a, this.PrimaryCondition = n, this.SecondaryCondition = o
}

function DeleteActionTemplate(t, e) {
    this.Name = t, this.PlanName = e
}

function PlansData(t) {
    this.Created = Date.now(), this.List = t
}

// Events
$(document).ready(function () {
    //localStorage.hasOwnProperty("Plans") ? (Plans = JSON.parse(localStorage.getItem("Plans")), localStorage.clear(), Date.now() - Plans.Created >= 6e4 && GetAllPlans()) : GetAllPlans()
    SetDefaultValues();
});

$(window).unload(function () {
    //localStorage.setItem('Plans', JSON.stringify(Plans));
});

// var
var Plans, System, RequestUrls = ["/../api/SystemInformation/", "/../api/PlanAction/"];