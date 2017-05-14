tasks:([`u#tiseq:`symbol$()]actn:`int$();per:`long$();obs:`long$();loc:`symbol$());
/ tiseq -> task identification sequence
/ actn -> action to perform (1: open valve; 2:close valve;)
/ per -> period of this task (sec)
/ obs -> one example for a time when this task is executed (observation) (unix time)
/ loc -> where to perform the task, typically a valve  

jobs:([`u#jiseq:`symbol$()]t:`tasks$());
/ jiseq -> job identification sequence 
/ t -> a task of this job

/ mkt -> make a task 
/ actn = "a": "2" -> 2
/ per = "D'D'HH:MM:SS:mmmmmmmmm": "9D12:55:21.734357411" -> 9D12:55:21.734357411
/ obs = "YYYY-MM-DD'T'HH:MM:SS.mmmmmmmmm": "2007-08-09T12:55:21.734357411" -> 2007.08.09D12:55:21.734357411
mkt:{[actn;per;obs;loc];

	actn: "I"$actn
	if[actn<0 and actn>2;'"actn ∈ [1; 2]"];

	per: `long$"N"$per
	if[per<1;'"per ∈ [1; ∞)"]

	obs: `long$"P"$obs
	obs: obs mod per 
};