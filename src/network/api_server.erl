-module(api_server).
-behaviour(gen_server).

-export([start/0, start/1, init/1, terminate/2, handle_call/3, handle_cast/2, server/1, server_init/1, loop/1]).

%%====================================================================
%% API
%%====================================================================

start(Args) -> gen_server:start_link(?MODULE, Args, []).
start() -> start({{lkdbcon, <<"127.0.0.1">>, 1843}, 1844}).

%%--------------------------------------------------------------------
init({C1, C2}) ->
	{ok, _} = server_init(C2),
	{ok, _} = ikb:bldc(C1),
	{ok, []}.

%%--------------------------------------------------------------------
terminate(_, _) -> ok.

%%--------------------------------------------------------------------
handle_call(shutdown, _, S) -> terminate(shutdown, S);
handle_call({qry, Q}, _, S) -> 
	case re:run(Q, "[a-z0-9]+(%[a-z0-9]*)*") of 
		{match, _} -> 
			case ikb:act(Q) of 
				{error, R} -> {reply, {error, R}, S};
				R -> {reply, R, S}
			end;
		nomatch -> ok
	end.

%%--------------------------------------------------------------------
handle_cast(shutdown, S) -> terminate(shutdown, S).

%%====================================================================
%% internal functions
%%====================================================================

server_init(P) ->
	case gen_tcp:listen(P, [binary]) of  
		{ok, LSock} -> {ok, spawn(?MODULE, server, [LSock])};
		{error, R} -> {error, R} 
	end.

%%--------------------------------------------------------------------
server(LSock) -> 
	case gen_tcp:accept(LSock) of 
		{ok, Sock} -> loop(Sock), server(LSock);
		{error, _} -> server(LSock)
	end. 

%%--------------------------------------------------------------------
loop(Sock) -> 
	receive 
		{tcp, Client, Data} ->
			gen_tcp:send(Client, gen_server:call(?MODULE, binary_to_term(Data))),
			loop(Sock);
		{tcp_closed, _} -> ok
	end.