-module(ikb).
-export([defj/1, mkj/5, ssj/2, rmj/1, gnt/0, rmt/1, scs/0, lhs/0]).

query(Q) -> 
	{ok, P} = q:connect(<<"127.0.0.1">>, 1843)
	R = q:execute(P, <<Q>>)
	q:close(P)
	R.


defj(J) -> 


mkj(P,D,O,L,J) -> erlang:error("not implemented").
ssj(J,S) -> erlang:error("not implemented").
rmj(J) -> erlang:error("not implemented").
gnt() -> erlang:error("not implemented").
rmt(T) -> erlang:error("not implemented").
scs() -> erlang:error("not implemented").
lhs() -> erlang:error("not implemented").