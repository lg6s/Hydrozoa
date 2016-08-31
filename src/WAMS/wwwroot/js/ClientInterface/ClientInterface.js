// Events
$(document).ready(function () {
    ApiGetAllPlans()
    setInterval(function () { GetPlanStatuses }, 60000)
});

$(window).resize(function () { CenterContainer(); });

$(window).unload(function () { localStorage.setItem('Plans', JSON.stringify(Plans)); });

// var
var p, Plans, System, RequestUrls = ["/api/SystemInformation/", "/api/PlanAction/"], SSBID = ['#SSB0', '#SSB1', '#SSB2'],
PlanPieces = [
    '<div class="plan" id="', '"><div class="status"><h3 id="', '">', '</h3></div><div class="description"><p id="', '">',
    '</p></div><div class="button" onclick="javascript: ApiTogglePlan(\'',
    '\');"><img src="/images/actions/start-stop.png" /></div><div class="button" onclick="javascript: toggle_sscreen(\'#ss_settings\', \'\', \'',
    '\');"><img src="/images/actions/settings.png" /></div><div class="button" onclick="javascript: ApiDeletePlan(\'',
    '\');"><img src="/images/actions/delete.png" /></div></div>'];

// GP functions
function ProcessNPForm() {
    p = $('input[name=NPN]').val()
    if (p.length < 5 || p == null) { DisplayFormException('NPFI', 'Der Name muss 5 Zeichen enthalten!') }
    else if (GetIndexofPlan(p) > -1) { DisplayFormException('NPFI', 'Der Name ist schon vergeben!') }
    else { ApiAddPlan(p, $('input[name=SDR]').val(), $('input[name=DR]').val()), HideFormException('NPFI') }
}

function ProccessPlanDeletion(t) {
    $(('#').concat(t)).remove()
    Plans.List.splice(GetIndexofPlan(t), 1)
}

function ProccessPlanToggling(t) {
    document.getElementById((t).concat('-AS')).innerText = function () { return document.getElementById((t).concat('-D')).innerText === 'A' ? 'I' : 'A' }
    var i = GetIndexofPlan(t);
    Plans.List[i] = function () { return Plans.List[i] === true ? false : true }
}

function GetIndexofPlan(t) {
    for (var i = 0; i < Plans.List.length; i++) { if (Plans.List[i]['Content']['Name'] === t) { return i } } return -1
}

function GetIndexofAction(t, e){
    var ti = GetIndexofPlan(e)
    for (var i = 0; i < Plans.List[ti]['Content']['Elements'].length; i++){
        if (Plans.List[ti]['Content']['Elements'][i]['Name'] == t){ return i }
    }
}

/* ToDo
    {Design Settings, GenerateSettingContent}
    Design Info
    responsive
*/

// Animations
function toggle_sscreen(t, e, a) {
    GenerateSettingContent(a);
    $(t).css("display", function (b, c) { return "none" === c ? "initial" : "none" })
    CenterContainer()
    if (e != null) { HideFormException(e); }
}

function DisplayRangeValue(t) {
    p = t.split(' ')
    document.getElementById(p[0]).textContent = $('input[name=' + p[1] + ']').val()
}

function CenterContainer() {
    for (var i = 0; i < SSBID.length; i++) {
        $(SSBID[i]).css('margin-top', $(SSBID[i]).height() / -2)
        $(SSBID[i]).css('margin-left', $(SSBID[i]).innerWidth() / -2)
    }
}

function HideFormException(t) {
    document.getElementById(t.concat('P')).textContent = '';
    $(('#').concat(t)).css('display', 'none');
}

function DisplayFormException(t, m) {
    document.getElementById(t.concat('P')).textContent = m;
    $(('#').concat(t)).css('display', 'block');
}

function GeneratePlanElement(t, a) {
    var e = PlanPieces[0];
    e = e.concat(t);
    e = e.concat(PlanPieces[1]);
    e = e.concat(t.concat('-AS'));
    e = e.concat(PlanPieces[2]);
    e = e.concat(a).concat(PlanPieces[3]);
    e = e.concat(t.concat('-D'));
    e = e.concat(PlanPieces[4]);
    e = e.concat(t);
    e = e.concat(PlanPieces[5]);
    e = e.concat(t);
    e = e.concat(PlanPieces[6]);
    e = e.concat(t);
    e = e.concat(PlanPieces[7]);
    e = e.concat(t);
    e = e.concat(PlanPieces[8]);
    $('#plan-container').append(e);
}

function GenerateSettingContent(t) {

}

