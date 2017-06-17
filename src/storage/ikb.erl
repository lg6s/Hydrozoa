-module(ikb).
-export([act/2, act/3, qry/1, qry/2, delc/0, delc/1, bldc/0, bldc/3]).

bldc() -> bldc(lkdbcon, <<"127.0.0.1">>, 1843).
bldc(C, H, P) -> 
	case ets:info(kdbcons) of 
		undefined -> ets:new(kdbcons, [set, protected, {keypos,1}, {heir,none}, {write_concurrency,false}, {read_concurrency,false}, named_table]), bldc(C, H, P);
		_ -> case q:connect(H, P) of
					{ok, Pid} -> ets:insert(kdbcons, {C, Pid});
					{error, E} -> erlang:error(E);
					_ -> erlang:error("unknown error")
			end
	end.

delc(C) -> q:close(ets:lookup_element(kdbcons, C, 2)), ets:delete(kdbcons, C).
delc() -> 
	case ets:first(kdbcons) of 
		'$end_of_table' -> ets:delete(kdbcons);
		C -> q:close(ets:lookup_element(kdbcons, C, 2)),
			ets:delete(kdbcons, C),
			delc()
	end.

qry(Q) -> qry(Q, lkdbcon).
qry(Q, C) -> 
	case q:execute(ets:lookup_element(kdbcons, C, 2), Q) of
		{error, E} -> erlang:error(E);
		R -> R
	end.

%	A=defj	B=mkj
%	C=ssj	D=rmj
%	E=gnt	F=rmt
%	G=scs	H=lhs
act(I, A) -> act(I, A, lkdbcon).
act(I, A, C) -> 
	P = lists:foldl(fun(N, R) -> R, " ", N end, "", A),
	case I of
		"A" -> qry(<<"defj", P>>, C);
		"B" -> qry(<<"mkj", P>>, C); 
		"C" -> qry(<<"ssj", P>>, C);
		"D" -> qry(<<"rmj", P>>, C);
		"E" -> qry(<<"gnt", P>>, C);
		"F" -> qry(<<"rmt", P>>, C);
		"G" -> qry(<<"scs", P>>, C);
		"H" -> qry(<<"lhs", P>>, C)
	end.