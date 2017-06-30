-module(hydrozoa).
-behaviour(application).

%% Application callbacks
-export([start/2, stop/1]).

%%====================================================================
%% API
%%====================================================================

start(_StartType, _StartArgs) -> hydrozoa_sup:start().

%%--------------------------------------------------------------------
stop(_State) -> ok.