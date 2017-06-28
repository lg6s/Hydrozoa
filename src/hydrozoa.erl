-module(hydrozoa).
-behaviour(application).

%% Application callbacks
-export([start/2, stop/1]).

%%====================================================================
%% API
%%====================================================================

start(_StartType, _StartArgs) -> erlang:error("not implemented").

%%--------------------------------------------------------------------
stop(_State) -> ok.