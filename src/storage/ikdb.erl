-module(ikdb).

-import(ikb, [act/1, delc/0, bldc/1]).
-export([start/1, stop/0]).

%%====================================================================
%% internal functions
%%====================================================================	
start(C1) ->
	{ok, DIR} = file:get_cwd(),
	[BASE, _] = string:split(DIR, "_build"),
	os:cmd(string:concat([BASE, "resources/scripts/q/q_start ", BASE, "src/storage/kb.q"])),
	{ok, _} = ikb:bldc(C1),
	ikb:act("lhs"),
	ok.

stop() -> 
	{ok, DIR} = file:get_cwd(),
	[BASE, _] = string:split(DIR, "_build"),
	os:cmd(string:concat([BASE, "resources/scripts/q/q_stop ", lists:droplast(os:cmd("echo $HOME")), "/ ", BASE, "src/storage/kb.q"])),
	ikb:act("scs"),
	ikb:delc(),
	ok.