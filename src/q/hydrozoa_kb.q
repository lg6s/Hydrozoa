tasks:([`u#tiseq:`byte$()]actn:`int$();per:`long$();`s#obs:`long$();loc:`symbol$());
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
	o: (`long$"P"$o) mod p;
	p: `long$"N"$p;
	d: `long$"N"$d;
	l: `$l; 
	n: `$n;

	if[p<1; '"per ∈ [1; ∞)"]; 
	if[d<1 or d>1; '"1 < dur < per"]; 
	if[all jobs[`nom] <> n; '"unknown job"]; 

	n: select actn, obs-o from tasks where loc = l;
	r: select actn from n where obs < 0, obs = max obs;
	if[r[`actn] = 1; '"integrity (wn.1.1)"];

	r: select actn from n where obs-d > 0, obs = min obs;
	if[r[`actn] = 1; '"integrity wn.1.2)"];

	r: select sum (p mod per) from n where obs < 0;
	if[r[`per] > 0; '"integrity (wn.2.1)"];

	seq: md5 `char$ 1, p, o, l;
	tasks,:(seq; 1; p; o; l);
	rel,:(n; seq);

	seq: md5 `char$ 2, p, (o+d), l;
	tasks,:(seq; 2; p; (o+d); l);
	rel,:(n;seq); };

/ defj -> define job | n = nom
defj:{[n]jobs,:((`$n), 0b) }