jobs:([`u#jb:`symbol$()]stat:`boolean$());
/ jb -> name of the job
/ stat -> status of the job

tasks:([`u#tiseq:`symbol$()]actn:`int$();per:`long$();`s#obs:`long$();loc:`symbol$();jb:`jobs$());
/ tiseq -> task identification sequence
/ actn -> action to perform (1: open valve; 2:close valve;)
/ per -> period of this task (sec)
/ obs -> one example for a time when this task is executed (observation) (unix time)
/ loc -> where to perform the task, typically a valve
/ jb -> job

ld: 0b 				/ lock down variable
ts: 7200000000000 	/ time shift (+2h)

/ mkj -> make a job 
/ p = per = "D'D'HH:MM:SS:mmmmmmmmm": "9D12:55:21.734357411" -> 9D12:55:21.734357411
/ o = obs = "YYYY-MM-DD'T'HH:MM:SS.mmmmmmmmm": "2007-08-09T12:55:21.734357411" -> 2007.08.09D12:55:21.734357411
/ d = dur -> duration (definition equal to p)
/ l = loc 
mkj:{[p;d;o;l;j] 
	p: `long$"N"$p; d: `long$"N"$d; 
	o: (`long$"P"$o) mod p; 
	l: `$l; j: `$j; 

	if[p<1; '"per ∈ [1; ∞)"]; if[d<1 or d>1; '"1 < dur < per"]; 
	if[(all (key jobs) <> j)[`jb]; '"unknown job"]; 

	q: select actn, per, obs-o from tasks where loc = l; 
	if[count q > 0; 
		r: select actn from q where obs < 0, obs = max obs; 
		if[first r[`actn] = 1; '"integrity (wn.1.1)"]; 
		r: select actn from q where (obs-d) > 0, obs = min obs; 
		if[first r[`actn] = 2; '"integrity wn.1.2)"]; 
		r: select sum (p mod per) from q where obs < 0; 
		if[first r[`per] > 0; '"integrity (wn.2.1)"]; 
	] 

	seq: `$("" sv string md5 "." sv ({[x] string x} each (1, p, o, l))); 
	tasks,:(seq; 1; p; o; l; j);
	seq: `$("" sv string md5 "." sv ({[x] string x} each (2, p, (o+d), l))); 
	tasks,:(seq; 2; p; (o+d); l; j); }; 

/ defj -> define job | j = jb 
defj:{[j]jobs,:((`$j), 0b) }

/ ssj -> set status of job 
/ j = jb | s = stat ("0" or "1")
ssj:{[j;s]update stat:(s = "1") from `jobs where jb = `$j } 

/ gnt -> get next task 
gnt:{if[ld; '"lock down in effect"]; t: ts + `long$.z.p;
	q: select jb from jobs where stat = 1
	q: select actn, loc, (obs-t)+per*ceiling((t-obs)%per) from tasks where jb in q[`jb]
	q: select first actn, loc, obs from q where obs = min obs };