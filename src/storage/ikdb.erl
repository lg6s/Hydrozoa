-module(ikdb).
-behaviour(gen_server).

-export([start/0, init/1, terminate/2, handle_call/3, handle_cast/2]).

%%====================================================================
%% API
%%====================================================================

start() ->
	gen_server:start_link(?MODULE, [], []).

%%--------------------------------------------------------------------
init(_) ->
	process_flag(trap_exit, true),
	{ok, DIR} = file:get_cwd(), [BASE, _] = string:split(DIR, "_build"),
	os:cmd(lists:concat([BASE, "resources/scripts/q/q_start ", BASE, "src/storage/kb.q"])),
	error_logger:info_msg("The kdb server is now running"),
	{ok, []}.

%%--------------------------------------------------------------------
terminate(_, _) -> 
	{ok, DIR} = file:get_cwd(),
	[BASE, _] = string:split(DIR, "_build"),
	os:cmd(lists:concat([BASE, "resources/scripts/q/q_stop"])),
	error_logger:info_msg("The kdb server was shut down"),
	ok.

%%--------------------------------------------------------------------
handle_call(shutdown, _, S) -> 
	terminate(shutdown, S).

%%--------------------------------------------------------------------
handle_cast(shutdown, S) -> 
	terminate(shutdown, S).