// API Section
function ApiAddPlan(t, e, a) {
    $.ajax({
        method: "PUT", url: RequestUrls[1].concat("AddPlan"), data: { NewPlan: new PlanTemplate(t, e, a) },
        complete: function (r) { r.status !== 200 ? alert("An unhandled server Error occurred") : GeneratePlanElement(t, 'Na'), Plans.List.push(new PlanWrapper(null, new Plan(t, e, a, null))) },
        dataType: "json"
    })
}

function ApiTogglePlan(t) {
    $.ajax({
        method: "POST", url: RequestUrls[1].concat("TogglePlan"), data: { Name: t },
        complete: function (r) { r.status !== 200 ? alert("An unhandled server Error occurred") : ProccessPlanToggling(t) },
        dataType: "json"
    })
}

function ApiDeletePlan(t) {
    $.ajax({
        method: "DELETE", url: RequestUrls[1].concat("DeletePlan"), data: { Name: t },
        complete: function (t) { t.status !== 200 ? alert("An unhandled server Error occurred") : ProccessPlanDeletion(t) },
        dataType: "json"
    })
}

function ApiGetAllPlans() {
    $.ajax({
        method: "GET", url: RequestUrls[1].concat("GetAllPlans"), data: {},
        complete: function (t) { if (t.status === 200) {
            Plans = new PlansData(t.responseJSON)
            for (var i = 0; i < Plans.List.length; i++) {
                var z = Plans.List[i]['Active'] === true ? 'A' : 'I'
                GeneratePlanElement(Plans.List[i]['Content']['Name'], z)
            } console.log(Plans)
        } else { alert("An unhandled server Error occurred") } },
        contentType: 'application/json', dataType: "json"
    })
}

function GetPlanStatuses() {
    $.ajax({
        method: 'GET', url: RequestUrls[1].concat("GetPlanStatuses"), data: {},
        complete: function (t) {
            if (t.status === 200) {
                for (var i = 0; i < Plans.List.length; i++) {
                    if (Plans.List[i]['Content']['Name'] in t.responseJSON) {
                        Plans.List[i]['Active'] = t.responseJSON[Plans.List[i]['Content']['Name']]
                    }
                }
            } else { alert("An unhandled server Error occurred") } },
        contentType: 'application/json', dataType: "json"
    })
}

function ApiAddAction(t, e, a, n, o) {
    $.ajax({
        method: "PUT", url: RequestUrls[1].concat("AddAction"), data: { NewAction: new ActionTemplate(t, e, a, n, o) },
        complete: function (t) { t.status !== 200 ? alert("An unhandled server Error occurred") : Plans.List[GetIndexofPlan(e)]['Content']['Elements'].push(new Action(t, e, a, n, o)) },
        dataType: "json"
    })
}

function ApiDeleteAction(t, e) {
    $.ajax({
        method: "DELETE", url: RequestUrls[1].concat("DeleteAction"), data: { NewAction: new ActionTemplate(t, e) },
        complete: function (t) { t.status !== 200 ? alert("An unhandled server Error occurred") : Plans.List[GetIndexofPlan(e)]['Content']['Elements'].splice(GetIndexofAction(t, e), 1)},
        dataType: "json"
    })
}

function ApiGetWarnings() {
    $.ajax({
        method: "Get", url: RequestUrls[0].concat("GetWarnings"), data: {},
        complete: function (t) { t.status === 200 ? alert('Not Implemented') : alert("An unhandled server Error occurred") },
        contentType: 'application/json', dataType: "json"
    })
}

function ApiGetValveStatus() {
    $.ajax({
        method: "Get", url: RequestUrls[0].concat("GetValveStatus"), data: {},
        complete: function (t) { t.status === 200 ? alert('Not Implemented') : alert("An unhandled server Error occurred") },
        contentType: 'application/json', dataType: "json"
    })
}

function ApiGetSystemStatus() {
    $.ajax({
        method: "Get", url: RequestUrls[0].concat("GetSystemStatus"), data: {},
        complete: function (t) { t.status === 200 ? alert('Not Implemented') : alert("An unhandled server Error occurred") },
        contentType: 'application/json', dataType: "json"
    })
}

// Objects
function Action(t, e, a, n, o) {
    this.Name = t, this.PlanName = e, this.Duration = a, this.PrimaryCondition = n, this.SecondaryCondition = o;
}

function Plan (t, e, a, n) {
    this.Name = t, this.StartCondition = e, this.Duration = a, this.Elements = n
}

function PlanWrapper(t, e) {
    this.Active = t, this.Content = e
}

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