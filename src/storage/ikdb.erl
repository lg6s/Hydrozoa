-module(ikdb).
-export([start/0, stop/0]).

%%====================================================================
%% internal functions
%%====================================================================	

start() ->
	{ok, DIR} = file:get_cwd(),
	[BASE, _] = string:split(DIR, "_build"),
	os:cmd(lists:concat([BASE, "resources/scripts/q/q_start ", BASE, "src/storage/kb.q"])),
	error_logger:info_msg("The kdb server is now running"),
	ok.

%%--------------------------------------------------------------------
stop() -> 
	{ok, DIR} = file:get_cwd(),
	[BASE, _] = string:split(DIR, "_build"),
	os:cmd(lists:concat([BASE, "resources/scripts/q/q_stop"])),
	error_logger:info_msg("The kdb server was shut down"),
	ok.