-module(cli_server).
-behaviour(gen_server).

-import(ikb, [act/1, act/2, qry/1, qry/2, delc/0, delc/1, bldc/0, bldc/3]).
-export([start/0, start/1, init/1, terminate/2, handle_call/3]).

start(Args) -> gen_server:start_link(?MODULE, Args, []).
start() -> start({lkdbcon, <<"127.0.0.1">>, 1843, 1844}).

init({C, H, PI, PII}) -> 
	process_flag(trap_exit, true), 
	ikb:bldc(C, H, PI), 
	{ok, []}.

terminate(shutdown, S) -> ikb:delc(), ok.

handle_call(shutdown, _, S) -> terminate(shutdown, S).
handle_call({action, Q}, _, S) -> 