-module(hydrozoa).
-behaviour(application).

%% Application callbacks
-export([start/2, prep_stop/1, stop/1]).

-import(ikdb, [start/0, stop/0]).

%%====================================================================
%% API
%%====================================================================

start(_StartType, _StartArgs) ->
	ok = ikdb:start(),
	hydrozoa_sup:start().

%%--------------------------------------------------------------------
prep_stop(_State) ->
	ok = ikdb:stop().

%%--------------------------------------------------------------------
stop(_State) -> 
	ok.