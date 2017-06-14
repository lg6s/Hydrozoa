-module(ikb).
-export([defj/1, mkj/5, ssj/2, rmj/1, gnt/0, rmt/1, scs/0, lhs/0]).

bld_kdb() -> ets:new(kdbcons), add_kdb(lkdbcon, "127.0.0.1", 1843).

bld_kdb(K, H, P) > 
	case q:connect(<<H>>, P) of
		{ok, Pid} -> ets:insert(kdbcons, {K, Pid});
		{error, E} -> erlang:error(E);
		_ -> erlang:error("unknown error");
	end.

del_kdb(K) -> q:close(ets:lookup(kdbcons, K)), ets:delete(kdbcons, K).

del_kdb() -> 
	case ets:first(kdbcons) of 
		'$end_of_table' -> ets:delete(kdbcons);
		K -> q:close(ets:lookup(kdbcons, K)),
			ets:delete(kdbcons, K),
			del_kdbconn()
	end.


defj(J) -> erlang:error("not implemented").
mkj(P,D,O,L,J) -> erlang:error("not implemented").
ssj(J,S) -> erlang:error("not implemented").
rmj(J) -> erlang:error("not implemented").
gnt() -> erlang:error("not implemented").
rmt(T) -> erlang:error("not implemented").
scs() -> erlang:error("not implemented").
lhs() -> erlang:error("not implemented").