-module(hydrozoa_sup).
-behaviour(supervisor).

-export([start/0, init/1]).

%%====================================================================
%% API functions
%%====================================================================

start() ->
    supervisor:start_link({local, ?MODULE}, ?MODULE, []).

%%====================================================================
%% Supervisor callbacks
%%====================================================================

init([]) ->
    {ok, {{one_for_one, 1, 5}, [
    		{api_server, {api_server, start, []}, permanent, brutal_kill, worker, [api_server]}
    ]}}.