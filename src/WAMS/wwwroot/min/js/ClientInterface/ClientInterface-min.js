function ProcessNPForm(){p=$("input[name=NPN]").val(),p.length<5||null==p?DisplayFormException("NPFI","Der Name muss 5 Zeichen enthalten!"):GetIndexofPlan(p)>-1?DisplayFormException("NPFI","Der Name ist schon vergeben!"):(ApiAddPlan(p,$("input[name=SDR]").val(),$("input[name=DR]").val()),HideFormException("NPFI"))}function ProccessPlanDeletion(n){$("#".concat(n)).remove(),Plans.List.splice(GetIndexofPlan(n),1)}function ProccessPlanToggling(n){document.getElementById(n.concat("-AS")).innerText="A"==document.getElementById(n.concat("-AS")).innerText?"I":"A";var e=GetIndexofPlan(n);Plans.List[e].Active=Plans.List[e].Active!==!0}function GetIndexofPlan(n){for(var e=0;e<Plans.List.length;e++)if(Plans.List[e].Content.Name===n)return e;return-1}function GetIndexofAction(n,e){for(var t=GetIndexofPlan(e),a=0;a<Plans.List[t].Content.Elements.length;a++)if(Plans.List[t].Content.Elements[a].Name==n)return a}function toggle_sscreen(n,e,t){GenerateSettingContent(t),$(n).css("display",function(n,e){return"none"===e?"initial":"none"}),CenterContainer(),null!=e&&HideFormException(e)}function toggle_settings(n){}function DisplayRangeValue(n){p=n.split(" "),document.getElementById(p[0]).textContent=$("input[name="+p[1]+"]").val()}function CenterContainer(){for(var n=0;n<SSBID.length;n++)$(SSBID[n]).css("margin-top",$(SSBID[n]).height()/-2),$(SSBID[n]).css("margin-left",$(SSBID[n]).innerWidth()/-2)}function HideFormException(n){""!=n&&(document.getElementById(n.concat("P")).textContent=""),$("#".concat(n)).css("display","none")}function DisplayFormException(n,e){document.getElementById(n.concat("P")).textContent=e,$("#".concat(n)).css("display","block")}function GeneratePlanElement(n,e){var t=PlanPieces[0];t=t.concat(n),t=t.concat(PlanPieces[1]),t=t.concat(n.concat("-AS")),t=t.concat(PlanPieces[2]),t=t.concat(e).concat(PlanPieces[3]),t=t.concat(n.concat("-D")),t=t.concat(PlanPieces[4]),t=t.concat(n),t=t.concat(PlanPieces[5]),t=t.concat(n),t=t.concat(PlanPieces[6]),t=t.concat(n),t=t.concat(PlanPieces[7]),t=t.concat(n),t=t.concat(PlanPieces[8]),$("#plan-container").append(t)}function GenerateSettingContent(n){var e=Plans.List[GetIndexofPlan(n)].Content;document.getElementById("switch-caption").innerText=e.Name}function ApiAddPlan(n,e,t){$.ajax({method:"PUT",url:RequestUrls[1].concat("AddPlan"),data:{NewPlan:new AddPlanTemplate(n,e,t)},complete:function(a){200!==a.status?alert("An unhandled server Error occurred"):GeneratePlanElement(n,"Na"),Plans.List.push(new PlanWrapper(null,new Plan(n,e,t,null)))},dataType:"json"})}function ApiTogglePlan(n){$.ajax({method:"POST",url:RequestUrls[1].concat("TogglePlan"),data:{Name:n},complete:function(e){200!==e.status?alert("An unhandled server Error occurred"):ProccessPlanToggling(n)},dataType:"json"})}function ApiAlterPlan(n,e,t,a){var o=Plans.List[GetIndexofPlan(n)].Content.Elements;$.ajax({method:"POST",url:RequestUrls[1].concat("AlterPlan"),data:{NewPlan:new AlterPlanTemplate(n,new Plan(e,t,a,o))},complete:function(s){200!==s.status?alert("An unhandled server Error occurred"):ProccessPlanDeletion(n),GeneratePlanElement(e,"Na"),Plans.List.push(new PlanWrapper(null,new Plan(e,t,a,o)))},dataType:"json"})}function ApiDeletePlan(n){$.ajax({method:"DELETE",url:RequestUrls[1].concat("DeletePlan"),data:{Name:n},complete:function(e){200!==e.status?alert("An unhandled server Error occurred"):ProccessPlanDeletion(n)},dataType:"json"})}function ApiGetAllPlans(){$.ajax({method:"GET",url:RequestUrls[1].concat("GetAllPlans"),data:{},complete:function(n){if(200===n.status){Plans=new PlansData(n.responseJSON);for(var e=0;e<Plans.List.length;e++){var t=Plans.List[e].Active===!0?"A":"I";GeneratePlanElement(Plans.List[e].Content.Name,t)}console.log(Plans)}else alert("An unhandled server Error occurred")},contentType:"application/json",dataType:"json"})}function ApiGetPlanStatuses(){$.ajax({method:"GET",url:RequestUrls[1].concat("GetPlanStatuses"),data:{},complete:function(n){if(200===n.status)for(var e=0;e<Plans.List.length;e++)Plans.List[e].Content.Name in n.responseJSON&&(Plans.List[e].Active=n.responseJSON[Plans.List[e].Content.Name]);else alert("An unhandled server Error occurred")},contentType:"application/json",dataType:"json"})}function ApiAddAction(n,e,t,a,o){$.ajax({method:"PUT",url:RequestUrls[1].concat("AddAction"),data:{NewAction:new ActionTemplate(n,e,t,a,o)},complete:function(s){200!==s.status?alert("An unhandled server Error occurred"):Plans.List[GetIndexofPlan(e)].Content.Elements.push(new Action(n,e,t,a,o))},dataType:"json"})}function ApiDeleteAction(n,e){$.ajax({method:"DELETE",url:RequestUrls[1].concat("DeleteAction"),data:{NewAction:new ActionTemplate(n,e)},complete:function(t){200!==t.status?alert("An unhandled server Error occurred"):Plans.List[GetIndexofPlan(e)].Content.Elements.splice(GetIndexofAction(n,e),1)},dataType:"json"})}function ApiGetWarnings(){$.ajax({method:"Get",url:RequestUrls[0].concat("GetWarnings"),data:{},complete:function(n){200===n.status?alert("Not Implemented"):alert("An unhandled server Error occurred")},contentType:"application/json",dataType:"json"})}function ApiGetValveStatus(){$.ajax({method:"Get",url:RequestUrls[0].concat("GetValveStatus"),data:{},complete:function(n){200===n.status?alert("Not Implemented"):alert("An unhandled server Error occurred")},contentType:"application/json",dataType:"json"})}function ApiGetSystemStatus(){$.ajax({method:"Get",url:RequestUrls[0].concat("GetSystemStatus"),data:{},complete:function(n){200===n.status?(document.getElementById("ddate").innerText=n.responseJSON.Date.concat(". Tag des Jahres"),document.getElementById("dvalves").innerText="Das Ventil ist ".concat(n.responseJSON.ValveStatus),document.getElementById("dwarnings").innerText=Object.keys(n.responseJSON.Warnings).length>0?n.responseJSON.Warnings[0]:"Es liegen keine Warnungen vor"):alert("An unhandled server Error occurred")},contentType:"application/json",dataType:"json"})}function Action(n,e,t,a,o){this.Name=n,this.PlanName=e,this.Duration=t,this.PrimaryCondition=a,this.SecondaryCondition=o}function Plan(n,e,t,a){this.Name=n,this.StartCondition=e,this.Duration=t,this.Elements=a}function PlanWrapper(n,e){this.Active=n,this.Content=e}function AddPlanTemplate(n,e,t){this.Name=n,this.StartCondition=e,this.Duration=t}function AlterPlanTemplate(n,e){this.OldPlanName=n,this.NewPlan=e}function ActionTemplate(n,e,t,a,o){this.Name=n,this.PlanName=e,this.Duration=t,this.PrimaryCondition=a,this.SecondaryCondition=o}function DeleteActionTemplate(n,e){this.Name=n,this.PlanName=e}function PlansData(n){this.Created=Date.now(),this.List=n}$(document).ready(function(){ApiGetAllPlans(),ApiGetSystemStatus(),setInterval(function(){ApiGetPlanStatuses(),ApiGetSystemStatus()},6e4)}),$(window).resize(function(){CenterContainer()}),$(window).click(function(n){"ss"==n.target.id.split("_")[0]&&toggle_sscreen("#".concat(n.target.id),"")}),$(window).unload(function(){localStorage.setItem("Plans",JSON.stringify(Plans))});var p,Plans,System,RequestUrls=["/api/SystemInformation/","/api/PlanAction/"],SSBID=["#SSB0","#SSB1","#SSB2"],PlanPieces=['<div class="plan" id="','"><div class="status"><h3 id="','">','</h3></div><div class="description"><p id="','">','</p></div><div class="button" onclick="javascript: ApiTogglePlan(\'',"');\"><img src=\"/images/actions/start-stop.png\" /></div><div class=\"button\" onclick=\"javascript: toggle_sscreen('#ss_settings', '', '",'\');"><img src="/images/actions/settings.png" /></div><div class="button" onclick="javascript: ApiDeletePlan(\'','\');"><img src="/images/actions/delete.png" /></div></div>'];