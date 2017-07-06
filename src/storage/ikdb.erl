-module(ikdb).
-behaviour(gen_server).

%% API export
-export([start/0, init/1, terminate/2, handle_call/3, handle_cast/2]).

%% internal functions export
-export([act/1, act/2, qry/1, qry/2, delc/0, delc/1, bldc/0, bldc/1]).

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

%%====================================================================
%% internal functions
%%====================================================================

bldc() -> bldc({lkdbcon, <<"127.0.0.1">>, 1843}).
bldc({C, H, P}) -> 
	case ets:info(kdbcons) of 
		undefined -> ets:new(kdbcons, [set, protected, {keypos,1}, {heir,none}, {write_concurrency,false}, {read_concurrency,false}, named_table]), bldc({C, H, P});
		_ -> case q:connect(H, P) of
					{ok, Pid} -> 	
						error_logger:info_report(["A new db-connection was created", {C, H, P}]),
						ets:insert(kdbcons, {C, Pid}), 
						act("lhs", C), {ok, Pid};
					{error, E} -> erlang:error(E);
					_ -> erlang:error("unknown error")
			end
	end.

%%--------------------------------------------------------------------
delc(C) -> 
	act("scs", C), q:close(ets:lookup_element(kdbcons, C, 2)), 
	ets:delete(kdbcons, C), error_logger:info_report(["A db-connection was closed ~p~n", C]).
delc() -> 
	case ets:first(kdbcons) of 
		'$end_of_table' -> ets:delete(kdbcons);
		C -> delc(C), delc()
	end.

%%--------------------------------------------------------------------
qry(Q) -> qry(Q, lkdbcon).
qry(Q, C) -> 
	case q:execute(ets:lookup_element(kdbcons, C, 2), Q) of
		{error, E} -> erlang:error(E);
		R -> R
	end.

%%--------------------------------------------------------------------
% I = [defj, mkj, ssj, rmj, gnt, rmt, scs, lhs]
% pattern qry: [a-z]+(%[a-z0-9]*)*
act(Q) -> act(Q, lkdbcon).
act(Q, C) -> 
	[I|A] = string:split(Q, "%"), 
	qry(list_to_binary(lists:concat([I,"[",string:replace(A, "%", ";", all),"]"])), C).