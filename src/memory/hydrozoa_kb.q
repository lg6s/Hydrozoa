tasks:([`u#tiseq:`symbol$()]actn:`int$();per:`long$();`s#obs:`long$();loc:`symbol$());
/ tiseq -> task identification sequence
/ actn -> action to perform (1: open valve; 2:close valve;)
/ per -> period of this task (sec)
/ obs -> one example for a time when this task is executed (observation) (unix time)
/ loc -> where to perform the task, typically a valve  

jobs:([`u#nom:`symbol$()]stat:`boolean$());
/ nom -> name of the job
/ stat -> status of the job

rel:([]j:`jobs$();t:`tasks$());
/ j -> job  
/ t -> a task of this job

/ mkj -> make a job 
/ p = per = "D'D'HH:MM:SS:mmmmmmmmm": "9D12:55:21.734357411" -> 9D12:55:21.734357411
/ o = obs = "YYYY-MM-DD'T'HH:MM:SS.mmmmmmmmm": "2007-08-09T12:55:21.734357411" -> 2007.08.09D12:55:21.734357411
/ d = dur -> duration (definition equal to p)
/ l = loc 
mkj:{[n;p;o;d;l]
	p: `long$"N"$p; d: `long$"N"$d;
	o: (`long$"P"$o) mod p;
	l: `$l; n: `$n;

	if[p<1; '"per ∈ [1; ∞)"]; 
	if[d<1 or d>1; '"1 < dur < per"]; 
	if[(all (key jobs) <> n)[`nom]; '"unknown job"]; 

	q: select actn, obs-o from tasks where loc = l;
	if[count q > 0;
		r: select actn from q where obs < 0, obs = max obs;
		if[first r[`actn] = 1; '"integrity (wn.1.1)"];
		r: select actn from q where obs-d > 0, obs = min obs;
		if[first r[`actn] = 1; '"integrity wn.1.2)"];
		r: select sum (p mod per) from q where obs < 0;
		if[first r[`per] > 0; '"integrity (wn.2.1)"];
	]

	seq: `$("" sv string md5 "." sv ({[x] string x} each (1, p, o, l)));
	tasks,:(seq; 1; p; o; l);
	rel,:(n; seq);

	seq: `$("" sv string md5 "." sv ({[x] string x} each (2, p, (o+d), l)));
	tasks,:(seq; 2; p; (o+d); l);
	rel,:(n;seq); };

/ defj -> define job | n = nom
defj:{[n]jobs,:((`$n), 0b) }