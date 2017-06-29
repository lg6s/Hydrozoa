-module(cli_server).
-behaviour(gen_server).

-import(ikb, [act/1, delc/0, bldc/3]).
-export([start/0, start/1, init/1, terminate/2, handle_call/3]).

%%====================================================================
%% API
%%====================================================================

start(Args) -> gen_server:start_link(?MODULE, Args, []).
start() -> start({{lkdbcon, <<"127.0.0.1">>, 1843}, 1844}).

init({C1, C2}) -> 
	process_flag(trap_exit, true), 
	{ok, _} = server_init(C2),
	{ok, _} = ikb:bldc(C1), 
	{ok, []}.

terminate(shutdown, _) -> ikb:delc(), ok.

handle_call(shutdown, _, S) -> terminate(shutdown, S);
handle_call({qry, Q}, _, S) -> 
	case re:run(Q, "[a-z0-9]+(%[a-z0-9]*)*") of 
		{match, _} -> 
			case ikb:act(Q) of 
				{error, R} -> {reply, {error, R}, S};
				R -> {reply, R, S}
			end;
		nomatch -> ok
	end;
handle_call({actn, A}, _, _) -> erlang:error("not implemented").

%%====================================================================
%% internal functions
%%====================================================================

server_init(P) ->
	case gen_tcp:listen(P, [binary]) of  
		{ok, LSock} -> {ok, spawn(?MODULE, server, [LSock])};
		{error, R} -> {error, R} 
	end.

server(LSock) -> 
	case gen_tcp:accept(LSock) of 
		{ok, Sock} -> loop(Sock), server(LSock);
		{error, _} -> server(LSock)
	end. 

loop(Sock) -> 
	receive 
		{tcp, Client, Data} ->
			gen_tcp:send(Client, gen_server:call(?MODULE, binary_to_term(Data))),
			loop(Sock);
		{tcp_closed, _} -> ok
	end.